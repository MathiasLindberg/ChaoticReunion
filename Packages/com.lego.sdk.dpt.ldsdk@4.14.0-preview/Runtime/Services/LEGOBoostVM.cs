using System;
using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGOBoostVM : LEGOService, ILEGOBaseVM
    {
        public enum Mode
        {
            Status   = 0,
            Canvas   = 1,
            Variable = 2
        }

        public override string ServiceName { get { return "BoostVM"; } }
        internal LEGOBoostVM(Builder builder) : base(builder) 
        {
            // Strip triggered
            var stripTriggeredFormat = new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type, (int)LEGOBoostVM.Mode.Status, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitSI, true);
            UpdateInputFormat(stripTriggeredFormat);

            // Get Variable 
            var getVariableFormat = new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type, (int)LEGOBoostVM.Mode.Variable, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitSI, true);
            UpdateInputFormat(getVariableFormat);
        }

        public abstract class VMCommand : LEGOServiceCommand
        {
            protected VMCommand()
            {
                MayBeReplaced = false; // default is true
            }
            private static readonly ICollection<IOType> BoostVM = new List<IOType> { IOType.LEIOTypeBoostVM };

            public override ICollection<IOType> SupportedIOTypes
            {
                get { return BoostVM; }
            }
        }

        public class SelectCanvasCommand : VMCommand
        {

            public int CanvasId;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                                          new byte[] { (byte)Mode.Canvas, (byte)CanvasId });
            }
        }

        public class SetVariableCommand : VMCommand
        {
            public byte Address;
            public Int32 Data;

            protected override CommandPayload MakeCommandPayload()
            {
                byte[] data = BitConverter.GetBytes(Data);
                List<byte> payload = new List<byte>() { (byte)Mode.Variable, Address };
                payload.AddRange(data);
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                                          payload.ToArray());
            }
        }

        public class SubscribeToVariableCommand : VMCommand
        {
            public byte Address;

            protected override CommandPayload MakeCommandPayload()
            {
                List<byte> payload = new List<byte>() { (byte)Mode.Variable, Address };
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                                          payload.ToArray());
            }
        }

        public class TriggerStripCommand : VMCommand
        {
            public byte StripId;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                                          new byte[] { (byte)Mode.Status, StripId });
            }
        }

    }
}