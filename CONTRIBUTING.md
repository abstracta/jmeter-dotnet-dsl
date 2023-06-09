# Contributing

This guide will introduce you to the code, to help you understand better how it works and send pull requests to submit contributions.

Before continuing, if you are not familiar with JMeter, please review [JMeter test plan elements](https://jmeter.apache.org/usermanual/test_plan.html) for a basic understanding of JMeter core concepts.

Also, take into consideration that .Net DSL is heavily based on [Java DSL](https://abstracta.github.io/jmeter-java-dsl). So having at least a basic understanding of [JMeter Java DSL contribution guidelines](https://github.com/abstracta/jmeter-java-dsl/blob/master/CONTRIBUTING.md) is recommended.

Let's start looking at each of the main classes, and later on, show a diagram to get an overview of their relations.

## Core classes

[JmeterDsl](Abstracta.JmeterDsl/JmeterDsl.cs) is the main entry point of the library and provides factory methods that allow the creation and execution of test plans. Each factory method receives as parameters the main attributes of a test element, which are required in most cases, and, when it is a natural container of other test elements (i.e.: it's of no use when no included children), the list of children test elements to nest on it (eg: thread groups to put on a test plan). 

Test elements are classes that implement the [IDslTestElement](Abstracta.JmeterDsl/Core/IDslTestElement.cs) interface, which set up JMeter test elements to be included in a test plan. They might also provide fluent API methods to set optional test element attributes (eg: HTTP method for HTTP sampler) or add children to them.

.Net DSL additionally provides mostly the same hierarchy of abstract classes and interfaces that JMeter Java DSL provides, which eases the creation of the different test elements that may compose a test plan.

In general, .Net DSL follows the same structure and rules as Java DSL, but the .Net DSL library is quite different in how it internally works, and in fact, this is caused by it just being a thin .Net adapter of Jmeter Java DSL.

## .Net DSL internals

As for you to better understand how .Net DSL works, and its dependency on Jmeter Java DSL, let's briefly review .Net DSL internal logic.

The .Net DSL library runs a test plan by serializing into an intermediary format (YAML) every element in a test plan. Then, uses the JMeter Java DSL Bridge module to run the serialized test plan. Every time a test plan requires execution, the .Net library invokes Java, providing all required jars (which are embedded resources of the .Net package) as classpath of the JVM, and invoking the BridgeService main class with the YAML test plan sent through the Java process std input. While the test plan runs, all java process std output and std error are redirected to the std output and error of the .Net library executing process. Once execution completes, the java process executing BridgeService generates an output YAML file, which the .Net library uses to provide collected statistics to performance test code.

The important thing to understand here is that every test element class in the .Net library is just a class that follows certain conventions to serialize it to YAML and then deserialize it in the Java process for test plan execution.

To get an idea of general test element hierarchy and DSL classes, you might check the [Java DSL class diagram](https://github.com/abstracta/jmeter-java-dsl/blob/master/CONTRIBUTING.md#class-diagram).

## .Net DSL classes conventions

Here are the main rules when defining a test element class in the .Net DSL:
* Namespace must match the Java package, but follow .Net conventions. For example: `us.abstracta.jmeter.javadsl.core.samplers` in Java is assumed to be `Abstracta.JmeterDsl.Core.Samplers` in .Net. 
* Class name must match the name of the Java builder method (with a potential `Dsl` prefix), or the class name if there is no builder method. Eg: `DslDefaultThreadGroup` in Java is actually `DslThreadGroup` (matching `threadGroup` Java builder method) in .Net. `AzureEngine` in Java is `AzureEngine` in .Net as well (there is no builder method associated to the engine). Even though `AzureEngine` is not actually a test element, since is not part of a test plan, and is in fact a JMeter DSL engine, in general, engine classes apply the same rules as test elements in .Net library.
* Class extends base classes and interfaces that match Java DSL equivalent base classes and interfaces. Eg: `DslHttpSampler` extends `BaseSampler` as in Java code.
* Class declares a protected field (with underscore prefixing) for each builder method parameter and optional test element properties method. Eg: `DslHttpSampler` declares fields `_url` (builder method), `_method` (optional property method), `_body` (optional property method), and inherits `_name` (used in builder methods) from `BaseTestElement`, and `_children` from `TestElementContainer`.
* Class declares constructor with required properties (same as Java DSL).
* Class declares optional properties methods that allow setting optional properties and return an instance of the test element for fluent API usage (same as Java DSL). Eg: `DslHttpSampler` declares `Method` and `Body` methods.
* Class declares additional optional property methods which are just abstractions and simplifications for setting some test element properties. Eg: `DslHttpSampler` declares `Post`, `Header`, and `ContentType` methods. `Post` is just a simplification that actually uses `Method`, `ContentType`, and `Body` methods. `Header` simplifies setting children elements. `ContentType` is a simplified way of using the `Header` method.
* Include xmldoc documentation which contains most of the already contained documentation in the Java docs analogous class, with potential clarifications for the .Net ecosystem.
* Include builder methods in the `JmeterDsl` class to ease the creation of test elements and require the user to just import one namespace and class (`JmeterDsl`).
* Include the test element in a package that is analogous to the Jmeter Java DSL modules. Eg: `DslHttpSampler` is included in `Abstracta.JmeterDsl` as is in Java in `jmeter-java-dsl`. `AzureEngine` is included in `Abstracta.JmeterDsl.Azure` as is in Java in `jmeter-java-dsl-azure`.

In general, when implementing a new element, it is advisable to explore .Net DSL code, check existing implemented elements that might be similar, and follow the same conventions.

## Test runs

As in Java DSL, when you want to run a test plan, it needs to run in a JMeter engine. By default, DslTestPlan uses [EmbeddedJmeterEngine](Abstracta.JmeterDsl/Core/Engines/EmbeddedJmeterEngine.cs), which is the fastest and easiest way to run a test plan, but you might use EmbeddedJmeterEngine as an example and implement your custom engine (for example to run tests in some cloud provider like Azure Load Testing).

When a test plan runs, the engine returns an instance of [TestPlanStats](Abstracta.JmeterDsl/Core/TestPlanStats.cs), grouping information by test element name (aka label). This allows users to check the expected statistics and verify that everything worked within expected boundaries.

## Implementing a new DSL test element or feature

Here we will detail the main steps and things to take into consideration when implementing a new test element, or extending an existing one.

1. Check if you want something that is already supported by Java DSL and JMeter itself.
   * This is very important and implementing support in the .Net library for something that is already supported in Java DSL is super simple. But, implementing something that has not yet support in the JMeter Java DSL, would require you to first contribute it to Java DSL, and then to the .Net library. Implementing something that is not even supported by JMeter, would require probably even more work, but don't despair, and always ask for help or support!
   * If you need something that is already supported by Java DSL, follow previously mentioned conventions and continue reading :).
   * If you need something that is not supported by Java DSL, then follow [Java DSL guidelines](https://github.com/abstracta/jmeter-java-dsl/blob/master/CONTRIBUTING.md#implementing-a-new-dsl-test-element-or-feature) and, after contributing the changes to Java DSL, follow previously mentioned conventions and continue reading!.
2. Implement tests that verify the expected behavior of the test element in a test plan execution. This way, you verify that you properly initialize JMeter properties and that your interpretation of the test element properties and behavior is right.
   * Check [DslHttpSamplerTest](Abstracta.JmeterDsl.Tests/Http/DslHttpSamplerTest.cs) for some sample test cases.
3.  Run `dotnet build` and `dotnet test` and fix any potential code styling issues or failing tests.
4.  Add a new section [user guide](docs/guide), by adding a new md file and proper `<!-- @include: -->` in parent section, describing the new feature. Consider running in the `docs` directory `pnpm install` and `pnpm dev` (this requires node 18+ and pnpm installed on your machine) to run a local server for docs, where you can review that new changes are properly showing.
5.  Commit changes to git, using as a comment a subject line that describes general changes, and if necessary, some additional details describing the reason why the change is necessary.
6.  Submit a pull request to the repository including a meaningful name.
7.  Check GitHub Actions execution to verify that no test fails on the CI pipeline.
8.  When the PR is merged and release is triggered. Enjoy the pleasure and the pride of contributing with an OSS tool :).

## General coding guidelines

* Review existing code, and get a general idea of main classes & conventions. It is important to try to keep code consistent to ease maintenance.
* In general, avoid code duplication to ease maintenance and readability.
* Use meaningful names for variables, methods, classes, etc. Avoid acronyms unless they are super intuitive.
* Strive for simplicity and reduce code and complexity whenever possible. Avoid over-engineering (implementing things for potential future scenarios).
* Use comments to describe the reason for some code (the "why"), either because is not the natural/obvious expected code, or because additional clarification is needed. Do not describe the "what". You can use variables, methods & class names to describe the "what".
* Provide xmldoc documentation for all public classes and methods that help users understand when to use the method, test element, etc.
* Avoid leaving `TODO` comments in the code. Create an issue in the GitHub repository, discussion, or include some comment in PR instead.
* Don't leave dead code (commented-out code).
* Avoid including backward incompatible changes (unless required), that would require users to change existing code where they use the API.
* Be gentle and thoughtful when you review code, contribute and submit pull requests :).

## FAQ

### I don't understand and still don't know how to implement what I need. What can I do?

Just create an issue in the repository stating what you need and why, and we will do our best to implement what you need :).

Or, check existing code. It contains embedded documentation with additional details, and the code never lies.
