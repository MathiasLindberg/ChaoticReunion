using System;

namespace CoreUnityBleBridge
{
    /// <summary>
    /// Flags governing the details of packet sending, especially with respect to the packet replacement policy.
    /// For details on how these flags affect sending and queuing, see the "Packet replacement" section in /doc/CUBB-semantics.md .
    /// </summary>
    [Flags]
    public enum SendFlags
    {
        None = 0,
        /// <summary>
        /// This packet should not be replaced (overwritten or removed) by the packet replacement algorithm.
        /// </summary>
        NonReplaceable = 0x01,
        /// <summary>
        /// This packet should not overtake others when it is added to the send queue.
        /// </summary>
        NonOvertaking  = 0x02,
    }
}