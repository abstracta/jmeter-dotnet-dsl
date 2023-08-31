using System;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    public class EnumConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
            => type.IsEnum;

        public object ReadYaml(IParser parser, Type type)
            => throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object value, Type type)
            => emitter.Emit(new Scalar(ToSnakeCase(value.ToString())));

        private string ToSnakeCase(string val)
            => Regex.Replace(val, @"([a-z0-9])([A-Z])", "$1_$2").ToUpper();
    }
}