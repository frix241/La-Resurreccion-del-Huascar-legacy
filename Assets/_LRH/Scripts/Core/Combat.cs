using System;

namespace LRH.Core
{
    public enum Faction { Player, Enemy, Ally, Neutral }

    public static class DamageRules
    {
        public static bool CanHit(Faction source, Faction target) =>
            source != target && source != Faction.Neutral && target != Faction.Neutral &&
            ((source == Faction.Enemy) != (target == Faction.Enemy));
    }

    public readonly struct CombatDecision
    {
        public float Throttle { get; }
        public bool Fire { get; }
        public CombatDecision(float throttle, bool fire) { Throttle = throttle; Fire = fire; }
    }

    public interface IAttackStrategy
    {
        CombatDecision Decide(float distance, float range);
    }

    public sealed class ChaseStrategy : IAttackStrategy
    {
        public CombatDecision Decide(float distance, float range) =>
            new CombatDecision(distance > range ? 1 : 0, distance <= range);
    }

    public sealed class KeepDistanceStrategy : IAttackStrategy
    {
        public CombatDecision Decide(float distance, float range) =>
            new CombatDecision(distance > range ? 1 : distance < range * .7f ? -1 : 0, distance <= range);
    }

    public sealed class WeaponClock
    {
        private readonly double cooldown;
        private double nextShot = double.NegativeInfinity;
        public WeaponClock(double cooldownSeconds)
        {
            if (cooldownSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(cooldownSeconds));
            cooldown = cooldownSeconds;
        }
        public bool TryFire(double now)
        {
            if (now < nextShot) return false;
            nextShot = now + cooldown;
            return true;
        }
    }
}
