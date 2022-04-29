namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortConnectivityRelated_Visitor<T> {
        void handle_MessageHubAttachedIODetached(MessageHubAttachedIODetached msg, T arg);
        void handle_MessageHubAttachedIOAttached(MessageHubAttachedIOAttached msg, T arg);
        void handle_MessageHubAttachedIOVirtualAttached(MessageHubAttachedIOVirtualAttached msg, T arg);
    }
}
