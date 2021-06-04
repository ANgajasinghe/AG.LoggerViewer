using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.LoggerViewer.Application.Services
{
    public class DateTimeService
    {
        public string GetDateTime  { get => DateTime.Now.ToString("yyyyMMdd");}

        public string FormatDate(DateTime dateTime) => dateTime.ToString("yyyyMMdd");
    }
}
