using LRH.Core;
using UnityEngine;

namespace LRH.Runtime
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class ShipActor : MonoBehaviour
    {
        private GameRoot root;
        private ShipConfig config;
        private Rigidbody2D body;
        private BoxCollider2D hitbox;
        private WeaponClock weapon;
        private IAttackStrategy strategy;
        private ShipActor target;
        private IInputSource input;
        private ShipCommand command;
        private Transform turret;
        private Vector2 aimDirection = Vector2.up;
        private float turnDegrees = 155;
        private readonly RaycastHit2D[] movementHits = new RaycastHit2D[32];
        private readonly Collider2D[] rotationHits = new Collider2D[32];
        private readonly ContactFilter2D solidFilter = new ContactFilter2D { useTriggers = false };

        public Health Health { get; private set; }
        public string Id { get; private set; }
        public Faction Faction { get; private set; }
        public bool Alive => Health != null && !Health.IsDead;

        public static ShipActor Spawn(GameRoot root, string id, string kind, Faction faction,
            Vector2 at, ShipActor target = null, IAttackStrategy strategy = null, IInputSource playerInput = null)
        {
            var config = ShipConfig.Load(kind);
            var go = new GameObject(id);
            go.transform.SetParent(root.Arena, false);
            go.transform.position = at;
            var actor = go.AddComponent<ShipActor>();
            actor.Configure(root, id, kind, faction, config, target, strategy, playerInput);
            root.Register(actor);
            return actor;
        }

        private void Configure(GameRoot game, string id, string kind, Faction side, ShipConfig stats,
            ShipActor chaseTarget, IAttackStrategy attack, IInputSource playerInput)
        {
            root = game;
            Id = id;
            Faction = side;
            config = stats;
            target = chaseTarget;
            strategy = attack;
            Health = new Health(config.health);
            weapon = new WeaponClock(config.cooldown);
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0;
            body.bodyType = RigidbodyType2D.Kinematic;
            body.useFullKinematicContacts = true;
            body.linearDamping = 3;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.angularDamping = 100;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            hitbox = GetComponent<BoxCollider2D>();
            hitbox.size = kind == "battery" ? Vector2.one * 1.15f : new Vector2(.9f, 1.65f);
            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Visuals.Sprite(kind);
            renderer.color = Color.white;
            renderer.sortingOrder = 10;
            transform.localScale = Vector3.one * config.scale;
            AddAtmosphericDetails(side, kind);
            if (side == Faction.Player) input = playerInput ?? new UnityInputAdapter();
            if (side != Faction.Ally && kind != "battery")
            {
                turret = new GameObject("AimPivot").transform;
                turret.SetParent(transform, false);
                var barrel = new GameObject("Cannon");
                barrel.transform.SetParent(turret, false);
                barrel.transform.localPosition = new Vector3(0, .55f, 0);
                barrel.transform.localScale = new Vector3(.065f, .45f, 1);
                var barrelRenderer = barrel.AddComponent<SpriteRenderer>();
                barrelRenderer.sprite = Visuals.White;
                barrelRenderer.color = side == Faction.Enemy ? new Color(.35f, .15f, .15f) : new Color(.14f, .23f, .26f);
                barrelRenderer.sortingOrder = 12;
            }
            Health.Died += OnDeath;
        }

        private void AddAtmosphericDetails(Faction side, string kind)
        {
            var shadow = new GameObject("Sombra del casco");
            shadow.transform.SetParent(transform, false);
            shadow.transform.localPosition = new Vector3(.16f, -.18f, .08f);
            shadow.transform.localRotation = Quaternion.Euler(0, 0, -12);
            shadow.transform.localScale = kind == "battery"
                ? new Vector3(1.05f, .62f, 1)
                : new Vector3(.72f, 1.16f, 1);
            var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            shadowRenderer.sprite = Visuals.Sprite(kind);
            shadowRenderer.color = new Color(.015f, .045f, .055f, .22f);
            shadowRenderer.sortingOrder = 7;

            if (kind == "battery") return;
            var wake = new GameObject("Estela y espuma");
            wake.transform.SetParent(transform, false);
            wake.transform.localPosition = new Vector3(0, -.92f, .06f);
            wake.transform.localScale = new Vector3(.9f, .65f, 1);
            var wakeRenderer = wake.AddComponent<SpriteRenderer>();
            wakeRenderer.sprite = Visuals.Sprite("wake");
            wakeRenderer.color = side == Faction.Enemy
                ? new Color(.55f, .29f, .25f, .25f)
                : new Color(.58f, .78f, .72f, .3f);
            wakeRenderer.sortingOrder = 8;

            var wakeTip = new GameObject("Espuma de proa");
            wakeTip.transform.SetParent(transform, false);
            wakeTip.transform.localPosition = new Vector3(0, .74f, .05f);
            wakeTip.transform.localScale = new Vector3(.2f, .28f, 1);
            var tipRenderer = wakeTip.AddComponent<SpriteRenderer>();
            tipRenderer.sprite = Visuals.Sprite("wake");
            tipRenderer.color = new Color(.76f, .87f, .78f, .18f);
            tipRenderer.sortingOrder = 8;
        }

        private void Update()
        {
            if (!Alive || root.IsPaused || root.CurrentState != MissionState.Playing) return;
            if (input != null)
            {
                command = input.Read(root.GameCamera);
                Vector2 delta = command.Aim - (Vector2)transform.position;
                if (delta.sqrMagnitude > .001f) Aim(delta);
                if (command.Fire) Fire();
            }
        }

        private void FixedUpdate()
        {
            if (!Alive || root.IsPaused || root.CurrentState != MissionState.Playing) return;
            if (Faction == Faction.Ally)
            {
                var next = SweepMove(Vector2.right * (config.speed * Time.fixedDeltaTime));
                if (next.x >= 9f) root.Arrive(Id);
                return;
            }
            if (input != null)
            {
                RotateWithoutPushing(body.rotation + command.Turn * turnDegrees * Time.fixedDeltaTime);
                Move(command.Forward);
                return;
            }
            if (target == null || !target.Alive) return;
            Vector2 diff = target.transform.position - transform.position;
            float distance = diff.magnitude;
            if (distance > .001f)
            {
                Aim(diff);
                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90;
                RotateWithoutPushing(Mathf.MoveTowardsAngle(body.rotation, angle, turnDegrees * Time.fixedDeltaTime));
            }
            var decision = strategy.Decide(distance, config.range);
            Move(decision.Throttle);
            if (decision.Fire) Fire();
        }

        private void Move(float throttle)
        {
            var next = body.position + (Vector2)transform.up * (throttle * config.speed * Time.fixedDeltaTime);
            float margin = hitbox.size.magnitude * .5f * config.scale;
            next.x = Mathf.Clamp(next.x, GameRoot.WorldMinX+margin, GameRoot.WorldMaxX-margin);
            next.y = Mathf.Clamp(next.y, GameRoot.WorldMinY+margin, GameRoot.WorldMaxY-margin);
            SweepMove(next-body.position);
        }

        private Vector2 SweepMove(Vector2 delta)
        {
            float distance=delta.magnitude;
            if(distance<.00001f)return body.position;
            int count=body.Cast(delta/distance,solidFilter,movementHits,distance+.025f);
            float allowed=distance;
            for(int i=0;i<count;++i)
            {
                var hit=movementHits[i];
                if(hit.collider==null || hit.collider.attachedRigidbody==body)continue;
                allowed=Mathf.Min(allowed,Mathf.Max(0,hit.distance-.025f));
            }
            var next=body.position+delta/distance*allowed;
            body.MovePosition(next);
            return next;
        }

        private void RotateWithoutPushing(float angle)
        {
            if(Mathf.Abs(Mathf.DeltaAngle(body.rotation,angle))<.001f)return;
            if(config.speed>0)
            {
                int count=Physics2D.OverlapBox(body.position,hitbox.size*config.scale,angle,solidFilter,rotationHits);
                for(int i=0;i<count;++i)
                    if(rotationHits[i]!=null && rotationHits[i].attachedRigidbody!=body)return;
            }
            body.MoveRotation(angle);
        }

        private void Aim(Vector2 direction)
        {
            aimDirection = direction.normalized;
            if (turret != null)
                turret.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90);
        }

        private void Fire()
        {
            if (config.damage <= 0 || !weapon.TryFire(Time.timeAsDouble)) return;
            root.Pool.Spawn(this, (Vector2)transform.position + aimDirection * 1.15f,
                aimDirection, config.damage);
            root.Sounds.Fire();
        }

        public void Receive(int amount, string source)
        {
            if (!Alive) return;
            int before = Health.Current;
            IDamageable damageable = Health;
            damageable.TakeDamage(amount);
            root.Sounds.Hit();
            root.Record(new CombatEvent(before - Health.Current, Health.IsDead, source));
        }

        private void OnDeath()
        {
            hitbox.enabled = false;
            root.Sounds.Explosion();
            root.Destroyed(Id, Faction == Faction.Player);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Health != null) Health.Died -= OnDeath;
        }
    }
}
