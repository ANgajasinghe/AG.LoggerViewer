using System;

namespace AG.LoggerViewer.UI.Application.Services
{
    public class DateTimeService
    {
        public string GetDateTime => DateTime.Now.ToString("yyyyMMdd");

        public string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }
    }
}