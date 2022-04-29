namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
    public abstract class Abstract_DownstreamMessage_Visitor<T> : DownstreamMessage_Visitor<T> {
        public void handle_EraseFlashReq(EraseFlashReq msg, T arg) {}
        public void handle_ProgramFlashData(ProgramFlashData msg, T arg) {}
        public void handle_StartAppReq(StartAppReq msg, T arg) {}
        public void handle_InitiateLoaderReq(InitiateLoaderReq msg, T arg) {}
        public void handle_GetInfoReq(GetInfoReq msg, T arg) {}
        public void handle_GetChecksumReq(GetChecksumReq msg, T arg) {}
        public void handle_GetFlashStateReq(GetFlashStateReq msg, T arg) {}
        public void handle_Disconnect(Disconnect msg, T arg) {}
    }
}
