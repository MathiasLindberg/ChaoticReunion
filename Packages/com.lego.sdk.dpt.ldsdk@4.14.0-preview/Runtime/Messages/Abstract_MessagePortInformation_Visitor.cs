namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortInformation_Visitor<T> : MessagePortInformation_Visitor<T> {
        public void handle_MessagePortInformationModeInfo(MessagePortInformationModeInfo msg, T arg) {}
        public void handle_MessagePortInformationAllowedCombinations(MessagePortInformationAllowedCombinations msg, T arg) {}
    }
}
