using LEGODeviceUnitySDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TachoMotor : Motor
{
    public TachoMotorMode Mode { get => mode; set { if (mode != value) { mode = value; UpdateInputFormat(); } } }

    public int Position
    {
        get => position; // Degrees.
        private set
        {
            position = value;
            PositionChanged.Invoke(position);
        }
    }

    public int Speed
    {
        get => speed; // Unsure.
        private set
        {
            speed = value;
            SpeedChanged.Invoke(speed);
        }
    }

    public int Power
    {
        get => power; // Unsure.
        private set
        {
            power = value;
            PowerChanged.Invoke(power);
        }
    }

    public UnityEvent<int> PositionChanged;
    public UnityEvent<int> SpeedChanged;
    public UnityEvent<int> PowerChanged;

    public void GoToPosition(int pos, bool brake = false, int speed = 100)
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        var cmd = new LEGOTachoMotorCommon.SetSpeedPositionCommand()
        {
            Position = pos,
            Speed = speed
        };
        cmd.SetEndState(brake ? MotorWithTachoEndState.Braking : MotorWithTachoEndState.Drifting);
        motor.SendCommand(cmd);
    }

    public void HoldPosition(int pos)
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        var cmd = new LEGOTachoMotorCommon.SetSpeedPositionCommand()
        {
            Position = pos,
            Speed = 100
        };
        cmd.SetEndState(MotorWithTachoEndState.Holding);
        motor.SendCommand(cmd);
    }

    public void SpinForTime(int time, int speed = 100, bool brake = false)
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        var cmd = new LEGOTachoMotorCommon.SetSpeedMilliSecondsCommand()
        {
            MilliSeconds = time,
            Speed = speed
        };
        cmd.SetEndState(brake ? MotorWithTachoEndState.Braking : MotorWithTachoEndState.Drifting);
        motor.SendCommand(cmd);
    }

    public void Hold()
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        var stopCmd = new LEGOTachoMotorCommon.HoldCommand(); // max speed?
        motor.SendCommand(stopCmd);
    }

    public void ResetPosition()
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        var resetCmd = new LEGOTachoMotorCommon.PresetEncoderCommand()
        {
            Preset = 0
        };
        motor.SendCommand(resetCmd);
    }

    #region internals
    [Flags]
    public enum TachoMotorIOTypes
    {
        Clear = 0,
        MotorWithTacho = 1 << 3,
        InternalMotorWithTacho = 1 << 4,
        TechnicAzureAngularMotorS = 1 << 5,
        TechnicAzureAngularMotorM = 1 << 6,
        TechnicAzureAngularMotorL = 1 << 7,
        TechnicGreyAngularMotorM = 1 << 8,
        TechnicGreyAngularMotorL = 1 << 9,
        TechnicMotorL = 1 << 10,
        TechnicMotorXL = 1 << 11,
        Any = ~0
    }
    public enum TachoMotorIOTypesWithoutClear // could be done with obsolete but it will say Clear (Obsolete)
    {
        Any = ~0,
        MotorWithTacho = 1 << 3,
        InternalMotorWithTacho = 1 << 4,
        TechnicAzureAngularMotorS = 1 << 5,
        TechnicAzureAngularMotorM = 1 << 6,
        TechnicAzureAngularMotorL = 1 << 7,
        TechnicGreyAngularMotorM = 1 << 8,
        TechnicGreyAngularMotorL = 1 << 9,
        TechnicMotorL = 1 << 10,
        TechnicMotorXL = 1 << 11
    }
    public enum TachoMotorMode { Power, Speed, Position, AbsolutePosition, SpeedAndPosition }
    private new ILEGOTachoMotor motor;
    [SerializeField] private TachoMotorMode mode = TachoMotorMode.Power;
    private int position;
    private int speed;
    private int power;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        if (!base.Setup(services))
        {
            return false;
        }
        motor = base.motor as ILEGOTachoMotor;
        if (motor == null)
        {
            // should not happen
            IsConnected = false;
            base.motor.UnregisterDelegate(this);
            base.motor = null;
            Debug.LogWarning(name + " service is not tacho motor");
            return false;
        }
        UpdateInputFormat();
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    private void UpdateInputFormat()
    {
        if (motor == null)
        {
            return;
        }

        if (mode == TachoMotorMode.SpeedAndPosition)
        {
            motor.ResetCombinedModesConfiguration();
            motor.AddCombinedMode((int)MotorWithTachoMode.Speed, 1);
            motor.AddCombinedMode((int)MotorWithTachoMode.Position, 1);
            motor.ActivateCombinedModes();
            return;
        }

        int modeNo = motor.DefaultInputFormat.Mode;
        switch (mode)
        {
            case TachoMotorMode.Power: modeNo = motor.PowerModeNo; break;
            case TachoMotorMode.Speed: modeNo = motor.SpeedModeNo; break;
            case TachoMotorMode.Position: modeNo = motor.PositionModeNo; break;
            case TachoMotorMode.AbsolutePosition: modeNo = motor.AbsolutePositionModeNo; break;
        }
        motor.UpdateInputFormat(new LEGOInputFormat(motor.ConnectInfo.PortID, motor.ioType, modeNo, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
    }

    public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
    {
        if (newState == ServiceState.Disconnected)
        {
            motor = null;
        }
        base.DidChangeState(service, oldState, newState);
    }

    public override void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue)
    {
        if (newValue.Mode == motor.PositionModeNo)
        {
            Position = (int)newValue.RawValues[0];
        }
        else if (newValue.Mode == motor.AbsolutePositionModeNo)
        {
            Position = (int)newValue.RawValues[0];
        }
        else if (newValue.Mode == motor.SpeedModeNo)
        {
            Speed = (int)newValue.RawValues[0];
        }
        else if (newValue.Mode == motor.PowerModeNo)
        {
            Power = (int)newValue.RawValues[0];
        }
    }
    #endregion
}
