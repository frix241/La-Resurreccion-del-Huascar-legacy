using System;
using UnityEngine;

namespace LRH.Runtime
{
    [Serializable]
    public sealed class TerrainProp
    {
        public string kind;
        public float x, y, scale = 1;
        public bool blocking;
    }

    [Serializable]
    public sealed class TerrainMap
    {
        public int width, height, originX, originY;
        public string[] rows;
        public TerrainProp[] props;
        private static TerrainMap pacific;
        private static Sprite chart;
        public static TerrainMap Pacific => pacific ?? (pacific = Load());

        public static void Reload()
        {
            pacific = null;
            if (chart != null)
            {
                if (Application.isPlaying) { UnityEngine.Object.Destroy(chart.texture); UnityEngine.Object.Destroy(chart); }
                else { UnityEngine.Object.DestroyImmediate(chart.texture); UnityEngine.Object.DestroyImmediate(chart); }
            }
            chart = null;
        }

        private static TerrainMap Load()
        {
            var resource = Resources.Load<TextAsset>("Maps/pacific");
            if (resource == null) throw new InvalidOperationException("Falta Maps/pacific.json");
            var map = JsonUtility.FromJson<TerrainMap>(resource.text);
            if (map.width <= 0 || map.height <= 0 || map.rows == null || map.rows.Length != map.height)
                throw new InvalidOperationException("Mapa de terreno inválido");
            foreach (var row in map.rows)
            {
                if (row == null || row.Length != map.width) throw new InvalidOperationException("Fila de mapa inválida");
                foreach (char cell in row) if ("~:.#".IndexOf(cell) < 0) throw new InvalidOperationException("Tipo de terreno inválido");
            }
            return map;
        }

        public char Cell(int x, int y) => x < 0 || y < 0 || x >= width || y >= height ? '~' : rows[y][x];
        public bool Solid(int x, int y) => Cell(x, y) == '.' || Cell(x, y) == '#';
        public bool SolidAt(Vector2 position) => Solid(Mathf.FloorToInt(position.x) - originX,
            Mathf.FloorToInt(position.y) - originY);

        // Cut only the exposed corners. The identical polygon feeds rendering and physics.
        public Vector2[] Polygon(int x, int y)
        {
            bool left = Solid(x - 1, y), right = Solid(x + 1, y);
            bool down = Solid(x, y - 1), up = Solid(x, y + 1);
            var vertices = new System.Collections.Generic.List<Vector2>();
            Corner(vertices, -.5f, -.5f, !left && !down);
            Corner(vertices, .5f, -.5f, !right && !down);
            Corner(vertices, .5f, .5f, !right && !up);
            Corner(vertices, -.5f, .5f, !left && !up);
            return vertices.ToArray();
        }
        private static void Corner(System.Collections.Generic.List<Vector2> points, float x, float y, bool cut)
        {
            if (!cut) { points.Add(new Vector2(x, y)); return; }
            const float inset = .28f;
            // Counterclockwise traversal.
            if (x * y > 0) { points.Add(new Vector2(x, y - Mathf.Sign(y) * inset)); points.Add(new Vector2(x - Mathf.Sign(x) * inset, y)); }
            else { points.Add(new Vector2(x - Mathf.Sign(x) * inset, y)); points.Add(new Vector2(x, y - Mathf.Sign(y) * inset)); }
        }

        public static Sprite Chart
        {
            get
            {
                if (chart != null) return chart;
                var map = Pacific;
                const int scale = 12;
                var texture = new Texture2D(map.width * scale, map.height * scale, TextureFormat.RGBA32, false);
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                var pixels = new Color[texture.width * texture.height];
                for (int y = 0; y < texture.height; ++y)
                for (int x = 0; x < texture.width; ++x)
                {
                    char cell = map.Cell(x / scale, y / scale);
                    pixels[y * texture.width + x] = CellColor(cell);
                }
                if (map.props != null) foreach (var prop in map.props)
                {
                    int px = Mathf.RoundToInt((prop.x - map.originX) * scale);
                    int py = Mathf.RoundToInt((prop.y - map.originY) * scale);
                    const int radius = 5;
                    for (int dy = -radius; dy <= radius; ++dy)
                    for (int dx = -radius; dx <= radius; ++dx)
                        if (dx * dx + dy * dy <= radius * radius && px + dx >= 0 && py + dy >= 0 &&
                            px + dx < texture.width && py + dy < texture.height)
                            pixels[(py + dy) * texture.width + px + dx] = prop.blocking ? new Color(.36f,.32f,.25f) : new Color(.36f,.46f,.29f);
                }
                texture.SetPixels(pixels);
                texture.Apply(false, true);
                chart = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f,.5f), 100,0,SpriteMeshType.FullRect,Vector4.zero,false);
                return chart;
            }
        }
        public static Color CellColor(char cell) => cell=='#'?new Color32(72,81,95,255):
            cell=='.'?new Color32(197,154,93,255):cell==':'?new Color32(53,109,112,255):new Color32(11,32,46,255);
    }
}
