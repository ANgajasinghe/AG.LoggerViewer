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

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10);


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
                throw new AGLoggerExceptions("Cannot get files from given logger path please check your logger path",
                    ex);
            }
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

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10)
        {
            try
            {
                var keyValueDtos = new List<KeyValueDto>();

                List<string> records;
                if (limit == -1)
                    records = GetFilesFromLoggerPath().ToList();
                else
                    records = GetFilesFromLoggerPath().Take(limit)
                        .OrderBy(x=>x)
                        .ToList();

                for (var i = records.Count - 1; i >= 0; i--)
                {
                    var path = records[i];

                    var fileName = Path.GetFileName(path);

                    keyValueDtos.Add(new KeyValueDto {Key = fileName, Value = path});
                }

                return keyValueDtos;
            }
            catch (Exception ex)
            {
                throw new AGLoggerExceptions("Cannot get top files, please check your logger path", ex);
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


        private static List<JsonLoggerModel> ReadLogFile(string filePath)
        {
            try
            {
                var list = new List<JsonLoggerModel>();

                if (!File.Exists(filePath))
                    throw new AGLoggerExceptions(
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
                throw new AGLoggerExceptions("Cannot get read log file", ex);
            }
        }
    }

   
}