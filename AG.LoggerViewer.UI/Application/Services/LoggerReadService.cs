using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using AG.LoggerViewer.UI.Application.Common;
using AG.LoggerViewer.UI.Application.Common.Dto;
using AG.LoggerViewer.UI.Application.Common.Extensions;
using AG.LoggerViewer.UI.Application.Common.Models;
using AG.LoggerViewer.UI.Application.Util;

namespace AG.LoggerViewer.UI.Application.Services
{

    public interface ILoggerReadService
    {
        public string[] GetFilesFromLoggerPath();
        public string GetFileNameFromDate(DateTime? date);

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int? limit = 10);
        public List<KeyValueDto> GetFileNamesAndPathFromDateFilter(DateTime? startDate, DateTime? endDate);


        public string GetJsonStringFromLoggerObject(object fileData);
        public LoggerStatsModel GetDailyLoggerStats(List<JsonLoggerModel> fileData);

        List<JsonLoggerModel> ReadLoggerFileFromFileName(string fileName);

        string GetTodayFileName();
    }


    public class LoggerReadService : ILoggerReadService
    {
        private readonly DateTimeService _dateTimeService;
        private readonly LoggerUtility _loggerUtility;

        public LoggerReadService(LoggerUtility loggerUtility, DateTimeService dateTimeService)
        {
            _loggerUtility = loggerUtility;
            _dateTimeService = dateTimeService;
        }

        public string[] GetFilesFromLoggerPath()
        {
            try
            {
                return Directory.GetFiles(_loggerUtility.LoggerPath, $"*{_loggerUtility.FileExtension}");
            }
            catch (Exception ex)
            {
                throw new AgLoggerExceptions("Cannot get files from given logger path please check your logger path",
                    ex);
            }
        }

        public string GetFileNameFromDate(DateTime? date)
        {
            return $"{_loggerUtility.LoggerFileNameWithOutDate}{date?.ToString("yyyyMMdd") ?? DateTime.Now.ToString("yyyyMMdd")}{_loggerUtility.FileExtension}";
        }


        public string GetJsonStringFromLoggerObject(object fileData)
        {
            var data = fileData.ToJson();
            return data;
            // return data;
        }

        public LoggerStatsModel GetDailyLoggerStats(List<JsonLoggerModel> fileData)
        {
            return new LoggerStatsModel
            {
                NumberOfLines = fileData.Count,
                ErrorCount = fileData.Where(x => x.Level == "Error").ToList().Count,
                InformationCount = fileData.Where(x => x.Level == "Information").ToList().Count,
                WarningCount = fileData.Where(x => x.Level == "Warning").ToList().Count
            };
        }

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int? limit = 10)
        {
            try
            {
                var keyValueDtos = new List<KeyValueDto>();

                List<string> records;
                if (limit is null)
                    records = GetFilesFromLoggerPath().ToList();
                else
                    records = GetFilesFromLoggerPath()
                        .OrderByDescending(x=>x)
                        .Take(limit.GetValueOrDefault(1))
                        .ToList();

                for (var i = 0; i < records.Count ; i++)
                {
                    var path = records[i];

                    var fileName = Path.GetFileName(path);

                    keyValueDtos.Add(new KeyValueDto {Key = fileName, Value = path});
                }

                return keyValueDtos;
            }
            catch (Exception ex)
            {
                throw new AgLoggerExceptions("Cannot get top files, please check your logger path", ex);
            }
        }
        
        public List<KeyValueDto> GetFileNamesAndPathFromDateFilter(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var keyValueDtos = new List<KeyValueDto>();
                
                var records = GetDateRangeFileNames(startDate, endDate).OrderByDescending(x=>x).ToList();

                for (var i = 0; i < records.Count ; i++)
                {
                    var path = records[i];

                    var fileName = Path.GetFileName(path);

                    keyValueDtos.Add(new KeyValueDto {Key = fileName, Value = path});
                }

                return keyValueDtos;
            }
            catch (Exception ex)
            {
                throw new AgLoggerExceptions("Cannot get files with selected date range", ex);
            }
        }


        public List<JsonLoggerModel> ReadLoggerFileFromFileName(string fileName)
        {
            var filePath = Path.Combine(_loggerUtility.LoggerPath, fileName);

            return ReadLogFile(filePath);
        }

        public string GetTodayFileName()
        {
            return
                $"{_loggerUtility.LoggerFileNameWithOutDate}{_dateTimeService.GetDateTime}{_loggerUtility.FileExtension}";
        }

        private List<string> GetDateRangeFileNames(DateTime? startDate, DateTime? endDate)
        {
            var dateRangeFileNames = new List<string>();

            var startDateString = _dateTimeService.FormatDate(startDate ?? DateTime.Now);
            var endDateString = _dateTimeService.FormatDate(endDate ?? DateTime.Now);

            var startDateInt = int.Parse(startDateString);
            var endDateInt = int.Parse(endDateString);

            var files = GetFilesFromLoggerPath();

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);

                var date = Regex.Match(fileName, @"\d+").Value;

                var dateInt = int.Parse(date);

                if (dateInt >= startDateInt && dateInt <= endDateInt)
                    dateRangeFileNames.Add(file);
            }

            return dateRangeFileNames;
        }

        private static List<JsonLoggerModel> ReadLogFile(string filePath)
        {
            try
            {
                var list = new List<JsonLoggerModel>();

                if (!File.Exists(filePath))
                    throw new AgLoggerExceptions(
                        $"{Path.GetFileName(filePath)} this file could'n find from {filePath}");

                Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (var file = new StreamReader(stream))
                {
                   // var x = @"{"Timestamp":"2021-09-19T03:07:05.1993971+05:30","Level":"Information","MessageTemplate":"{State:l}","Properties":{"State":"Using the following options for Hangfire Server:\r\n    Worker count: 20\r\n    Listening queues: 'default'\r\n    Shutdown timeout: 00:00:15\r\n    Schedule polling interval: 00:00:15","SourceContext":"Hangfire.BackgroundJobServer","Application":"Cube360 WMS"},"Renderings":{"State":[{"Format":"l","Rendering":"Using the following options for Hangfire Server:\r\n    Worker count: 20\r\n    Listening queues: 'default'\r\n    Shutdown timeout: 00:00:15\r\n    Schedule polling interval: 00:00:15"}]}}"
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        var replacedData = Regex.Replace(ln , @"\\r\\n" ,",  ");
                        list.Add(replacedData.FromJson<JsonLoggerModel>());
                    }
                    
                    file.Close();
                }

                stream.Close();

                return list.OrderByDescending(x => x.Timestamp).ToList();
            }
            catch (Exception ex)
            {
                throw new AgLoggerExceptions("Cannot get read log file", ex);
            }
        }
    }

   
}