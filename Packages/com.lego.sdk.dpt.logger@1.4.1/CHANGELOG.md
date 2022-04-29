# Changelog
All notable changes to this project will be documented in this file.

## [1.4.0]
### Changed
- Bumped the version to 1.4.0 to signal the change to UPM releases.

## [1.3.0]
### Added
- Added new events to listen to for when a packet was sent, received or dropped.

### Fixed
- Fixed a bug where LogMessage prefabs were being instantiated when editor stopped playing, which created ghost gameobjects in the scene heirachy.

## [1.2.1]
### Added
- The Logger now uses a config file created in the client project

### Changed
- LogManager is now disposable

### Fixed
- Fixed a stack trace source identification error in certain cases.
- Fixed broken prefabs
- Fixed ADB log appender

## [1.2.0]
### Changed
- Project is now structured as a package for use with the Unity Package Manager. See [Package layout](https://docs.unity3d.com/2019.1/Documentation/Manual/cus-layout.html).
