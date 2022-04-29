using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CoreUnityBleBridge.Editor.WSA
{
    [UsedImplicitly]
    public class AppxManifestFilePostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder { get; private set; }
        public void OnPostprocessBuild(BuildReport report)
        {
            if(report.summary.platform != BuildTarget.WSAPlayer)
                return;
            
            PatchAppxManifest(report.summary.outputPath);
        }

        private static void PatchAppxManifest(string path)
        {
            Debug.Log(typeof(AppxManifestFilePostProcessor).Name + ": Patching appxmanifest...");

            try
            {
                //Construct path
                var buildPath = path;
                var projectPath = Path.Combine(buildPath, Application.productName);
                var manifestPath = Path.Combine(projectPath, "Package.appxmanifest");

                //Load XML Document
                var root = XElement.Load(manifestPath);
                var descendants = root.Descendants().ToArray();

                //Set minimum windows version
                var targetDeviceFamilyElement = descendants.First(el => el.Name.LocalName.Equals("TargetDeviceFamily"));
                var minVersionAttribute = targetDeviceFamilyElement
                    .Attributes()
                    .First(at => at.Name.LocalName.Equals("MinVersion"));
                minVersionAttribute.SetValue("10.0.15063.0");

                //Add Capabilities
                AddDeviceCapability(ref root, "radios");
                AddDeviceCapability(ref root, "bluetooth");

                //Save
                root.Save(manifestPath);
            }
            catch (Exception e)
            {
                throw new BuildFailedException(e);
            }

            Debug.Log(typeof(AppxManifestFilePostProcessor).Name + ": Success");
        }

        /// <summary>
        /// Adds a non-existing 'DeviceCapability' element to the packagemanitest.
        /// </summary>
        /// <remarks>
        /// Some capabilities have 'Capability' and 'uap:Capability' attribute names.
        /// These are not covered by this method.
        /// </remarks>
        /// <param name="root"></param>
        /// <param name="capability"></param>
        private static void AddDeviceCapability(ref XElement root, string capability)
        {
            var isCapabilitiesElementPresent = root.Descendants().Any(el => el.Name.LocalName.Equals("Capabilities")); 
            if(!isCapabilitiesElementPresent)
                root.Add(new XElement(root.Name.Namespace + "Capabilities"));
            
            var capabilitiesElement = root
                .Descendants()
                .First(el => el.Name.LocalName.Equals("Capabilities"));

            var matchingAttributes = capabilitiesElement
                .Descendants()
                .Attributes()
                .Where(at => at.Value.Equals(capability))
                .Select(at => at.Parent);

            if (matchingAttributes.Any())
                return;

            //Add the missing element
            var radioCapabilityElement = new XElement(root.Name.Namespace + "DeviceCapability");
            radioCapabilityElement.SetAttributeValue("Name", capability);
            capabilitiesElement.Add(radioCapabilityElement);
        }
    }
}
