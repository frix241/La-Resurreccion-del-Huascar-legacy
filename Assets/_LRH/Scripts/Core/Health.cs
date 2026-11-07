using System;

namespace LRH.Core
{
    public interface IDamageable { void TakeDamage(int amount); }

    public sealed class Health : IDamageable
    {
        public int Max { get; }
        public int Current { get; private set; }
        public bool IsDead => Current == 0;
        public event Action<int, int> Changed;
        public event Action Died;

        public Health(int max)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));
            Max = Current = max;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount == 0 || IsDead) return;
            Current = Math.Max(0, Current - amount);
            Changed?.Invoke(Current, Max);
            if (IsDead) Died?.Invoke();
        }
    }
}
