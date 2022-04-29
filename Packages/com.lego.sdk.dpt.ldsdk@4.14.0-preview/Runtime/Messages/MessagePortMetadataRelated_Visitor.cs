namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortMetadataRelated_Visitor<T> {
        void handle_MessagePortInformationRequest(MessagePortInformationRequest msg, T arg);
        void handle_MessagePortModeInformationRequest(MessagePortModeInformationRequest msg, T arg);
        void handle_MessagePortInformation(MessagePortInformation msg, T arg);
        void handle_MessagePortModeInformation(MessagePortModeInformation msg, T arg);
    }
}
