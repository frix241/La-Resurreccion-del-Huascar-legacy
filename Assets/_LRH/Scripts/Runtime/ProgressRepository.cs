using System;
using System.IO;
using LRH.Core;
using UnityEngine;

namespace LRH.Runtime
{
    public interface IProgressRepository
    {
        Progress Load();
        bool Save(Progress progress);
    }

    public sealed class JsonProgressRepository : IProgressRepository
    {
        private readonly ProgressStore store;
        public JsonProgressRepository(string directory)
        {
            store = new ProgressStore(Path.Combine(directory, "lrh-progress.json"));
        }

        public Progress Load()
        {
            try
            {
                return store.Load();
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogWarning("No se pudo leer el progreso: " + e.Message);
            }
            return new Progress();
        }

        public bool Save(Progress progress)
        {
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            try
            {
                store.Save(progress);
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogWarning("No se pudo guardar el progreso: " + e.Message);
                return false;
            }
        }
    }
}
