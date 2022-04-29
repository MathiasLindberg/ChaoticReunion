using System;

namespace CoreUnityBleBridge
{
    [Serializable]
    public class GattCharacteristic
    {
        public string Uuid;
        public bool NotifyValue;

        public GattCharacteristic(string uuid, bool notifyValue)
        {
            Uuid = uuid;
            NotifyValue = notifyValue;
        }
    }
}