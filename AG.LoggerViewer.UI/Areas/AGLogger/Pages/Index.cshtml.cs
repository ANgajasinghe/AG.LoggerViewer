using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AG.LoggerViewer.Application.Commin.Dto;
using AG.LoggerViewer.Application.Common.Models;
using AG.LoggerViewer.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AG.LoggerViewer.UI.Areas.AGLogger.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DateTimeService _dateTimeService;
        private readonly ILoggerReadService _loggerReadService;

        public IndexModel(DateTimeService dateTimeService, ILoggerReadService loggerReadService)
        {
            _dateTimeService = dateTimeService;
           _loggerReadService = loggerReadService;
        }


        public string Message { get; set; }

        public int FileCountFromLoggerPath { get; set; }
        
        public string LoggerFileData { get; set; }

        public List<KeyValueDto> KeyValueDtos { get; set; } = new List<KeyValueDto>();

        public LoggerStatsModel LoggerStats { get; set; }

        // public string loggerFileData

        public void OnGet()
        {
            Message = _dateTimeService.GetDateTime;
            var data =  _loggerReadService.ReadLoggerFileFromDate(DateTime.Now);
            
            LoggerStats = _loggerReadService.GetDailyLoggerStats(data);
            
            
            FileCountFromLoggerPath = _loggerReadService.GetFilesFromLoggerPath().Length;
            LoggerFileData = _loggerReadService.GetJsonStringFromLoggerObject(data);
            KeyValueDtos = _loggerReadService.GetTopMostFileNamesAndPath(-1);
            // ILogger dd = new Logger<IndexModel>();
            // dd.Log(LogLevel.Critical);

        }
    }
}
