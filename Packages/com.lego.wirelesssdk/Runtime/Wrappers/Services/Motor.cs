using LEGODeviceUnitySDK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Motor : ServiceBase
{
    public int typeBitMask = -1;
    public bool virtualConn = false; // applies only to tacho motors

    public void SetPower(int power) // -100 to 100.
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        motor.SendCommand(motor.SetPowerCommand(Mathf.Clamp(power, -100, 100)));
    }

    public void Brake()
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        motor.SendCommand(motor.BrakeCommand());
    }

    public void Drift()
    {
        if (motor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        motor.SendCommand(motor.DriftCommand());
    }

    #region internals
    [Flags]
    public enum MotorIOTypes
    {
        Clear = 0,
        Motor = 1 << 0,
        TrainMotor = 1 << 1,
        DTMotor = 1 << 2,
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

    public enum MotorIOTypesWithoutClear // could be done with obsolete but it will say Clear (Obsolete)
    {
        Any = ~0,
        Motor = 1 << 0,
        TrainMotor = 1 << 1,
        DTMotor = 1 << 2,
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
    protected ILEGOMotorBase motor;

    private bool MatchingService(ILEGOService service)
    {
        if (virtualConn != service.ConnectInfo.VirtualConnection || (port != -1 && port != service.ConnectInfo.PortID) || service.ServiceName == "Undefined")
        {
            return false;
        }
        MotorIOTypes t;
        if (!Enum.TryParse(service.ioType.ToString().Substring("LEIOType".Length), out t))
        {
            return false;
        }
        return (typeBitMask & (int)t) == (int)t;
    }

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        IEnumerable<IOType> types = GetTypes();
        motor = services.FirstOrDefault(MatchingService) as ILEGOMotorBase;
        if (motor == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(motor);
        motor.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    private IEnumerable<IOType> GetTypes()
    {
        return Enum.GetValues(typeof(MotorIOTypes)).Cast<MotorIOTypes>()
            .Where(t => (typeBitMask & (int)t) == (int)t)
            .Select(t => (IOType)Enum.Parse(typeof(IOType), "LEIOType" + t.ToString()));
    }

    private void OnDestroy()
    {
        motor?.UnregisterDelegate(this);
    }

    public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
    {
        if (newState == ServiceState.Disconnected)
        {
            Debug.LogWarning(name + " disconnected");
            motor.UnregisterDelegate(this);
            motor = null;
            IsConnected = false;
        }
    }
    #endregion  
}
