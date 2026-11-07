using System.Collections;
using LRH.Core;
using LRH.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LRH.Tests
{
    public sealed class MissionPlayTests
    {
        private sealed class MemoryRepository : IProgressRepository
        {
            private string saved = new Progress().ToJson();
            public MemoryRepository(bool unlockSecond = false, bool unlockThird = false)
            {
                if (unlockSecond) { var progress = new Progress(); progress.Complete(1); if(unlockThird)progress.Complete(2); saved = progress.ToJson(); }
            }
            public int SaveCount { get; private set; }
            public Progress Load() => Progress.FromJson(saved);
            public bool Save(Progress progress)
            {
                saved = progress.ToJson();
                SaveCount++;
                return true;
            }
        }

        private sealed class ForwardInput : IInputSource
        {
            public ShipCommand Read(Camera camera) => new ShipCommand(1, 0, false, Vector2.up);
        }

        private GameRoot root;
        private MemoryRepository repository;
        private float previousVolume;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            previousVolume = AudioListener.volume;
            // The runtime entry point may have created a director in the test scene.
            foreach (var existing in Object.FindObjectsByType<GameRoot>(FindObjectsSortMode.None))
            {
                existing.ReturnMenu();
                Object.Destroy(existing.gameObject);
            }
            yield return null;
            yield return null;
            repository = new MemoryRepository();
            var director = new GameObject("Test director");
            director.SetActive(false);
            root = director.AddComponent<GameRoot>();
            root.ConfigureRepository(repository);
            director.SetActive(true);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (root != null)
            {
                root.ReturnMenu();
                Object.Destroy(root.gameObject);
            }
            Time.timeScale = 1;
            yield return null;
            yield return null;
            AudioListener.volume = previousVolume;
        }

        private ShipActor Ship(string id)
        {
            foreach (var ship in root.Arena.GetComponentsInChildren<ShipActor>())
                if (ship.Id == id) return ship;
            Assert.Fail("No se encontró la nave: " + id);
            return null;
        }

        [UnityTest]
        public IEnumerator Victory_UsesOwnBackgroundAndContinueStartsUnlockedMission()
        {
            root.StartMission(1);Ship("patrol").Receive(1000,"test");yield return null;
            Assert.AreEqual("victory-background",GameObject.Find("Resultado / victoria").GetComponent<Image>().sprite.texture.name);
            Button next=null;
            foreach(var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
                if(button.GetComponentInChildren<Text>().text=="Continuar campaña")next=button;
            Assert.IsNotNull(next);next.onClick.Invoke();yield return null;
            Button start=null;
            foreach(var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
                if(button.GetComponentInChildren<Text>().text=="Iniciar misión")start=button;
            Assert.IsNotNull(start);start.onClick.Invoke();
            Assert.AreEqual(2,root.MissionNumber);
        }

        [UnityTest]
        public IEnumerator Defeat_UsesDifferentBackgroundAndRetryRestoresMission()
        {
            root.StartMission(1);root.Player.Receive(1000,"test");yield return null;
            Assert.AreEqual("defeat-background",GameObject.Find("Resultado / derrota").GetComponent<Image>().sprite.texture.name);
            Button retry=null;
            foreach(var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
                if(button.GetComponentInChildren<Text>().text=="Reintentar misión")retry=button;
            Assert.IsNotNull(retry);retry.onClick.Invoke();yield return null;
            Assert.AreEqual(MissionState.Playing,root.CurrentState);
            Assert.AreEqual(100,root.Player.Health.Current);
        }

        [UnityTest]
        public IEnumerator AudioAssets_ContainAudibleFiniteSamples()
        {
            var clips=Resources.LoadAll<AudioClip>("Audio");
            Assert.AreEqual(10,clips.Length);
            foreach(var clip in clips)
            {
                clip.LoadAudioData();
                yield return null;
                var samples=new float[Mathf.Min(clip.samples,22050)*clip.channels];
                Assert.IsTrue(clip.GetData(samples,0),clip.name);
                double energy=0;
                foreach(float sample in samples)
                {
                    Assert.IsFalse(float.IsNaN(sample) || float.IsInfinity(sample),clip.name);
                    energy+=sample*sample;
                }
                Assert.Greater(energy/samples.Length,.000001,clip.name+" must not be silent.");
            }
        }

        [UnityTest]
        public IEnumerator Music_FollowsMenuBattlePauseAndResult()
        {
            Assert.AreEqual("menu_theme",root.Sounds.CurrentTheme.name);
            root.StartMission(1);
            Assert.AreEqual("battle_theme",root.Sounds.CurrentTheme.name);
            Assert.Greater(root.Sounds.CurrentTheme.length,30);
            root.Pause();yield return null;
            Assert.AreEqual("battle_theme",root.Sounds.CurrentTheme.name);
            root.Resume();root.Player.Receive(1000,"test");yield return null;
            Assert.IsNull(root.Sounds.CurrentTheme,"Result fanfare should replace the combat loop.");
            root.ReturnMenu();
            Assert.AreEqual("menu_theme",root.Sounds.CurrentTheme.name);
        }

        [UnityTest]
        public IEnumerator PlayerCannotPushAlliedOrEnemyHull()
        {
            root.StartMission(1);
            var enemy=ShipActor.Spawn(root,"stationary-enemy","patrol",Faction.Enemy,new Vector2(-30,-17));
            var ally=ShipActor.Spawn(root,"moving-ally","convoy",Faction.Ally,new Vector2(-20,-17));
            ShipActor.Spawn(root,"enemy-pusher","huascar",Faction.Player,new Vector2(-30,-21),null,null,new ForwardInput());
            ShipActor.Spawn(root,"ally-pusher","huascar",Faction.Player,new Vector2(-20,-21),null,null,new ForwardInput());
            for(int i=0;i<130;++i)yield return new WaitForFixedUpdate();
            float enemyDrift=Mathf.Abs(enemy.GetComponent<Rigidbody2D>().position.y+17);
            float allyDrift=Mathf.Abs(ally.GetComponent<Rigidbody2D>().position.y+17);
            Assert.Less(Mathf.Max(enemyDrift,allyDrift),.08f,
                "Contact displacement: enemy="+enemyDrift+", ally="+allyDrift);
        }

        [UnityTest]
        public IEnumerator VolumeSlider_TrackAcceptsPointerInput()
        {
            yield return null;
            var slider=Object.FindFirstObjectByType<Slider>();
            var rect=(RectTransform)slider.transform;
            var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointer.button=UnityEngine.EventSystems.PointerEventData.InputButton.Left;
            pointer.position=RectTransformUtility.WorldToScreenPoint(slider.GetComponentInParent<Canvas>().worldCamera,rect.TransformPoint(new Vector3(rect.rect.xMin+rect.rect.width*.15f,0,0)));
            pointer.pointerPressRaycast=new UnityEngine.EventSystems.RaycastResult {module=slider.GetComponentInParent<GraphicRaycaster>(),gameObject=slider.gameObject};
            slider.OnPointerDown(pointer);
            Assert.Less(slider.value,.3f,"The track must respond to clicks rather than stay decorative.");
            slider.OnPointerUp(pointer);
        }

        [UnityTest]
        public IEnumerator MapBoundary_StopsHullAndCameraAtRegionEdge()
        {
            root.StartMission(1);
            var probe=ShipActor.Spawn(root,"edge-probe","huascar",Faction.Player,
                new Vector2(GameRoot.WorldMinX+1.2f,-20),null,null,new ForwardInput());
            probe.GetComponent<Rigidbody2D>().rotation=90;
            yield return null;
            for(int i=0;i<60;++i)yield return new WaitForFixedUpdate();
            Assert.Greater(probe.transform.position.x,GameRoot.WorldMinX+.4f);
            root.Player.transform.position=new Vector3(GameRoot.WorldMinX+1,-20,0);
            yield return new WaitForSeconds(1.3f);
            float left=root.GameCamera.transform.position.x-root.GameCamera.orthographicSize*root.GameCamera.aspect;
            Assert.GreaterOrEqual(left,GameRoot.WorldMinX-.1f);
        }

        [UnityTest]
        public IEnumerator NearestObjective_RecalculatesAfterMovingAndDestroyingTarget()
        {
            Object.Destroy(root.gameObject);yield return null;
            var director=new GameObject("Nearest target director");director.SetActive(false);
            root=director.AddComponent<GameRoot>();root.ConfigureRepository(new MemoryRepository(true,true));director.SetActive(true);
            root.StartMission(3);
            var first=Ship("battery1");var second=Ship("battery2");
            root.Player.transform.position=first.transform.position+Vector3.left*3;
            Assert.AreSame(first,root.NavigationTarget);
            root.Player.transform.position=second.transform.position+Vector3.left*3;
            Assert.AreSame(second,root.NavigationTarget);
            second.Receive(1000,"test");
            Assert.AreSame(first,root.NavigationTarget);
            yield return null;
            bool countUpdated=false;
            foreach(var label in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
                if(label.text=="1 / 2 objetivos neutralizados")countUpdated=true;
            Assert.IsTrue(countUpdated,"Checklist must count destroyed objectives while the mission continues.");
        }

        [UnityTest]
        public IEnumerator Hud_HealthFillReflectsActualDamage()
        {
            root.StartMission(1);root.Player.Receive(35,"test");yield return null;
            var bar=GameObject.Find("Huáscar / barra de vida").GetComponent<Image>();
            Assert.AreEqual(.65f,bar.fillAmount,.001f);
        }

        [UnityTest]
        public IEnumerator Credits_ContainsOnlyProvidedDeveloperHandlesAndNoEmailsOrSvgClaim()
        {
            Object.FindFirstObjectByType<GameUi>().ShowCredits();yield return null;
            string text="";foreach(var label in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))text+=label.text+"\n";
            foreach(string name in new[]{"MauroSullcaMamani","SrEstrada","ChristianAlexYana","frix241"})StringAssert.Contains(name,text);
            StringAssert.DoesNotContain("@",text);StringAssert.DoesNotContain("SVG",text);StringAssert.DoesNotContain("DEV-",text);
        }

        [UnityTest]
        public IEnumerator Terrain_CollisionMatchesSolidCellsAndStaysFixedWhenCameraMoves()
        {
            root.StartMission(1);
            yield return new WaitForFixedUpdate();
            var terrain = root.Arena.GetComponentInChildren<OpenSea>();
            var map = TerrainMap.Pacific;
            int solidX = 0;
            while (!map.Solid(solidX,30)) ++solidX;
            Vector2 land = new Vector2(map.originX+solidX+.5f,map.originY+30.5f);
            Assert.IsTrue(terrain.LandCollider.OverlapPoint(land), "Solid rendered cells must block ships.");
            Assert.IsFalse(terrain.LandCollider.OverlapPoint(new Vector2(-30,-18)), "Open water must not be solid.");
            var fixedPosition = terrain.Ground.transform.position;
            root.Player.transform.position = new Vector3(-30,-18,0);
            yield return new WaitForSeconds(.5f);
            Assert.AreEqual(fixedPosition, terrain.Ground.transform.position, "Terrain must never follow the camera.");
        }

        [UnityTest]
        public IEnumerator Terrain_ShipCannotSailThroughAnIsland()
        {
            root.StartMission(1);
            var map = TerrainMap.Pacific;
            int x = 44, y = 0;
            while (!map.Solid(x,y)) ++y;
            float shore = map.originY+y;
            var probe = ShipActor.Spawn(root,"collision-probe","huascar",Faction.Player,
                new Vector2(map.originX+x+.5f,shore-3),null,null,new ForwardInput());
            for (int i = 0; i < 120; ++i) yield return new WaitForFixedUpdate();
            Assert.Greater(probe.transform.position.y,shore-2.8f,"Probe must try to move toward the island.");
            Assert.Less(probe.transform.position.y,shore+.2f,"The island collider must stop the moving hull.");
        }

        [UnityTest]
        public IEnumerator OpenSea_MovementContinuesBeyondOldArenaAndCameraFollows()
        {
            root.StartMission(1);
            var probe = ShipActor.Spawn(root, "open-water-probe", "huascar", Faction.Player,
                new Vector2(-20, 8), null, null, new ForwardInput());
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.Greater(probe.transform.position.y, 8, "Navigation must not clamp to the old arena.");
            root.Player.transform.position = new Vector3(-30, -18, 0);
            yield return new WaitForSeconds(1.2f);
            Assert.Less(Vector2.Distance(root.GameCamera.transform.position, root.Player.transform.position), .6f);
        }

        [UnityTest]
        public IEnumerator Escort_ConvoyIsNavigationTargetWithVisibleDirectionLabel()
        {
            Object.Destroy(root.gameObject);
            yield return null;
            var director = new GameObject("Escort director");
            director.SetActive(false);
            root = director.AddComponent<GameRoot>();
            root.ConfigureRepository(new MemoryRepository(true));
            director.SetActive(true);
            root.StartMission(2);
            Assert.AreEqual("convoy", root.NavigationTarget.Id);
            root.Player.transform.position = new Vector3(-30, -18, 0);
            yield return new WaitForSeconds(1);
            bool found = false;
            foreach (var label in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
                if (label.text.StartsWith("CONVOY ALIADO")) found = label.gameObject.activeInHierarchy;
            Assert.IsTrue(found, "The offscreen convoy must keep a visible direction label.");
        }

        [UnityTest]
        public IEnumerator InjectedInput_MovesShipForward()
        {
            root.StartMission(1);
            var ship = ShipActor.Spawn(root, "control-probe", "huascar", Faction.Player,
                new Vector2(-10, 4), null, null, new ForwardInput());
            yield return null; // Update samples the injected command before the physics steps.
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.Greater(ship.transform.position.y, 4);
        }

        [UnityTest]
        public IEnumerator PatrolHit_AppliesDamageOnce()
        {
            root.StartMission(1);
            var player = Ship("huascar");
            var patrol = Ship("patrol");
            int original = patrol.Health.Current;
            root.Pool.Spawn(player, patrol.transform.position, Vector2.up, 25);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(original - 25, patrol.Health.Current);
        }

        [UnityTest]
        public IEnumerator DestroyingPlayer_EndsInDefeat()
        {
            root.StartMission(1);
            Ship("huascar").Receive(10000, "patrol");
            yield return null;
            yield return null;
            Assert.AreEqual(MissionState.Defeat, root.CurrentState);
            Assert.AreEqual(0, repository.SaveCount);
            Assert.AreEqual(0, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator DestroyingPatrol_WinsAndUnlocksNextMissionOnce()
        {
            root.StartMission(1);
            Ship("patrol").Receive(10000, "huascar");
            yield return null;
            yield return null;
            Assert.AreEqual(MissionState.Victory, root.CurrentState);
            Assert.AreEqual(1, repository.SaveCount);
            Assert.IsTrue(repository.Load().IsCompleted(1));
            Assert.IsTrue(repository.Load().CanPlay(2));
            Assert.AreEqual(0, Time.timeScale);
            yield return null;
            Assert.AreEqual(1, repository.SaveCount);
            root.ReturnMenu();
            root.StartMission(2);
            Assert.AreEqual(MissionState.Playing, root.CurrentState);
            Assert.IsNotNull(Ship("convoy"));
        }

        [UnityTest]
        public IEnumerator Pause_FreezesMovement_ResumeRestoresSimulation()
        {
            root.StartMission(1);
            var ship = ShipActor.Spawn(root, "pause-probe", "huascar", Faction.Player,
                new Vector2(-10, 4), null, null, new ForwardInput());
            yield return null;
            root.Pause();
            var position = ship.transform.position;
            yield return new WaitForSecondsRealtime(.1f);
            Assert.IsTrue(root.IsPaused);
            Assert.AreEqual(0, Time.timeScale);
            Assert.AreEqual(position, ship.transform.position);
            root.Resume();
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(root.IsPaused);
            Assert.AreEqual(1, Time.timeScale);
            Assert.Greater(ship.transform.position.y, position.y);
        }

        [UnityTest]
        public IEnumerator RetryAfterDefeat_CreatesFreshMission()
        {
            root.StartMission(1);
            var previousArena = root.Arena;
            Ship("huascar").Receive(10000, "patrol");
            yield return null;
            yield return null;
            Assert.AreEqual(MissionState.Defeat, root.CurrentState);
            root.Retry();
            yield return null;
            Assert.IsTrue(previousArena == null);
            Assert.AreEqual(MissionState.Playing, root.CurrentState);
            Assert.AreEqual(1, Time.timeScale);
            Assert.AreEqual(Ship("huascar").Health.Max, Ship("huascar").Health.Current);
            Assert.AreEqual(2, root.Arena.GetComponentsInChildren<ShipActor>().Length);
        }

        [UnityTest]
        public IEnumerator ReturnMenuWhilePaused_RemovesArenaAndAllowsRestart()
        {
            root.StartMission(1);
            var previousArena = root.Arena;
            root.Pause();
            root.ReturnMenu();
            yield return null;
            Assert.IsTrue(previousArena == null);
            Assert.IsNull(root.Arena);
            Assert.IsNull(root.Pool);
            Assert.IsFalse(root.IsPaused);
            Assert.AreEqual(1, Time.timeScale);
            root.StartMission(1);
            Assert.AreEqual(MissionState.Playing, root.CurrentState);
            Assert.IsNotNull(Ship("patrol"));
        }

        [UnityTest]
        public IEnumerator LockedMission_DoesNotCreateArena()
        {
            root.StartMission(2);
            yield return null;
            Assert.IsNull(root.Arena);
            Assert.AreEqual(0, repository.SaveCount);
        }

        [UnityTest]
        public IEnumerator MenuButtons_ReceiveGraphicRaycasts()
        {
            yield return null;
            var buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(buttons.Length, 4);
            foreach (var button in buttons)
            {
                if (!button.interactable) continue;
                var graphic = button.targetGraphic as Graphic;
                Assert.IsNotNull(graphic, button.name + " debe tener un Graphic objetivo");
                Assert.IsTrue(graphic.raycastTarget, button.name + " debe aceptar raycasts");
            }

            var interfaceObject = GameObject.Find("LRH / Interface");
            Assert.IsNotNull(interfaceObject);
            foreach (var graphic in interfaceObject.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic.GetComponent<Button>() != null || graphic.GetComponentInParent<Slider>() != null) continue;
                Assert.IsFalse(graphic.raycastTarget, graphic.name + " no debe bloquear botones");
            }
        }
    }
}
