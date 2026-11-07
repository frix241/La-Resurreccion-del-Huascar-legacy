using System.IO;
using LRH.Runtime;
using UnityEditor;
using UnityEngine;

namespace LRH.Editor
{
    public sealed class TerrainMapEditor : EditorWindow
    {
        private const string Path = "Assets/_LRH/Resources/Maps/pacific.json";
        [SerializeField] private TerrainMap map;
        private Vector2 scroll;
        private int brush;
        private bool objects;
        private string kind = "rocks";
        private float scale = 2;
        private bool blocking = true;
        private const float CellSize = 9;
        private readonly string[] brushes = { "Mar", "Bajío", "Arena sólida", "Roca sólida" };
        private readonly string[] kinds = { "rocks", "reef", "shrub", "palms", "pier", "ruins", "cliff", "crates" };

        [MenuItem("LRH/Editor de terreno")]
        public static void Open() => GetWindow<TerrainMapEditor>("Terreno LRH");

        [InitializeOnLoadMethod]
        private static void RemovePreviewBeforePlay()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.ExitingEditMode) return;
                var preview = GameObject.Find("LRH / Vista previa de terreno");
                if (preview != null) DestroyImmediate(preview);
            };
        }
        private void OnEnable() => Load();
        private void Load() => map = JsonUtility.FromJson<TerrainMap>(File.ReadAllText(Path));

        private void OnGUI()
        {
            if (map == null) { if (GUILayout.Button("Cargar mapa")) Load(); return; }
            EditorGUILayout.HelpBox("Pinta las celdas o coloca objetos con clic. Guarda para usar el mismo mapa en juego y minimapa. El mar continúa fuera de la región.", MessageType.Info);
            objects = GUILayout.Toolbar(objects ? 1 : 0, new[] { "Terreno", "Objetos fijos" }) == 1;
            if (!objects) brush = GUILayout.Toolbar(brush, brushes);
            else
            {
                int index = Mathf.Max(0, System.Array.IndexOf(kinds, kind));
                kind = kinds[EditorGUILayout.Popup("Objeto", index, kinds)];
                scale = EditorGUILayout.Slider("Escala", scale, .5f, 5);
                blocking = EditorGUILayout.Toggle("Tiene colisión", blocking);
                EditorGUILayout.LabelField("Clic derecho: eliminar el objeto más cercano (radio 2 u).");
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Guardar mapa")) Save();
            if (GUILayout.Button("Recargar")) Load();
            if (GUILayout.Button("Vista previa en Scene"))
            {
                Save();
                var existing = GameObject.Find("LRH / Vista previa de terreno");
                if (existing != null) DestroyImmediate(existing);
                var preview = new GameObject("LRH / Vista previa de terreno");
                preview.hideFlags = HideFlags.DontSave;
                preview.AddComponent<OpenSea>().Initialize(null);
                SceneView.lastActiveSceneView?.Repaint();
            }
            EditorGUILayout.EndHorizontal();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            var rect = GUILayoutUtility.GetRect(map.width * CellSize, map.height * CellSize, GUILayout.ExpandWidth(false));
            for (int y = 0; y < map.height; ++y)
            for (int x = 0; x < map.width; ++x)
                EditorGUI.DrawRect(new Rect(rect.x+x*CellSize, rect.y+(map.height-1-y)*CellSize, CellSize, CellSize), TerrainMap.CellColor(map.Cell(x,y)));
            if (map.props != null) foreach (var prop in map.props)
                EditorGUI.DrawRect(new Rect(rect.x+(prop.x-map.originX)*CellSize-3,
                    rect.y+(map.height-(prop.y-map.originY))*CellSize-3,6,6), prop.blocking ? Color.red : Color.green);
            var e = Event.current;
            if (rect.Contains(e.mousePosition) && (e.type == EventType.MouseDown || (!objects && e.type == EventType.MouseDrag)))
            {
                int x = Mathf.Clamp((int)((e.mousePosition.x-rect.x)/CellSize),0,map.width-1);
                int y = map.height-1-Mathf.Clamp((int)((e.mousePosition.y-rect.y)/CellSize),0,map.height-1);
                Undo.RecordObject(this,"Editar terreno LRH");
                if (!objects)
                {
                    var row = map.rows[y].ToCharArray(); row[x] = "~:.#"[brush]; map.rows[y] = new string(row);
                }
                else
                {
                    var props = new System.Collections.Generic.List<TerrainProp>(map.props ?? new TerrainProp[0]);
                    float wx = map.originX+x+.5f, wy = map.originY+y+.5f;
                    if (e.button == 1) props.RemoveAll(p => Vector2.Distance(new Vector2(p.x,p.y),new Vector2(wx,wy)) < 2);
                    else props.Add(new TerrainProp { kind=kind, x=wx, y=wy, scale=scale, blocking=blocking });
                    map.props = props.ToArray();
                }
                e.Use(); Repaint();
            }
            EditorGUILayout.EndScrollView();
        }

        private void Save()
        {
            File.WriteAllText(Path, JsonUtility.ToJson(map,true)+"\n");
            AssetDatabase.ImportAsset(Path);
            TerrainMap.Reload();
        }
    }
}
