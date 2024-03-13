using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    public class BridgedObjectConverter : IYamlTypeConverter
    {
        private const string DslClassPrefix = "Dsl";
        private const string NamespacePrefix = "Abstracta.JmeterDsl.";

        public IValueSerializer ValueSerializer { get; set; }

        public bool Accepts(Type type) =>
            typeof(IDslTestElement).IsAssignableFrom(type)
            || typeof(IDslProperty).IsAssignableFrom(type)
            || typeof(IDslJmeterEngine).IsAssignableFrom(type)
            || typeof(TestPlanExecution).IsAssignableFrom(type);

        public object ReadYaml(IParser parser, Type type) =>
            throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var valueType = value.GetType();
            var tagName = BuildTagName(valueType);
            emitter.Emit(new MappingStart(null, tagName, false, MappingStyle.Any));
            var fields = valueType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            fields = fields.Where(f => !(f.Name == "_name" && f.GetValue(value) == null)).ToArray();
            if (fields.Length == 1 && typeof(IDictionary<string, string>).IsAssignableFrom(fields[0].FieldType))
            {
                WriteDictionaryYaml((IDictionary<string, string>)fields[0].GetValue(value), emitter);
            }
            else
            {
                // this set is used to avoid processing fields that have been hidden by subclass members (using new keyword)
                ISet<string> processedFields = new HashSet<string>();
                foreach (FieldInfo field in fields)
                {
                    if (!processedFields.Contains(field.Name) && !IsIgnoredField(field))
                    {
                        WriteFieldYaml(field, field.GetValue(value), emitter);
                    }
                    processedFields.Add(field.Name);
                }
            }
            emitter.Emit(new MappingEnd());
        }

        private string BuildTagName(Type valueType) =>
            "!" + (IsCoreElement(valueType) ? BuildSimpleTagName(valueType)
                : BuildCompleteTagName(valueType));

        private bool IsCoreElement(Type valueType)
        {
            var typeFullName = valueType.FullName;
            return typeFullName.StartsWith(NamespacePrefix + "Core.")
                || typeFullName.StartsWith(NamespacePrefix + "Http.")
                || typeFullName.StartsWith(NamespacePrefix + "Java.");
        }

        private string BuildSimpleTagName(Type valueType)
        {
            var ret = valueType.Name;
            if (ret.StartsWith(DslClassPrefix))
            {
                ret = ret.Substring(DslClassPrefix.Length);
            }
            ret = LowerFirstChar(ret);
            return ret;
        }

        private string LowerFirstChar(string val) =>
            val.Substring(0, 1).ToLower() + val.Substring(1);

        private string BuildCompleteTagName(Type valueType)
        {
            var ret = valueType.FullName;
            if (ret.StartsWith(NamespacePrefix))
            {
                ret = ret.Substring(NamespacePrefix.Length);
            }
            return LowerNamespaces(ret);
        }

        private string LowerNamespaces(string fullName)
        {
            var ret = string.Empty;
            int start = 0;
            int end = fullName.IndexOf('.');
            while (end != -1)
            {
                ret += fullName.Substring(start, end - start + 1).ToLower();
                start = end + 1;
                end = fullName.IndexOf('.', start);
            }
            ret += fullName.Substring(start);
            return ret;
        }

        private void WriteDictionaryYaml(IDictionary<string, string> value, IEmitter emitter)
        {
            foreach (KeyValuePair<string, string> entry in value)
            {
                emitter.Emit(new Scalar(entry.Key));
                emitter.Emit(new Scalar(entry.Value));
            }
        }

        private bool IsIgnoredField(FieldInfo field) =>
            field.GetCustomAttribute(typeof(YamlIgnoreAttribute)) != null;

        private void WriteFieldYaml(FieldInfo field, object value, IEmitter emitter)
        {
            if (value == null || (value is ICollection collection && collection.Count == 0))
            {
                return;
            }

            // removing first character since fields are prefixed with underscore
            emitter.Emit(new Scalar(field.Name.Substring(1)));
            ValueSerializer.SerializeValue(emitter, value, field.FieldType == typeof(object) ? value.GetType() : field.FieldType);
        }
    }
}
