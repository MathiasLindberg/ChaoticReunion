using System;

namespace CoreUnityBleBridge
{
    [Serializable]
    public class GattService
    {
        public string Uuid;
        public GattCharacteristic[] Characteristics;

        public GattService(string uuid, GattCharacteristic[] characteristics)
        {
            Uuid = uuid;
            Characteristics = characteristics;
        }
        
        private static GattService[] gattServices =
        {
            new GattService(Constants.LDSDK.V3_SERVICE, 
                new [] {
                    new GattCharacteristic(Constants.LDSDK.V3_CHARACTERISTIC, true)
                }),
            new GattService(Constants.LDSDK.V3_BOOTLOADER_SERVICE,
                new [] {
                    new GattCharacteristic(Constants.LDSDK.V3_BOOTLOADER_CHARACTERISTIC, true)
                }),
            new GattService(Constants.OAD.SERVICE, 
                new []
                {
                    new GattCharacteristic(Constants.OAD.Characteristics.IMAGE_NOTIFY, true),
                    new GattCharacteristic(Constants.OAD.Characteristics.IMAGE_BLOCK_REQUEST, true),
                    new GattCharacteristic(Constants.OAD.Characteristics.COUNT, false),
                    new GattCharacteristic(Constants.OAD.Characteristics.STATUS, false),
                    new GattCharacteristic(Constants.OAD.Characteristics.CONTROL, true)
                })
        };

        public static GattService[] AllServices => gattServices;
    }
}