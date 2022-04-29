using System.Collections.Generic;
using System.Linq;


namespace LEGODeviceUnitySDK
{
    public class LEGOConnectInfo
    {
        public int PortID { get; private set; }

        private const int FIRST_INTERNAL_PORT_ID = 50;
        public bool IsInternal { get { return PortID >= FIRST_INTERNAL_PORT_ID; } }

        public LEGORevision HardwareRevision { get; private set; }

        public LEGORevision SoftwareRevision { get; private set; }

        public IOType Type { get; private set; }

        public bool VirtualConnection { get; private set; }

        public int PortID1 { get; private set; }
        public int PortID2 { get; private set; }

        public LEGOConnectInfo(int portID, LEGORevision hardwareRevision, LEGORevision softwareRevision, IOType type, bool virtualConnection, int portID1, int portID2)
        {
            this.PortID = portID;
            this.HardwareRevision = hardwareRevision;
            this.SoftwareRevision = softwareRevision;
            this.Type = type;
            this.VirtualConnection = virtualConnection;
            this.PortID1 = portID1;
            this.PortID2 = portID2;
        }
    }

    public static class IOTypes
    {
        public static readonly ICollection<IOType> PercentageSetableLights = new List<IOType>
        {
            IOType.LEIOTypeLight,
            //IOType.LEIOTypeTechnicColorSensor, Not enabled. Consider whether its a good idea to allow the color sensor to turn off the light, since color readings might be less accurate
            IOType.LEIOTypeTechnicDistanceSensor
        };

        public static readonly ICollection<IOType> ColorSensors = new List<IOType>
        {
            IOType.LEIOTypeVisionSensor,
            IOType.LEIOTypeDuploTrainColorSensor,
            IOType.LEIOTypeDBotColorSensor
        };
        
        public static readonly ICollection<IOType> AbsolutePositionMotors = new List<IOType>
        {
            IOType.LEIOTypeTechnicAzureAngularMotorS,
            IOType.LEIOTypeTechnicAzureAngularMotorM,
            IOType.LEIOTypeTechnicAzureAngularMotorL,
            IOType.LEIOTypeTechnicGreyAngularMotorM,
            IOType.LEIOTypeTechnicGreyAngularMotorL,
            IOType.LEIOTypeTechnicMotorL,
            IOType.LEIOTypeTechnicMotorXL
        };

        public static readonly ICollection<IOType> TachoMotors = new List<IOType>
        {
            IOType.LEIOTypeMotorWithTacho,
            IOType.LEIOTypeInternalMotorWithTacho,
            IOType.LEIOTypeTechnicAzureAngularMotorS,
            IOType.LEIOTypeTechnicAzureAngularMotorM,
            IOType.LEIOTypeTechnicAzureAngularMotorL,
            IOType.LEIOTypeTechnicGreyAngularMotorM,
            IOType.LEIOTypeTechnicGreyAngularMotorL,
            IOType.LEIOTypeTechnicMotorL,
            IOType.LEIOTypeTechnicMotorXL
        };

        public static readonly ICollection<IOType> PowerOnlyMotors = new List<IOType>
        {
            IOType.LEIOTypeMotor,
            IOType.LEIOTypeTrainMotor,
            IOType.LEIOTypeDTMotor
        };

        public static readonly ICollection<IOType> ExternalSensors = new List<IOType>
        {
            IOType.LEIOTypeMotionSensor,
            IOType.LEIOTypeVisionSensor,
            IOType.LEIOTypeMoveSensor
        };
        
        public static readonly ICollection<IOType> RGBLights = new List<IOType>
        {
            IOType.LEIOTypeRGBLight
        };

        public static readonly ICollection<IOType> LEAFServices = new List<IOType>
        {
            IOType.LEIOTypeLEAFGameEngine,
            IOType.LEIOTypeLEAFGesture,
            IOType.LEIOTypeLEAFDisplay,
            IOType.LEIOTypeLEAFPants,
            IOType.LEIOTypeLEAFTag,
            IOType.LEIOTypeLEAFFriendship
            
        };

        public static readonly ICollection<IOType> AllMotors = TachoMotors.Concat(PowerOnlyMotors).ToList();

        public static readonly ICollection<IOType> HasUARTConnection = TachoMotors.Concat(ExternalSensors).ToList();

