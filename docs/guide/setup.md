## Setup

To use the DSL just include it in your project:

```powershell
dotnet add package Abstracta.JmeterDsl --version 0.1
```

::: tip
[Here](https://github.com/abstracta/jmeter-dotnet-dsl-sample) is a sample project in case you want to start one from scratch.
:::

::: warning
JMeter .Net DSL uses existing JMeter Java DSL which in turn uses JMeter. JMeter Java DSL and JMeter are Java based tools. So, **Java 8+ is required** for the proper execution of DSL test plans. One option is downloading a JVM from [Adoptium](https://adoptium.net/) if you don't have one already.
:::