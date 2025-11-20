using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data;
using System.Linq;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Custom Editor cho CropDataSO - Giúp quản lý danh sách sprite dễ dàng hơn
    /// </summary>
    [CustomEditor(typeof(CropDataSO))]
    public class CropDataSOEditor : UnityEditor.Editor
    {
        private CropDataSO cropData;

        private void OnEnable()
        {
            cropData = (CropDataSO)target;
        }

        public override void OnInspectorGUI()
        {
            // Vẽ inspector mặc định
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Sprite Management", EditorStyles.boldLabel);

            // Buttons để sắp xếp và quản lý sprites
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Sort Growth Sprites"))
            {
                SortSpritesByDay(cropData.growthSprites);
            }

            if (GUILayout.Button("Sort Wet Sprites"))
            {
                SortSpritesByDay(cropData.wetSprites);
            }

            if (GUILayout.Button("Sort Wilted Sprites"))
            {
                SortSpritesByDay(cropData.wiltedSprites);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Buttons để thêm sprite mới
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Growth Sprite"))
            {
                AddNewSprite(cropData.growthSprites);
            }

            if (GUILayout.Button("Add Wet Sprite (Optional)"))
            {
                AddNewSprite(cropData.wetSprites);
            }

            if (GUILayout.Button("Add Wilted Sprite (Optional)"))
            {
                AddNewSprite(cropData.wiltedSprites);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Hiển thị preview
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "💡 ĐƠN GIẢN: Chỉ cần thêm 1 sprite vào 'Growth Sprites' là đủ!\n" +
                "💡 Nếu chỉ có 1 sprite, sẽ dùng cho tất cả các giai đoạn.\n" +
                "💡 Nhiều sprite: Thêm nhiều sprite với dayToShow khác nhau để cây lớn lên.\n" +
                "💡 Logic: Tưới nước → lớn lên, không tưới → héo\n" +
                "⚠️ LƯU Ý: Wilted Sprites KHÔNG CẦN dayToShow (cây héo sẽ mãi héo, chỉ cần 1 sprite)",
                MessageType.Info
            );

            // Preview với số ngày test
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Test Days:", GUILayout.Width(80));
            int testDays = EditorGUILayout.IntField(5, GUILayout.Width(50));
            EditorGUILayout.LabelField("Days Since Watered:", GUILayout.Width(150));
            int testDaysSinceWatered = EditorGUILayout.IntField(0, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview (Not Watered)"))
            {
                var sprite = cropData.GetSpriteForDay(testDays, false, testDaysSinceWatered, cropData.daysToWilt);
                ShowSpritePreview(sprite, "Not Watered");
            }
            if (GUILayout.Button("Preview (Watered Today)"))
            {
                var sprite = cropData.GetSpriteForDay(testDays, true, 0, cropData.daysToWilt);
                ShowSpritePreview(sprite, "Watered Today");
            }
            if (GUILayout.Button("Preview (Wilted)"))
            {
                var sprite = cropData.GetSpriteForDay(testDays, false, cropData.daysToWilt, cropData.daysToWilt);
                ShowSpritePreview(sprite, "Wilted");
            }
            EditorGUILayout.EndHorizontal();

            // Đánh dấu dirty để save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(cropData);
            }
        }

        private void SortSpritesByDay(System.Collections.Generic.List<CropSpriteData> sprites)
        {
            if (sprites == null || sprites.Count == 0)
                return;

            // Sắp xếp theo dayToShow
            sprites.Sort((a, b) => a.dayToShow.CompareTo(b.dayToShow));

            // Đảm bảo sprite đầu tiên có dayToShow = 0
            if (sprites.Count > 0 && sprites[0].dayToShow != 0)
            {
                sprites[0].dayToShow = 0;
            }

            EditorUtility.SetDirty(cropData);
            Debug.Log($"Đã sắp xếp {sprites.Count} sprites theo ngày");
        }

        private void AddNewSprite(System.Collections.Generic.List<CropSpriteData> sprites)
        {
            if (sprites == null)
                return;

            var newSprite = new CropSpriteData
            {
                sprite = null,
                dayToShow = sprites.Count > 0 ? sprites.Max(s => s.dayToShow) + 1 : 0,
                description = $"Sprite {sprites.Count + 1}"
            };

            sprites.Add(newSprite);
            EditorUtility.SetDirty(cropData);
        }

        private void ShowSpritePreview(Sprite sprite, string stateName)
        {
            if (sprite == null)
            {
                EditorGUILayout.HelpBox($"Không tìm thấy sprite cho {stateName} state", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"{stateName} Sprite:", sprite.name);
            
            // Hiển thị texture preview
            var texture = AssetPreview.GetAssetPreview(sprite);
            if (texture != null)
            {
                GUILayout.Label(texture, GUILayout.Width(64), GUILayout.Height(64));
            }
        }
    }
}

