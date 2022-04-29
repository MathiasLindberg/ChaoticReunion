using System;
using System.Diagnostics.CodeAnalysis;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class BleSettings
    {
        private static readonly ILog Logger = LogManager.GetLogger<BleSettings>();
        
        public readonly Filtering Filter = new Filtering();
        public bool EnableSelfCheck = false;

        public BleSettings()
        {
        }

        public BleSettings(Filtering filter)
        {
            Filter = filter;
        }

        public class Filtering
        {
            public GattService[] GattServices = { };

            public override string ToString()
            {
                var value = "{\"GattServices\":[";

                var serviceCount = 0;
                foreach (var service in GattServices)
                {
                    if (serviceCount == 0)
                    {
                        value += "{";
                    }
                    else
                    {
                        value += ",{";
                    }

                    serviceCount += 1;

                    value += "\"Uuid\":\"" + service.Uuid + "\"";
                    value += ",";

                    value += "\"Characteristics\":[";
                    var charCount = 0;
                    foreach (var characteristic in service.Characteristics)
                    {
                        if (charCount == 0)
                        {
                            value += "{";
                        }
                        else
                        {
                            value += ",{";
                        }

                        charCount += 1;

                        value += "\"Uuid\":\"" + characteristic.Uuid + "\"";
                        value += ",";
                        value += "\"NotifyValue\":\"" + characteristic.NotifyValue + "\"";
                        value += "}";
                    }

                    value += "]";

                    value += "}";
                }

                value += "]}";

                //Logger.Debug("BleSettings::GattServices:ToString " + value);
                return value;
            }
        }
    }
}