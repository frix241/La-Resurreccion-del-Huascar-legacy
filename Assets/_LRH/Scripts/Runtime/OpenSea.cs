using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LRH.Runtime
{
    // All terrain stays at its authored world coordinates. Camera movement never relocates it.
    public sealed class OpenSea : MonoBehaviour
    {
        public Tilemap Ground { get; private set; }
        public CompositeCollider2D LandCollider { get; private set; }
        private readonly List<Object> owned = new List<Object>();
        private readonly Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

        public void Initialize(Camera camera)
        {
            var map = TerrainMap.Pacific;
            CreateWater();
            CreateBoundaries(map);
            var grid = new GameObject("Terreno fijo / Grid");
            grid.transform.SetParent(transform, false);
            grid.transform.position = new Vector3(map.originX, map.originY, 0);
            grid.AddComponent<Grid>();
            var terrain = new GameObject("Celdas / agua, playa y roca");
            terrain.transform.SetParent(grid.transform, false);
            Ground = terrain.AddComponent<Tilemap>();
            terrain.AddComponent<TilemapRenderer>().enabled = false;
            CreateSurface(map);
            var body = terrain.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            var collider = terrain.AddComponent<TilemapCollider2D>();
            LandCollider = terrain.AddComponent<CompositeCollider2D>();
            LandCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
            collider.compositeOperation = Collider2D.CompositeOperation.Merge;
            var positions = new List<Vector3Int>();
            var placed = new List<TileBase>();
            for (int y = 0; y < map.height; ++y)
            for (int x = 0; x < map.width; ++x)
            {
                positions.Add(new Vector3Int(x, y, 0));
                placed.Add(GetTile(map, x, y));
            }
            Ground.SetTiles(positions.ToArray(), placed.ToArray());
            collider.ProcessTilemapChanges();
            if (map.props != null) foreach (var prop in map.props) CreateProp(prop);
        }

        private void CreateBoundaries(TerrainMap map)
        {
            float left=map.originX, right=left+map.width, bottom=map.originY, top=bottom+map.height;
            AddWall(new Vector2(left-1,(bottom+top)/2),new Vector2(2,map.height+4));
            AddWall(new Vector2(right+1,(bottom+top)/2),new Vector2(2,map.height+4));
            AddWall(new Vector2((left+right)/2,bottom-1),new Vector2(map.width+4,2));
            AddWall(new Vector2((left+right)/2,top+1),new Vector2(map.width+4,2));
            var line=new GameObject("Límite de operaciones / línea costera").AddComponent<LineRenderer>();
            line.transform.SetParent(transform,false);
            var material=new Material(Shader.Find("Sprites/Default")); owned.Add(material);
            line.sharedMaterial=material; line.useWorldSpace=true; line.loop=true; line.positionCount=4;
            line.SetPositions(new[]{new Vector3(left,bottom,0),new Vector3(right,bottom,0),new Vector3(right,top,0),new Vector3(left,top,0)});
            line.startWidth=line.endWidth=.12f;
            line.startColor=line.endColor=new Color(.76f,.57f,.29f,.8f); line.sortingOrder=4;
        }
        private void AddWall(Vector2 position,Vector2 size)
        {
            var wall=new GameObject("Límite físico de la región"); wall.transform.SetParent(transform,false);
            wall.transform.position=position; wall.AddComponent<BoxCollider2D>().size=size;
        }

        private void CreateWater()
        {
            var texture = Resources.Load<Texture2D>("Retro/water0");
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Mirror;
            var mesh = new Mesh { name = "Agua / UV fija al mundo" };
            mesh.vertices = new[] { new Vector3(-4096,-4096,0), new Vector3(4096,-4096,0), new Vector3(4096,4096,0), new Vector3(-4096,4096,0) };
            mesh.uv = new[] { Vector2.zero, new Vector2(8192,0), new Vector2(8192,8192), new Vector2(0,8192) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateBounds();
            var material = new Material(Shader.Find("Sprites/Default")) { mainTexture = texture };
            var water = new GameObject("Mar navegable / textura repetible fija");
            water.transform.SetParent(transform, false);
            water.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = water.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.sortingOrder = -100;
            owned.Add(mesh); owned.Add(material);
        }

        private void CreateSurface(TerrainMap map)
        {
            var weights = new Texture2D(map.width,map.height,TextureFormat.RGBA32,false,true);
            weights.filterMode = FilterMode.Bilinear;
            weights.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color[map.width*map.height];
            for (int y=0;y<map.height;++y)
            for (int x=0;x<map.width;++x)
            {
                char cell=map.Cell(x,y);
                pixels[y*map.width+x]=cell=='#' ? new Color(0,0,0,1) : cell=='.' ? new Color(0,0,1,0)
                    : cell==':' ? new Color(0,1,0,0) : new Color(1,0,0,0);
            }
            weights.SetPixels(pixels); weights.Apply(false,true);
            var material = new Material(Resources.Load<Shader>("Shaders/TerrainBlend"));
            material.SetTexture("_Splat",weights);
            string[] properties={"_Water","_Shallow","_Sand","_Stone"};
            string[] names={"water0","shallow0","sand0","stone0"};
            for(int i=0;i<names.Length;++i)
            {
                var texture=Resources.Load<Texture2D>("Retro/"+names[i]);
                texture.filterMode=FilterMode.Bilinear;
                texture.wrapMode=TextureWrapMode.Mirror;
                material.SetTexture(properties[i],texture);
            }
            // Keep each illustrated surface motif broad so mirrored repeats do not
            // create a small checker-like pattern across the whole coastline.
            material.SetVector("_Size",new Vector4(map.width/9f,map.height/9f,0,0));
            var mesh=new Mesh { name="Superficie / mezcla de las mismas celdas" };
            float left=map.originX, bottom=map.originY, right=left+map.width, top=bottom+map.height;
            mesh.vertices=new[]{new Vector3(left,bottom,0),new Vector3(right,bottom,0),new Vector3(right,top,0),new Vector3(left,top,0)};
            mesh.uv=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up};
            mesh.triangles=new[]{0,1,2,0,2,3}; mesh.RecalculateBounds();
            var surface=new GameObject("Materiales mezclados / costa suave");
            surface.transform.SetParent(transform,false);
            surface.AddComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=surface.AddComponent<MeshRenderer>(); renderer.sharedMaterial=material; renderer.sortingOrder=-50;
            owned.Add(weights); owned.Add(material); owned.Add(mesh);
        }

        private Tile GetTile(TerrainMap map, int x, int y)
        {
            char cell = map.Cell(x, y);
            bool solid = map.Solid(x, y);
            int mask = (map.Solid(x-1,y) ? 1 : 0) | (map.Solid(x+1,y) ? 2 : 0) |
                (map.Solid(x,y-1) ? 4 : 0) | (map.Solid(x,y+1) ? 8 : 0);
            int variant = (x * 17 + y * 31) % 2;
            string key = cell + "/" + (solid ? mask : 0) + "/" + variant;
            if (tiles.TryGetValue(key, out var cached)) return cached;
            string name = cell == '#' ? "stone" : cell == '.' ? "sand" : cell == ':' ? "shallow" : "water";
            var texture = Resources.Load<Texture2D>("Retro/" + name + variant);
            if (texture == null) throw new System.InvalidOperationException("Falta textura Terrain/" + name + variant);
            texture.filterMode = FilterMode.Bilinear;
            var sprite = Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(.5f,.5f), texture.width,0,SpriteMeshType.FullRect,Vector4.zero,false);
            if (solid)
            {
                var polygon = map.Polygon(x,y);
                var triangles = new ushort[(polygon.Length-2)*3];
                for (int i = 0; i < polygon.Length-2; ++i)
                { triangles[i*3] = 0; triangles[i*3+1] = (ushort)(i+1); triangles[i*3+2] = (ushort)(i+2); }
                var pixelVertices = new Vector2[polygon.Length];
                for (int i = 0; i < polygon.Length; ++i) pixelVertices[i] = (polygon[i] + Vector2.one * .5f) * texture.width;
                sprite.OverrideGeometry(pixelVertices, triangles);
                sprite.OverridePhysicsShape(new List<Vector2[]> { pixelVertices });
            }
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = name + " / " + key;
            tile.sprite = sprite;
            tile.colliderType = solid ? Tile.ColliderType.Sprite : Tile.ColliderType.None;
            owned.Add(sprite); owned.Add(tile); tiles.Add(key, tile);
            return tile;
        }

        private void CreateProp(TerrainProp prop)
        {
            var texture = Resources.Load<Texture2D>("Retro/" + prop.kind);
            if (texture == null) throw new System.InvalidOperationException("Falta objeto Terrain/" + prop.kind);
            texture.filterMode = FilterMode.Bilinear;
            var sprite = Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(.5f,.5f), texture.width,0,SpriteMeshType.FullRect,Vector4.zero,false);
            owned.Add(sprite);
            var go = new GameObject("Objeto fijo / " + prop.kind);
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(prop.x,prop.y,0);
            go.transform.localScale = Vector3.one * prop.scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 3;
            if (!prop.blocking) return;
            if (prop.kind == "pier" || prop.kind == "ruins" || prop.kind == "crates")
                go.AddComponent<BoxCollider2D>().size = prop.kind == "pier" ? new Vector2(.3f,.8f) : new Vector2(.65f,.55f);
            else go.AddComponent<CircleCollider2D>().radius = .35f;
        }

        private void OnDestroy()
        {
            foreach (var item in owned) if (item != null)
            { if (Application.isPlaying) Destroy(item); else DestroyImmediate(item); }
        }
    }
}
