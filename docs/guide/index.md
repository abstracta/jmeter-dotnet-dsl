# User guide

Here we share some tips and examples on how to use the DSL to tackle common use cases.

Provided examples use [Nunit](https://nunit.org/), but you can use other test libraries.

Explore the DSL in your preferred IDE to discover all available features, and consider reviewing [existing tests](/Abstracta.JmeterDsl.Tests) for additional examples.

The .Net DSL currently does not support all use cases supported by the [Java Dsl](https://abstracta.github.io/jmeter-java-dsl/), and currently only focuses on a limited set of features that cover the most commonly used cases. If you identify any particular scenario (or JMeter feature) that you need and is not currently supported, or easy to use, **please let us know by [creating an issue](https://github.com/abstracta/jmeter-dotnet-dsl/issues)** and we will try to implement it as soon as possible. Usually porting JMeter features is quite fast, and porting existing Java DSL features is even faster.

::: tip
If you like this project, **please give it a star ⭐ in [GitHub](https://github.com/abstracta/jmeter-dotnet-dsl)!**. This helps the project be more visible, gain relevance and encourages us to invest more effort in new features.
:::

For an intro to JMeter concepts and components, you can check [JMeter official documentation](http://jmeter.apache.org/usermanual/get-started.html).

<!-- @include: setup.md -->
<!-- @include: simple-test-plan.md -->
<!-- @include: scale/index.md -->
<!-- @include: thread-groups/index.md -->
<!-- @include: debugging/index.md -->
<!-- @include: reporting/index.md -->
<!-- @include: response-processing/index.md -->
<!-- @include: protocols/index.md -->
