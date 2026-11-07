using System.Collections.Generic;
using LRH.Core;
using UnityEngine;

namespace LRH.Runtime
{
    public sealed class ProjectilePool : MonoBehaviour
    {
        private readonly Queue<Projectile> idle = new Queue<Projectile>();
        private int count;
        public SoundFx Sounds { get; set; }
        public void Spawn(ShipActor owner, Vector2 position, Vector2 direction, int amount)
        {
            if (idle.Count == 0 && count >= 80) return;
            Projectile projectile;
            if (idle.Count > 0) projectile = idle.Dequeue();
            else
            {
                var go = new GameObject("Projectile");
                go.transform.SetParent(transform, false);
                projectile = go.AddComponent<Projectile>();
                projectile.Setup(this);
                count++;
            }
            projectile.Launch(owner, position, direction, amount);
        }
        internal void Recycle(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            idle.Enqueue(projectile);
        }
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public sealed class Projectile : MonoBehaviour
    {
        private ProjectilePool pool;
        private Rigidbody2D body;
        private ShipActor owner;
        private Vector2 direction;
        private int amount;
        private float expires;
        private bool active;

        internal void Setup(ProjectilePool source)
        {
            pool = source;
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            GetComponent<CircleCollider2D>().isTrigger = true;
            GetComponent<CircleCollider2D>().radius = .15f;
            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Visuals.Sprite("projectile");
            renderer.sortingOrder = 20;
            transform.localScale = Vector3.one * .42f;
        }

        internal void Launch(ShipActor shooter, Vector2 at, Vector2 along, int damage)
        {
            owner = shooter;
            amount = damage;
            direction = along.normalized;
            transform.position = at;
            expires = Time.time + 3f;
            active = true;
            gameObject.SetActive(true);
            body.position = at;
        }

        private void FixedUpdate()
        {
            if (!active) return;
            if (Time.time >= expires) { pool.Sounds?.Splash(); Stop(); return; }
            body.MovePosition(body.position + direction * (11f * Time.fixedDeltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!active) return;
            var target = other.GetComponent<ShipActor>();
            if (target == owner) return;
            if (target != null)
            {
                if (!target.Alive || !DamageRules.CanHit(owner.Faction, target.Faction)) return;
                active = false; // idempotence, including multiple colliders during the same physics step
                target.Receive(amount, owner.Id);
                pool.Recycle(this);
            }
            else if (other.GetComponent<UnityEngine.Tilemaps.Tilemap>() == null && other.gameObject.CompareTag("Untagged")) Stop();
        }

        private void Stop()
        {
            if (!active) return;
            active = false;
            pool.Recycle(this);
        }
    }
}
