using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class LoggerConfigInjector : AssetPostprocessor
{
    static string ConfigPath { get { return "/LPAF/Resources/"; } }
    static string ConfigFileName { get { return "Logger Config.asset"; } }

    static string FullPath { get { return Application.dataPath + ConfigPath; } }
    static string FullPathWithFileName { get { return FullPath + ConfigFileName; } }
    static string RelativePathWithFileName { get { return "Assets" + ConfigPath + ConfigFileName; } }

    //Check on Unity startup
    static LoggerConfigInjector()
    {
        //InitializeOnLoad will cause run each InitializeOnLoad on enter or exit play mode, it will cause unity editor slow, so ignore this case
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (!File.Exists(FullPathWithFileName))
            {
                Debug.Log("Creating Logger config file at " + RelativePathWithFileName);

                // Create the target directory if it do not exist
                Directory.CreateDirectory(FullPath);

                LoggerConfig config = ScriptableObject.CreateInstance<LoggerConfig>();
                config.name = "Logger Config";

                AssetDatabase.CreateAsset(config, RelativePathWithFileName);
                AssetDatabase.Refresh();
            }
        }
    }
}
