using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public partial class ResourceEditor : EditorWindow
    {
        private class ResourceItem
        {
            private static Texture s_CachedUnknownIcon = null;
            private static Texture s_CachedAssetIcon = null;
            private static Texture s_CachedSceneIcon = null;
        }
    }
}