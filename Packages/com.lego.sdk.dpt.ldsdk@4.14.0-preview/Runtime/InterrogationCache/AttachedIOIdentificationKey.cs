using System;
using dk.lego.devicesdk.device;

namespace LEGODeviceUnitySDK
{
    public class AttachedIOIdentificationKey
    {

        private readonly IOType typeID;
        private readonly LEGORevision firmwareRevision;
        private readonly LEGORevision hardwareRevision;

        public AttachedIOIdentificationKey(IOType typeID, LEGORevision firmwareRevision, LEGORevision hardwareRevision)
        {
            this.typeID = typeID;
            this.firmwareRevision = firmwareRevision;
            this.hardwareRevision = hardwareRevision;
        }

        public String getIdentifier() {
            return ((int)typeID) + "-" + firmwareRevision + "-" + hardwareRevision;
        }

    }
}

