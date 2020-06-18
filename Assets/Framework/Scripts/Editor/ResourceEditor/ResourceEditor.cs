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
        private ResourceEditorController m_Controller = null;
        private MenuState m_MenuState = MenuState.Normal;
        private Resource m_SelectedResource = null;
        private ResourceFolder m_ResourceRoot = null;
        private HashSet<string> m_ExpandedResourceFolderNames = null;
        private HashSet<Asset> m_SelectedAssetsInSelectedResource = null;
        private HashSet<SourceFolder> m_ExpandedSourceFolders = null;
        private HashSet<SourceAsset> m_SelectedSourceAssets = null;
        private Texture m_MissingSourceAssetIcon = null;

        private HashSet<SourceFolder> m_CachedSelectedSourceFolders = null;
        private HashSet<SourceFolder> m_CachedUnselectedSourceFolders = null;
        private HashSet<SourceFolder> m_CachedAssignedSourceFolders = null;
        private HashSet<SourceFolder> m_CachedUnassignedSourceFolders = null;
        private HashSet<SourceAsset> m_CachedAssignedSourceAssets = null;
        private HashSet<SourceAsset> m_CachedUnassignedSourceAssets = null;

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
            m_Controller = new ResourceEditorController();
            m_Controller.OnLoadingResource += OnLoadingResource;
            m_Controller.OnLoadingAsset += OnLoadingAsset;
            m_Controller.OnLoadCompleted += OnLoadCompleted;
            m_Controller.OnAssetAssigned += OnAssetAssigned;
            m_Controller.OnAssetUnassigned += OnAssetUnassigned;

            m_MenuState = MenuState.Normal;
            m_SelectedResource = null;
            m_ResourceRoot = new ResourceFolder("Resources", null);
            m_ExpandedResourceFolderNames = new HashSet<string>();
            m_SelectedAssetsInSelectedResource = new HashSet<Asset>();
            m_ExpandedSourceFolders = new HashSet<SourceFolder>();
            m_SelectedSourceAssets = new HashSet<SourceAsset>();
            m_MissingSourceAssetIcon = EditorGUIUtility.IconContent("console.warnicon.sml").image;

            m_CachedSelectedSourceFolders = new HashSet<SourceFolder>();
            m_CachedUnselectedSourceFolders = new HashSet<SourceFolder>();
            m_CachedAssignedSourceFolders = new HashSet<SourceFolder>();
            m_CachedUnassignedSourceFolders = new HashSet<SourceFolder>();
            m_CachedAssignedSourceAssets = new HashSet<SourceAsset>();
            m_CachedUnassignedSourceAssets = new HashSet<SourceAsset>();

            m_ResourcesViewScroll = Vector2.zero;
            m_ResourceViewScroll = Vector2.zero;
            m_SourceAssetsViewScroll = Vector2.zero;
            m_InputResourceName = null;
            m_InputResourceVariant = null;
            m_HideAssignedSourceAssets = false;
            m_CurrentResourceContentCount = 0;
            m_CurrentResourceRowOnDraw = 0;
            m_CurrentSourceRowOnDraw = 0;

            if (m_Controller.Load())
            {
                Debug.Log("Load configuration success.");
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }

            EditorUtility.DisplayProgressBar("Prepare Resource Editor", "Processing...", 0f);
            RefreshResourceTree();
            EditorUtility.ClearProgressBar();
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
                DrawResourceFolder(m_ResourceRoot);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawResourcesMenu()
        {
            switch (m_MenuState)
            {
                case MenuState.Normal:
                    DrawResourcesMenu_Normal();
                    break;

                case MenuState.Add:
                    DrawResourcesMenu_Add();
                    break;

                case MenuState.Rename:
                    //DrawResourcesMenu_Rename();
                    break;

                case MenuState.Remove:
                    //DrawResourcesMenu_Remove();
                    break;
            }
        }

        private void DrawResourcesMenu_Normal()
        {
            if (GUILayout.Button("Add", GUILayout.Width(65f)))
            {
                m_MenuState = MenuState.Add;
                m_InputResourceName = null;
                m_InputResourceVariant = null;
                GUI.FocusControl(null);
            }
            EditorGUI.BeginDisabledGroup(m_SelectedResource == null);
            {
                if (GUILayout.Button("Rename", GUILayout.Width(65f)))
                {
                    m_MenuState = MenuState.Rename;
                    m_InputResourceName = m_SelectedResource != null ? m_SelectedResource.Name : null;
                    m_InputResourceVariant = m_SelectedResource != null ? m_SelectedResource.Variant : null;
                    GUI.FocusControl(null);
                }
                if (GUILayout.Button("Remove", GUILayout.Width(65f)))
                {
                    m_MenuState = MenuState.Remove;
                }
                if (m_SelectedResource == null)
                {
                    EditorGUILayout.EnumPopup(LoadType.LoadFromFile);
                }
                else
                {
                    LoadType loadType = (LoadType)EditorGUILayout.EnumPopup(m_SelectedResource.LoadType);
                    if (loadType != m_SelectedResource.LoadType)
                    {
                        SetResourceLoadType(loadType);
                    }
                }
                bool packed = EditorGUILayout.ToggleLeft("Packed", m_SelectedResource != null && m_SelectedResource.Packed, GUILayout.Width(65f));
                if (m_SelectedResource != null && packed != m_SelectedResource.Packed)
                {
                    SetResourcePacked(packed);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawResourcesMenu_Add()
        {
            GUI.SetNextControlName("NewResourceNameTextField");
            m_InputResourceName = EditorGUILayout.TextField(m_InputResourceName);
            GUI.SetNextControlName("NewResourceVariantTextField");
            m_InputResourceVariant = EditorGUILayout.TextField(m_InputResourceVariant, GUILayout.Width(60f));

            if (GUI.GetNameOfFocusedControl() == "NewResourceNameTextField" || GUI.GetNameOfFocusedControl() == "NewResourceVariantTextField")
            {
                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    EditorUtility.DisplayProgressBar("Add Resource", "Processing...", 0f);
                    AddResource(m_InputResourceName, m_InputResourceVariant, true);
                    EditorUtility.ClearProgressBar();
                    Repaint();
                }
            }

            if (GUILayout.Button("Add", GUILayout.Width(50f)))
            {
                EditorUtility.DisplayProgressBar("Add Resource", "Processing...", 0f);
                AddResource(m_InputResourceName, m_InputResourceVariant, true);
                EditorUtility.ClearProgressBar();
            }

            if (GUILayout.Button("Back", GUILayout.Width(50f)))
            {
                m_MenuState = MenuState.Normal;
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

        /// <summary>
        /// 绘制资源列表
        /// </summary>
        private void DrawSourceAssetsView()
        {
            m_CurrentSourceRowOnDraw = 0;
            m_SourceAssetsViewScroll = EditorGUILayout.BeginScrollView(m_SourceAssetsViewScroll);
            {
                DrawSourceFolder(m_Controller.SourceAssetRoot);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSourceAssetsMenu()
        {

        }

        private void DrawResourceFolder(ResourceFolder folder)
        {
            //文件夹是否展开
            bool expand = IsExpandedResourceFolder(folder);
            EditorGUILayout.BeginHorizontal();
            {
#if UNITY_2019_3_OR_NEWER
                bool foldout = EditorGUI.Foldout(new Rect(18f + 14f * folder.Depth, 20f * m_CurrentResourceRowOnDraw + 4f, int.MaxValue, 14f), expand, string.Empty, true);
#else
                bool foldout = EditorGUI.Foldout(new Rect(18f + 14f * folder.Depth, 20f * m_CurrentResourceRowOnDraw + 2f, int.MaxValue, 14f), expand, string.Empty, true);
#endif
                if (expand != foldout)
                {
                    expand = !expand;
                    SetExpandedResourceFolder(folder, expand);
                }

#if UNITY_2019_3_OR_NEWER
                GUI.DrawTexture(new Rect(32f + 14f * folder.Depth, 20f * m_CurrentResourceRowOnDraw + 3f, 16f, 16f), ResourceFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(44f + 14f * folder.Depth), GUILayout.Height(18f));
#else
                GUI.DrawTexture(new Rect(32f + 14f * folder.Depth, 20f * m_CurrentResourceRowOnDraw + 1f, 16f, 16f), ResourceFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(40f + 14f * folder.Depth), GUILayout.Height(18f));
#endif
                EditorGUILayout.LabelField(folder.Name);
            }
            EditorGUILayout.EndHorizontal();

            m_CurrentResourceRowOnDraw++;

            if (expand)
            {
                foreach (ResourceFolder subFolder in folder.GetFolders())
                {
                    DrawResourceFolder(subFolder);
                }

                foreach (ResourceItem resourceItem in folder.GetItems())
                {
                    DrawResourceItem(resourceItem);
                }
            }
        }

        private void DrawResourceItem(ResourceItem resourceItem)
        {
            EditorGUILayout.BeginHorizontal();
            {
                string title = resourceItem.Name;
                if (resourceItem.Resource.Packed)
                {
                    title = "[Packed] " + title;
                }

                float emptySpace = position.width;
                if (EditorGUILayout.Toggle(m_SelectedResource == resourceItem.Resource, GUILayout.Width(emptySpace - 12f)))
                {
                    ChangeSelectedResource(resourceItem.Resource);
                }
                else if (m_SelectedResource == resourceItem.Resource)
                {
                    ChangeSelectedResource(null);
                }

                GUILayout.Space(-emptySpace + 24f);
#if UNITY_2019_3_OR_NEWER
                GUI.DrawTexture(new Rect(32f + 14f * resourceItem.Depth, 20f * m_CurrentResourceRowOnDraw + 3f, 16f, 16f), resourceItem.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(30f + 14f * resourceItem.Depth), GUILayout.Height(18f));
#else
                GUI.DrawTexture(new Rect(32f + 14f * resourceItem.Depth, 20f * m_CurrentResourceRowOnDraw + 1f, 16f, 16f), resourceItem.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * resourceItem.Depth), GUILayout.Height(18f));
#endif
                EditorGUILayout.LabelField(title);
            }
            EditorGUILayout.EndHorizontal();
            m_CurrentResourceRowOnDraw++;
        }

        private void DrawSourceFolder(SourceFolder sourceFolder)
        {
            if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(sourceFolder))
            {
                return;
            }

            bool expand = IsExpandedSourceFolder(sourceFolder);
            EditorGUILayout.BeginHorizontal();
            {
                bool select = IsSelectedSourceFolder(sourceFolder);
                if (select != EditorGUILayout.Toggle(select, GUILayout.Width(12f + 14f * sourceFolder.Depth)))
                {
                    select = !select;
                    SetSelectedSourceFolder(sourceFolder, select);
                }

                GUILayout.Space(-14f * sourceFolder.Depth);
#if UNITY_2019_3_OR_NEWER
                bool foldout = EditorGUI.Foldout(new Rect(18f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 4f, int.MaxValue, 14f), expand, string.Empty, true);
#else
                bool foldout = EditorGUI.Foldout(new Rect(18f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 2f, int.MaxValue, 14f), expand, string.Empty, true);
#endif
                if (expand != foldout)
                {
                    expand = !expand;
                    SetExpandedSourceFolder(sourceFolder, expand);
                }

#if UNITY_2019_3_OR_NEWER
                GUI.DrawTexture(new Rect(32f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 3f, 16f, 16f), SourceFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(30f + 14f * sourceFolder.Depth), GUILayout.Height(18f));
#else
                GUI.DrawTexture(new Rect(32f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 1f, 16f, 16f), SourceFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * sourceFolder.Depth), GUILayout.Height(18f));
#endif
                EditorGUILayout.LabelField(sourceFolder.Name);
            }
            EditorGUILayout.EndHorizontal();

            m_CurrentSourceRowOnDraw++;

            if (expand)
            {
                foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
                {
                    DrawSourceFolder(subSourceFolder);
                }

                foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
                {
                    DrawSourceAsset(sourceAsset);
                }
            }
        }

        private bool IsExpandedSourceFolder(SourceFolder sourceFolder)
        {
            return m_ExpandedSourceFolders.Contains(sourceFolder);
        }

        private bool IsSelectedSourceFolder(SourceFolder sourceFolder)
        {
            if (m_CachedSelectedSourceFolders.Contains(sourceFolder))
            {
                return true;
            }

            if (m_CachedUnselectedSourceFolders.Contains(sourceFolder))
            {
                return false;
            }

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
                {
                    continue;
                }

                if (!IsSelectedSourceAsset(sourceAsset))
                {
                    m_CachedUnselectedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(sourceFolder))
                {
                    continue;
                }

                if (!IsSelectedSourceFolder(subSourceFolder))
                {
                    m_CachedUnselectedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            m_CachedSelectedSourceFolders.Add(sourceFolder);
            return true;
        }

        private void SetSelectedSourceFolder(SourceFolder sourceFolder, bool select)
        {
            if (select)
            {
                m_CachedSelectedSourceFolders.Add(sourceFolder);
                m_CachedUnselectedSourceFolders.Remove(sourceFolder);

                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedUnselectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
            else
            {
                m_CachedSelectedSourceFolders.Remove(sourceFolder);
                m_CachedUnselectedSourceFolders.Add(sourceFolder);

                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
                {
                    continue;
                }

                SetSelectedSourceAsset(sourceAsset, select);
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(subSourceFolder))
                {
                    continue;
                }

                SetSelectedSourceFolder(subSourceFolder, select);
            }
        }

        private bool IsAssignedSourceFolder(SourceFolder sourceFolder)
        {
            if (m_CachedAssignedSourceFolders.Contains(sourceFolder))
            {
                return true;
            }

            if (m_CachedUnassignedSourceFolders.Contains(sourceFolder))
            {
                return false;
            }

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (!IsAssignedSourceAsset(sourceAsset))
                {
                    m_CachedUnassignedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (!IsAssignedSourceFolder(subSourceFolder))
                {
                    m_CachedUnassignedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            m_CachedAssignedSourceFolders.Add(sourceFolder);
            return true;
        }

        private void DrawSourceAsset(SourceAsset sourceAsset)
        {
            if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                float emptySpace = position.width;
                bool select = IsSelectedSourceAsset(sourceAsset);
                if (select != EditorGUILayout.Toggle(select, GUILayout.Width(emptySpace - 12f)))
                {
                    select = !select;
                    SetSelectedSourceAsset(sourceAsset, select);
                }

                GUILayout.Space(-emptySpace + 24f);
#if UNITY_2019_3_OR_NEWER
                GUI.DrawTexture(new Rect(32f + 14f * sourceAsset.Depth, 20f * m_CurrentSourceRowOnDraw + 3f, 16f, 16f), sourceAsset.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(30f + 14f * sourceAsset.Depth), GUILayout.Height(18f));
#else
                GUI.DrawTexture(new Rect(32f + 14f * sourceAsset.Depth, 20f * m_CurrentSourceRowOnDraw + 1f, 16f, 16f), sourceAsset.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * sourceAsset.Depth), GUILayout.Height(18f));
#endif
                EditorGUILayout.LabelField(sourceAsset.Name);
                Asset asset = m_Controller.GetAsset(sourceAsset.Guid);
                EditorGUILayout.LabelField(asset != null ? GetResourceFullName(asset.Resource.Name, asset.Resource.Variant) : string.Empty, GUILayout.Width(position.width * 0.15f));
            }
            EditorGUILayout.EndHorizontal();
            m_CurrentSourceRowOnDraw++;
        }

        private bool IsSelectedSourceAsset(SourceAsset sourceAsset)
        {
            return m_SelectedSourceAssets.Contains(sourceAsset);
        }

        private void SetSelectedSourceAsset(SourceAsset sourceAsset, bool select)
        {
            if (select)
            {
                m_SelectedSourceAssets.Add(sourceAsset);

                SourceFolder folder = sourceAsset.Folder;
                while (folder != null)
                {
                    m_CachedUnselectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
            else
            {
                m_SelectedSourceAssets.Remove(sourceAsset);

                SourceFolder folder = sourceAsset.Folder;
                while (folder != null)
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
        }

        private bool IsAssignedSourceAsset(SourceAsset sourceAsset)
        {
            if (m_CachedAssignedSourceAssets.Contains(sourceAsset))
            {
                return true;
            }

            if (m_CachedUnassignedSourceAssets.Contains(sourceAsset))
            {
                return false;
            }

            return m_Controller.GetAsset(sourceAsset.Guid) != null;
        }

        private void SetExpandedSourceFolder(SourceFolder sourceFolder, bool expand)
        {
            if (expand)
            {
                m_ExpandedSourceFolders.Add(sourceFolder);
            }
            else
            {
                m_ExpandedSourceFolders.Remove(sourceFolder);
            }
        }

        private void ChangeSelectedResource(Resource resource)
        {
            if (m_SelectedResource == resource)
            {
                return;
            }

            m_SelectedResource = resource;
            m_SelectedAssetsInSelectedResource.Clear();
        }

        private void RefreshResourceTree()
        {
            m_ResourceRoot.Clear();
            Resource[] resources = m_Controller.GetResources();
            foreach (Resource resource in resources)
            {
                string[] splitPath = resource.Name.Split('/');
                ResourceFolder folder = m_ResourceRoot;
                for (int i = 0; i < splitPath.Length - 1; i++)
                {
                    ResourceFolder subFolder = folder.GetFolder(splitPath[i]);
                    folder = subFolder == null ? folder.AddFolder(splitPath[i]) : subFolder;
                }

                string fullName = resource.Variant != null ? string.Format("{0}.{1}", splitPath[splitPath.Length - 1], resource.Variant) : splitPath[splitPath.Length - 1];
                folder.AddItem(fullName, resource);
            }
        }

        private bool IsExpandedResourceFolder(ResourceFolder folder)
        {
            return m_ExpandedResourceFolderNames.Contains(folder.FromRootPath);
        }

        private void SetExpandedResourceFolder(ResourceFolder folder, bool expand)
        {
            if (expand)
            {
                m_ExpandedResourceFolderNames.Add(folder.FromRootPath);
            }
            else
            {
                m_ExpandedResourceFolderNames.Remove(folder.FromRootPath);
            }
        }

        private void SetResourceLoadType(LoadType loadType)
        {
            string fullName = m_SelectedResource.FullName;
            if (m_Controller.SetResourceLoadType(m_SelectedResource.Name, m_SelectedResource.Variant, loadType))
            {
                Debug.Log(string.Format("Set resource '{0}' load type to '{1}' success.", fullName, loadType.ToString()));
            }
            else
            {
                Debug.LogWarning(string.Format("Set resource '{0}' load type to '{1}' failure.", fullName, loadType.ToString()));
            }
        }

        private void SetResourcePacked(bool packed)
        {
            string fullName = m_SelectedResource.FullName;
            if (m_Controller.SetResourcePacked(m_SelectedResource.Name, m_SelectedResource.Variant, packed))
            {
                Debug.Log(string.Format("{1} resource '{0}' success.", fullName, packed ? "Pack" : "Unpack"));
            }
            else
            {
                Debug.LogWarning(string.Format("{1} resource '{0}' failure.", fullName, packed ? "Pack" : "Unpack"));
            }
        }

        private void AddResource(string name, string variant, bool refresh)
        {
            if (variant == string.Empty)
            {
                variant = null;
            }

            string fullName = GetResourceFullName(name, variant);
            if (m_Controller.AddResource(name, variant, LoadType.LoadFromFile, false))
            {
                if (refresh)
                {
                    RefreshResourceTree();
                }

                Debug.Log(string.Format("Add resource '{0}' success.", fullName));
                m_MenuState = MenuState.Normal;
            }
            else
            {
                Debug.LogWarning(string.Format("Add resource '{0}' failure.", fullName));
            }
        }

        private string GetResourceFullName(string name, string variant)
        {
            return variant != null ? string.Format("{0}.{1}", name, variant) : name;
        }

        private void OnLoadingResource(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Resources", string.Format("Loading resources, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets", string.Format("Loading assets, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private void OnAssetAssigned(SourceAsset[] sourceAssets)
        {
            HashSet<SourceFolder> affectedFolders = new HashSet<SourceFolder>();
            foreach (SourceAsset sourceAsset in sourceAssets)
            {
                m_CachedAssignedSourceAssets.Add(sourceAsset);
                m_CachedUnassignedSourceAssets.Remove(sourceAsset);

                affectedFolders.Add(sourceAsset.Folder);
            }

            foreach (SourceFolder sourceFolder in affectedFolders)
            {
                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedUnassignedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
        }

        private void OnAssetUnassigned(SourceAsset[] sourceAssets)
        {
            HashSet<SourceFolder> affectedFolders = new HashSet<SourceFolder>();
            foreach (SourceAsset sourceAsset in sourceAssets)
            {
                m_CachedAssignedSourceAssets.Remove(sourceAsset);
                m_CachedUnassignedSourceAssets.Add(sourceAsset);

                affectedFolders.Add(sourceAsset.Folder);
            }

            foreach (SourceFolder sourceFolder in affectedFolders)
            {
                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    m_CachedAssignedSourceFolders.Remove(folder);
                    m_CachedUnassignedSourceFolders.Add(folder);
                    folder = folder.Folder;
                }
            }
        }
    } 
}
