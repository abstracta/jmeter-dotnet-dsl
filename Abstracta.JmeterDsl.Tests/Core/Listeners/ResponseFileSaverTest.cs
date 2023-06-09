using System.IO;

namespace Abstracta.JmeterDsl.Core.Listeners
{
    using static JmeterDsl;

    public class ResponseFileSaverTest
    {
        [Test]
        public void ShouldWriteFileWithResponseContentWhenResponseFileSaverInPlan()
        {
            var workDir = new DirectoryInfo("responses");
            try
            {
                var body = "ok";
                TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler(body),
                        ResponseFileSaver(Path.Combine(workDir.FullName, "response"))
                    )).Run();
                var responseFileBody = File.ReadAllText(Path.Combine(workDir.FullName, "response1.unknown"));
                Assert.That(responseFileBody, Is.EqualTo(body));
            }
            finally
            {
                workDir.Delete(true);
            }
        }
    }
}
