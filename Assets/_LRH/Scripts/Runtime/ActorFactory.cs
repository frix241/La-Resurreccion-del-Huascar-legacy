using System;
using LRH.Core;
using UnityEngine;

namespace LRH.Runtime
{
    [Serializable]
    public sealed class ShipConfig
    {
        public int health;
        public int damage;
        public float speed;
        public float cooldown;
        public float range;
        public float scale = 1;

        public static ShipConfig Load(string name)
        {
            var asset = Resources.Load<TextAsset>("Configs/" + name);
            if (asset == null) throw new InvalidOperationException("Falta Resources/Configs/" + name + ".json");
            var config = JsonUtility.FromJson<ShipConfig>(asset.text);
            if (config == null || config.health <= 0 || config.cooldown <= 0 || config.range <= 0)
                throw new InvalidOperationException("Config inválida: " + name);
            return config;
        }
    }

    // Factory Method: the creator's method is overridden, not a switch disguised as GoF.
    public abstract class EnemyFactory : MonoBehaviour
    {
        public abstract ShipActor Create(GameRoot root, string id, Vector2 position, ShipActor target);
    }

    public sealed class PatrolFactory : EnemyFactory
    {
        public override ShipActor Create(GameRoot root, string id, Vector2 position, ShipActor target) =>
            ShipActor.Spawn(root, id, "patrol", Faction.Enemy, position, target, new ChaseStrategy());
    }

    public sealed class GunboatFactory : EnemyFactory
    {
        public override ShipActor Create(GameRoot root, string id, Vector2 position, ShipActor target) =>
            ShipActor.Spawn(root, id, "gunboat", Faction.Enemy, position, target, new KeepDistanceStrategy());
    }
}
