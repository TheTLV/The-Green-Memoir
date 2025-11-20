using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Base Custom Editor cho ScriptableObjects có list
    /// Tự động tạo ReorderableList với nút + và dễ kéo thả
    /// Flexible: Không bắt buộc field nào
    /// </summary>
    public abstract class ScriptableObjectListEditor : UnityEditor.Editor
    {
        private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        protected virtual void OnEnable()
        {
            // Tự động tìm tất cả SerializedProperty là list/array
            SerializedProperty iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (iterator.isArray && iterator.arrayElementType != "char")
                    {
                        CreateReorderableList(iterator.propertyPath, iterator);
                    }
                }
                while (iterator.NextVisible(false));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw default inspector cho các field không phải list
            DrawDefaultInspectorWithoutLists();

            EditorGUILayout.Space();

            // Draw tất cả ReorderableLists
            foreach (var kvp in reorderableLists)
            {
                DrawReorderableList(kvp.Key, kvp.Value);
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw default inspector nhưng bỏ qua các list (đã có ReorderableList)
        /// </summary>
        private void DrawDefaultInspectorWithoutLists()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Bỏ qua các list (đã có ReorderableList)
                if (iterator.isArray && iterator.arrayElementType != "char")
                {
                    continue;
                }

                // Bỏ qua script field
                if (iterator.propertyPath == "m_Script")
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }
        }

        /// <summary>
        /// Tạo ReorderableList cho một property
        /// </summary>
        private void CreateReorderableList(string propertyPath, SerializedProperty property)
        {
            if (reorderableLists.ContainsKey(propertyPath))
                return;

            ReorderableList list = new ReorderableList(
                serializedObject,
                property,
                true, // draggable
                true, // displayHeader
                true, // displayAddButton
                true  // displayRemoveButton
            );

            // Custom header
            list.drawHeaderCallback = (Rect rect) =>
            {
                string displayName = GetDisplayName(propertyPath);
                EditorGUI.LabelField(rect, displayName);
            };

            // Custom element drawing
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                // Draw element với label rút gọn
                string elementLabel = GetElementLabel(element, index);
                EditorGUI.PropertyField(rect, element, new GUIContent(elementLabel), true);
            };

            // Element height
            list.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4;
            };

            // Add button callback
            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                GenericMenu menu = new GenericMenu();
                
                // Option 1: Add null (để kéo thả sau)
                menu.AddItem(new GUIContent("Add Empty"), false, () =>
                {
                    int index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                    serializedObject.ApplyModifiedProperties();
                });

                // Option 2: Tạo mới SO nếu là list của SOs
                if (IsScriptableObjectList(property))
                {
                    menu.AddItem(new GUIContent("Create New..."), false, () =>
                    {
                        CreateNewScriptableObject(property);
                    });
                }

                menu.ShowAsContext();
            };

            // Remove button callback
            list.onRemoveCallback = (ReorderableList l) =>
            {
                if (EditorUtility.DisplayDialog("Confirm", "Remove this element?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                    serializedObject.ApplyModifiedProperties();
                }
            };

            reorderableLists[propertyPath] = list;
        }

        /// <summary>
        /// Draw ReorderableList với foldout
        /// </summary>
        private void DrawReorderableList(string propertyPath, ReorderableList list)
        {
            if (!foldoutStates.ContainsKey(propertyPath))
            {
                foldoutStates[propertyPath] = true;
            }

            SerializedProperty property = serializedObject.FindProperty(propertyPath);
            if (property == null) return;

            string displayName = GetDisplayName(propertyPath);
            string countText = $" ({property.arraySize})";

            // Foldout
            foldoutStates[propertyPath] = EditorGUILayout.Foldout(
                foldoutStates[propertyPath],
                displayName + countText,
                true
            );

            if (foldoutStates[propertyPath])
            {
                EditorGUI.indentLevel++;
                list.DoLayoutList();
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Lấy display name từ property path
        /// </summary>
        private string GetDisplayName(string propertyPath)
        {
            // Convert "someField" -> "Some Field"
            string name = propertyPath;
            if (name.Contains("_"))
            {
                name = name.Replace("_", " ");
            }
            
            // Capitalize first letter
            if (name.Length > 0)
            {
                name = char.ToUpper(name[0]) + name.Substring(1);
            }

            return name;
        }

        /// <summary>
        /// Lấy label cho element
        /// </summary>
        private string GetElementLabel(SerializedProperty element, int index)
        {
            // Thử lấy tên từ các field phổ biến
            string[] nameFields = { "name", "itemName", "cropName", "toolName", "npcName", "displayName", "id", "itemId", "cropId", "toolId", "npcId" };
            
            foreach (string fieldName in nameFields)
            {
                SerializedProperty nameProp = element.FindPropertyRelative(fieldName);
                if (nameProp != null && nameProp.propertyType == SerializedPropertyType.String)
                {
                    string value = nameProp.stringValue;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return $"[{index}] {value}";
                    }
                }
            }

            // Nếu là ObjectReference, lấy tên object
            if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue != null)
            {
                return $"[{index}] {element.objectReferenceValue.name}";
            }

            return $"[{index}] Element";
        }

        /// <summary>
        /// Kiểm tra xem list có phải là list của ScriptableObjects không
        /// </summary>
        private bool IsScriptableObjectList(SerializedProperty property)
        {
            if (property.arrayElementType.StartsWith("PPtr"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tạo mới ScriptableObject và thêm vào list
        /// </summary>
        private void CreateNewScriptableObject(SerializedProperty property)
        {
            // Lấy type từ property
            System.Type elementType = GetElementType(property);
            if (elementType == null || !typeof(ScriptableObject).IsAssignableFrom(elementType))
            {
                Debug.LogWarning($"Cannot create ScriptableObject for type: {elementType}");
                return;
            }

            // Tạo instance
            ScriptableObject newSO = ScriptableObject.CreateInstance(elementType);

            // Save dialog
            string path = EditorUtility.SaveFilePanelInProject(
                $"Create {elementType.Name}",
                $"New{elementType.Name}",
                "asset",
                $"Create a new {elementType.Name}"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newSO, path);
                AssetDatabase.SaveAssets();

                // Thêm vào list
                int index = property.arraySize;
                property.arraySize++;
                property.GetArrayElementAtIndex(index).objectReferenceValue = newSO;
                serializedObject.ApplyModifiedProperties();

                Selection.activeObject = newSO;
            }
            else
            {
                DestroyImmediate(newSO);
            }
        }

        /// <summary>
        /// Lấy type của element trong list
        /// </summary>
        private System.Type GetElementType(SerializedProperty property)
        {
            string typeName = property.arrayElementType;
            
            // Parse từ "PPtr<$TypeName>"
            if (typeName.StartsWith("PPtr<$"))
            {
                typeName = typeName.Substring(6, typeName.Length - 7);
            }

            // Tìm type trong tất cả assemblies
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}

