using System;
using System.Collections;
using System.Collections.Generic;
using LEGO.Logger.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LEGO.Logger.Appenders
{
    internal class RuntimeLogViewAppender : MonoBehaviour, ILogAppender
    {
        //TODO: Snap to first fatal error and lock
        
        [Header("Dependencies - Prefabs")]
        [SerializeField] private LogMessageView logMessageViewPrefab;

        [Header("Dependencies - local")] 
        [SerializeField] private Dropdown logLevelDropDown;
        [SerializeField] private InputField contextInputField;
        [SerializeField] private InputField contentInputField;
        [SerializeField] private Toggle sourceToggle;
        [SerializeField] private Toggle logViewToggle;
        [SerializeField] private ScrollRect scrollRect;

        private const int MaxMessageAmount = 200;
        private readonly Queue<LogMessageView> messageViews = new Queue<LogMessageView>(MaxMessageAmount);
        private readonly Filter filter = new Filter();

        private void Awake()
        {
            filter.LogLevel = LogManager.RootLevel;
            
            logViewToggle.onValueChanged.AddListener(arg0 =>
            {
                gameObject.SetActive(arg0);
            });
            logViewToggle.isOn = false;

            InitializeDropDown();

            InitializeContextField();

            InitializeContentFilter();

            InitializeSourceToggle();
            
            var coroutineRunner = CoroutineRunner.Create();
            DontDestroyOnLoad(coroutineRunner.gameObject);
            coroutineRunner.StartCoroutine(ListenOnInput());
        }

        private void InitializeDropDown()
        {
            var options = new List<string>();
            for (var i = 0; i <= (int) LogLevel.OFF ; i++)
            {
                var meh = ((LogLevel) i).ToString();
                options.Add(meh);
            }

            logLevelDropDown.ClearOptions();
            logLevelDropDown.AddOptions(options);

            logLevelDropDown.onValueChanged.AddListener(arg0 =>
            {
                filter.LogLevel = (LogLevel) arg0;
                LogManager.RootLevel =  filter.LogLevel;
                UpdateList();
            });

            logLevelDropDown.value = (int)filter.LogLevel;
        }

        private void InitializeContextField()
        {
            contextInputField.onValueChanged.AddListener(arg0 =>
            {
                filter.Context = arg0;
                UpdateList();
            });
        }

        private void InitializeContentFilter()
        {
            contentInputField.onValueChanged.AddListener(arg0 =>
            {
                filter.Content = arg0;
                UpdateList();
            });
        }

        private void InitializeSourceToggle()
        {
            sourceToggle.onValueChanged.AddListener(arg0 =>
            {
                filter.ShowSource = arg0;
                UpdateList();
            });
            sourceToggle.isOn = false;
        }

        private void UpdateList()
        {
            var views = messageViews.ToArray();
            foreach (var logMessageView in views)
            {
                logMessageView.UpdateVisibility(filter);
            }
        }
        
        private IEnumerator ListenOnInput()
        {
            while (true)
            {
                yield return null;
                
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
                    logViewToggle.isOn = !logViewToggle.isOn;
            
                if(Input.touchCount == 0)
                    continue;

                if (Input.GetTouch(0).tapCount >= 5)
                    logViewToggle.isOn = true;
            }
        }

        public LogLevel LevelFilter { get; set; }
        void ILogAppender.Print(ILogMessage message)
        {
            //Maybe because the GameObject  for RuntimeLogViewAppender is marked as DontDestroyOnLoad, it stays alive too long when playback in editor is stopped, this causes warnings from IDE to be caught by appender which outputs ghost LogViewMessages
            //To avoid this we just return here as to not Instantiate any extra LogViewMessages when we are not playing
            if (!Application.isPlaying)
            {
                return;
            }
            
            var messageView = messageViews.Count >= MaxMessageAmount 
                ? messageViews.Dequeue() 
                : Instantiate(logMessageViewPrefab, scrollRect.content);
            
            messageView.SetContent(message);
            messageView.UpdateVisibility(filter);

            if (message.Level == LogLevel.FATAL)
            {
                logViewToggle.isOn = true;
            }

            messageViews.Enqueue(messageView);
        }

        public static class Factory
        {
            public static ILogAppender Create()
            {
                if (EventSystem.current == null)
                {
                    Debug.LogWarning("EventSystem Missing. Adding default event system to scene.");
                    var eventSystem = new GameObject("LoggerEventSystem").AddComponent<StandaloneInputModule>();
                    DontDestroyOnLoad(eventSystem.gameObject);
                }

                var path = "Logging/" + typeof(RuntimeLogViewAppender).Name;
                var prefab = Resources.Load<RuntimeLogViewAppender>(path);
                var instance = Instantiate(prefab);

                DontDestroyOnLoad(instance.gameObject);

                return instance;
            }
        }
    }
}