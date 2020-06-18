using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    public partial class ResourceEditor : EditorWindow
    {
        private enum MenuState :byte
        {
            Normal,
            Add,
            Rename,
            Remove,
        }
    }
}