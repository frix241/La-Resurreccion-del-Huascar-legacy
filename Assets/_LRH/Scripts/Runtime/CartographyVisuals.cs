using System.Collections.Generic;
using UnityEngine;

namespace LRH.Runtime
{
    public static class CartographyVisuals
    {
        private static Sprite chartSprite;

        public static Sprite PacificChart
        {
            get
            {
                if (chartSprite == null) chartSprite = CreateChart();
                return chartSprite;
            }
        }

        private static Sprite CreateChart()
        {
            const int width = 320;
            const int height = 180;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "LRH / Carta estilizada del litoral peruano",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[width * height];
            var sea = new Color(.025f, .095f, .12f, 1);
            var seaLight = new Color(.07f, .22f, .23f, 1);
            var seaDark = new Color(.012f, .055f, .08f, 1);
            for (int i = 0; i < pixels.Length; ++i) pixels[i] = sea;
            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                float nx = x / (float)(width - 1);
                float ny = y / (float)(height - 1);
                float basin = Mathf.Sin(nx * 9f + ny * 3f) * .025f + Mathf.Sin(nx * 27f - ny * 12f) * .012f;
                pixels[y * width + x] = Color.Lerp(seaDark, sea, Mathf.Clamp01(.55f + basin));
                int wave = (x * 17 + y * 31 + (x / 7) * 13) % 29;
                if (wave < 3) pixels[y * width + x] = Color.Lerp(pixels[y * width + x], seaLight, .45f);
            }

            var coast = new[]
            {
                new Vector2(.76f, .02f), new Vector2(.82f, .13f), new Vector2(.78f, .25f),
                new Vector2(.86f, .37f), new Vector2(.80f, .49f), new Vector2(.91f, .62f),
                new Vector2(.86f, .75f), new Vector2(.96f, .88f), new Vector2(1f, 1f),
                new Vector2(1f, 0f), new Vector2(.76f, 0f)
            };
            FillPolygon(pixels, width, height, coast, new Color(.72f, .66f, .48f, 1));
            DrawLine(pixels, width, height, new Vector2(.76f, .03f), new Vector2(.76f, .98f), new Color(.86f, .76f, .52f, .45f));
            DrawLine(pixels, width, height, new Vector2(.76f, .03f), new Vector2(.82f, .13f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.82f, .13f), new Vector2(.78f, .25f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.78f, .25f), new Vector2(.86f, .37f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.86f, .37f), new Vector2(.80f, .49f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.80f, .49f), new Vector2(.91f, .62f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.91f, .62f), new Vector2(.86f, .75f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.86f, .75f), new Vector2(.96f, .88f), new Color(.04f, .13f, .16f, 1));
            DrawLine(pixels, width, height, new Vector2(.11f, .18f), new Vector2(.62f, .74f), new Color(.27f, .48f, .45f, .7f));
            DrawLine(pixels, width, height, new Vector2(.04f, .69f), new Vector2(.52f, .12f), new Color(.27f, .48f, .45f, .7f));
            DrawRoute(pixels, width, height, new Vector2(.12f, .24f), new Vector2(.68f, .78f), new Color(.91f, .67f, .31f, .9f));
            DrawRoute(pixels, width, height, new Vector2(.10f, .72f), new Vector2(.62f, .2f), new Color(.72f, .37f, .28f, .7f));
            DrawGrid(pixels, width, height, new Color(.62f, .74f, .65f, .18f));
            DrawCompass(pixels, width, height, new Vector2(.14f, .82f));
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f), 100f);
        }

        private static void FillPolygon(Color[] pixels, int width, int height, IReadOnlyList<Vector2> polygon, Color color)
        {
            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                var point = new Vector2((x + .5f) / width, (y + .5f) / height);
                bool inside = false;
                for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
                {
                    bool crosses = polygon[i].y > point.y != polygon[j].y > point.y;
                    if (crosses && point.x < (polygon[j].x - polygon[i].x) *
                        (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
                        inside = !inside;
                }
                if (inside) pixels[y * width + x] = color;
            }
        }

        private static void DrawLine(Color[] pixels, int width, int height, Vector2 from, Vector2 to, Color color)
        {
            int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to) * width * 1.5f));
            for (int i = 0; i <= steps; ++i)
            {
                var point = Vector2.Lerp(from, to, i / (float)steps);
                int x = Mathf.Clamp(Mathf.RoundToInt(point.x * (width - 1)), 0, width - 1);
                int y = Mathf.Clamp(Mathf.RoundToInt(point.y * (height - 1)), 0, height - 1);
                pixels[y * width + x] = color;
            }
        }

        private static void DrawCompass(Color[] pixels, int width, int height, Vector2 center)
        {
            DrawLine(pixels, width, height, center + new Vector2(-.045f, 0), center + new Vector2(.045f, 0), new Color(.88f, .78f, .54f, 1));
            DrawLine(pixels, width, height, center + new Vector2(0, -.07f), center + new Vector2(0, .07f), new Color(.88f, .78f, .54f, 1));
            DrawLine(pixels, width, height, center, center + new Vector2(.03f, .06f), new Color(.95f, .55f, .36f, 1));
        }

        private static void DrawRoute(Color[] pixels, int width, int height, Vector2 from, Vector2 to, Color color)
        {
            int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to) * width));
            for (int i = 0; i <= steps; ++i)
            {
                if ((i / 5) % 2 == 1) continue;
                var point = Vector2.Lerp(from, to, i / (float)steps);
                int x = Mathf.Clamp(Mathf.RoundToInt(point.x * (width - 1)), 0, width - 1);
                int y = Mathf.Clamp(Mathf.RoundToInt(point.y * (height - 1)), 0, height - 1);
                pixels[y * width + x] = color;
                if (x + 1 < width) pixels[y * width + x + 1] = color;
            }
        }

        private static void DrawGrid(Color[] pixels, int width, int height, Color color)
        {
            for (int x = width / 8; x < width; x += width / 8)
                DrawLine(pixels, width, height, new Vector2(x / (float)width, .03f), new Vector2(x / (float)width, .97f), color);
            for (int y = height / 6; y < height; y += height / 6)
                DrawLine(pixels, width, height, new Vector2(.03f, y / (float)height), new Vector2(.97f, y / (float)height), color);
        }
    }
}
