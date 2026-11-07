using UnityEngine;

namespace LRH.Runtime
{
    public static class UiVisuals
    {
        private static Sprite backdrop;
        private static Sprite surface;
        private static readonly System.Collections.Generic.Dictionary<string, Sprite> assets =
            new System.Collections.Generic.Dictionary<string, Sprite>();

        public static Sprite Backdrop
        {
            get
            {
                if (backdrop == null) backdrop = CreateBackdrop();
                return backdrop;
            }
        }

        public static Sprite Surface
        {
            get
            {
                if (surface == null) surface = CreateSurface();
                return surface;
            }
        }

        public static Sprite Asset(string resourcePath, Vector4 border = default(Vector4))
        {
            resourcePath=resourcePath.Replace("Artstyle/","Retro/");
            if (resourcePath == "UI/Panels/tactical_map") return TerrainMap.Chart;
            if (assets.TryGetValue(resourcePath, out var cached)) return cached;
            string pixelPath = null;
            if (resourcePath == "UI/Brand/menu_background") pixelPath = "menu";
            else if (resourcePath == "UI/Brand/logo") pixelPath = "logo";
            else if (resourcePath == "UI/Panels/tactical_map") pixelPath = "map";
            else if (resourcePath.StartsWith("UI/Buttons/"))
            {
                pixelPath = resourcePath.Substring(resourcePath.LastIndexOf('_') + 1);
                border = new Vector4(24, 24, 24, 24);
            }
            else if (resourcePath == "UI/Panels/panel_base") pixelPath = "panel";
            else if (resourcePath == "UI/Hud/top_hud") pixelPath = "hud";
            if (pixelPath == "panel" || pixelPath == "hud") border = new Vector4(24, 24, 24, 24);
            resourcePath=resourcePath.Replace("Artstyle/","Retro/");
            string texturePath=resourcePath=="UI/Brand/menu_background" ? "Retro/menu-background" :
                pixelPath=="panel" ? "Retro/panel" : pixelPath=="hud" ? "Retro/header" :
                resourcePath.StartsWith("UI/Buttons/") ? "Retro/"+pixelPath :
                pixelPath == null ? resourcePath : "Pixel/" + (pixelPath == "logo" ? pixelPath : pixelPath + "-v2");
            var texture = Resources.Load<Texture2D>(texturePath);
            if (texture != null) texture.filterMode = FilterMode.Bilinear;
            if (texture == null) throw new System.InvalidOperationException("Falta recurso UI: " + resourcePath);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect, border, false);
            assets.Add(resourcePath, sprite);
            return sprite;
        }

        private static Sprite CreateBackdrop()
        {
            const int width = 512;
            const int height = 288;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "LRH / fondo UI moderno",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[width * height];
            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                float nx = x / (float)(width - 1) - .5f;
                float ny = y / (float)(height - 1) - .5f;
                float vignette = Mathf.Clamp01(1f - Mathf.Sqrt(nx * nx + ny * ny) * 1.42f);
                float glow = Mathf.Exp(-Mathf.Pow((ny + .12f) * 4.2f, 2f));
                var deep = new Color(.008f, .016f, .03f, 1);
                var upper = new Color(.035f, .105f, .14f, 1);
                var color = Color.Lerp(deep, upper, Mathf.Clamp01(.18f + glow * .46f));
                color *= .62f + vignette * .38f;
                pixels[y * width + x] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f), 1f);
        }

        private static Sprite CreateSurface()
        {
            const int width = 256;
            const int height = 128;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "LRH / superficie UI redondeada",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[width * height];
            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                float edgeX = Mathf.Min(x, width - 1 - x);
                float edgeY = Mathf.Min(y, height - 1 - y);
                float corner = Mathf.Max(0, 16f - Mathf.Min(edgeX, edgeY));
                float alpha = Mathf.Clamp01(1f - corner / 16f);
                float gradient = y / (float)(height - 1);
                pixels[y * width + x] = new Color(.025f + gradient * .018f, .09f + gradient * .035f,
                    .12f + gradient * .04f, alpha * .96f);
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f), 100f,
                0, SpriteMeshType.FullRect, new Vector4(32, 32, 32, 32), false);
        }
    }
}
