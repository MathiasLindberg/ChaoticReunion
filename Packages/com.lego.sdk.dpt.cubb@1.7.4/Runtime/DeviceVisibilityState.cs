namespace CoreUnityBleBridge
{
    public enum DeviceVisibilityState
    {
        /// <summary>
        /// The device was not seen through BLE discovery very recently.
        /// </summary>
        Invisible = 0,
        /// <summary>
        /// The device was seen through BLE discovery very recently.
        /// </summary>
        Visible = 1,
    }
}