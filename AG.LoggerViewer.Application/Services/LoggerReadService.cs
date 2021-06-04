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

        public string GetJsonStringFromObject(List<object> fileData)
        {

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(fileData, options);
        }

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10)
        {
            try
            {
                List<KeyValueDto> keyValueDtos = new List<KeyValueDto>();

                var topReccods = GetFilesFromLoggerPath().Take(limit).ToArray();

                for (int i = topReccods.Length - 1; i >= 0; i--)
                {
                    var path = topReccods[i];

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

        public List<object> ReadLogFile(string filePath)
        {
            try
            {

                List<object> list = new List<object>();

                if (!File.Exists(filePath)) throw new AGLoggerExceptions($"{Path.GetFileName(filePath)} this file could'n find from {filePath}");

                Stream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (StreamReader file = new StreamReader(stream))
                {
                    int counter = 0;
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        list.Add(JsonSerializer.Deserialize<object>(ln));
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

        public List<object> ReadLoggerFileFromDate(DateTime dateTime)
        {
            var filePath = Path.Combine(_loggerUtility.LoggerPath, $"{_loggerUtility.LoggerFileNameWithOutDate}{_dateTimeService.FormatDate(dateTime)}{_loggerUtility.FileExtension}");

            return ReadLogFile(filePath);
        }
    }

    public interface ILoggerReadService 
    {

        public string[] GetFilesFromLoggerPath();

        public List<KeyValueDto> GetTopMostFileNamesAndPath(int limit = 10);

        public List<object> ReadLogFile(string filePath);

        public List<object> ReadLoggerFileFromDate(DateTime dateTime);

        public string GetJsonStringFromObject(List<Object> fileData);
    }
}
