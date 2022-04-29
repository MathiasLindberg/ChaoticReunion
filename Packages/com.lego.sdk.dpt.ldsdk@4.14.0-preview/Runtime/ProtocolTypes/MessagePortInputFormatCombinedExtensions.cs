using System;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public static class MessagePortInputFormatCombinedExtensions
    {

        public static uint CombinationIndex(this MessagePortInputFormatCombined self) {
            return self.combinedControlByte & 0x0Fu;
        }

        public static bool MultiUpdateEnabled(this MessagePortInputFormatCombined self) {
            return (self.combinedControlByte & 0x80) != 0;
        }
    }
}

