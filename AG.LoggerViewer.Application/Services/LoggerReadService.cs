using AG.LoggerViewer.Application.Commin;
using AG.LoggerViewer.Application.Commin.Dto;
using AG.LoggerViewer.Application.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
                return Directory.GetFiles(_loggerUtility.LoggerPath, $"*{_loggerUtility.FileExtension}") ?? new string[] { };
            }
            catch (Exception ex)
            {

                throw new AGLoggerExceptions("Cannot get files from given logger path please check your logger path",ex);
            }
        }

        public string GetJsonStringFromLoggerObject(List<JsonLoggerModel> fileData)
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
                List<KeyValueDto> keyValueDtos = new List<KeyValueDto>();

                var records = new List<string>();
                if (limit == -1)
                {
                    records =  GetFilesFromLoggerPath().ToList();
                }
                else
                {
                    records =  GetFilesFromLoggerPath().Take(limit).ToList();
                }



                for (int i = records.Count - 1; i >= 0; i--)
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

        public List<JsonLoggerModel> ReadLogFile(string filePath)
        {
            try
            {

                List<JsonLoggerModel> list = new List<JsonLoggerModel>();

                if (!File.Exists(filePath)) throw new AGLoggerExceptions($"{Path.GetFileName(filePath)} this file could'n find from {filePath}");

                Stream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (StreamReader file = new StreamReader(stream))
                {
                    int counter = 0;
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        list.Add(JsonSerializer.Deserialize<JsonLoggerModel>(ln));
                        counter++;
                    }

                    file.Close();
                }

                stream.Close();

                return list;
            }
            catch (Exception ex)
            {

                throw new AGLoggerExceptions("Cannot get read log file", ex);
            }
        }

        public List<JsonLoggerModel> ReadLoggerFileFromDate(DateTime dateTime)
        {
            var filePath = Path.Combine(_loggerUtility.LoggerPath, $"{_loggerUtility.LoggerFileNameWithOutDate}{_dateTimeService.FormatDate(dateTime)}{_loggerUtility.FileExtension}");

            return ReadLogFile(filePath);
        }
    }

    public interface ILoggerReadService 
    {

        public string[] GetFilesFromLoggerPath();

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10);

        public List<JsonLoggerModel> ReadLogFile(string filePath);

        public List<JsonLoggerModel> ReadLoggerFileFromDate(DateTime dateTime);

        public string GetJsonStringFromLoggerObject(List<JsonLoggerModel> fileData);
        public LoggerStatsModel GetDailyLoggerStats(List<JsonLoggerModel> fileData);
        
        
    }
}
