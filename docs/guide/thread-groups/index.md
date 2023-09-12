## Advanced threads configuration

JMeter DSL provides two simple ways of creating thread groups which are used in most scenarios:

* specifying threads and the number of iterations each thread should execute before ending the test plan
* specifying threads and duration for which each thread should execute before the test plan ends

This is how they look in code:

```cs
ThreadGroup(10, 20, ...) // 10 threads for 20 iterations each
ThreadGroup(10, TimeSpan.FromSeconds(20), ...) // 10 threads for 20 seconds each
```

But these options are not good when working with many threads or when trying to configure some complex test scenarios (like when doing incremental or peak tests).

<!-- @include: ramps-and-holds.md -->
