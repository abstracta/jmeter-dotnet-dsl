using System;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    /// <summary>
    /// Represents the standard thread group test element included by JMeter.
    /// </summary>
    public class DslThreadGroup : BaseThreadGroup
    {
        private readonly int? _threads;
        private readonly int? _iterations;
        private readonly TimeSpan? _duration;

        public DslThreadGroup(string name, int threads, int iterations, IThreadGroupChild[] children)
            : base(name, children)
        {
            _threads = threads;
            _iterations = iterations;
        }

        public DslThreadGroup(string name, int threads, TimeSpan duration, IThreadGroupChild[] children)
            : base(name, children)
        {
            _threads = threads;
            _duration = duration;
        }
    }
}
