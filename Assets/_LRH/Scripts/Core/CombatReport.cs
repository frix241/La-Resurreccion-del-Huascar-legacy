using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LRH.Core
{
    public readonly struct CombatEvent
    {
        public int Damage { get; }
        public bool Destroyed { get; }
        public string Source { get; }
        public CombatEvent(int damage, bool destroyed, string source = "")
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            Damage = damage; Destroyed = destroyed; Source = source ?? "";
        }
    }
    public readonly struct CombatReport
    {
        public int TotalDamage { get; }
        public int Destroyed { get; }
        public string TopSource { get; }
        public CombatReport(int totalDamage, int destroyed, string topSource)
        { TotalDamage = totalDamage; Destroyed = destroyed; TopSource = topSource; }
    }
    public static class CombatReporter
    {
        public static Task<CombatReport> AnalyzeAsync(CombatEvent[] events, CancellationToken token)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            var copy = (CombatEvent[])events.Clone(); // never share mutable input with the worker
            return Task.Run(() =>
            {
                int damage = 0, destroyed = 0;
                var hitCounts = new Dictionary<string, int>();
                string topSource = "";
                int topHits = 0;
                foreach (var entry in copy)
                {
                    token.ThrowIfCancellationRequested();
                    damage = checked(damage + entry.Damage);
                    if (entry.Destroyed) destroyed++;
                    if (!string.IsNullOrEmpty(entry.Source))
                    {
                        hitCounts.TryGetValue(entry.Source, out int hits);
                        hits++;
                        hitCounts[entry.Source] = hits;
                        if (hits > topHits || (hits == topHits &&
                            string.CompareOrdinal(entry.Source, topSource) < 0))
                        { topHits = hits; topSource = entry.Source; }
                    }
                }
                token.ThrowIfCancellationRequested();
                return new CombatReport(damage, destroyed, topSource);
            }, token);
        }
    }
}
