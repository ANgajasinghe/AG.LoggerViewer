using AG.LoggerViewer.Application.Commin;
using AG.LoggerViewer.Application.Commin.Dto;
using AG.LoggerViewer.Application.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AG.LoggerViewer.Application.Common.Models;

namespace AG.LoggerViewer.Application.Services
{
    public class LoggerReadService : ILoggerReadService
    {
        private readonly LoggerUtitlity _loggerUtility;
        private readonly DateTimeService _dateTimeService;

        public LoggerReadService(LoggerUtitlity loggerUtility,DateTimeService dateTimeService)
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

                throw new AGLoggerExceptions("Cannot get files from given logger path please check your logger path",ex);
            }
        }

        public string GetJsonStringFromLoggerObject(object fileData)
        {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(fileData, options);
        }

        public LoggerStatsModel GetDailyLoggerStats(List<JsonLoggerModel> fileData)
        {
            return new LoggerStatsModel
            {
                NumberOfLines = fileData.Count,
                ErrorCount = fileData.Where(x => x.Level == "Error").ToList().Count,
                InformationCount = fileData.Where(x => x.Level == "Information").ToList().Count,
                WarningCount = fileData.Where(x => x.Level == "Warning").ToList().Count,
            };
        }

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10)
        {
            try
            {
                var keyValueDtos = new List<KeyValueDto>();

                List<string> records;
                if (limit == -1)
                {
                    records =  GetFilesFromLoggerPath().ToList();
                }
                else
                {
                    records =  GetFilesFromLoggerPath().Take(limit).ToList();
                }
                
                for (var i = records.Count - 1; i >= 0; i--)
                {
                    var path = records[i];

                    var fileName = Path.GetFileName(path);

                    keyValueDtos.Add(new KeyValueDto { Key = fileName, Value = path });
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
            var filePath = Path.Combine(_loggerUtility.LoggerPath,fileName);

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

                if (!File.Exists(filePath)) throw new AGLoggerExceptions($"{Path.GetFileName(filePath)} this file could'n find from {filePath}");

                Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (StreamReader file = new StreamReader(stream))
                {
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        list.Add(JsonSerializer.Deserialize<JsonLoggerModel>(ln));
                    }

                    file.Close();
                }

                stream.Close();

                return list.OrderByDescending(x=>x.Timestamp).ToList();
            }
            catch (Exception ex)
            {

                throw new AGLoggerExceptions("Cannot get read log file", ex);
            }
        }
        
        
    }

    public interface ILoggerReadService 
    {

        public string[] GetFilesFromLoggerPath();

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10);
        

        public string GetJsonStringFromLoggerObject(object fileData);
        public LoggerStatsModel GetDailyLoggerStats(List<JsonLoggerModel> fileData);

        List<JsonLoggerModel> ReadLoggerFileFromFileName(string fileName);

        string GetTodayFileName();

    }
}
