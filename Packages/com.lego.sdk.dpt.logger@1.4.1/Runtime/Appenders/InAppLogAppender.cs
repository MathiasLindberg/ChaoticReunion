using UnityEngine;
using System.Collections.Generic;
using System;

namespace LEGO.Logger
{


    /// <summary>
    /// This class will allow you to display a log-window in your app, running on a physical device. 
    /// 
    /// IMPORTANT: To add a log-window to the app you must add the SiCS_Logger_LogWindow prefap to your scene. 
    /// 
    /// The log-window will automatically pop up in your app if an error/exception is logged. 
    /// 
    /// You can always show/hide the log-window by tapping 5 times fast after each other, in the center
    /// of the screen.
    /// 
    /// </summary>
    public class InAppLogAppender : MonoBehaviour, ILogAppender
    {
        //Everything in the app is layed out according to the screen size below
        //The top lines of the OnGUI() takes care of scaling accoring to the resolution of the actual device
        //Reason: this means we do not have to consider differenct DPIs in our layout, so things will not look tiny og XHDPI and huge on LHDPI. 
        const float nativeWidth = 1024;
        const float nativeHeight = 768;

        //Size and position of of titlebar and log window
        Rect titleBarRect = new Rect(0, 0, 10000, 20);
        const int logWindowMargin = 100;
        Rect logWindowShownRect = new Rect(logWindowMargin, logWindowMargin, nativeWidth - (logWindowMargin * 2), nativeHeight - (logWindowMargin * 2));
        Rect logWindowHiddenRect = new Rect(nativeWidth - 200, 15, 150, 20);
        Vector2 logWindowScrollPosition;

        //Set to true if the user taps in the dead-center of the screen five times in a row
        //or if an error is logged
        bool isInAppLogAppenderEnabled;

        //Size of buttons
        const int clearButtonWidth = 50;
        const int showLogToggleWidth = 80;
        const int hideDebugToggleWidth = 100;
        const int hideDuplicatesToggleWidth = 110;

        //State og toggle buttons
        bool isShowLogToggleSet = true;
        bool isEnableDebugToggleSet;
        bool isCollapseToggleSet = true;

        //Wiedth of columns in log text windowLayout numbers
        const int columnSpacingWidth = 10;
        const int columnLogLevelWidth = 50;
        const int columnClassNameWidth = 140;

        //Keep all receieved log message in this list
        readonly List<ILogMessage> logMessages = new List<ILogMessage>();
        const int maxNumberOfLogEntries = 100;

        public bool DestroyInDebugBuilds = true;

        void OnEnable()
        {
            if (!Debug.isDebugBuild && DestroyInDebugBuilds)
                Destroy(gameObject);
            else
            {
                LevelFilter = LogLevel.WARN;
                LogManager.AddAppender(this);
            }
        }

        void OnDisable()
        {
            LogManager.RemoveAppender(this);
        }

        void OnGUI()
        {
            //set up scaling
            float rx = Screen.width / nativeWidth;
            float ry = Screen.height / nativeHeight;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(rx, ry, 1)); 

            LevelFilter = isEnableDebugToggleSet ? LogLevel.DEBUG : LogLevel.WARN;

            if (isShowLogToggleSet)
                logWindowShownRect = GUILayout.Window(123456, logWindowShownRect, ConsoleWindow, "Console");
            else
            {
                logWindowHiddenRect = GUILayout.Window(123456, logWindowHiddenRect, ConsoleWindow, "Console");
            }
        }

        void ConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();

            LayoutHeaderFilter();
            LayoutLogConsoleScrollView();

            GUILayout.EndVertical();

            //Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        void LayoutHeaderFilter()
        {
            GUILayout.BeginVertical();
            {
                isShowLogToggleSet = GUILayout.Toggle(isShowLogToggleSet, "Show Log", GUILayout.Width(showLogToggleWidth));
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Clear", GUILayout.Width(clearButtonWidth)))
                        ClearLog();

                    isEnableDebugToggleSet = GUILayout.Toggle(isEnableDebugToggleSet, "Enable Debug", GUILayout.Width(hideDebugToggleWidth));
                    isCollapseToggleSet = GUILayout.Toggle(isCollapseToggleSet, "Collapse", GUILayout.Width(hideDuplicatesToggleWidth));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }

        void LayoutLogConsoleScrollView()
        {
            logWindowScrollPosition = GUILayout.BeginScrollView(logWindowScrollPosition, 
                GUILayout.Width(logWindowShownRect.width), GUILayout.Height(logWindowShownRect.height - titleBarRect.height - 100));
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < logMessages.Count; i++)
                {
                    var logMessage = logMessages[i];
                    var sameAsPrevoius = false;
                    if (isCollapseToggleSet && i > 0)
                    {
                        var previousLogMessage = logMessages[i - 1];
                        sameAsPrevoius = logMessage.Text.Equals(previousLogMessage.Text);
                    }

                    if (!sameAsPrevoius)
                        PrintLogStatement(logMessage);
                }
            }
            GUILayout.EndScrollView();
        }

        /// Will print the each LogMessage on the format, eg. 
        /// [ERROR]  [MyClass:10]  The log message
        void PrintLogStatement(ILogMessage message)
        {
            GUILayout.BeginHorizontal();

            GUI.contentColor = Color.white;

            var labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.wordWrap = true;
            labelStyle.normal.textColor = Color.white;

            //Log Level
            GUILayout.Label("", labelStyle, GUILayout.Width(columnSpacingWidth)); //space
            GUILayout.Label("[" + message.Level.ToString().ToUpper() + "]", labelStyle, GUILayout.Width(columnLogLevelWidth));
            GUILayout.Label("", labelStyle, GUILayout.Width(columnSpacingWidth)); //space

            //Logger name
            GUILayout.Label("[" + message.SourceDescription + "]", labelStyle, GUILayout.Width(columnClassNameWidth));
                           
            //Logger text 
            GUILayout.Label("", labelStyle, GUILayout.Width(columnSpacingWidth)); //space
            AdjustColor(message.Level);

            var textWidth = logWindowShownRect.width - columnLogLevelWidth - columnClassNameWidth - columnSpacingWidth * 15;
            GUILayout.Label(message.Text, labelStyle, GUILayout.Width(textWidth));
            GUILayout.Label("", labelStyle, GUILayout.Width(columnSpacingWidth * 2)); //space
            GUILayout.EndHorizontal();
        }


        void AdjustColor(LogLevel level)
        {
            if ((int)level >= (int)LogLevel.WARN)
                GUI.contentColor = Color.red;
            else if ((int)level >= (int)LogLevel.INFO)
                GUI.contentColor = Color.green;
            else
                GUI.contentColor = Color.white;
        }


        void ClearLog()
        {
            logMessages.Clear();
        }

        #region ILogAppender implementation
        public void Print(ILogMessage message)
        {
            if (message.Level < LevelFilter)
                return;

            logMessages.Add(message);

            if (logMessages.Count > maxNumberOfLogEntries)
                logMessages.RemoveRange(0, 10);

            if (!isInAppLogAppenderEnabled && message.Level >= LogLevel.WARN)
            {
                isInAppLogAppenderEnabled = true;
                isShowLogToggleSet = true;
            }

            logWindowScrollPosition = new Vector2(logWindowScrollPosition.x, Mathf.Infinity);
        }

        public LogLevel LevelFilter { get; set; }
        #endregion
    }
}