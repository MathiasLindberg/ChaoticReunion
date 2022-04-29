namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortModeInformation_Visitor<T> : MessagePortModeInformation_Visitor<T> {
        public void handle_MessagePortModeInformationName(MessagePortModeInformationName msg, T arg) {}
        public void handle_MessagePortModeInformationRaw(MessagePortModeInformationRaw msg, T arg) {}
        public void handle_MessagePortModeInformationPct(MessagePortModeInformationPct msg, T arg) {}
        public void handle_MessagePortModeInformationSI(MessagePortModeInformationSI msg, T arg) {}
        public void handle_MessagePortModeInformationSymbol(MessagePortModeInformationSymbol msg, T arg) {}
        public void handle_MessagePortModeInformationMapping(MessagePortModeInformationMapping msg, T arg) {}
        public void handle_MessagePortModeInformationValueFormat(MessagePortModeInformationValueFormat msg, T arg) {}
    }
}
