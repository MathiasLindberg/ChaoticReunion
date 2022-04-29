# Unity LEGO Wireless SDK

This package contains the LEGO Wireless SDK for Unity.
The following documents specific things to keep in mind, 
when making use of this package on a given platofrm.

## iOS
* None

## Mac OS X
* The SDK currently only works with 64-bit targets. Make sure to select the "x86_64" architecture in Unity build settings.

## Android
* Make sure to include following lines in the `AndroidManifest.xml` file in your project in order to request access to device’s Bluetooth capabilities:

		<uses-permission android:name="android.permission.BLUETOOTH"/>
		<uses-permission android:name="android.permission.BLUETOOTH_ADMIN"/>
		<!-- Since Android 6 (API 23) and all later versions, the ACCESS_COARSE_LOCATION permission is required in order to read the scanresult -->
		<uses-permission-sdk-23 android:name="android.permission.ACCESS_COARSE_LOCATION"/>
		<uses-feature android:name="android.hardware.bluetooth_le" android:required="true"/>

## Windows Store App 8.1 (Metro)
### Pairing
* Because of how Bluetooth Low Energy works in Windows 8.1, a LEGO device needs to be paired with the Windows device in order to be discoverable by it. This can be achieved by going to Settings -> Bluetooth on the Windows device and tapping on the LEGO device you wish to pair with.

### Configuration

* The following lines must be added to the Package.appxmanifest project file (part of the WSA/WP8 player build) within the root `<Package .../>` element:

		<Capabilities>
		  <m2:DeviceCapability Name="bluetooth.genericAttributeProfile">
		    <m2:Device Id="any">
		      <m2:Function Type="serviceId:00001523-1212-efde-1523-785feabcd123" />
		      <m2:Function Type="serviceId:00004f0e-1212-efde-1523-785feabcd123" />
		      <m2:Function Type="serviceId:0000180a-0000-1000-8000-00805f9b34fb" />
		      <m2:Function Type="serviceId:0000180f-0000-1000-8000-00805f9b34fb" />
		    </m2:Device>
		  </m2:DeviceCapability>
		</Capabilities>

### Build Errors
* **Problem** When building a Windows Store or Windows Phone 8 player in Unity the process would almost certainly fail with the following error:
	
		"Error building Player: Exception: Failed to run Serialization weaver with cmdline (...)
		[...]
		Mono.Cecil.AssemblyResolutionException: Failed to resolve assembly: 'Windows, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null' (...)"

	**Fix** Press the "Player Settings..." button in the "Build Settings" window and open the "Publishing Settings" section. Expand "Unprocessed Plugins" under "Misc" and increase the "Size" field value by 1. Then press Tab and fill the extra entry with `LegoDeviceSDK.dll`.

	**Explanation** Unity's internal "serialization weaver" tool fails to process the `LegoDeviceSDK.dll` assembly which references Windows 8 Bluetooth API. This tool seems to only perform relevant actions for assemblies that contain Unity-related game logic. This is not the case here, thus excluding LegoDeviceSDK.dll is safe. 

* **Problem** Building the generated Visual Studio project will fail with the following error:
		
		"1>  System.IO.FileNotFoundException: Assembly "LegoDeviceSDK, Version=2.4.0.0, Culture=neutral, PublicKeyToken=null" file not found."

	**Fix** Open project properties -> Build Events -> Edit Pre-build and scroll to the bottom. The penultimate line should end with something similar to `$(ProjectDir)\WinRTLegacy.dll`. Append additional entry: `"$(ProjectDir)\LegoDeviceSDK.dll"` (including quotations marks).
	
	The project should build successfully at this point.

## Universal Windows Platform (Windows SDK 10)
* The same Serialization Weaver error as in WSA 8.1 will occur on UWP as well unless the `LegoDeviceSDK.UWP.dll` assembly is marked as unprocessed. This should be done automatically after importing the SDK package into a Unity project. If this is not the case, please consult instructions for WSA 8.1 and do it manually.

* The UWP application must have the Bluetooth capability enabled in the package manifest.

* In-app pairing of WeDo hubs is possible in UWP but takes a long time (anywhere from 30 seconds to 1 minute). This is a platform limitation though, i.e. pairing in OS Bluetooth Settings takes the same amount of time.

## Windows 7 (Bluegiga BLED112 USB dongle)
* Set "Api Compatibility Level" to ".NET 2.0" in Player Settings -> Other Settings -> Optimization.
* Set the BLED112 dongle port to the correct value in LEGOPlatformUtils ("COM3" by default). This value might differ from machine to machine thus it is best to allow the user to set this value on runtime. In the future the wrapper will support automatic port detection as implemented in the native Windows 7 SDK.
