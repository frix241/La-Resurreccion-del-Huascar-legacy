using System;
using System.IO;

namespace LRH.Core
{
    // File-based persistence is independent of Unity: test against a real temporary directory.
    public sealed class ProgressStore
    {
        public string Path { get; }
        public ProgressStore(string path) { Path = path ?? throw new ArgumentNullException(nameof(path)); }

        public Progress Load()
        {
            if (!File.Exists(Path)) return new Progress();
            string raw = File.ReadAllText(Path);
            if (Progress.TryFromJson(raw, out var progress)) return progress;
            File.Copy(Path, Path + ".corrupt", true);
            return new Progress();
        }

        public void Save(Progress progress)
        {
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            string pending = Path + ".tmp";
            File.WriteAllText(pending, progress.ToJson());
            if (File.Exists(Path)) File.Replace(pending, Path, Path + ".bak");
            else File.Move(pending, Path);
        }
    }
}
