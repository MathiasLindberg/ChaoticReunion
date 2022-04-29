namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortInformation_Visitor<T> {
        void handle_MessagePortInformationModeInfo(MessagePortInformationModeInfo msg, T arg);
        void handle_MessagePortInformationAllowedCombinations(MessagePortInformationAllowedCombinations msg, T arg);
    }
}
