using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class Shortcut
    {
        [MenuItem("qtDino/Open scene/Menu scene &1")]
        private static void OpenMenuScene()
        {
            OpenScene("MenuScene");
        }
        private static void OpenScene(string name)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
            }    
        }

        [MenuItem("qtDino/Data/Clear player data")]
        private static void ClearPlayerData()
        {
            if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
            {
                File.Delete(Application.persistentDataPath + "/playerData.dat");
                Debug.LogError("Clear player data");
            }
        }
        
        [MenuItem("qtDino/Data/Clear player prefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.LogError("Clear player prefs");
        }
        
        private static void OpenScene(string name)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_UnityScene/" + name + ".unity");
            }
        }
        
        private static void OpenPrefab(string path)
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>($"{path}.prefab"));
        }

        /// <summary>
        /// Selects a folder in the project window and shows its content.
        /// Opens a new project window, if none is open yet.
        /// </summary>
        /// <param name="folderInstanceID">The instance of the folder asset to open.</param>
        private static void ShowFolderContents(int folderInstanceID)
        {
            // Find the internal ProjectBrowser class in the editor assembly.
            Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;
            System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

            // This is the internal method, which performs the desired action.
            // Should only be called if the project window is in two column mode.
            MethodInfo showFolderContents = projectBrowserType.GetMethod(
                "ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic);

            // Find any open project browser windows.
            Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

            if (projectBrowserInstances.Length > 0)
            {
                for (int i = 0; i < projectBrowserInstances.Length; i++)
                    ShowFolderContentsInternal(projectBrowserInstances[i], showFolderContents, folderInstanceID);
            }
            else
            {
                EditorWindow projectBrowser = OpenNewProjectBrowser(projectBrowserType);
                ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
            }
        }

        private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents,
            int folderInstanceID)
        {
            // Sadly, there is no method to check for the view mode.
            // We can use the serialized object to find the private property.
            SerializedObject serializedObject = new SerializedObject(projectBrowser);
            bool inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;

            if (!inTwoColumnMode)
            {
                // If the browser is not in two column mode, we must set it to show the folder contents.
                MethodInfo setTwoColumns = projectBrowser.GetType().GetMethod(
                    "SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic);
                setTwoColumns.Invoke(projectBrowser, null);
            }

            bool revealAndFrameInFolderTree = true;
            showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, revealAndFrameInFolderTree });
        }

        private static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
        {
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            projectBrowser.Show();

            // Unity does some special initialization logic, which we must call,
            // before we can use the ShowFolderContents method (else we get a NullReferenceException).
            MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
            init.Invoke(projectBrowser, null);

            return projectBrowser;
        }
    }
}