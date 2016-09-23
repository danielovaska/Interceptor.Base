using System;
using System.IO;
using Newtonsoft.Json;
using EPiServer.Logging;

namespace Mogul.Interceptor.Base.Infrastructure.Logging
{
    public interface IObjectLogger
    {
        /// <summary>
        /// Dumps the content of an object to string in json format for logging and debugging purposes
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        string Dump(object objectToDump);

        /// <summary>
        /// Dumps the content of an object to string in json format for logging and debugging purposes
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        string Dump(object objectToDump, bool removeNullValues);
    }
    public class ObjectLogger : IObjectLogger
    {
        private readonly ILogger _log;
        public ObjectLogger()
        {
            _log = LogManager.GetLogger(typeof(ObjectLogger));
        }
        /// <summary>
        /// Dumps the content of an object to string in json format for logging and debugging purposes
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        public string Dump(object objectToDump)
        {
            try
            {
                return Dump(objectToDump, true);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to dump contents of object", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// Dumps the content of an object to string in json format for logging and debugging purposes
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        public string Dump(object objectToDump, bool removeNullValues)
        {
            try
            {
                string objectContent;
                if (removeNullValues)
                {
                    var textWriter = new StringWriter();
                    var serializer = JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore,Formatting = Formatting.Indented});
                    serializer.Serialize(textWriter, objectToDump);
                    objectContent = textWriter.ToString();
                }
                else
                {
                    objectContent = JsonConvert.SerializeObject(objectToDump);
                }
                return objectContent;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to dump contents of object", ex);
            }
            return string.Empty;
        }
    }
}
