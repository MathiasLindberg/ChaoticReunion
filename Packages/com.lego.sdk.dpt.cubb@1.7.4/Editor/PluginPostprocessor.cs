using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreUnityBleBridge.Editor
{
    public class PluginPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessAsset()
        {
            if (!assetPath.Contains("Packages/com.lego.modules.cubb/Plugins/"))
                return;
            
            Debug.Log("New CUBB Plugin imported at: " + assetPath);
        }
    }
}
