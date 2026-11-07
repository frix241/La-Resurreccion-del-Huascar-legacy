using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LRH.Core;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LRH.Runtime
{
    public sealed class GameRoot : MonoBehaviour
    {
        public const float WorldMinX = -48f;
        public const float WorldMaxX = 48f;
        public const float WorldMinY = -32f;
        public const float WorldMaxY = 32f;
        private readonly List<CombatEvent> events = new List<CombatEvent>();
        private readonly List<ShipActor> hostiles = new List<ShipActor>();
        private Registry<ShipActor> actors = new Registry<ShipActor>();
        private IProgressRepository repository;
        private Progress progress;
        private Mission mission;
        private GameUi ui;
        private PatrolFactory patrolFactory;
        private GunboatFactory gunboatFactory;
        private CancellationTokenSource reportCancellation;
        private ShipActor player;
        private ShipActor convoy;

        public Transform Arena { get; private set; }
        public Camera GameCamera { get; private set; }
        public ProjectilePool Pool { get; private set; }
        public SoundFx Sounds { get; private set; }
        public bool IsPaused { get; private set; }
        public ShipActor Player => player;
        public ShipActor Convoy => convoy;
        public int MissionNumber => mission == null ? 0 : mission.Number;
        public ShipActor NavigationTarget
        {
            get
            {
                if (player == null) return null;
                ShipActor closest = convoy != null && convoy.Alive ? convoy : null;
                float best = closest == null ? float.PositiveInfinity :
                    (closest.transform.position - player.transform.position).sqrMagnitude;
                foreach (var actor in hostiles)
                {
                    if (actor == null || !actor.Alive) continue;
                    float distance = (actor.transform.position - player.transform.position).sqrMagnitude;
                    if (distance < best) { closest = actor; best = distance; }
                }
                return closest;
            }
        }
        private Vector3 cameraVelocity;
        private Vector3 cameraPosition;
        public MissionState CurrentState => mission == null ? MissionState.Defeat : mission.State;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Entry()
        {
            if (FindAnyObjectByType<GameRoot>() == null)
                new GameObject("LRH / Director").AddComponent<GameRoot>();
        }

        // Configure on an inactive GameObject before Awake; normal scenes use disk storage.
        public void ConfigureRepository(IProgressRepository progressRepository)
        {
            if (ui != null) throw new InvalidOperationException("El director ya está inicializado.");
            repository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        }

        private void Awake()
        {
            Time.timeScale = 1;
            if (repository == null)
                repository = new JsonProgressRepository(Application.persistentDataPath);
            progress = repository.Load();
            AudioListener.volume = progress.MasterVolume;
            patrolFactory = gameObject.AddComponent<PatrolFactory>();
            gunboatFactory = gameObject.AddComponent<GunboatFactory>();
            var cameraObject = new GameObject("Ocean Camera");
            cameraObject.tag = "MainCamera";
            GameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<PixelPresentation>();
            GameCamera.orthographic = true;
            GameCamera.orthographicSize = 7.5f;
            GameCamera.transform.position = new Vector3(0, 0, -20);
            GameCamera.backgroundColor = new Color(.012f, .045f, .07f);
            GameCamera.clearFlags = CameraClearFlags.SolidColor;
            Sounds = gameObject.AddComponent<SoundFx>();
            ui = new GameObject("LRH / Interface").AddComponent<GameUi>();
            ui.Initialize(this);
            ReturnMenu();
        }

        private void Update()
        {
            if (mission == null || mission.State != MissionState.Playing) return;
#if ENABLE_INPUT_SYSTEM
            bool escape = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
            bool escape = Input.GetKeyDown(KeyCode.Escape);
#endif
            if (escape)
            {
                if (IsPaused) Resume();
                else Pause();
            }
        }

        private void LateUpdate()
        {
            if (mission != null && !IsPaused) mission.Resolve();
            if (player != null && player.Alive && !IsPaused)
            {
                float halfHeight = GameCamera.orthographicSize;
                float halfWidth = halfHeight * GameCamera.aspect;
                var destination = new Vector3(
                    Mathf.Clamp(player.transform.position.x, WorldMinX+halfWidth, WorldMaxX-halfWidth),
                    Mathf.Clamp(player.transform.position.y, WorldMinY+halfHeight, WorldMaxY-halfHeight), -20);
                cameraPosition = Vector3.SmoothDamp(cameraPosition,
                    destination, ref cameraVelocity, .22f);
                var snapped=cameraPosition;
                snapped.x=Mathf.Round(snapped.x*36)/36;snapped.y=Mathf.Round(snapped.y*36)/36;
                GameCamera.transform.position=snapped;
            }
        }

        public void ReturnMenu()
        {
            reportCancellation?.Cancel();
            reportCancellation?.Dispose();
            reportCancellation = null;
            Time.timeScale = 1;
            IsPaused = false;
            mission = null;
            if (Arena != null) Destroy(Arena.gameObject);
            Arena = null;
            Pool = null;
            player = null;
            convoy = null;
            hostiles.Clear();
            actors = new Registry<ShipActor>();
            Sounds.Menu();
            ui.ShowMenu(progress);
        }

        public void StartMission(int number)
        {
            if (!progress.CanPlay(number)) return;
            reportCancellation?.Cancel();
            reportCancellation?.Dispose();
            reportCancellation = null;
            if (Arena != null) Destroy(Arena.gameObject);
            Time.timeScale = 1;
            IsPaused = false;
            events.Clear();
            hostiles.Clear();
            actors = new Registry<ShipActor>();
            Arena = new GameObject("Mission " + number + " / Arena").transform;
            var sea = new GameObject("Océano abierto").AddComponent<OpenSea>();
            sea.transform.SetParent(Arena, false);
            sea.Initialize(GameCamera);
            Pool = new GameObject("Projectiles").AddComponent<ProjectilePool>();
            Pool.transform.SetParent(Arena, false);
            Pool.Sounds=Sounds;
            player = ShipActor.Spawn(this, "huascar", "huascar", Faction.Player, new Vector2(-8, -1));
            if (number == 1)
            {
                mission = new Mission(1, new EliminateObjective("patrol"));
                hostiles.Add(patrolFactory.Create(this, "patrol", new Vector2(5, 2), player));

            }
            else if (number == 2)
            {
                mission = new Mission(2, new EscortObjective("convoy"));
                convoy = ShipActor.Spawn(this, "convoy", "convoy", Faction.Ally, new Vector2(-9, -4));
                convoy.transform.rotation = Quaternion.Euler(0, 0, -90);
                hostiles.Add(patrolFactory.Create(this, "patrol-2", new Vector2(1, -1), convoy));
                hostiles.Add(gunboatFactory.Create(this, "gunboat", new Vector2(8, -1), convoy));

            }
            else
            {
                mission = new Mission(3, new EliminateManyObjective("battery1", "battery2"));
                hostiles.Add(ShipActor.Spawn(this, "battery1", "battery", Faction.Enemy, new Vector2(34, 18), player, new ChaseStrategy()));
                hostiles.Add(ShipActor.Spawn(this, "battery2", "battery", Faction.Enemy, new Vector2(32, -18), player, new ChaseStrategy()));


            }
            cameraVelocity = Vector3.zero;
            cameraPosition = player.transform.position + new Vector3(0, 0, -20);
            GameCamera.transform.position = cameraPosition;
            mission.Finished += OnFinished;
            Sounds.Battle();
            ui.ShowHud(number, player, player.Health, convoy, convoy == null ? null : convoy.Health, hostiles);
        }

        public void PreviewMission(int number)
        {
            if (progress.CanPlay(number)) ui.ShowIntro(number);
        }

        private void Island(Vector2 position, float scale)
        {
            var go = new GameObject("Isla / obstáculo");
            go.transform.SetParent(Arena, false);
            go.transform.position = position;
            go.transform.localScale = Vector3.one * scale;
            var shadow = new GameObject("Sombra de isla");
            shadow.transform.SetParent(go.transform, false);
            shadow.transform.localPosition = new Vector3(.22f, -.2f, .08f);
            shadow.transform.localScale = new Vector3(1.18f, .72f, 1);
            var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            shadowRenderer.sprite = Visuals.Sprite("island");
            shadowRenderer.color = new Color(.002f, .012f, .018f, .5f);
            shadowRenderer.sortingOrder = 4;
            go.AddComponent<SpriteRenderer>().sprite = Visuals.Sprite("island");
            var collider = go.AddComponent<CircleCollider2D>();
            collider.radius = 1.05f;
        }

        public void Destroyed(string id, bool isPlayer)
        {
            if (mission == null || !actors.Remove(id)) return;
            if (isPlayer) mission.KillPlayer();
            else mission.Destroy(id);
        }
        internal void Register(ShipActor actor)
        {
            if (!actors.Register(actor.Id, actor))
                throw new InvalidOperationException("ID de entidad repetido: " + actor.Id);
        }
        public void Arrive(string id) { if (mission != null) mission.Arrive(id); }
        public void Record(CombatEvent entry) { if (mission != null) events.Add(entry); }

        public void SetVolume(float value)
        {
            progress.SetVolume(value);
            AudioListener.volume = value;
            if (!repository.Save(progress)) Debug.LogWarning("No se guardó el ajuste de volumen.");
        }

        public void Pause()
        {
            if (mission == null || mission.State != MissionState.Playing || IsPaused) return;
            IsPaused = true;
            Time.timeScale = 0;
            Sounds.Pause(true);
            ui.ShowPause();
        }

        public void Resume()
        {
            if (!IsPaused || mission == null) return;
            IsPaused = false;
            Time.timeScale = 1;
            Sounds.Pause(false);
            ui.ShowHud(mission.Number, player, player.Health, convoy, convoy == null ? null : convoy.Health, hostiles);
        }

        public void Retry() { if (mission != null) StartMission(mission.Number); }

        private void OnFinished(MissionState result)
        {
            if (result == MissionState.Victory)
            {
                progress.Complete(mission.Number);
                if (!repository.Save(progress)) Debug.LogWarning("Progreso no guardado; revisa permisos de disco.");
            }
            Time.timeScale = 0;
            Sounds.Result(result==MissionState.Victory);
            ui.ShowResult(mission.Number, result, mission.FailureReason);
            reportCancellation = new CancellationTokenSource();
            ShowReportAsync(mission, events.ToArray(), reportCancellation.Token);
        }

        private async void ShowReportAsync(Mission source, CombatEvent[] snapshot, CancellationToken token)
        {
            try
            {
                var report = await CombatReporter.AnalyzeAsync(snapshot, token);
                if (this != null && !token.IsCancellationRequested && mission == source)
                    ui.UpdateReport("Daño total: " + report.TotalDamage + "  ·  Destruidos: " + report.Destroyed +
                        (report.TopSource.Length == 0 ? "" : "  ·  Más impactos: " + report.TopSource));
            }
            catch (OperationCanceledException) { /* Leaving a mission cancels obsolete results. */ }
            catch (Exception e) { if (this != null) Debug.LogWarning("Informe de combate: " + e.Message); }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
            reportCancellation?.Cancel();
            reportCancellation?.Dispose();
            if (ui != null) Destroy(ui.gameObject);
            if (GameCamera != null) Destroy(GameCamera.gameObject);
        }
    }
}
