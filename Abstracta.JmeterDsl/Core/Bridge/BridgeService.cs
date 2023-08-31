using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Abstracta.JmeterDsl.Core.Bridge
{
    public class BridgeService
    {
        private string _jvmArgs = string.Empty;

        public BridgeService JvmArgs(string args)
        {
            _jvmArgs = args;
            return this;
        }

        public TestPlanStats RunTestPlanInEngine(DslTestPlan testPlan, IDslJmeterEngine engine)
        {
            var executionId = Guid.NewGuid().ToString();
            var tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), executionId));
            tempDir.Create();
            try
            {
                var responseFile = new FileInfo(Path.Combine(tempDir.FullName, "stats.yml"));
                RunBridgeCommand("run", new TestPlanExecution(engine, testPlan), $"\"{responseFile.FullName}\"");
                return responseFile.Exists ? ParseResponse<TestPlanStats>(responseFile) : null;
            }
            finally
            {
                tempDir.Delete(true);
            }
        }

        private void RunBridgeCommand(string command, object testElement, string args)
        {
            var classPath = SolveClassPath(testElement);
            var mainClass = "us.abstracta.jmeter.javadsl.bridge.BridgeService";
            var jvmArgs = _jvmArgs;
            var log4jConfigFile = new FileInfo("log4j2.xml");
            if (log4jConfigFile.Exists)
            {
                jvmArgs += $" -Dlog4j2.configurationFile=\"{log4jConfigFile.FullName}\"";
            }
            jvmArgs += $" -Dus.abstracta.jmeterdsl.userAgent=jmeter-dotnet-dsl/{Assembly.GetExecutingAssembly().GetName().Version}";
            using (var process = StartJvmProcess($"{jvmArgs} -cp \"{classPath}\" {mainClass} {command}" + (args != null ? " " + args : string.Empty)))
            {
                using (var stdin = process.StandardInput)
                {
                    SerializeObjectToWriter(testElement, stdin);
                }
                WaitJvmProcessExit(process);
            }
        }

        private void SerializeObjectToWriter(object val, TextWriter writer)
        {
            var testElementConverter = new BridgedObjectConverter();
            var builder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new EnumConverter())
                .WithTypeConverter(testElementConverter)
                .WithTypeConverter(new TimespanConverter());
            testElementConverter.ValueSerializer = builder.BuildValueSerializer();
            builder.Build().Serialize(writer, val);
        }

        private string SolveClassPath(object testElement)
        {
            var jarsDir = SolveJarsDir();
            if (JarsDirRequiresUpdate(jarsDir))
            {
                if (jarsDir.Exists)
                {
                    jarsDir.Delete(true);
                }
                jarsDir.Create();
            }
            SortedSet<string> jars = new SortedSet<string>();
            CopyRequiredJarsToDir(testElement, jarsDir, jars, new HashSet<string>(), new HashSet<string>());
            return string.Join(SolveClassPathSeparator(), jars);
        }

        private DirectoryInfo SolveJarsDir()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var homePath = Environment.GetEnvironmentVariable(envHome);
            return new DirectoryInfo(Path.Combine(homePath, ".jmeter-dsl", "jars"));
        }

        private bool JarsDirRequiresUpdate(DirectoryInfo jarsDir)
        {
            var jarName = ExtractJarNameFromResourceName(Assembly.GetExecutingAssembly().GetManifestResourceNames()[0]);
            return !new FileInfo(Path.Combine(jarsDir.FullName, jarName)).Exists;
        }

        private string ExtractJarNameFromResourceName(string resourceName)
        {
            var prefixDelimiter = ".artifacts.";
            var delimiterPos = resourceName.IndexOf(prefixDelimiter);
            return delimiterPos >= 0 && resourceName.EndsWith(".jar") ? resourceName.Substring(delimiterPos + prefixDelimiter.Length) : null;
        }

        private string SolveClassPathSeparator() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":";

        private void CopyRequiredJarsToDir(object element, DirectoryInfo dir, SortedSet<string> jars,
            ISet<string> processedAssemblies, ISet<string> processedTypes)
        {
            var elementType = element.GetType();
            if (!processedTypes.Add(elementType.FullName))
            {
                return;
            }
            var assembly = Assembly.GetAssembly(elementType);
            if (assembly == null || !processedAssemblies.Add(assembly.FullName))
            {
                return;
            }
            CopyAssemblyJarsToDir(assembly, dir, jars);
            var fields = element.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                var fieldValue = field.GetValue(element);
                if (fieldValue != null)
                {
                    CopyRequiredJarsToDir(fieldValue, dir, jars, processedAssemblies, processedTypes);
                }
            }
        }

        private void CopyAssemblyJarsToDir(Assembly assembly, DirectoryInfo dir, SortedSet<string> jars)
        {
            foreach (var r in assembly.GetManifestResourceNames())
            {
                CopyAssemblyJarResourceToDir(r, assembly, dir, jars);
            }
        }

        private void CopyAssemblyJarResourceToDir(string resourceName, Assembly assembly, DirectoryInfo dir, SortedSet<string> jars)
        {
            var jarName = ExtractJarNameFromResourceName(resourceName);
            if (jarName == null)
            {
                return;
            }
            var targetFile = new FileInfo(Path.Combine(dir.FullName, jarName));
            if (!targetFile.Exists)
            {
                using (var targetFileStream = targetFile.Create())
                {
                    assembly.GetManifestResourceStream(resourceName).CopyTo(targetFileStream);
                }
            }
            jars.Add(targetFile.FullName);
        }

        private Process StartJvmProcess(string jvmArgs)
        {
            var process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "java",
                Arguments = jvmArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.Error.WriteLine(e.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

        private void WaitJvmProcessExit(Process process)
        {
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new JvmException();
            }
        }

        private T ParseResponse<T>(FileInfo responseFile)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new TimespanConverter())
                .Build();
            using (var reader = new StreamReader(responseFile.FullName))
            {
                return deserializer.Deserialize<T>(reader);
            }
        }

        public void SaveTestPlanAsJmx(DslTestPlan testPlan, string filePath) =>
            RunBridgeCommand("saveAsJmx", testPlan, $"\"{filePath}\"");

        public void ShowTestElementInGui(IDslTestElement testElement) =>
            RunBridgeCommand("showInGui", testElement, null);
    }
}
