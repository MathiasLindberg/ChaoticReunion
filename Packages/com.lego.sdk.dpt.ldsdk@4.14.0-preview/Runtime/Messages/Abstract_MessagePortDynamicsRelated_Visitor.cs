namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public abstract class Abstract_MessagePortDynamicsRelated_Visitor<T> : MessagePortDynamicsRelated_Visitor<T> {
        public void handle_MessagePortInputFormatSetupSingle(MessagePortInputFormatSetupSingle msg, T arg) {}
        public void handle_MessagePortInputFormatSetupCombined(MessagePortInputFormatSetupCombined msg, T arg) {}
        public void handle_MessagePortValueCombined(MessagePortValueCombined msg, T arg) {}
        public void handle_MessagePortInputFormatSingle(MessagePortInputFormatSingle msg, T arg) {}
        public void handle_MessagePortInputFormatCombined(MessagePortInputFormatCombined msg, T arg) {}
        public void handle_MessagePortOutputCommand(MessagePortOutputCommand msg, T arg) {}
    }
}