        public static readonly ICollection<IOType> All = new List<IOType>
        {
            IOType.LEIOTypeGeneric,
            IOType.LEIOTypeMotor,
            IOType.LEIOTypeTrainMotor,
            IOType.LEIOTypeLight,
            IOType.LEIOTypeVoltage,
            IOType.LEIOTypeCurrent,
            IOType.LEIOTypePiezoTone,
            IOType.LEIOTypeRGBLight,
            IOType.LEIOTypeTiltSensor,
            IOType.LEIOTypeMotionSensor,
            IOType.LEIOTypeVisionSensor,
            IOType.LEIOTypeMotorWithTacho,
            IOType.LEIOTypeInternalMotorWithTacho,
            IOType.LEIOTypeInternalTiltSensorThreeAxis,
            IOType.LEIOTypeDTMotor,
            IOType.LEIOTypeSoundPlayer,
            IOType.LEIOTypeDuploTrainColorSensor,
            IOType.LEIOTypeMoveSensor,
            IOType.LEIOTypeTechnicMotorL,
            IOType.LEIOTypeTechnicMotorXL, 
            IOType.LEIOTypeTechnicAzureAngularMotorM, 
            IOType.LEIOTypeTechnicAzureAngularMotorL,
            IOType.LEIOTypeTechnicGreyAngularMotorM,
            IOType.LEIOTypeTechnicGreyAngularMotorL,
            IOType.LEIOTypeTechnicAzureAngularMotorS,
            IOType.LEIOTypeTechnic3AxisAccelerometer,
            IOType.LEIOTypeTechnic3AxisGyroSensor,
            IOType.LEIOTypeTechnic3AxisOrientationSensor,
            IOType.LEIOTypeTechnicTemperatureSensor,
            IOType.LEIOTypeTechnicForceSensor,
            IOType.LEIOTypeTechnicColorSensor,
            IOType.LEIOTypeTechnicDistanceSensor,
            IOType.LEIOTypeRemoteControlButtonSensor,
            
            IOType.LEIOTypeLEAFGameEngine,
            IOType.LEIOTypeLEAFGesture,
            IOType.LEIOTypeLEAFDisplay,
            IOType.LEIOTypeLEAFPants,
            IOType.LEIOTypeLEAFTag,
            IOType.LEIOTypeLEAFFriendship
        };


        public static readonly ICollection<IOType> AllEvenUnknown = Enumerable.Range(0, 256).Cast<IOType>().ToList();

    }


    //When adding a new IOType here please also add it to the IOType small lib
    //StreamingAssets/LCC/EnumLibrary/IOType - add entry in enum.json and add name to .HowToGenerate.txt and run it
    //Considered creating the small lib dynamically but that isn't possible with current small lib setup and would require image generation 
    public enum IOType
    {
        LEIOTypeGeneric = 0,
        LEIOTypeMotor = 1,
        LEIOTypeTrainMotor = 2,
        LEIOTypeLight = 8,
        LEIOTypeVoltage = 20,
        LEIOTypeCurrent = 21,
        LEIOTypePiezoTone = 22,
        LEIOTypeRGBLight = 23,
        LEIOTypeTiltSensor = 34,
        LEIOTypeMotionSensor = 35,
        LEIOTypeVisionSensor = 37,
        LEIOTypeMotorWithTacho = 38,
        LEIOTypeInternalMotorWithTacho = 39,
        LEIOTypeInternalTiltSensorThreeAxis = 40,
        LEIOTypeDTMotor = 41,
        LEIOTypeSoundPlayer = 42,
        LEIOTypeDuploTrainColorSensor = 43,
        LEIOTypeMoveSensor = 44,
        // Technic peripherals:
        LEIOTypeTechnicMotorL = 46,
        LEIOTypeTechnicMotorXL = 47,

        // flipper motors
        // medium right angled drive motor        
        LEIOTypeTechnicAzureAngularMotorM = 48,
        // large right angled drive motor
        LEIOTypeTechnicAzureAngularMotorL = 49,
        
        
        LEIOTypeTechnicGreyAngularMotorM = 75,
        
        LEIOTypeTechnicGreyAngularMotorL = 76,
        
        //Gecko tacho motor
        LEIOTypeTechnicAzureAngularMotorS = 65,
        
        LEIOTypeRemoteControlButtonSensor = 55,

        LEIOTypeTechnic3AxisAccelerometer = 57,
        LEIOTypeTechnic3AxisGyroSensor = 58,
        LEIOTypeTechnic3AxisOrientationSensor = 59,
        LEIOTypeTechnicTemperatureSensor = 60,
        LEIOTypeTechnicColorSensor = 61,
        LEIOTypeTechnicDistanceSensor = 62,
        LEIOTypeTechnicForceSensor = 63,
        
        LEIOTypeBoostVM = 66,
        
        LEIOTypeDBotColorSensor = 68,
        
        LEIOTypeLEAFGameEngine = 70,
        LEIOTypeLEAFGesture = 71,
        LEIOTypeLEAFDisplay = 72,
        LEIOTypeLEAFTag = 73,
        LEIOTypeLEAFPants = 74,
        LEIOTypeLEAFFriendship = 85
    }

    /*
    public enum IOSubType
    {
        LEGOIOTypeTiltAllSensor = 0,
        LEGOIOTypeTiltFWDSensor,
        LEGOIOTypeTiltSideSensor
    }
    */
}