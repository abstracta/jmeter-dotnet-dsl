namespace Abstracta.JmeterDsl.Core.Configs
{
    using static JmeterDsl;

    public class DslCsvDataSetTest
    {
        [Test]
        public void ShouldGetExpectedSamplesWhenTestPlanWithSampleNamesFromCsvDataSet()
        {
            var stats = TestPlan(
                CsvDataSet("Core/Configs/data.csv"),
                ThreadGroup(1, 2,
                    DummySampler("${VAR1}-${VAR2}", "ok")
                )).Run();
            Assert.Multiple(() =>
            {
                Assert.That(stats.Labels["val1-val2"].SamplesCount, Is.EqualTo(1));
                Assert.That(stats.Labels["val,3-val4"].SamplesCount, Is.EqualTo(1));
            });
        }
    }
}
