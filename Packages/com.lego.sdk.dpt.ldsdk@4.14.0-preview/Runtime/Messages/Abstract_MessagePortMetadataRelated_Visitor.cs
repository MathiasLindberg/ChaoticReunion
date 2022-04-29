namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortMetadataRelated_Visitor<T> : MessagePortMetadataRelated_Visitor<T> {
        public void handle_MessagePortInformationRequest(MessagePortInformationRequest msg, T arg) {}
        public void handle_MessagePortModeInformationRequest(MessagePortModeInformationRequest msg, T arg) {}
        public void handle_MessagePortInformation(MessagePortInformation msg, T arg) {}
        public void handle_MessagePortModeInformation(MessagePortModeInformation msg, T arg) {}
    }
}
