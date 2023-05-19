using System;

namespace AG.LoggerViewer.UI.Application.Common
{
    public class AgLoggerExceptions : Exception
    {
        public AgLoggerExceptions()
        {
        }

        public AgLoggerExceptions(string message) : base(message)
        {
        }

        public AgLoggerExceptions(string message, Exception inner) : base(message, inner)
        {
        }
    }
}