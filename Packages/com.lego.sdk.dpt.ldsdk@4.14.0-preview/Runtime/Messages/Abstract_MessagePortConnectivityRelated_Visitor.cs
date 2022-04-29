namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortConnectivityRelated_Visitor<T> : MessagePortConnectivityRelated_Visitor<T> {
        public void handle_MessageHubAttachedIODetached(MessageHubAttachedIODetached msg, T arg) {}
        public void handle_MessageHubAttachedIOAttached(MessageHubAttachedIOAttached msg, T arg) {}
        public void handle_MessageHubAttachedIOVirtualAttached(MessageHubAttachedIOVirtualAttached msg, T arg) {}
    }
}
