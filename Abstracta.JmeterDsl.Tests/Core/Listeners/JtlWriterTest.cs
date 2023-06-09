using System.IO;

namespace Abstracta.JmeterDsl.Core.Listeners
{
    using static JmeterDsl;

    public class JtlWriterTest
    {
        [Test]
        public void ShouldWriteResultsToFileWhenJtlWriterAtTestPlan()
        {
            DirectoryInfo workDir = new DirectoryInfo("jtls");
            try
            {
                TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler("ok"),
                        JtlWriter(workDir.FullName)
                    )).Run();
                Assert.That(GetFileLinesCount(FindJtlFileInDirectory(workDir)), Is.EqualTo(2));
            }
            finally
            {
                workDir.Delete(true);
            }
        }

        private FileInfo FindJtlFileInDirectory(DirectoryInfo workDir) =>
            workDir.GetFiles("*.jtl")[0];

        private int GetFileLinesCount(FileInfo jtlFile) =>
            File.ReadAllLines(jtlFile.FullName).Length;
    }
}
