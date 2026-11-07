using LRH.Core;

var cases = new (string name, Action test)[]
{
    ("T01 damage decreases health", () => { var h = new Health(100); h.TakeDamage(25); Check(h.Current == 75); }),
    ("T02 death fires once and clamps", () => { var h = new Health(50); int deaths = 0; h.Died += () => deaths++; h.TakeDamage(90); h.TakeDamage(10); Check(h.Current == 0 && deaths == 1); }),
    ("T03 invalid maximum rejected", () => Throws<ArgumentOutOfRangeException>(() => new Health(0))),
    ("T04 negative damage rejected", () => { var h = new Health(50); Throws<ArgumentOutOfRangeException>(() => h.TakeDamage(-1)); Check(h.Current == 50); }),
    ("T05 patrol chases outside range", () => { var d = new ChaseStrategy().Decide(12, 5); Check(d.Throttle > 0 && !d.Fire); }),
    ("T06 patrol fires inside range", () => { var d = new ChaseStrategy().Decide(3, 5); Check(d.Throttle == 0 && d.Fire); }),
    ("T07 gunboat retreats if too close", () => Check(new KeepDistanceStrategy().Decide(1, 5).Throttle < 0)),
    ("T08 friendly fire excluded", () => Check(!DamageRules.CanHit(Faction.Player, Faction.Ally) && DamageRules.CanHit(Faction.Enemy, Faction.Ally))),
    ("T09 weapon cooldown", () => { var w = new WeaponClock(.8); Check(w.TryFire(0) && !w.TryFire(.5) && w.TryFire(.8)); }),
    ("T10 eliminate objective completes once", () => { var m = new Mission(1, new EliminateObjective("patrol")); m.Destroy("patrol"); m.Resolve(); Check(m.State == MissionState.Victory); m.KillPlayer(); Check(m.State == MissionState.Victory); }),
    ("T11 player death takes precedence in same tick", () => { var m = new Mission(1, new EliminateObjective("patrol")); m.Destroy("patrol"); m.KillPlayer(); m.Resolve(); Check(m.State == MissionState.Defeat); }),
    ("T12 convoy arrives alive", () => { var m = new Mission(2, new EscortObjective("convoy")); m.Arrive("convoy"); m.Resolve(); Check(m.State == MissionState.Victory); }),
    ("T13 destroyed convoy loses", () => { var m = new Mission(2, new EscortObjective("convoy")); m.Arrive("convoy"); m.Destroy("convoy"); m.Resolve(); Check(m.State == MissionState.Defeat); }),
    ("T14 all batteries must die", () => { var m = new Mission(3, new EliminateManyObjective("battery1", "battery2")); m.Destroy("battery1"); m.Resolve(); Check(m.State == MissionState.Playing); m.Destroy("battery2"); m.Resolve(); Check(m.State == MissionState.Victory); }),
    ("T15 progress unlocks in order", () => { var p = new Progress(); p.Complete(2); Check(p.HighestUnlocked == 1); p.Complete(1); Check(p.HighestUnlocked == 2); p.Complete(2); Check(p.HighestUnlocked == 3); }),
    ("T16 progress roundtrip", () => { var p = new Progress(); p.Complete(1); var q = Progress.FromJson(p.ToJson()); Check(q.HighestUnlocked == 2 && q.IsCompleted(1)); }),
    ("T17 invalid progress falls back", () => Check(Progress.FromJson("{not json}").HighestUnlocked == 1)),
    ("T18 async report uses snapshot", () => { var source = new[] { new CombatEvent(25, true), new CombatEvent(10, false) }; var task = CombatReporter.AnalyzeAsync(source, CancellationToken.None); source[0] = new CombatEvent(999, false); var report = task.GetAwaiter().GetResult(); Check(report.TotalDamage == 35 && report.Destroyed == 1); }),
    ("T19 cancelled report", () => { using var cts = new CancellationTokenSource(); cts.Cancel(); Throws<OperationCanceledException>(() => CombatReporter.AnalyzeAsync(new[] { new CombatEvent(1, false) }, cts.Token).GetAwaiter().GetResult()); }),
    ("T20 registry rejects duplicate IDs", () => { var r = new Registry<Health>(); Check(r.Register("player", new Health(10))); Check(!r.Register("player", new Health(20))); Check(r.TryGet("player", out var h) && h.Max == 10); }),
    ("T21 no progress file starts fresh", () => WithStore(store => Check(store.Load().HighestUnlocked == 1))),
    ("T22 corrupt save is preserved", () => WithStore(store => { File.WriteAllText(store.Path, "{broken}"); Check(store.Load().HighestUnlocked == 1); Check(File.ReadAllText(store.Path + ".corrupt") == "{broken}"); })),
    ("T23 save survives restart", () => WithStore(store => { var p = new Progress(); p.Complete(1); store.Save(p); Check(new ProgressStore(store.Path).Load().HighestUnlocked == 2); })),
    ("T24 overwriting save preserves backup", () => WithStore(store => { var p = new Progress(); store.Save(p); string first = File.ReadAllText(store.Path); p.Complete(1); store.Save(p); Check(File.ReadAllText(store.Path + ".bak") == first && store.Load().HighestUnlocked == 2); })),
    ("T25 volume persists across sessions", () => { var p = new Progress(); p.SetVolume(.35f); Check(Math.Abs(Progress.FromJson(p.ToJson()).MasterVolume - .35f) < .001); }),
    ("T26 volume outside range is rejected", () => { var p = new Progress(); Throws<ArgumentOutOfRangeException>(() => p.SetVolume(1.1f)); Check(Math.Abs(p.MasterVolume - .8f) < .001); }),
    ("T27 combat report identifies most active source", () => { var rows = new[] { new CombatEvent(25, false, "Huáscar"), new CombatEvent(10, false, "patrol"), new CombatEvent(25, true, "Huáscar") }; var report = CombatReporter.AnalyzeAsync(rows, CancellationToken.None).GetAwaiter().GetResult(); Check(report.TotalDamage == 60 && report.Destroyed == 1 && report.TopSource == "Huáscar"); }),
    ("T28 negative combat event is rejected", () => Throws<ArgumentOutOfRangeException>(() => new CombatEvent(-1, false, "patrol"))),
    ("T29 default event snapshot is safe", () => { var report = CombatReporter.AnalyzeAsync(new CombatEvent[1], CancellationToken.None).GetAwaiter().GetResult(); Check(report.TotalDamage == 0 && report.TopSource == ""); }),
    ("T30 formatted valid save keeps unlocked mission", () => WithStore(store => { File.WriteAllText(store.Path, "{ \"schemaVersion\": 1, \"highestUnlockedMission\": 2, \"completedMissions\": [1], \"masterVolume\": 0.8 }"); Check(store.Load().HighestUnlocked == 2 && !File.Exists(store.Path + ".corrupt")); })),
    ("T31 mission tells convoy loss from player loss", () => { var m = new Mission(2, new EscortObjective("convoy")); m.Destroy("convoy"); m.Resolve(); Check(m.FailureReason == MissionFailure.ObjectiveDestroyed); }),
    ("T32 simultaneous convoy and player loss prioritizes player", () => { var m = new Mission(2, new EscortObjective("convoy")); m.Destroy("convoy"); m.KillPlayer(); m.Resolve(); Check(m.FailureReason == MissionFailure.PlayerDestroyed); }),
    ("T33 damage contract dispatches to health", () => { IDamageable target = new Health(20); target.TakeDamage(7); Check(((Health)target).Current == 13); }),
};

int failures = 0;
foreach (var (name, test) in cases)
{
    try { test(); Console.WriteLine($"PASS {name}"); }
    catch (Exception e) { failures++; Console.WriteLine($"FAIL {name}: {e}"); }
}
Console.WriteLine($"{cases.Length - failures}/{cases.Length} passed");
return failures == 0 ? 0 : 1;

static void Check(bool condition) { if (!condition) throw new Exception("Assertion failed"); }
static void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}
static void WithStore(Action<ProgressStore> action)
{
    string dir = Path.Combine(Path.GetTempPath(), "opencode", "lrh-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(dir);
    try { action(new ProgressStore(Path.Combine(dir, "progress.json"))); }
    finally { Directory.Delete(dir, true); }
}
