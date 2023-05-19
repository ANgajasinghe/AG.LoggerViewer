using System;

namespace AG.LoggerViewer.UI.Application.Common.Models
{
    public class JsonLoggerModel
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }
        public object Properties { get; set; }
        public object Renderings { get; set; }
        
        public  string GetBatchClasses()
        {
            return Level switch
            {
                AgLoggerConst.Error => "badge badge-danger",
                AgLoggerConst.Warning => "badge badge-warning",
                AgLoggerConst.Information => "badge badge-info",
                _ => ""
            };
        }
    }
}