using System;

namespace AG.LoggerViewer.Application.Common.Models
{
    public class JsonLoggerModel
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }
        public object Properties { get; set; }
        public object Renderings { get; set; }
    }
}