using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    /// <summary>
    /// Contains common logic for all Thread Groups.
    /// </summary>
    public abstract class BaseThreadGroup<T> : TestElementContainer<T, IThreadGroupChild>, ITestPlanChild
        where T : BaseThreadGroup<T>
    {
        protected DslSampleErrorAction? _sampleErrorAction;

        protected BaseThreadGroup(string name, IThreadGroupChild[] children)
            : base(name, children)
        {
        }

        /// <summary>
        /// Specifies an action to be taken by thread group when a sample error is detected.
        /// </summary>
        public enum DslSampleErrorAction
        {
            /// <summary>
            /// Ignores the error and continues execution with the next element in children elements, or
            /// starts a new iteration.
            /// </summary>
            Continue,

            /// <summary>
            /// Does not execute following elements in current iteration and jumps to a new iteration.
            /// </summary>
            StartNextIteration,

            /// <summary>
            /// Stops the thread, not executing any further children elements or iterations.
            /// </summary>
            StopThread,

            /// <summary>
            /// Stops the test plan, with all associated threads, when all current samples end.
            /// </summary>
            StopTest,

            /// <summary>
            /// Stops the test plan abruptly, with all associated threads, interrupting current samples.
            /// </summary>
            StopTestNow,
        }

        /// <summary>
        /// Specifies what action to be taken when a sample error is detected.
        /// </summary>
        /// <param name="sampleErrorAction">specifies the action to be taken on sample error. By default, thread
        /// groups just ignore the error and continue with following sample in
        /// children elements.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="DslSampleErrorAction"/>
        public T SampleErrorAction(DslSampleErrorAction sampleErrorAction)
        {
            _sampleErrorAction = sampleErrorAction;
            return (T)this;
        }
    }
}
