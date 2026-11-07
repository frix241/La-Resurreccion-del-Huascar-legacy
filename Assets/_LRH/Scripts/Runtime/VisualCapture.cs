using System;
using System.Collections;
using System.IO;
using UnityEngine;
using LRH.Core;

namespace LRH.Runtime
{
    // Opt-in visual smoke check. Normal player startup never creates this component.
    public sealed class VisualCapture : MonoBehaviour
    {
        private string directory;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, "--lrh-capture-dir");
            if (index < 0 || index + 1 >= args.Length) return;
            var capture = new GameObject("Visual capture").AddComponent<VisualCapture>();
            capture.directory = Path.GetFullPath(args[index + 1]);
        }

        private IEnumerator Start()
        {
            Directory.CreateDirectory(directory);
            yield return new WaitForSecondsRealtime(2);
            var root = FindFirstObjectByType<GameRoot>();
            if (root == null) { Debug.LogError("Visual capture: GameRoot missing"); yield break; }
            root.ReturnMenu();
            Destroy(root.gameObject);
            yield return null;
            var director = new GameObject("Visual capture director");
            director.SetActive(false);
            root = director.AddComponent<GameRoot>();
            root.ConfigureRepository(new CaptureRepository());
            director.SetActive(true);
            yield return null;
            yield return Capture("menu");
            FindFirstObjectByType<GameUi>().ShowCredits();
            yield return Capture("credits");
            root.ReturnMenu();
            root.PreviewMission(1);
            yield return Capture("briefing");
            root.StartMission(1);
            yield return new WaitForSecondsRealtime(.5f);
            root.Player.Receive(35,"capture");
            root.Pause();
            yield return Capture("pause");
            root.Resume();
            yield return Capture("combat");
            root.Player.transform.position = new Vector3(22,-5,0);
            yield return new WaitForSecondsRealtime(1);
            yield return Capture("terrain-coast");
            root.ReturnMenu();
            root.StartMission(2);
            yield return Capture("convoy-near");
            root.Player.transform.position = new Vector3(-28, -14, 0);
            yield return new WaitForSecondsRealtime(1);
            yield return Capture("convoy-direction");
            root.ReturnMenu();root.StartMission(3);
            root.Player.transform.position=new Vector3(27,15,0);
            yield return new WaitForSecondsRealtime(1);
            yield return Capture("batteries");
            root.ReturnMenu();root.StartMission(1);
            foreach(var actor in root.Arena.GetComponentsInChildren<ShipActor>())
                if(actor.Faction==Faction.Enemy)actor.Receive(1000,"capture");
            yield return new WaitForSecondsRealtime(.3f);
            yield return Capture("victory");
            root.ReturnMenu();root.StartMission(1);root.Player.Receive(1000,"capture");
            yield return new WaitForSecondsRealtime(.3f);
            yield return Capture("defeat");
            root.ReturnMenu();root.StartMission(2);root.Convoy.Receive(1000,"capture");
            yield return new WaitForSecondsRealtime(.3f);
            yield return Capture("defeat-convoy");
            Application.Quit();
        }

        private sealed class CaptureRepository : IProgressRepository
        {
            public Progress Load() { var progress = new Progress(); progress.Complete(1); progress.Complete(2); return progress; }
            public bool Save(Progress progress) => true;
        }

        private IEnumerator Capture(string name)
        {
            yield return new WaitForSecondsRealtime(.15f);
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(directory, name + ".png"));
            yield return new WaitForSecondsRealtime(.4f);
        }
    }
}
