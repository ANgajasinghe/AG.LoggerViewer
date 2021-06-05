using System;

namespace AG.LoggerViewer.UI.Application.Common
{
    public class AGLoggerExceptions : Exception
    {
        public AGLoggerExceptions()
        {
        }

        public AGLoggerExceptions(string message) : base(message)
        {
        }

        public AGLoggerExceptions(string message, Exception inner) : base(message, inner)
        {
        }
    }
}