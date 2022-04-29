namespace CoreUnityBleBridge
{
    public enum DeviceConnectionState
    {
        /// <summary>
        /// We are not connected to the device in question.
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// We are not connected to the device in question, but a process to establish a connection is in progress.
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// We are presently connected to the device in question.
        /// </summary>
        Connected = 2,
    }
}