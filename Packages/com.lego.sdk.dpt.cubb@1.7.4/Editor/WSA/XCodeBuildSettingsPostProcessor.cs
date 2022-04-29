using UnityEditor;
using UnityEditor.Callbacks;

//Guard in-case the developer does not have the iOS build package installed.
//NOTE: We do not guard the entire file in order to utilize our IDE tools ; )

#if UNITY_IOS 
using UnityEditor.iOS.Xcode;
#endif

namespace UnitySwift 
{ 
    public static class XCodeBuildSettingsPostProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) 
        {
            if(buildTarget != BuildTarget.iOS)
                return;

            #if UNITY_IOS
                //Since 'PBXProject.GetPBXProjectPath' returns the wrong path, we need to construct the path manually
                var PBXProjectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                var pbxProject = new PBXProject();
                pbxProject.ReadFromFile(PBXProjectPath);

            #if UNITY_2019_4_OR_NEWER
                var targetGuid = pbxProject.GetUnityMainTargetGuid();
            #else
                var targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
            #endif


            //Configure build settings
                pbxProject.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
                pbxProject.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

                pbxProject.WriteToFile(PBXProjectPath);
            #endif
        }
    }
}
