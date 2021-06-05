using System.Collections.Generic;
using AG.LoggerViewer.UI.Application.Common.Dto;
using AG.LoggerViewer.UI.Application.Common.Models;
using AG.LoggerViewer.UI.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AG.LoggerViewer.UI.Areas.AGLogger.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DateTimeService _dateTimeService;
        private readonly ILoggerReadService _loggerReadService;

        public IndexModel(
            DateTimeService dateTimeService,
            ILoggerReadService loggerReadService)
        {
            _dateTimeService = dateTimeService;
            _loggerReadService = loggerReadService;
        }


        public string Message { get; set; }

        public int FileCountFromLoggerPath { get; set; }

        public string SelectedFileName { get; set; }

        public List<KeyValueDto> KeyValueDtos { get; set; } = new List<KeyValueDto>();

        public LoggerStatsModel LoggerStats { get; set; }

        public List<JsonLoggerModel> JsonLoggerModels { get; set; }

        // public string loggerFileData

        public void OnGet(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) file = _loggerReadService.GetTodayFileName();

            JsonLoggerModels = _loggerReadService.ReadLoggerFileFromFileName(file);
            SelectedFileName = file;

            LoggerStats = _loggerReadService.GetDailyLoggerStats(JsonLoggerModels);
            FileCountFromLoggerPath = _loggerReadService.GetFilesFromLoggerPath().Length;

            KeyValueDtos = _loggerReadService.GetTopMostFileNamesAndPath(-1);
        }
    }
}