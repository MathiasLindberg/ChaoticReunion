using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// A Service represent an IO made available by the Device, typically some motor or sensor.
    /// See IOType for a list of supported service types
    /// </summary>
    public interface ILEGOService
    {

        /// <summary>
        /// The name of the service is human readable form
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The connect state of the service.
        /// Register for callback on the LEGODevice.OnServiceConnectionChanged to be notified when services are connected/disconnected
        /// </summary>
        ServiceState State { get; }

        /// <summary>
        /// General service meta-data info (type, port, etc).
        /// Info will not change while connected.
        /// </summary>
        LEGOConnectInfo ConnectInfo { get; }

        /// <summary>
        /// The Device this service is associated with
        /// </summary>
        ILEGODevice Device { get; }

        /// <summary>
        /// Returns true if this is an internal service, flase otherwise.
        /// An internal service represents somthing that is build into the Device,
        /// e.g. the Intertal Drivebase Motor (as opposed to the external motor connected by a port)
        /// </summary>
        bool IsInternalService { get; }

        /// <summary>
        /// Gets the default input format for this service.
        /// </summary>
        LEGOInputFormat DefaultInputFormat { get; }

        /// <summary>
        /// Gets the current input format set for this service.
        /// </summary>
        LEGOInputFormat InputFormat { get; }

        /// <summary>
        /// Gets the type of this service (motor, vision sensor, etc. )
        /// </summary>
        IOType ioType { get; }

        /// <summary>
        /// The associated information for all service modes
        /// </summary>
        LEGOAttachedIOModeInformation ModeInformation { get; }

        /// <summary>
        /// The current combined input format for this service. If there isnt any, null is returned
        /// </summary>
        LEGOInputFormatCombined InputFormatCombined { get; }

        /// <summary>
        /// For IOs that supports combined modes, use this method to set up which data you would like.
        /// For instance, a tacho motor may support getting value updates for both speed and encoder at the same time.
        /// </summary>
        void AddCombinedMode(int modeNumber, int deltaInterval);

        /// <summary>
        /// Once you have configured which combined modes you would like using AddCombinedMode above,
        /// use this method to active the changes (e.g. send the message to the Device).
        /// </summary>
        void ActivateCombinedModes();

        /// <summary>
        /// Select and activate a built-in behavior for the service.
        /// Behaviors can e.g. be customized motor controls resulting in shaking or slow reacting motor movement.
        /// Attention: Not all services will support behaviors. In these cases, selecting a behavior will have no effect.
        /// </summary>
        /// <param name="behavior">One of the built-in behaviors. Min: 0 Max: 9</param>
        void ActivateBehaviorNumber(int behavior);

        /// <summary>
        /// Deactivates any active behavior.
        /// </summary>
        void DeactivateActiveBehavior();

        /// <summary>
        /// If the notifications is disabled for the service in the inputFormat through [LEInputFormat notificationsEnabled]
        /// you will have to use this method to request an updated value for the service.
        /// The updated value will be delivered through the delegate method LEGOServiceCallback.DidUpdateValueData.
        /// </summary>
        void SendReadValueRequest();

        /// <summary>
        /// This will send a reset command to the Device for this service. Some services has state, such as a bump-count for a tilt sensor that you may wish to reset.
        /// </summary>
        void SendResetStateRequest();

        /// <summary>
        /// Gets the most recent value for the given mode
        /// </summary>
        LEGOValue ValueForMode(int mode);

        /// <summary>
        /// Set a new mode
        /// </summary>
        void UpdateCurrentInputFormatWithNewMode(int mode);
        
        /// <summary>
        /// Set a new input format for this server.
        ///
        /// Per default no input format is set once a service is discovered, so you will need
        /// to this before being able to recieve updates from the service.
        ///
        /// </summary>
        void UpdateInputFormat(LEGOInputFormat inputFormat);

        /// <summary>
        /// Retrieve mode information associated with a given mode number
        /// </summary>
        /// <param name="mode">The mode to lookup mode information for</param>
        LEGOModeInformation modeInformationForModeNo(int mode);

        /// <summary>
        /// Retrieve mode information associated with a given mode name
        /// </summary>
        /// <param name="modeName"> The mode to lookup mode information for</param>
        LEGOModeInformation modeInformationForModeName(string modeName);

        /// <summary>
        /// Reset added configurations and service combined mode setup
        /// When this method is called all added configurations are cleared, and a combine mode reset is sent to the Hub.
        /// </summary>
        void ResetCombinedModesConfiguration();

        void RegisterDelegate(ILEGOServiceDelegate legoServiceDelegate);

        void UnregisterDelegate(ILEGOServiceDelegate legoServiceDelegate);

        /// <summary>
        /// Sends a Service Command and performs the completionAction once
        /// the command has completed.
        ///
        /// If RequestFeedback is set to false on the command, the completionAction
        /// will be executed right away. If RequestFeedback is set to tru, the
        /// completionAction is executed once the Hub signals back that the command has completed.
        /// Example: Sending a MotorSpeedForDuration(speed, seconds) with RequestFeedback true means
        /// that the completionAction will be excuted once the motor has completed running the specified duration.
        ///
        /// If the command completed (was not interrupted) the bool-parameter for the
        /// completionAction is true.
        /// </summary>
        void SendCommand(LEGOServiceCommand command, Action<bool> completionAction);

        /// <summary>
        /// Sends a Service Command without a completionAction.
        /// </summary>
        void SendCommand(LEGOServiceCommand command);

    }

    public enum ServiceState
    {
        Connected,
        Disconnected,
    }


}

