using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    /// <summary>
    /// 资源编辑器
    /// </summary>
    public partial class ResourceEditor : EditorWindow
    {
        private MenuState m_MenuState = MenuState.Normal;
        private HashSet<string> m_ExpandedResourceFolderNames = null;
        //private HashSet<Asset> m_SelectedAssetsInSelectedResource = null;
        //private HashSet<SourceFolder> m_ExpandedSourceFolders = null;
        //private HashSet<SourceAsset> m_SelectedSourceAssets = null;
        //private Texture m_MissingSourceAssetIcon = null;

        //private HashSet<SourceFolder> m_CachedSelectedSourceFolders = null;
        //private HashSet<SourceFolder> m_CachedUnselectedSourceFolders = null;
        //private HashSet<SourceFolder> m_CachedAssignedSourceFolders = null;
        //private HashSet<SourceFolder> m_CachedUnassignedSourceFolders = null;
        //private HashSet<SourceAsset> m_CachedAssignedSourceAssets = null;
        //private HashSet<SourceAsset> m_CachedUnassignedSourceAssets = null;

        private Vector2 m_ResourcesViewScroll = Vector2.zero;
        private Vector2 m_ResourceViewScroll = Vector2.zero;
        private Vector2 m_SourceAssetsViewScroll = Vector2.zero;
        private string m_InputResourceName = null;
        private string m_InputResourceVariant = null;
        private bool m_HideAssignedSourceAssets = false;
        private int m_CurrentResourceContentCount = 0;
        private int m_CurrentResourceRowOnDraw = 0;
        private int m_CurrentSourceRowOnDraw = 0;

        [MenuItem("Framework/Resource Tools/Resource Editor", false, 42)]
        private static void Open()
        {
            ResourceEditor window = GetWindow<ResourceEditor>("Resource Editor", true);
            window.minSize = new Vector2(1400f, 600f);
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(2f);
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.25f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(string.Format("Resource List ({0})", ""), EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawResourcesView();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawResourcesMenu();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.25f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(string.Format("Resource Content ({0})", ""), EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawResourceView();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawResourceMenu();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f - 16f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Asset List", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawSourceAssetsView();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawSourceAssetsMenu();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResourcesView()
        {
            m_CurrentResourceRowOnDraw = 0;
            m_ResourcesViewScroll = EditorGUILayout.BeginScrollView(m_ResourcesViewScroll);
            {
                //DrawResourceFolder(m_ResourceRoot);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawResourcesMenu()
        {
            switch (m_MenuState)
            {
                case MenuState.Normal:
                    //DrawResourcesMenu_Normal();
                    break;

                case MenuState.Add:
                    //DrawResourcesMenu_Add();
                    break;

                case MenuState.Rename:
                    //DrawResourcesMenu_Rename();
                    break;

                case MenuState.Remove:
                    //DrawResourcesMenu_Remove();
                    break;
            }
        }

        private void DrawResourceView()
        {
            m_ResourceViewScroll = EditorGUILayout.BeginScrollView(m_ResourceViewScroll);
            {
 
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawResourceMenu()
        {
            if (GUILayout.Button("All", GUILayout.Width(50f)))
            {

            }
            if (GUILayout.Button("None", GUILayout.Width(50f)))
            {
            }
            //m_Controller.AssetSorter = (AssetSorterType)EditorGUILayout.EnumPopup(m_Controller.AssetSorter, GUILayout.Width(60f));
            GUILayout.Label(string.Empty);

            EditorGUI.EndDisabledGroup();
        }

        private void DrawSourceAssetsView()
        {
            m_CurrentSourceRowOnDraw = 0;
            m_SourceAssetsViewScroll = EditorGUILayout.BeginScrollView(m_SourceAssetsViewScroll);
            {
                //DrawSourceFolder(m_Controller.SourceAssetRoot);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSourceAssetsMenu()
        {

        }

        private void DrawResourceFolder(ResourceFolder folder)
        {

        }
    } 
}
