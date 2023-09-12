using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    public class TimespanConverter : IYamlTypeConverter
    {
        private const string DurationPrefix = "PT";

        public bool Accepts(Type type) =>
            type == typeof(TimeSpan);

        public object ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>();
            var value = scalar.Value;
            if (!value.StartsWith(DurationPrefix))
            {
                throw new YamlException(scalar.Start, scalar.End, $"No valid duration value '{value}'");
            }
            int hours = 0, minutes = 0, seconds = 0, millis = 0;
            var lastPos = DurationPrefix.Length;
            var unitPos = value.IndexOf('H');
            if (unitPos >= 0)
            {
                hours = int.Parse(value.Substring(lastPos, unitPos - lastPos));
                lastPos = unitPos + 1;
            }
            unitPos = value.IndexOf('M');
            if (unitPos >= 0)
            {
                minutes = int.Parse(value.Substring(lastPos, unitPos - lastPos));
                lastPos = unitPos + 1;
            }
            unitPos = value.IndexOf('.');
            if (unitPos >= 0)
            {
                seconds = int.Parse(value.Substring(lastPos, unitPos - lastPos));
                lastPos = unitPos + 1;
                var millisStr = value.Substring(lastPos, Math.Min(3, value.Length - 1 - lastPos));
                millis = int.Parse(millisStr.PadLeft(3, '0'));
            }
            else if (value.Contains("S"))
            {
                seconds = int.Parse(value.Substring(lastPos, value.Length - lastPos - 1));
            }
            return new TimeSpan(0, hours, minutes, seconds, millis);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var ret = "PT";
            var duration = (TimeSpan)value;
            if (duration.Hours > 0)
            {
                ret += duration.Hours + "H";
            }
            if (duration.Minutes > 0)
            {
                ret += duration.Minutes + "M";
            }
            if ((duration.Hours == 0 && duration.Minutes == 0)
                || duration.Seconds > 0 || duration.Milliseconds > 0)
            {
                ret += duration.Seconds;
                if (duration.Milliseconds > 0)
                {
                    ret += "." + duration.Milliseconds.ToString().PadLeft(3, '0');
                }
                ret += "S";
            }
            emitter.Emit(new Scalar(ret));
        }
    }
}
