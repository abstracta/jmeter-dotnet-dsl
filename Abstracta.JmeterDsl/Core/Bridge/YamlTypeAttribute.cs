using System;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class YamlTypeAttribute : Attribute
    {
        public string TagName { get; set; }
    }
}