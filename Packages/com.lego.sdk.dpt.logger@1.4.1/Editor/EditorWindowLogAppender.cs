using System;
using System.Collections.Generic;
using LEGO.Logger;
using LEGO.Logger.Appenders;
using LEGO.Logger.Utilities;
using UnityEditor;
using UnityEngine;
using ColorUtility = LEGO.Logger.Utilities.ColorUtility;

namespace LEGO.Logger.Editor
{
    [Serializable]
    internal sealed class EditorWindowLogAppender : EditorWindow, ILogAppender
    {
        private Vector2 scrollPosition;
        private Color defaultBackgroundColor;
        private const int MaxDisplayCount = 500;
        private readonly Queue<ILogMessage> messages = new Queue<ILogMessage>(500);
        public LogLevel LevelFilter { get; set; }

        private readonly Filter filter = new Filter();

        [MenuItem("Window/SiCP Log")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(EditorWindowLogAppender));
            window.titleContent = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow");
            window.titleContent.text = "SiCP Log";
            window.hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnEnable()
        {
            defaultBackgroundColor = GUI.backgroundColor;

            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;

            LogManager.AddAppender(this);
        }

        private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                
                case PlayModeStateChange.ExitingEditMode:
                    if (LevelFilter == LogLevel.OFF)
                        LevelFilter= LogLevel.FATAL;
                    
                    messages.Clear();
                    break;
                
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException("obj", obj, null);
            }
        }

        private void OnDisable()
        {
            LogManager.RemoveAppender(this);
            
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
        }

         
        private void OnGUI()
        {
            GUILayout.Space(5);
            
            DrawHeaderToolbar();

            GUILayout.Space(5);
            
            DrawLogsView();
        }

        private void DrawHeaderToolbar()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(16), GUILayout.ExpandWidth(true));
            {
                GUILayout.Space(5);
                
                DrawLogLevelFilter();

                DrawContextFilterField();

                DrawMessageFilterField();

                DrawStackTraceToggle();

                DrawClearLogButton();

                DrawOpenFileButton();
                
                GUILayout.Space(5);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLogLevelFilter()
        {
            LevelFilter = (LogLevel) EditorGUILayout.EnumPopup(LevelFilter, GUILayout.Width(128), GUILayout.ExpandHeight(true));
        }

        private void DrawContextFilterField()
        {
            filter.Context = EditorGUILayout.TextField(filter.Context, GUILayout.Width(160), GUILayout.ExpandHeight(true));
        }

        private void DrawMessageFilterField()
        {                
            filter.Content = EditorGUILayout.TextField(filter.Content, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        private void DrawStackTraceToggle()
        {
            filter.ShowSource = EditorGUILayout.ToggleLeft("Source", filter.ShowSource, GUILayout.Width(98));
        }

        private void DrawClearLogButton()
        {
            if (GUILayout.Button("Clear", GUILayout.Width(64), GUILayout.ExpandHeight(true)))
            {
                ClearLog();
            }
        }

        private void ClearLog()
        {
            messages.Clear();
            
            Repaint();
        }

        private static void DrawOpenFileButton()
        {
            //TODO: Verify open button functionality on osx, and linux environments.
            if (GUILayout.Button("Open File", GUILayout.Width(64), GUILayout.ExpandHeight(true)))
            {
                System.Diagnostics.Process.Start(@Application.persistentDataPath);
            }
        }
        
        private void DrawLogsView()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    foreach (var message in messages)
                    {
                        DrawMessageLog(message);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DrawMessageLog(ILogMessage message)
        {
            if(message.Level < LevelFilter)
                return;
            
            if(!string.IsNullOrEmpty(filter.Context) && !message.ClassName.ContainsIgnoringCase(filter.Context)) 
                return;
            
            if(!string.IsNullOrEmpty(filter.Content) && !message.Text.ContainsIgnoringCase(filter.Content))
                return;
            
            var wordWrappedStyle = GUI.skin.label;
            wordWrappedStyle.wordWrap = true;

            var color = ColorUtility.GetLogLevelColor(message.Level, defaultBackgroundColor);
            GUI.backgroundColor = color;

            EditorGUILayout.BeginHorizontal("Box");
            {
                EditorGUILayout.LabelField(message.TimeStamp.ToString("HH:mm:ss.ffff"), GUILayout.Width(128));
                
                var context = GetFormatterContext(message);
                EditorGUILayout.LabelField(context, GUILayout.Width(160));

                var content = GetFormattedContent(message);
                EditorGUILayout.LabelField(content, wordWrappedStyle, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = defaultBackgroundColor;
        }

        private static string GetFormatterContext(ILogMessage message)
        {
            var context = message.LineNumber >= 0
                ? message.ClassName + ":" + message.LineNumber
                : message.ClassName;
            
            return context;
        }

        private string GetFormattedContent(ILogMessage message)
        {
            var content = filter.ShowSource || message.Level == LogLevel.FATAL 
                ? message.Text + Environment.NewLine + message.SourceDescription 
                : message.Text;
            
            return content;
        }

        void ILogAppender.Print(ILogMessage message)
        {
            if (message.Level < LevelFilter)
                return;

            if (messages.Count >= MaxDisplayCount)
            {
                RemoveNotification();
                ShowNotification(new GUIContent("Maximum simultaneous logs reached. \n\nRemoving by rule: first in, first out."));

                messages.Dequeue();
            }

            messages.Enqueue(message);

            scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);

            Repaint();
        }
    }
}
