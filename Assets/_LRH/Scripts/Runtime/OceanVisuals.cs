using UnityEngine;

namespace LRH.Runtime
{
    public static class OceanVisuals
    {
        private static Sprite oceanSprite;

        public static Sprite Ocean
        {
            get
            {
                if (oceanSprite == null)
                {
                    var texture = Resources.Load<Texture2D>("Retro/water0");
                    if (texture == null) throw new System.InvalidOperationException("Falta Retro/water0");
                    texture.filterMode = FilterMode.Bilinear;
                    oceanSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(.5f, .5f), texture.width / (1024f / 36f));
                }
                return oceanSprite;
            }
        }

        private static Sprite CreateOcean()
        {
            const int width = 1024;
            const int height = 576;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "LRH / Mar nocturno procedural",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color[width * height];
            var deep = new Color(.008f, .035f, .055f, 1);
            var shallow = new Color(.018f, .105f, .13f, 1);
            var foam = new Color(.26f, .49f, .48f, .23f);
            var moon = new Color(.72f, .71f, .56f, .08f);

            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                float nx = x / (float)(width - 1);
                float ny = y / (float)(height - 1);
                float gradient = Mathf.Clamp01(.16f + ny * .72f + nx * .08f);
                float swell = Mathf.Sin(nx * 18f + ny * 7f) * .035f +
                    Mathf.Sin(nx * 43f - ny * 11f) * .018f;
                var color = Color.Lerp(deep, shallow, Mathf.Clamp01(gradient + swell));

                float horizonGlow = Mathf.Exp(-Mathf.Pow((ny - .56f) * 5.2f, 2f));
                color += moon * horizonGlow;
                pixels[y * width + x] = color;
            }

            for (int band = 0; band < 34; ++band)
            {
                float y = .06f + band / 35f * .9f;
                float amplitude = .004f + (band % 4) * .0014f;
                float phase = band * 1.73f;
                float thickness = band % 5 == 0 ? 1.35f : .65f;
                DrawWave(pixels, width, height, y, amplitude, phase, thickness, foam);
            }

            for (int i = 0; i < 22; ++i)
            {
                float x = .04f + (i * .137f) % .92f;
                float y = .18f + ((i * .291f) % .68f);
                DrawGlow(pixels, width, height, new Vector2(x, y), .018f + (i % 3) * .006f, moon);
            }

            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f), 36f);
        }

        private static void DrawWave(Color[] pixels, int width, int height, float normalizedY,
            float amplitude, float phase, float thickness, Color color)
        {
            int steps = width;
            for (int x = 0; x < steps; ++x)
            {
                float nx = x / (float)(steps - 1);
                float y = normalizedY + Mathf.Sin(nx * 15f + phase) * amplitude +
                    Mathf.Sin(nx * 41f - phase) * amplitude * .32f;
                int pixelX = Mathf.Clamp(x, 0, width - 1);
                int pixelY = Mathf.Clamp(Mathf.RoundToInt(y * (height - 1)), 0, height - 1);
                int radius = Mathf.Max(0, Mathf.RoundToInt(thickness));
                for (int offset = -radius; offset <= radius; ++offset)
                {
                    int index = (pixelY + offset) * width + pixelX;
                    if (index >= 0 && index < pixels.Length)
                        pixels[index] = Color.Lerp(pixels[index], color, .44f);
                }
            }
        }

        private static void DrawGlow(Color[] pixels, int width, int height, Vector2 center,
            float radius, Color color)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt((center.x - radius) * width));
            int maxX = Mathf.Min(width - 1, Mathf.CeilToInt((center.x + radius) * width));
            int minY = Mathf.Max(0, Mathf.FloorToInt((center.y - radius) * height));
            int maxY = Mathf.Min(height - 1, Mathf.CeilToInt((center.y + radius) * height));
            for (int y = minY; y <= maxY; ++y)
            for (int x = minX; x <= maxX; ++x)
            {
                float distance = Vector2.Distance(new Vector2(x / (float)width, y / (float)height), center);
                if (distance > radius) continue;
                float alpha = 1f - distance / radius;
                int index = y * width + x;
                pixels[index] = Color.Lerp(pixels[index], color, alpha * .12f);
            }
        }
    }
}
