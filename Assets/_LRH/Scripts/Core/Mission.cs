using System;
using System.Collections.Generic;

namespace LRH.Core
{
    public enum MissionState { Playing, Victory, Defeat }
    public enum MissionFailure { None, PlayerDestroyed, ObjectiveDestroyed }

    public interface IMissionObjective
    {
        bool Complete { get; }
        bool Failed { get; }
        void Destroy(string id);
        void Arrive(string id);
    }

    public sealed class EliminateObjective : IMissionObjective
    {
        private readonly string id;
        public bool Complete { get; private set; }
        public bool Failed => false;
        public EliminateObjective(string id) { this.id = id ?? throw new ArgumentNullException(nameof(id)); }
        public void Destroy(string target) { if (target == id) Complete = true; }
        public void Arrive(string target) { }
    }

    public sealed class EscortObjective : IMissionObjective
    {
        private readonly string id;
        public bool Complete { get; private set; }
        public bool Failed { get; private set; }
        public EscortObjective(string id) { this.id = id ?? throw new ArgumentNullException(nameof(id)); }
        public void Destroy(string target) { if (target == id) Failed = true; }
        public void Arrive(string target) { if (target == id) Complete = true; }
    }

    public sealed class EliminateManyObjective : IMissionObjective
    {
        private readonly HashSet<string> remaining;
        public bool Complete => remaining.Count == 0;
        public bool Failed => false;
        public EliminateManyObjective(params string[] ids)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentException("Provide targets", nameof(ids));
            remaining = new HashSet<string>(ids);
        }
        public void Destroy(string id) { remaining.Remove(id); }
        public void Arrive(string id) { }
    }

    public sealed class Mission
    {
        private readonly IMissionObjective objective;
        private bool playerDead;
        public int Number { get; }
        public MissionState State { get; private set; } = MissionState.Playing;
        public MissionFailure FailureReason { get; private set; } = MissionFailure.None;
        public event Action<MissionState> Finished;
        public Mission(int number, IMissionObjective objective)
        {
            if (number < 1 || number > 3) throw new ArgumentOutOfRangeException(nameof(number));
            Number = number;
            this.objective = objective ?? throw new ArgumentNullException(nameof(objective));
        }
        public void KillPlayer() { if (State == MissionState.Playing) playerDead = true; }
        public void Destroy(string id) { if (State == MissionState.Playing) objective.Destroy(id); }
        public void Arrive(string id) { if (State == MissionState.Playing) objective.Arrive(id); }
        // Resolve once after the physics frame: defeat wins simultaneous kills.
        public void Resolve()
        {
            if (State != MissionState.Playing) return;
            if (playerDead) { State = MissionState.Defeat; FailureReason = MissionFailure.PlayerDestroyed; }
            else if (objective.Failed) { State = MissionState.Defeat; FailureReason = MissionFailure.ObjectiveDestroyed; }
            else if (objective.Complete) State = MissionState.Victory;
            if (State != MissionState.Playing) Finished?.Invoke(State);
        }
    }
}
