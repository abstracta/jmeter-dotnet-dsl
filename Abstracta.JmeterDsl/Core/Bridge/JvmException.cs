using System;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    /// <summary>
    /// Exception thrown when there is some problem interacting with Java Virutal Machine or executing a particular command.
    /// <br/>
    /// This might be to some exception generated in test plan execution, showing element in GUI, or event issues with JVM initialization.
    /// </summary>
    public class JvmException : Exception
    {
        public JvmException()
            : base("JVM execution failed. Check stderr and stdout for additional info.")
        {
        }
    }
}
