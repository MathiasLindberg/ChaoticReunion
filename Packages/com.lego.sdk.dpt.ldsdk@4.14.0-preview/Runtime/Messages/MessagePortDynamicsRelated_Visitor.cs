namespace dk.lego.devicesdk.bluetooth.V3.messages {
    public interface MessagePortDynamicsRelated_Visitor<T> {
        void handle_MessagePortInputFormatSetupSingle(MessagePortInputFormatSetupSingle msg, T arg);
        void handle_MessagePortInputFormatSetupCombined(MessagePortInputFormatSetupCombined msg, T arg);
        void handle_MessagePortValueCombined(MessagePortValueCombined msg, T arg);
        void handle_MessagePortInputFormatSingle(MessagePortInputFormatSingle msg, T arg);
        void handle_MessagePortInputFormatCombined(MessagePortInputFormatCombined msg, T arg);
        void handle_MessagePortOutputCommand(MessagePortOutputCommand msg, T arg);
    }
}
