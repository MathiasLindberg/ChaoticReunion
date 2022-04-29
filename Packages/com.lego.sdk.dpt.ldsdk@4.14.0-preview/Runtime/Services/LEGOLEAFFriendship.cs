using System;
using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public enum LEGOLEAFFriendshipMode
    {
        FRIENDSHIP_CODE = 0,
        EVENTS = 1
    }
    
    public class LEGOLEAFFriendship : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOLEAFFriendship));
        public override string ServiceName { get { return "LEAF Friendship Service"; } }
        
        public LEGOValue FriendShipCode { get { return ValueForMode((int) LEGOLEAFFriendshipMode.FRIENDSHIP_CODE); } }
        public LEGOValue Events { get { return ValueForMode((int) LEGOLEAFFriendshipMode.EVENTS); } }
        
        internal LEGOLEAFFriendship(Builder builder) : base(builder) 
        {
            // events
            var eventFormat = new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type, (int)LEGOLEAFFriendshipMode.EVENTS, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true);
            UpdateInputFormat(eventFormat);

            // Get Friendship code
            var friendshipFormat = new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type, (int)LEGOLEAFFriendshipMode.FRIENDSHIP_CODE, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true);
            UpdateInputFormat(friendshipFormat);
        }
        
        protected override int DefaultModeNumber { get { return (int)LEGOLEAFFriendshipMode.FRIENDSHIP_CODE; } }
        
        public LEGOValue FriendshipCode { get { return ValueForMode((int) LEGOLEAFFriendshipMode.FRIENDSHIP_CODE); } }
        
        
        public abstract class FriendshipCommand : LEGOServiceCommand
        {
            protected FriendshipCommand()
            {
                MayBeReplaced = false; // default is true - may want to have this as replaceble - 
            }
            private static readonly ICollection<IOType> LEAFFriendship = new List<IOType> { IOType.LEIOTypeLEAFFriendship };

            public override ICollection<IOType> SupportedIOTypes
            {
                get { return LEAFFriendship; }
            }
        }
        
        public class SetFriendshipCommand : FriendshipCommand
        {
            public Int16 FriendshipCode;

            protected override CommandPayload MakeCommandPayload()
            {
                byte[] data = BitConverter.GetBytes(FriendshipCode);
                List<byte> payload = new List<byte>() { (byte)LEGOLEAFFriendshipMode.FRIENDSHIP_CODE };
                payload.AddRange(data);
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE, payload.ToArray());
            }
        }
        
        public class SetEventsCommand : FriendshipCommand
        {
            public Int16 Event1Code, Event2Code;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new PayloadBuilder(4)
                        .Put((byte)LEGOLEAFFriendshipMode.EVENTS)
                        .PutInt16(Event1Code)
                        .PutInt16(Event2Code)
                        .GetBytes());
            }
        }
        
        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEGOLEAFFriendshipMode) newValue.Mode)
            {
                case LEGOLEAFFriendshipMode.FRIENDSHIP_CODE:
                    _delegates.OfType<ILEGOLEAFFriendshipDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateFriendshipCode(this, oldValue, newValue));
                    break;
                case LEGOLEAFFriendshipMode.EVENTS:
                    _delegates.OfType<ILEGOLEAFFriendshipDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateEvents(this, oldValue, newValue));
                    break;
                default:
                    logger.Warn("Received value update for unknown mode: " + newValue.Mode);
                    break;
            }
        }
    }
}