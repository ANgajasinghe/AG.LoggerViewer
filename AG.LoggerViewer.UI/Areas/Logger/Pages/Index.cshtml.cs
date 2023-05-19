using System;
using System.Collections.Generic;
using System.Linq;
using AG.LoggerViewer.UI.Application.Common;
using AG.LoggerViewer.UI.Application.Common.Dto;
using AG.LoggerViewer.UI.Application.Common.Models;
using AG.LoggerViewer.UI.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AG.LoggerViewer.UI.Areas.Logger.Pages
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        

        public List<KeyValueDto> KeyValueDtos { get; set; } = new List<KeyValueDto>();

        public LoggerStatsModel LoggerStats { get; set; }

        public List<JsonLoggerModel> JsonLoggerModels { get; set; }

        // public string loggerFileData

        public IActionResult  OnGet(string file, string filter, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (startDate != null && endDate != null)
                {
                    KeyValueDtos = _loggerReadService.GetFileNamesAndPathFromDateFilter(startDate,endDate);
                    StartDate = startDate;
                    EndDate = endDate;
                    file = _loggerReadService.GetFileNameFromDate(endDate);
                }
                else
                    KeyValueDtos = _loggerReadService.GetTopMostFileNamesAndPath(20);

                if (string.IsNullOrWhiteSpace(file)) file = _loggerReadService.GetTodayFileName();

                var loggerRes = _loggerReadService.ReadLoggerFileFromFileName(file);

                JsonLoggerModels = filter switch
                {
                    AgLoggerConst.Error => loggerRes.Where(x => x.Level == AgLoggerConst.Error).ToList(),
                    AgLoggerConst.Warning => loggerRes.Where(x => x.Level == AgLoggerConst.Warning).ToList(),
                    AgLoggerConst.Information => loggerRes.Where(x => x.Level == AgLoggerConst.Information).ToList(),
                    _ => loggerRes
                };

                SelectedFileName = file;

                LoggerStats = _loggerReadService.GetDailyLoggerStats(loggerRes);
                FileCountFromLoggerPath = _loggerReadService.GetFilesFromLoggerPath().Length;

             
                return Page();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToPage("./Error");
            }
        }


    }
}