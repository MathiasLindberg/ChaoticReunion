# Unity Logger

## Introduction

The LEGO Logger is a standard log framework with support for log-categories (eg debug, verbose, off) and provides a suite of built-in appenders for logging to file, android adb, consoles, and log-window overlay within a Unity application.

Logger is setup with the appropriate Appenders which format and possibly present log messages as they are injected into the system. Log messages are injected into the system via either calling a generic message log method with an appropriate log category specified, or via specific log message calls for the various categories. 

All Unity layers in LPAF can and do use logger so in essence logger is a cross layer facility. 

Logger can register to recieve Unity logs and process and format these to the appenders as instantiated and added to logger at runtime.

Logger is written entirely in C# and is dependant on Unity facilities. There are message creators written in native code specifically created for CUBB to send log messages across the native to C# boundary. These are written in Swift (iOS/OSX) and Java (Android).

## Installation

In the "main" C# class file (eg LDSDKExampleApp) the Logger system is initialised and "Appenders" added to the LogManager as required. Appenders are formatters of log input to output in the various forms available (see later)

Access to the logger package is via the Unity Package Manager system (UPM). To access the LEGO internal scoped registery contianing the logger package it is necessary to add the scoped registery to  your Unity apps manifest.json file.

    {
        "scopedRegistries": 
        [
            {
            "name": "LEGO Private",
            "url": "https://upm.services.lego.com/private",
            "scopes":
            [
                "com.lego.sdk.dpt"
            ]
            }
        ],
        "dependencies": 
        {
        ....
        }
    }

An access token to the LEGO upm registry is also required in the form of a .toml file. These can be obtained from DPT.

To incorporate logger into your Unity project from the Editor go to Window->PackageManager and select the LEGO registry. From there select the logger package and press the "install" button at the bottom right of the LEGO Logger package description window. 

### Using Declarations

#### for code wishing to log and for initialisation

    using LEGO.Logger;

#### for logger initialisation at app startup

    using LEGO.Logger.Appenders;

#### Initialisation Code Example 

    // set the current operative log level - can be changed at any point. Emits a OnLogLevelChanged event which can be subscribed to from LogManager.
    LogManager.RootLevel = LogLevel.OFF;

    // switch listening to Unity exceptions on - see UnityLogListener for details on logged error types 
    LogManager.ListenOnUnityExceptions();
    
    // add the RuntimeLogViewAppender for app user viewable error window (if not editor or in batch mode)
    LogManager.UseRuntimeLogView();          

    // create and add a file log appender - output goes to application file area as (in this case) ldsdk.log
    var fileLogAppender = new FileLogAppender(filenamePrefix: "ldsdk");
    LogManager.AddAppender(fileLogAppender);

    // android specific appender - appends from adb output
    #if UNITY_ANDROID
        var androidLogAppender = new AndroidAdbLogAppender();
        LogManager.AddAppender(androidLogAppender);
        // telling Unity to not include stack traces in log messages
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    #endif


### Available Appenders

#### AndroidAdbLogAppender

Formats and sends log messages to the default Android console for collection by an ADB session.

#### ConsoleLogAppender

Formats and writes messages to the default Unity log

#### FileLogAppender

Formats and writes log messages to the specified file. Not really useful for iOS/Android target devices.

#### InAppAppender

Writes and formats log messages to an in app Unity window which can be called by fast tapping the apps window 5 times. Uses legacy OnGUI direct mode UI.

#### RuntimeViewAppender

Similar to InAppAppender but uses later Unity GUI methodology with drop down log level selection. 

#### TPALogAppender (NB: deprecated)

Formats and sends log messages to The Perfect App (TPA) service.

#### UnityEngineLogAppender

Redirects logger messages to Unity console.

### Usage In Code

A static logger object is created in the class requiring log reporting

    public class ReportingClass
    {
        private static readonly  LEGO.Logger.ILog logger = LogManager.GetLogger(typeof(ReportingClass));

This object is then used to log messages to the set of appenders added eg: 

    logger.Error(@"Error in device {device.Name}");

    logger.Error(@"Exception in item {item}", exceptionCaught);

    logger.LogMessage(object, LogLevel.ERROR);

    logger.LogMessage(object, exception, LogeLevel.FATAL);

### CUBB native logger message senders

CUBB native sends log messages via the CUBB C# <-> native interface to the CUBB C# component which are then logged via a logger object. There is a set of logger classes in both Swift and Java native languages which format and send these messages - these can be found here:

Swift:
    Logging
        LELogger

Java:
    logging
        LDSDKLogger

## Support

Logger is supported and developed by LEGO Digital Product Pechnology (DPT) which is a part of LEGO System A/S  

## Status

Deployed and is a dependancy in all LPAF layers and applications that use the LPAF stack

## Copywrite and History

LEGO System A/S - created for LEGO originally by Trifork A/S
