namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
    public abstract class Abstract_UpstreamMessage_Visitor<T> : UpstreamMessage_Visitor<T> {
        public void handle_EraseFlashResp(EraseFlashResp msg, T arg) {}
        public void handle_ProgramFlashDone(ProgramFlashDone msg, T arg) {}
        public void handle_InitiateLoaderResp(InitiateLoaderResp msg, T arg) {}
        public void handle_GetInfoResp(GetInfoResp msg, T arg) {}
        public void handle_GetChecksumResp(GetChecksumResp msg, T arg) {}
        public void handle_GetFlashStateResp(GetFlashStateResp msg, T arg) {}
    }
}
