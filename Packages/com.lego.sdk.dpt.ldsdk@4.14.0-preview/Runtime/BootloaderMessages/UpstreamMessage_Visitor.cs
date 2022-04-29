namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
    public interface UpstreamMessage_Visitor<T> {
        void handle_EraseFlashResp(EraseFlashResp msg, T arg);
        void handle_ProgramFlashDone(ProgramFlashDone msg, T arg);
        void handle_InitiateLoaderResp(InitiateLoaderResp msg, T arg);
        void handle_GetInfoResp(GetInfoResp msg, T arg);
        void handle_GetChecksumResp(GetChecksumResp msg, T arg);
        void handle_GetFlashStateResp(GetFlashStateResp msg, T arg);
    }
}
