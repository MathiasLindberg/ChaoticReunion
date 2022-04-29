namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
    public interface DownstreamMessage_Visitor<T> {
        void handle_EraseFlashReq(EraseFlashReq msg, T arg);
        void handle_ProgramFlashData(ProgramFlashData msg, T arg);
        void handle_StartAppReq(StartAppReq msg, T arg);
        void handle_InitiateLoaderReq(InitiateLoaderReq msg, T arg);
        void handle_GetInfoReq(GetInfoReq msg, T arg);
        void handle_GetChecksumReq(GetChecksumReq msg, T arg);
        void handle_GetFlashStateReq(GetFlashStateReq msg, T arg);
        void handle_Disconnect(Disconnect msg, T arg);
    }
}
