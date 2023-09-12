using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    /// <summary>
    /// Represents the standard thread group test element included by JMeter.
    /// </summary>
    public class DslThreadGroup : BaseThreadGroup<DslThreadGroup>
    {
        private readonly List<Stage> __propsList = new List<Stage>();

        public DslThreadGroup(string name)
            : base(name, new IThreadGroupChild[0])
        {
        }

        public DslThreadGroup(string name, int threads, int iterations, IThreadGroupChild[] children)
            : base(name, children)
        {
            CheckThreadCount(threads);
            __propsList.Add(new DslRampTo(threads, 0));
            __propsList.Add(new DslHoldIterating(threads, iterations));
        }

        public DslThreadGroup(string name, int threads, TimeSpan duration, IThreadGroupChild[] children)
            : base(name, children)
        {
            CheckThreadCount(threads);
            __propsList.Add(new DslRampTo(threads, 0));
            __propsList.Add(new DslHoldFor(threads, RoundToSeconds(duration)));
        }

        private void CheckThreadCount(int threads)
        {
            if (threads <= 0)
            {
                throw new ArgumentException("Threads count must be >=1");
            }
        }

        private int RoundToSeconds(TimeSpan duration) =>
            (int)Math.Round((double)duration.TotalMilliseconds / 1000);

        /// <summary>
        /// Allows ramping up or down threads with a given duration.
        /// <br/>
        /// It is usually advised to use this method when working with considerable amount of threads to
        /// avoid load of creating all the threads at once to affect test results.
        /// <br/>
        /// JMeter will create (or remove) a thread every <c>rampUp.seconds * 1000 / threadCount</c> milliseconds.
        /// <br/>
        /// If you specify a thread duration time (instead of iterations), take into consideration that
        /// ramp up is not considered as part of thread duration time. For example: if you have a thread
        /// group duration of 10 seconds, and a ramp-up of 10 seconds, the last threads (and the test plan
        /// run) will run at least (duration may vary depending on test plan contents) after 20 seconds of
        /// starting the test.
        /// <br/>
        /// You can use this method multiple times in a thread group and in conjunction with
        /// <see cref="HoldFor(TimeSpan)"/> and <see cref="RampToAndHold(int, TimeSpan, TimeSpan)"/> to elaborate
        /// complex test plan profiles.
        /// <br/>
        /// Eg:
        /// <c>
        /// ThreadGroup()
        ///   .RampTo(10, TimeSpan.FromSeconds(10))
        ///   .RampTo(5, TimeSpan.FromSeconds(10))
        ///   .RampToAndHold(20, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10))
        ///   .RampTo(0, TimeSpan.FromSeconds(5))
        ///   .Children(...)
        /// </c>
        /// </summary>
        /// <param name="threadCount">specifies the final number of threads after the given period.</param>
        /// <param name="duration">duration taken to reach the given threadCount and move to the next stage or
        /// end the test plan. Since JMeter only supports specifying times in seconds,
        /// if you specify a smaller granularity (like milliseconds) it will be rounded
        /// up to seconds.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <exception cref="ArgumentException">if used after an iterations stage, since JMeter does not provide
        /// built-in thread group to support such scenario.</exception>
        public DslThreadGroup RampTo(int threadCount, TimeSpan duration)
        {
            if (threadCount < 0)
            {
                throw new ArgumentException("Thread count must be >=0");
            }
            CheckRampNotAfterIterations();
            AddStage(new DslRampTo(threadCount, RoundToSeconds(duration)));
            return this;
        }

        private void CheckRampNotAfterIterations()
        {
            if (IsLastStageHoldingForIterations())
            {
                throw new InvalidOperationException(
                    "Ramping up/down after holding for iterations is not supported. "
                        + "If you used constructor with iterations and some ramp "
                        + "(eg: threadGroup(X, Y, ...).rampTo(X, Z)), consider using "
                        + "threadGroup().rampTo(X, Z).holdIterating(Y) instead");
            }
        }

        private bool IsLastStageHoldingForIterations() =>
            __propsList.Any() && GetLastStage()._duration == null;

        private Stage GetLastStage() =>
            __propsList.Last();

        private void AddStage(Stage stage)
        {
            __propsList.Add(stage);
            if (!IsSimpleThreadGroup() && __propsList.Any(s => !s.IsFixedStage()))
            {
                __propsList.RemoveAt(__propsList.Count - 1);
                throw new NotSupportedException(
                    "The DSL does not yet support configuring multiple thread ramps with ramp or hold "
                        + "parameters using jmeter expressions. If you need this please create an issue in "
                        + "Github repository.");
            }
        }

        private bool IsSimpleThreadGroup()
        {
            return __propsList.Count <= 1
                || (__propsList.Count == 2
                    && (0.Equals(__propsList[0]._threadCount)
                    || __propsList[0]._threadCount.Equals(__propsList[1]._threadCount)))
                || (__propsList.Count == 3
                    && 0.Equals(__propsList[0]._threadCount)
                    && __propsList[1]._threadCount.Equals(__propsList[2]._threadCount));
        }

        /// <summary>
        /// Same as <see cref="RampTo(int, TimeSpan)"/> but allowing to use JMeter expressions (variables or
        /// functions) to solve the actual parameter values.
        /// <br/>
        /// This is usually used in combination with properties to define values that change between
        /// environments or different test runs. Eg: <c>RampTo("${THREADS}", "${RAMP}"}</c>.
        /// <br/>
        /// This method can only be used for simple thread group configurations. Allowed combinations are:
        /// RampTo, RampTo + HoldFor, HoldFor + RampTo + HoldFor, RampTo + HoldIterating, HoldFor + RampTo
        /// + HoldIterating.
        /// </summary>
        /// <param name="threadCount">a JMeter expression that returns the number of threads to ramp to.</param>
        /// <param name="duration">a JMeter expression that returns the number of seconds to take for the ramp.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="RampTo(int, TimeSpan)"/>
        public DslThreadGroup RampTo(string threadCount, string duration)
        {
            CheckRampNotAfterIterations();
            AddStage(new DslRampTo(threadCount, duration));
            return this;
        }

        /// <summary>
        /// Specifies to keep current number of threads for a given duration.
        /// <br/>
        /// This method is usually used in combination with <see cref="RampTo(int, TimeSpan)"/> to define the
        /// profile of the test plan.
        /// </summary>
        /// <param name="duration">duration to hold the current number of threads until moving to next stage or
        /// ending the test plan. Since JMeter only supports specifying times in seconds,
        /// if you specify a smaller granularity (like milliseconds) it will be rounded up
        /// to seconds.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="RampTo(int, TimeSpan)"/>
        public DslThreadGroup HoldFor(TimeSpan duration)
        {
            CheckHoldNotAfterIterations();
            AddStage(new DslHoldFor(GetPrevThreadsCount(), RoundToSeconds(duration)));
            return this;
        }

        private void CheckHoldNotAfterIterations()
        {
            if (IsLastStageHoldingForIterations())
            {
                throw new InvalidOperationException(
                    "Holding for duration after holding for iterations is not supported.");
            }
        }

        private object GetPrevThreadsCount() =>
            !__propsList.Any() ? 0 : GetLastStage()._threadCount;

        /// <summary>
        /// Same as <see cref="HoldFor(TimeSpan)"/> but allowing to use JMeter expressions (variables or
        /// functions) to solve the duration.
        /// <br/>
        /// This is usually used in combination with properties to define values that change between
        /// environments or different test runs. Eg: <c>HoldFor("${DURATION}"}</c>.
        /// <br/>
        /// This method can only be used for simple thread group configurations. Allowed combinations are:
        /// RampTo, RampTo + HoldFor, HoldFor + RampTo + HoldFor, RampTo + HoldIterating, HoldFor + RampTo
        /// + HoldIterating.
        /// </summary>
        /// <param name="duration">a JMeter expression that returns the number of seconds to hold current thread
        /// groups.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="HoldFor(TimeSpan)"/>
        public DslThreadGroup HoldFor(string duration)
        {
            object threadsCount = GetPrevThreadsCount();
            CheckHoldNotAfterIterations();
            AddStage(new DslHoldFor(threadsCount, duration));
            return this;
        }

        /// <summary>
        /// Specifies to keep current number of threads until they execute the given number of iterations
        /// each.
        /// <br/>
        /// <b>Warning: </b> holding for iterations can be added to a thread group that has an initial
        /// stage with 0 threads followed by a stage ramping up, or only a stage ramping up, or no stages
        /// at all.
        /// </summary>
        /// <param name="iterations">number of iterations to execute the test plan steps each thread.
        /// <br/>
        /// If you specify -1, then threads will iterate until test plan execution is
        /// interrupted (you manually stop the running process, there is an error and
        /// thread group is configured to stop on error, or some other explicit
        /// termination condition).
        /// <br/>
        /// <b>Setting this property to -1 is in general not advised</b>, since you
        /// might inadvertently end up running a test plan without limits consuming unnecessary
        /// computing power. Prefer specifying a big value as a safe limit for iterations
        /// or duration instead.
        /// </param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <exception cref="InvalidOperationException">when adding iterations would result in not supported JMeter
        /// thread group.</exception>
        public DslThreadGroup HoldIterating(int iterations)
        {
            CheckIterationsPreConditions();
            AddStage(new DslHoldIterating(GetLastStage()._threadCount, iterations));
            return this;
        }

        private void CheckIterationsPreConditions()
        {
            if (!((__propsList.Count == 1 && !0.Equals(__propsList[0]._threadCount))
                || (__propsList.Count == 2 && 0.Equals(__propsList[0]._threadCount)
                    && !0.Equals(__propsList[1]._threadCount))))
            {
                throw new InvalidOperationException(
                    "Holding for iterations is only supported after initial hold and ramp, or ramp.");
            }
            if (0.Equals(GetLastStage()._threadCount))
            {
                throw new InvalidOperationException("Can't hold for iterations with no threads.");
            }
        }

        /// <summary>
        /// Same as <see cref="HoldIterating(int)"/> but allowing to use JMeter expressions (variables or
        /// functions) to solve the iterations.
        /// <br/>
        /// This is usually used in combination with properties to define values that change between
        /// environments or different test runs. Eg: <c>HoldIterating("${ITERATIONS}"}</c>.
        /// <br/>
        /// This method can only be used for simple thread group configurations. Allowed combinations are:
        /// RampTo, RampTo + HoldFor, HoldFor + RampTo + HoldFor, RampTo + HoldIterating, HoldFor + RampTo
        /// + HoldIterating.
        /// </summary>
        /// <param name="iterations">a JMeter expression that returns the number of iterations for current threads
        /// to execute.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="HoldIterating(int)"/>
        public DslThreadGroup HoldIterating(string iterations)
        {
            CheckIterationsPreConditions();
            AddStage(new DslHoldIterating(GetLastStage()._threadCount, iterations));
            return this;
        }

        /// <summary>
        /// simply combines <see cref="RampTo(int, TimeSpan)"/> and <see cref="HoldFor(TimeSpan)"/> which are usually
        /// used in combination.
        /// </summary>
        /// <param name="threads">number of threads to ramp threads up/down to.</param>
        /// <param name="rampDuration">duration taken to reach the given threadCount to start holding that number
        /// of threads.</param>
        /// <param name="holdDuration">duration to hold the given number of threads, after the ramp, until moving
        /// to next stage or ending the test plan.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="RampTo(int, TimeSpan)"/>
        /// <seealso cref="HoldFor(TimeSpan)"/>
        public DslThreadGroup RampToAndHold(int threads, TimeSpan rampDuration, TimeSpan holdDuration) =>
            RampTo(threads, rampDuration)
                .HoldFor(holdDuration);

        /// <summary>
        /// Same as <see cref="RampToAndHold(int, TimeSpan, TimeSpan)"/> but allowing to use JMeter expressions
        /// (variables or functions) to solve the actual parameter values.
        /// <br/>
        /// This is usually used in combination with properties to define values that change between
        /// environments or different test runs. Eg: <c>RampToAndHold("${THREADS}", "${RAMP}" ,"${DURATION}"}</c>.
        /// <br/>
        /// This method can only be used for simple thread group configurations. Allowed combinations are:
        /// RampTo, RampTo + HoldFor, HoldFor + RampTo + HoldFor, RampTo + HoldIterating, HoldFor + RampTo
        /// + HoldIterating.
        /// </summary>
        /// <param name="threads">a JMeter expression that returns the number of threads to ramp to.</param>
        /// <param name="rampDuration">a JMeter expression that returns the number of seconds to take for the
        /// ramp.</param>
        /// <param name="holdDuration">a JMeter expression that returns the number of seconds to hold current
        /// thread groups.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        /// <seealso cref="RampToAndHold(int, TimeSpan, TimeSpan)"/>
        public DslThreadGroup RampToAndHold(string threads, string rampDuration, string holdDuration) =>
            RampTo(threads, rampDuration)
                .HoldFor(holdDuration);

        /// <summary>
        /// Allows specifying thread group children elements (samplers, listeners, post processors, etc.).
        /// <br/>
        /// This method is just an alternative to the constructor specification of children, and is handy
        /// when you want to keep general thread group settings together and then specify children (instead
        /// of specifying threadCount &amp; duration/iterations, then children, and at the end alternative
        /// settings like ramp-up period).
        /// </summary>
        /// <param name="children">list of test elements to add as children of the thread group.</param>
        /// <returns>the thread group for further configuration or usage.</returns>
        public new DslThreadGroup Children(params IThreadGroupChild[] children) =>
            base.Children(children);

        internal abstract class Stage : IDslTestElement
        {
            internal readonly object _threadCount;
            internal readonly object _duration;
            internal readonly object _iterations;

            protected Stage(object threadCount, object duration, object iterations)
            {
                _threadCount = threadCount;
                _duration = duration;
                _iterations = iterations;
            }

            public bool IsFixedStage() =>
                _threadCount is int
                    && (_duration == null || _duration is int)
                    && (_iterations == null || _iterations is int);

            public void ShowInGui() => throw new NotImplementedException();
        }

        internal class DslRampTo : Stage
        {
            public DslRampTo(object threadCount, object duration)
                : base(threadCount, duration, null)
            {
            }
        }

        internal class DslHoldFor : Stage
        {
            [YamlIgnore]
            internal new readonly object _threadCount;

            public DslHoldFor(object threadCount, object duration)
                : base(threadCount, duration, null)
            {
                _threadCount = threadCount;
            }
        }

        internal class DslHoldIterating : Stage
        {
            [YamlIgnore]
            internal new readonly object _threadCount;

            public DslHoldIterating(object threadCount, object iterations)
                : base(threadCount, null, iterations)
            {
                _threadCount = threadCount;
            }
        }
    }
}
