using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using EPiServer.Core;
using Newtonsoft.Json;
using EPiServer.Logging;
using EPiServer.Web.Routing;
using Newtonsoft.Json.Serialization;

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
    }
    public class ObjectLogger : IObjectLogger
    {
        private readonly ILogger _log;
        private readonly ObjectLoggerSettings _settings ;

        public ObjectLogger()
        {
            _log = LogManager.GetLogger(typeof(ObjectLogger));
            var jsonResolver = new IgnorableSerializerContractResolver();
            // ignore single datatype
            jsonResolver.Ignore(typeof(IContent));
            jsonResolver.Ignore(typeof(PageData));
            jsonResolver.Ignore(typeof(System.IO.Stream));
            jsonResolver.Ignore(typeof(HttpContextBase));
            
            _settings = new ObjectLoggerSettings
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = jsonResolver
                }
            };
        }
        public ObjectLogger(ObjectLoggerSettings settings)
        {
            _log = LogManager.GetLogger(typeof(ObjectLogger));
            _settings = settings;
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
                string objectContent;
                var textWriter = new StringWriter();
                var serializer = JsonSerializer.Create(_settings.SerializerSettings);
                serializer.Serialize(textWriter, objectToDump);
                objectContent = textWriter.ToString();
                return objectContent;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to dump contents of object", ex);
            }
            return string.Empty;
        }
        
    }
    /// <summary>
    /// Special JsonConvert resolver that allows you to ignore properties.  See http://stackoverflow.com/a/13588192/1037948
    /// </summary>
    public class IgnorableSerializerContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        public IgnorableSerializerContractResolver()
        {
            this.Ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>
        /// Explicitly ignore the given property(s) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
        public void Ignore(Type type, params string[] propertyName)
        {
            // start bucket if DNE
            if (!this.Ignores.ContainsKey(type)) this.Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
            {
                this.Ignores[type].Add(prop);
            }
        }

        /// <summary>
        /// Is the given property for the given type ignored?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!this.Ignores.ContainsKey(type)) return false;

            // if no properties provided, ignore the type entirely
            if (this.Ignores[type].Count == 0) return true;

            return this.Ignores[type].Contains(propertyName);
        }

        /// <summary>
        /// The decision logic goes here
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (this.IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = instance => { return false; };
            }

            return property;
        }
    }
    public class ObjectLoggerSettings
    {
        public JsonSerializerSettings SerializerSettings { get; set; }
    }
}
