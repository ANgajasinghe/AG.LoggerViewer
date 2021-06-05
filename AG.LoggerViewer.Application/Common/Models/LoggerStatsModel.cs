namespace AG.LoggerViewer.Application.Common.Models
{
    public class LoggerStatsModel
    {
        public int NumberOfLines { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int InformationCount { get; set; }
    }
}