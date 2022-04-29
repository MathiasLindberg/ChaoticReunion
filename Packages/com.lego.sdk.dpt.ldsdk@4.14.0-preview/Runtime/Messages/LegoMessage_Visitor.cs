namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface LegoMessage_Visitor<T> {
        void handle_MessagePortConnectivityRelated(MessagePortConnectivityRelated msg, T arg);
        void handle_MessagePortMetadataRelated(MessagePortMetadataRelated msg, T arg);
        void handle_MessagePortDynamicsRelated(MessagePortDynamicsRelated msg, T arg);
        void handle_MessageHubProperty(MessageHubProperty msg, T arg);
        void handle_MessageHubAction(MessageHubAction msg, T arg);
        void handle_MessageHubAlert(MessageHubAlert msg, T arg);
        void handle_MessageFirmwareBootMode(MessageFirmwareBootMode msg, T arg);
        void handle_MessageError(MessageError msg, T arg);
        void handle_MessagePortValueSingle(MessagePortValueSingle msg, T arg);
        void handle_MessageVirtualPortSetupDisconnect(MessageVirtualPortSetupDisconnect msg, T arg);
        void handle_MessageVirtualPortSetupConnect(MessageVirtualPortSetupConnect msg, T arg);
        void handle_MessagePortOutputCommandFeedback(MessagePortOutputCommandFeedback msg, T arg);
    }
}
