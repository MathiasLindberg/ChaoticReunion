namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortRelated_Visitor<T> : MessagePortRelated_Visitor<T> {
        public void handle_MessagePortConnectivityRelated(MessagePortConnectivityRelated msg, T arg) {}
        public void handle_MessagePortMetadataRelated(MessagePortMetadataRelated msg, T arg) {}
        public void handle_MessagePortDynamicsRelated(MessagePortDynamicsRelated msg, T arg) {}
    }
}
