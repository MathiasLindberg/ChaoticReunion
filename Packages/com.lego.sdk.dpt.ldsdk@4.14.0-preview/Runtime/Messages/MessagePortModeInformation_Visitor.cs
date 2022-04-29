namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortModeInformation_Visitor<T> {
        void handle_MessagePortModeInformationName(MessagePortModeInformationName msg, T arg);
        void handle_MessagePortModeInformationRaw(MessagePortModeInformationRaw msg, T arg);
        void handle_MessagePortModeInformationPct(MessagePortModeInformationPct msg, T arg);
        void handle_MessagePortModeInformationSI(MessagePortModeInformationSI msg, T arg);
        void handle_MessagePortModeInformationSymbol(MessagePortModeInformationSymbol msg, T arg);
        void handle_MessagePortModeInformationMapping(MessagePortModeInformationMapping msg, T arg);
        void handle_MessagePortModeInformationValueFormat(MessagePortModeInformationValueFormat msg, T arg);
    }
}
