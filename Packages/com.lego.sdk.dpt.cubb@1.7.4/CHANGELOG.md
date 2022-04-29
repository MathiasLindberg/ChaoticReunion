# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.7.2] - 2021-03-25
### Changed
- Now using UPM to include the CUBB package in the Unity Package Manager  - Remember to delete the local package present from the old submodule.
- Now using UPM to include the Logger package in the Unity Package Manager - alter dependency for Logger to suit (LPAF-667) - Remember to delete the local package present from the old submodule.
- Now using UPM to include the Build Tools package in the Unity Package Manager  - Remember to delete the local package present from the old submodule.

## [1.7.1] - 2021-02-15

### Fixed
- Fixed a bug that would make the Unity Editor freeze when compiling code changes when connected to a LEGO Device (LPAF-635).

### Dependencies
- logger 1.3.0 - (SHA 7c1265e3792f8e28d5124bfb9be34c20cc923e84)

## [1.7.0] - 2021-02-05
### Changed
- iOS managed to native communication changed from using Unity's builtin UnitySendMessage to a delegate approach (SHA f8bfb229b32594b87d0f111b8fe4605a2a6ab870)
### Fixed
- Fixed a bug where `BleCoroutineRunner` was throwing exceptions when using fake devices in the unity editor. (LPAF-290) (SHA ab9d0702f62e0e753599887bc06012e8cfe1d820)

## [1.6.1] - 2020-10-01
### Added
- Added event listeners for clients  to hook into to get updates for events PacketSent, PacketDropped, and PacketReceived.


## [1.6.0] - 2020-08-24
### Important
- Upgraded to Unity version 2019.4.2f1 


## [1.5.2] - 2020-08-05
### Changed 
- Updated dependency to Logger submodule to version 1.2.1 (commit SHA: f3d45e6f7849ef5143e66fb965429426518c5c77)

### Fixed
- Fixed issue that on some devices were failing to invoke the disconnect event, if the BT adaptor is disabled (LCC-2446)

## [1.5.1] - 2020-07-20
### Fixed
- ANDROID: app crash is OAD transfer is interrupted (eg. batteries out) fixed
- ANDROID: prevent missing device entry from crashing app when calling setNoAckParameters
- iOS: Prepared build postprocessing for Unity LTS 2019+

## [1.5.0] - 2020-04-27
### Added
- Added callback of WillShutDown to handle shutdowns seperately from disconnects.


## [1.4.0] - 2020-02-07
### Changed
- Module has been restructured into a Unity package
- Ability to obtain set MTU size for BLE transfers on Android - greatly speeding the ASSET transfer for LEAF 

## [1.3.0] - 2019-11-27
### Added
- Ability to obtain MTU size for BLE transfers (iOS and OSX only at the moment)

### Fixed
- Changed the SWIFT version set by post-processing scripts


## [1.2.0] - 2019-09-23
### Changed
- Updated SICP logger dependency to 1.1.6
- All log information from the native iOS/MacOS CUBB is now sent to Unity and logged via the SiCP logger
  This means that the native log can be seen alongside the unity log in the SiCP log output. 
- Experimental support for sending assets to LEAF hubs. This works for firmware upgrading and sending other assets.  
  To try it out, select a compatible firwmare file from the LDSDK example FW upgrade view. 
- Changed iOS Logging, to support multiple native loggers. Log level filtering applies. 
- Changed Android Logging, to support multiple native loggers. Log level filtering applies.
- Added a support for native messages being passed to Unity from Android. 
- Added ability to set noack parameters to CUBB

## [1.1.0] - 2019-07-01

### Added
- Updated Unity project version to 2018.4.0f1 LTS, 
  your editor should be updated accordingly.
- Updated scripting backend to .NET 4.x.
- Updated SICP logger dependency to 1.1.5

### Changed
**iOS**
- Updated SWIFT version to 5.0 to be able to target newer devices. 
  It requires Xcode 10.2+ to be installed to link the libraries, 
  your Xcode should be updated accordingly.
- Changed iOS bluetooth implementation to manually disconnect 
  all devices when CBManagerState changes from on to off. 
  Change was made because iOS dosen't report correct callbacks didDisconnectPeripheral 
  when bluetooth is disabled in Control Center (LDSDK-344).
- All log information from the native iOS/MacOS CUBB 
  is now sent to Unity and logged via. the SiCP logger.
  This means that the native log can be seen alongside 
  the unity log in the SiCP log output.

**Android**
- Fixed a bug when using SendStrategy.SoftAck, where too many acks were requested (LDSDK-343).

## [1.0.0] - 2019-01-23
### Added
- Updated Unity project version to 2018.2.11f1, your editor should be updated accordingly.

### Fixed
- When bluetooth is disabled, the device list is now cleared.

## [0.2.0] - 2019-01-21
### Added
- Send strategy is now configurable on a connection basis.  This
  feature is useful for connecting to City hubs, which at present have
  limitations because of hardware issues. Use SendStrategy.HardAck for
  these (the default is SoftAck); in bootloader mode, though, you'll
  need to use SendStrategy.NoAck.

- When SendStrategy.NoAck is in use, you will still get
  PacketTransmitted feedback - though in this mode, the feedback
  carries less guarantee: only that the packet is sent (passed to the
  Bluetooth library), not that it is acknowledged by the other end.

### Changed
- A NoAck send strategy implies timer-based throttling on both iOS and
  Android.

### Fixed
- The Android native part was not thread safe, which might lead to race-condition issues in some situations (LDSDK-152)
- Android API didn't support reliable full duplex on a single GATT characteristic. (LDSDK-199)
  A workaround has been implemented.
- Bluetooth adapter state is missing update on Disabled»StartScanning (LDSDK-150)
- Bluetooth adapter state is wrong after reenabling Bluetooth (LDSDK-151)

## [0.1.0] - 2018-11-14
(Changes: ??? - changelog not updated at the time.)

## [0.0.1] - 2018-11-13
### Added
- Initial Release - Added This CHANGELOG
- This version supports iOS, MacOS and Android (but not yet UWP)
