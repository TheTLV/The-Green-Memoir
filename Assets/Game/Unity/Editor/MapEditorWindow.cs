using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Map Editor Window - Editor để thiết kế maps (giống RPG Maker)
    /// </summary>
    public class MapEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private GameObject _selectedMap;

        [MenuItem("Window/Game Designer/Map Editor")]
        public static void ShowWindow()
        {
            GetWindow<MapEditorWindow>("Map Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Map Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Select map
            _selectedMap = (GameObject)EditorGUILayout.ObjectField("Selected Map", _selectedMap, typeof(GameObject), true);

            GUILayout.Space(10);

            if (GUILayout.Button("Create New Map"))
            {
                CreateNewMap();
            }

            if (GUILayout.Button("Open Tile Palette"))
            {
                // Mở Tile Palette window từ Unity menu
                // Unity 2019.2+: Window > 2D > Tile Palette
                EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Use Unity's Tile Palette to paint your map. This editor helps manage multiple maps.", MessageType.Info);
        }

        private void CreateNewMap()
        {
            // Tạo map mới với Grid và Tilemap
            GameObject map = new GameObject("NewMap");
            Grid grid = map.AddComponent<Grid>();
            grid.cellSize = new Vector3(1, 1, 0);

            // Tạo các layers
            GameObject background = new GameObject("Background");
            background.transform.SetParent(map.transform);
            background.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            background.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();

            GameObject ground = new GameObject("Ground");
            ground.transform.SetParent(map.transform);
            ground.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            ground.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();

            GameObject objects = new GameObject("Objects");
            objects.transform.SetParent(map.transform);
            objects.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            objects.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();

            Selection.activeGameObject = map;
            Debug.Log("Created new map: " + map.name);
        }
    }
}

