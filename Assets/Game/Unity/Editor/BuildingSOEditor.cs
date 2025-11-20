using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Custom Editor cho BuildingSO với ReorderableList
    /// </summary>
    [CustomEditor(typeof(BuildingSO))]
    public class BuildingSOEditor : ScriptableObjectListEditor
    {
        // Base class sẽ tự động xử lý tất cả lists
    }
}

