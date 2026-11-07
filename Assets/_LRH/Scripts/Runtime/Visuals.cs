using System.Collections.Generic;
using UnityEngine;

namespace LRH.Runtime
{
    public static class Visuals
    {
        private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, float> PixelsPerUnit = new Dictionary<string, float>
        {
            { "huascar", 96f },
            { "patrol", 96f },
            { "gunboat", 96f },
            { "convoy", 96f },
            { "battery", 160f },
            { "island", 80f },
            { "projectile", 240f },
            { "wake", 192f }
        };

        public static Sprite Sprite(string name)
        {
            if (sprites.TryGetValue(name, out var sprite)) return sprite;
            var texture = Resources.Load<Texture2D>("Retro/"+name);
            if (texture == null) throw new System.InvalidOperationException("Falta sprite: " + name);
            texture.filterMode = FilterMode.Bilinear;
            float pixelsPerUnit = PixelsPerUnit.TryGetValue(name, out var configured)
                ? configured
                : 100f;
            sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f), pixelsPerUnit, 0, SpriteMeshType.FullRect, Vector4.zero, false);
            sprites.Add(name, sprite);
            return sprite;
        }
        public static Sprite White => SpriteFromWhite();
        private static Sprite white;
        private static Sprite SpriteFromWhite()
        {
            if (white == null) white = UnityEngine.Sprite.Create(Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1), new Vector2(.5f, .5f), 1f);
            return white;
        }
    }
}
