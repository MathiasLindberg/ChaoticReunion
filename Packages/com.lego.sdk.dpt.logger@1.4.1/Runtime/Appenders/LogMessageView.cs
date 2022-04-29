using System;
using System.Text;
using LEGO.Logger.Utilities;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = LEGO.Logger.Utilities.ColorUtility;

namespace LEGO.Logger.Appenders
{
    internal class LogMessageView : MonoBehaviour
    {
        [SerializeField] private Text text;
        
        private ILogMessage message;
        private readonly StringBuilder buffer = new StringBuilder();

        public void SetContent(ILogMessage message)
        {
            this.message = message;

            text.text = GetFormattedMessage(message, buffer);
            text.color = ColorUtility.GetLogLevelColor(message.Level, Color.white);

            transform.SetAsFirstSibling();
        }

        private static string GetFormattedMessage(ILogMessage message, StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append("[");
            stringBuilder.Append(message.TimeStamp.ToString("HH:mm:ss.ffff")).Append("]");
            
            GetFormatterContext(message,stringBuilder);
            stringBuilder.Append(Environment.NewLine);
            
            GetFormattedContent(message, stringBuilder);
            
            return stringBuilder.ToString();
        }
        
        private static void GetFormatterContext(ILogMessage message, StringBuilder stringBuilder)
        {
            stringBuilder.Append("[");
            stringBuilder.Append(message.ClassName);
            if (message.LineNumber >= 0)
            {
                stringBuilder.Append(":").Append(message.LineNumber);
            }
            stringBuilder.Append("]");
        }

        
        private static void GetFormattedContent(ILogMessage message, StringBuilder stringBuilder)
        {
            stringBuilder.Append(message.Text);
            if (message.Level == LogLevel.FATAL)
            {
                stringBuilder
                    .Append(Environment.NewLine)
                    .Append(message.SourceDescription);
            }
        }

        public void UpdateVisibility(Filter filter)
        {
            var isActiveFilter = message.Level >= filter.LogLevel;
            gameObject.SetActive(isActiveFilter);
            if(!isActiveFilter)
                return;

            var isActiveContext = message.ClassName.ContainsIgnoringCase(filter.Context);
            gameObject.SetActive(isActiveContext);
            if(!isActiveContext)
                return;

            var isActiveContent = message.Text.ContainsIgnoringCase(filter.Content);
            gameObject.SetActive(isActiveContent);
            if(!isActiveContent)
                return;

            //TODO: Filter on source toggle. Currently disabled in UI prefab.
//            if (filter.ShowSource && message.Level != LogLevel.FATAL)
//            {
//                buffer.Append(Environment.NewLine).Append(message.SourceDescription);
//                text.text = buffer.ToString();
//            }
        }
    }
}