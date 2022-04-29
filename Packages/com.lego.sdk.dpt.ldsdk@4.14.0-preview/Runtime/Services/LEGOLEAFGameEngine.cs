using System;
using System.Collections.Generic;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public enum LEGOLEAFGameEngineMode
    {
        CHAL = 0,
        VERS = 1,
        EVENTS = 2
    }
    // TODO - when(if?!) the game engine service interface is defined there is much work to do here and in ILEGOLEAFGameEngineDelegate
    public class LEGOLEAFGameEngine : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOLEAFGameEngine));
        public override string ServiceName { get { return "LEAF Game Engine"; } }

        internal LEGOLEAFGameEngine(LEGOService.Builder builder) : base(builder) { }
        public LEGOValue Assets { get { return ValueForMode((int) LEGOLEAFGameEngineMode.VERS); } }
        public abstract class GameEngineCommand : LEGOServiceCommand
        {
            private static readonly ICollection<IOType> LEAFGameEngineOnly = new List<IOType> { IOType.LEIOTypeLEAFGameEngine };
            public override ICollection<IOType> SupportedIOTypes { get { return LEAFGameEngineOnly; } }
        }     

        protected override int DefaultModeNumber { get { return (int)LEGOLEAFGameEngineMode.VERS; } }
        
        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEGOLEAFGameEngineMode) newValue.Mode)
            {
                case LEGOLEAFGameEngineMode.CHAL:
                    _delegates.OfType<ILEGOLEAFGameEngineDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateChallenges(this, oldValue, newValue));
                    break;
                case LEGOLEAFGameEngineMode.VERS:
                    _delegates.OfType<ILEGOLEAFGameEngineDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateVersions(this, oldValue, newValue));
                    break;
                case LEGOLEAFGameEngineMode.EVENTS:
                    _delegates.OfType<ILEGOLEAFGameEngineDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateEvents(this, oldValue, newValue));
                    break;
                default:
                    logger.Warn("Received value update for unknown mode: " + newValue.Mode);
                    break;
            }
        }

        #region Internal
        public class SetChallengesCommand : GameEngineCommand
        {
            public UInt16 ID;
            public UInt16 Status;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new PayloadBuilder(5)
                        .Put((byte) LEGOLEAFGameEngineMode.CHAL)
                        .PutUInt16(ID)
                        .PutUInt16(Status)
                        .GetBytes());
            }
        }
        
        #endregion
        
    }
}