using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace LEGODeviceUnitySDK
{

    [CustomEditor(typeof(DeviceHandler))]
    public class DeviceHandlerEditor : Editor
    {
        private DeviceHandler _handler;
        private VisualElement _rootElement;
        private VisualTreeAsset _visualTree;
        private VisualElement _hubImage;

        public void OnEnable()
        {
            _handler = (DeviceHandler)target;
            _rootElement = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.lego.sdk.wirelesssdk/Editor/DeviceHandlerTemplate.uxml");

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.lego.sdk.wirelesssdk/Editor/DeviceHandlerStyle.uss");
            _rootElement.styleSheets.Add(styleSheet);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _visualTree.CloneTree(_rootElement);
            _rootElement.Query<PropertyField>().First().RegisterCallback<ChangeEvent<string>>(e => SelectHub(e.newValue));
            _hubImage = _rootElement.Query<VisualElement>("hubImage");

            SelectHub(ObjectNames.NicifyVariableName(_handler.hubType.ToString()));

            return _rootElement;
        }

        private void SelectHub(string hubName)
        {
            _hubImage.style.backgroundImage = Resources.Load<Texture2D>(hubName);
        }
    }
}
