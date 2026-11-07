using System;
using System.Threading;
using LRH.Core;
using NUnit.Framework;

namespace LRH.Tests
{
    public sealed class CoreTests
    {
        [Test] public void Damage_25_ReducesHealthTo75()
        { var health = new Health(100); health.TakeDamage(25); Assert.AreEqual(75, health.Current); }

        [Test] public void Overkill_DeathIsNotifiedOnlyOnce()
        { var health = new Health(50); int calls = 0; health.Died += () => calls++; health.TakeDamage(90); health.TakeDamage(1); Assert.AreEqual(0, health.Current); Assert.AreEqual(1, calls); }

        [Test] public void InvalidMaximum_IsRejected()
        { Assert.Throws<ArgumentOutOfRangeException>(() => new Health(0)); }

        [Test] public void NegativeDamage_LeavesHealthUnchanged()
        { var health = new Health(50); Assert.Throws<ArgumentOutOfRangeException>(() => health.TakeDamage(-1)); Assert.AreEqual(50, health.Current); }

        [Test] public void Patrol_OutsideRange_ChasesWithoutFiring()
        { var result = new ChaseStrategy().Decide(12, 5); Assert.AreEqual(1f, result.Throttle); Assert.IsFalse(result.Fire); }

        [Test] public void Patrol_InsideRange_StopsAndFires()
        { var result = new ChaseStrategy().Decide(3, 5); Assert.AreEqual(0f, result.Throttle); Assert.IsTrue(result.Fire); }

        [Test] public void SimultaneousDestruction_PrioritizesPlayerDefeat()
        { var mission = new Mission(1, new EliminateObjective("patrol")); mission.Destroy("patrol"); mission.KillPlayer(); mission.Resolve(); Assert.AreEqual(MissionState.Defeat, mission.State); }

        [Test] public void SaveRoundtrip_UnlocksNextMission()
        { var progress = new Progress(); progress.Complete(1); Assert.AreEqual(2, Progress.FromJson(progress.ToJson()).HighestUnlocked); }

        [Test] public void InvalidSave_ResetsToFirstMission()
        { Assert.AreEqual(1, Progress.FromJson("{not json}").HighestUnlocked); }

        [Test] public void Cancellation_StopsBackgroundReport()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                // Block synchronously on this already-cancelled task: Assert.ThrowsAsync in a
                // Unity EditMode void test deadlocks the batchmode runner.
                var task = CombatReporter.AnalyzeAsync(new[] { new CombatEvent(1, false) }, cts.Token);
                // Catch (not Throws) accepts the TaskCanceledException derived type.
                Assert.Catch<OperationCanceledException>(() => task.GetAwaiter().GetResult());
            }
        }
    }
}
