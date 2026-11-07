using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LRH.Core
{
    public sealed class Progress
    {
        private readonly HashSet<int> completed = new HashSet<int>();
        public int HighestUnlocked { get; private set; } = 1;
        public float MasterVolume { get; private set; } = .8f;
        public bool IsCompleted(int number) => completed.Contains(number);
        public bool CanPlay(int number) => number >= 1 && number <= HighestUnlocked;
        public void SetVolume(float value)
        {
            if (float.IsNaN(value) || value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(value));
            MasterVolume = value;
        }
        public void Complete(int number)
        {
            if (!CanPlay(number)) return;
            completed.Add(number);
            HighestUnlocked = Math.Min(3, Math.Max(HighestUnlocked, number + 1));
        }
        public string ToJson()
        {
            var ids = new List<int>(completed);
            ids.Sort();
            return "{\"schemaVersion\":1,\"highestUnlockedMission\":" + HighestUnlocked +
                   ",\"completedMissions\":[" + string.Join(",", ids) + "],\"masterVolume\":" +
                   MasterVolume.ToString("0.###", CultureInfo.InvariantCulture) + "}";
        }
        public static Progress FromJson(string json)
        {
            TryFromJson(json, out var progress);
            return progress;
        }

        public static bool TryFromJson(string json, out Progress progress)
        {
            var fresh = new Progress();
            progress = fresh;
            if (string.IsNullOrEmpty(json)) return false;
            // Only accept the small canonical save schema. Unknown/malformed saves fall back safely.
            var match = Regex.Match(json, @"^\s*\{\s*""schemaVersion""\s*:\s*1\s*,\s*""highestUnlockedMission""\s*:\s*([1-3])\s*,\s*""completedMissions""\s*:\s*\[\s*([0-9,\s]*)\]\s*,\s*""masterVolume""\s*:\s*(0(?:\.\d+)?|1(?:\.0+)?)\s*\}\s*$", RegexOptions.CultureInvariant);
            if (!match.Success) return false;
            int highest = int.Parse(match.Groups[1].Value);
            if (!float.TryParse(match.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                out float volume)) return false;
            fresh.SetVolume(volume);
            string list = match.Groups[2].Value.Trim();
            if (list.Length > 0)
            {
                foreach (var item in list.Split(','))
                {
                    if (!int.TryParse(item.Trim(), out int id) || id < 1 || id > 3 || id > highest || !fresh.completed.Add(id))
                        return false;
                }
            }
            int expected = fresh.completed.Contains(2) ? 3 : fresh.completed.Contains(1) ? 2 : 1;
            if (highest != expected || (fresh.completed.Contains(2) && !fresh.completed.Contains(1))) return false;
            fresh.HighestUnlocked = highest;
            progress = fresh;
            return true;
        }
    }
}
