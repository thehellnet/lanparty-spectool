using LanPartySpecTool.Annotations;
using log4net.Appender;
using log4net.Core;

namespace LanPartySpecTool
{
    [UsedImplicitly]
    internal class LogEvent : AppenderSkeleton
    {
        public delegate void LogEventHandler(Level level, string message);

        public static event LogEventHandler OnLogEvent;

        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent);
            OnLogEvent?.Invoke(loggingEvent.Level, message);
        }
    }
}