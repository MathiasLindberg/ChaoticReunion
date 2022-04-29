using System;

namespace LEGODeviceUnitySDK
{
    public class LEGOManufacturerData
    {
        public int Length { get; private set; }
        public byte DataTypeName { get; private set; }
        public UInt16 Identifier { get; private set; }
        public byte ButtonState { get; private set; }
        public byte SystemTypeAndDeviceNumber { get; private set; }
        public byte Capabilities { get; private set; }
        public byte LastNetwork { get; private set; }
        public byte Status { get; private set; }
        public byte Option { get; private set; }
        
        internal void SetLength(int value) {this.Length = value;}
        internal void SetDataTypeName(byte value) {this.DataTypeName = value;}
        internal void SetIdentifier(UInt16 value) {this.Identifier = value;}
        internal void SetButtonState(byte value) {this.ButtonState = value;}
        internal void SetSystemTypeAndDeviceNumber(byte value) {this.SystemTypeAndDeviceNumber = value;}
        internal void SetCapabilities(byte value) {this.Capabilities = value;}
        internal void SetLastNetwork(byte value) {this.LastNetwork = value;}
        internal void SetStatus(byte value) {this.Status = value;}
        internal void SetOption(byte value) {this.Option = value;}

        public LEGOManufacturerData()
        {
            
        }
        public LEGOManufacturerData(LEGOManufacturerData data)
        {
            if(data != null)
            {
                Length = data.Length;
                DataTypeName = data.DataTypeName;
                Identifier = data.Identifier;
                ButtonState = data.ButtonState;
                SystemTypeAndDeviceNumber = data.SystemTypeAndDeviceNumber;
                Capabilities = data.Capabilities;
                LastNetwork = data.LastNetwork;
                Status = data.Status;
                Option = data.Option;
            }
        }
        
        public override string ToString()
        {
            return string.Format("[LEGODeviceInfo: Length={0}, Data Type Name={1}, Identifier={2}, Button State={3}, SystemType And DeviceNumber={4}, Capabilities={5}, Last Network={6}, Status={7}, Option={8}]", Length, DataTypeName, Identifier, ButtonState, SystemTypeAndDeviceNumber,Capabilities,LastNetwork,Status,Option);
        }
    }
}
