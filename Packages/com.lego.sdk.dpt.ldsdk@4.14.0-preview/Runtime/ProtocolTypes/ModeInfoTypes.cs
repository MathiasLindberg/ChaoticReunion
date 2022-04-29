using System;

namespace LEGODeviceUnitySDK {

    /// <summary>
    /// Bitmask for use in MessagePortInformationModeInfo messages.
    /// </summary>
    public enum ServiceCapabilities {
        Output         = 1<<0,
        Input          = 1<<1,
        Combinable     = 1<<2,
        Synchronizable = 1<<3
    }
}

