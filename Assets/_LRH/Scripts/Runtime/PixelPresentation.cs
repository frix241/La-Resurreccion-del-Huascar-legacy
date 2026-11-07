using UnityEngine;

namespace LRH.Runtime
{
    // Full HD output with illustrated world assets rendered at native screen resolution.
    [RequireComponent(typeof(Camera))]
    public sealed class PixelPresentation : MonoBehaviour
    {
        public const int ArtWidth=1920, ArtHeight=1080;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FullHd()
        {
            if(!Application.isEditor)Screen.SetResolution(1920,1080,FullScreenMode.FullScreenWindow);
        }
        private void Awake()
        {
            var camera=GetComponent<Camera>();camera.allowMSAA=true;camera.allowHDR=false;
        }
        private void OnRenderImage(RenderTexture source,RenderTexture destination)
        {
            Graphics.Blit(source,destination);
        }
    }
}
