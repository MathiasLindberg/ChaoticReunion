#if UNITY_EDITOR
using LEGOModelImporter;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class InitializeProject
{
    internal static readonly string initializedMarkerPath = "InitializedMarker";

    internal static readonly string scenesPath = "Assets/Samples/Basic Connection Sample/Scenes";
    internal static readonly string sampleSceneName = "Sample Scene.unity";

    static void InitializeSampleSceneAndSettings()
    {
        LoadConnectionSampleScene();

        // Enable brick building and auto update hierarchy.
        ToolsSettings.IsBrickBuildingOn = true;
        ToolsSettings.AutoUpdateHierarchy = true;
    }

    [MenuItem("LEGO Tools/Open Connection Sample", false, priority = 201)]
    static void LoadConnectionSampleScene()
    {
        // Load sample scene.
        var sampleScenePath = Path.Combine(scenesPath, sampleSceneName);
        var sampleScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sampleScenePath);
        if (sampleScene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sampleScenePath);
            }
        }
    }

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        if (!File.Exists(initializedMarkerPath))
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                File.CreateText(initializedMarkerPath).Close();

                EditorApplication.delayCall += InitializeSampleSceneAndSettings;
            }
        }
    }
}
#endif
