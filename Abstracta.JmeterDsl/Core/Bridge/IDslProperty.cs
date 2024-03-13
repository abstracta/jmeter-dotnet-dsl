namespace Abstracta.JmeterDsl.Core.Bridge
{
    /// <summary>
    /// This is just a marker interface to properly serialize properties that can include multiple values.
    /// <br/>
    /// Such properties are added to a __propList c# class property and a class implementing IDslProperty is used to define the name of the property.
    /// <br/>
    /// <see cref="ThreadGroups.DslThreadGroup"/> and <see cref="Http.DslHttpSampler"/> for examples of classes using __propList and IDslProperty interface.
    /// </summary>
    public interface IDslProperty
    {
    }
}