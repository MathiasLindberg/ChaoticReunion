namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_LegoMessage_Visitor<T> : LegoMessage_Visitor<T> {
        public void handle_MessagePortConnectivityRelated(MessagePortConnectivityRelated msg, T arg) {}
        public void handle_MessagePortMetadataRelated(MessagePortMetadataRelated msg, T arg) {}
        public void handle_MessagePortDynamicsRelated(MessagePortDynamicsRelated msg, T arg) {}
        public void handle_MessageHubProperty(MessageHubProperty msg, T arg) {}
        public void handle_MessageHubAction(MessageHubAction msg, T arg) {}
        public void handle_MessageHubAlert(MessageHubAlert msg, T arg) {}
        public void handle_MessageFirmwareBootMode(MessageFirmwareBootMode msg, T arg) {}
        public void handle_MessageError(MessageError msg, T arg) {}
        public void handle_MessagePortValueSingle(MessagePortValueSingle msg, T arg) {}
        public void handle_MessageVirtualPortSetupDisconnect(MessageVirtualPortSetupDisconnect msg, T arg) {}
        public void handle_MessageVirtualPortSetupConnect(MessageVirtualPortSetupConnect msg, T arg) {}
        public void handle_MessagePortOutputCommandFeedback(MessagePortOutputCommandFeedback msg, T arg) {}
    }
}
