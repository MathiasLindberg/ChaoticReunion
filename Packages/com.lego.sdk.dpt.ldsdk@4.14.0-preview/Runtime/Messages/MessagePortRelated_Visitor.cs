namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortRelated_Visitor<T> {
        void handle_MessagePortConnectivityRelated(MessagePortConnectivityRelated msg, T arg);
        void handle_MessagePortMetadataRelated(MessagePortMetadataRelated msg, T arg);
        void handle_MessagePortDynamicsRelated(MessagePortDynamicsRelated msg, T arg);
    }
}
