namespace LEGODeviceUnitySDK
{
    public enum MotorWithTachoEndState
    {
        /** Drifting (Floating) */
        Drifting = 0,
        /** Holding */
        Holding = 126,
        /** Braking */
        Braking = 127
    };

    public enum MotorWithTachoDrivebaseSide
    {
        None,
        Left, 
        Right
    }

    public enum MotorWithTachoProfileConfiguration
    {
        /** Do not make use of acceleration and deceleration profiles */
        None = 0,
        /** Use acceleration profile when command starts */
        Start = 1,
        /** Use decelration profile when command ends */
        End = 2,
        /** Use both acceleration and decelation profiles */
        Both = 3
    };

    
    
	public interface ILEGOTachoMotor : ILEGOMotorBase
    {
        
        int PowerModeNo { get; }
        int SpeedModeNo { get; }
        int PositionModeNo { get; }
        int AbsolutePositionModeNo { get;  }
        
        LEGOValue Speed { get; }
        LEGOValue Position { get; }
        LEGOValue Power { get; }
        LEGOValue AbsolutePosition { get; }                
        
        bool IsInternalMotor { get; }
    }

    public interface ILEGODualTachoMotor : ILEGOTachoMotor
    {
        
    }

    public interface ILEGOSingleTachoMotor : ILEGOTachoMotor
    {
        
    }
    
    public interface ITachoMotorCommandWithProfileConfig
    {
        //Too bad with just can't declare a property that can be serialized...therefore a method to set it instead
        void SetProfileConfiguration(MotorWithTachoProfileConfiguration profileConfig);
    }

    public interface ITachoMotorCommandWithMaxPower 
    {
        //Too bad with just can't declare a property that can be serialized...therefore a method to set it instead
        void SetMaxPower(int maxPower);
    }

    public interface ITachoMotorCommandWithEndState 
    {
        //Too bad with just can't declare a property that can be serialized...therefore a method to set it instead
        void SetEndState(MotorWithTachoEndState endState);
    }
    
}