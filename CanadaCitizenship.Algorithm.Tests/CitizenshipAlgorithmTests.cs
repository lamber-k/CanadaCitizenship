namespace CanadaCitizenship.Algorithm.Tests
{
    [TestClass]
    public class CitizenshipAlgorithmTests
    {
        [TestMethod]
        public void NoExclusionAndTemporaryFull_Compute_Completed()
        {
            var profile = new Profile("Test")
            {
                TemporaryDate = new DateTime(2020, 01, 01),
                PRDate = new DateTime(2022, 01, 01),
            };

            CitizenshipResult result = CitizenshipAlgorithm.Compute(profile, new DateTime(2024, 01, 01));

            Assert.AreEqual(365, result.TemporaryDays);
            Assert.AreEqual(731, result.PRDays);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.StartTemporary);
            Assert.AreEqual(new DateTime(2024, 01, 01), result.ProjectedDate);
            Assert.AreEqual(2, result.Periods.Length);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[0].Type);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.Periods[0].Begin);
            Assert.AreEqual(new DateTime(2021, 12, 31), result.Periods[0].End);
            Assert.AreEqual(PeriodType.PR, result.Periods[1].Type);
            Assert.AreEqual(new DateTime(2022, 01, 01), result.Periods[1].Begin);
            Assert.AreEqual(new DateTime(2024, 01, 01), result.Periods[1].End);
        }

        [TestMethod]
        public void NoExclusionAndFullTemporaryAndStartPR_Compute_In2Years()
        {
            var profile = new Profile("Test")
            {
                TemporaryDate = new DateTime(2020, 01, 01),
                PRDate = new DateTime(2022, 01, 01),
            };

            CitizenshipResult result = CitizenshipAlgorithm.Compute(profile, new DateTime(2022, 01, 01));

            Assert.AreEqual(365, result.TemporaryDays);
            Assert.AreEqual(1, result.PRDays);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.StartTemporary);
            Assert.AreEqual(new DateTime(2024, 01, 01), result.ProjectedDate);
            Assert.AreEqual(2, result.Periods.Length);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[0].Type);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.Periods[0].Begin);
            Assert.AreEqual(new DateTime(2021, 12, 31), result.Periods[0].End);
            Assert.AreEqual(PeriodType.PR, result.Periods[1].Type);
            Assert.AreEqual(new DateTime(2022, 01, 01), result.Periods[1].Begin);
            Assert.AreEqual(new DateTime(2023, 12, 31), result.Periods[1].End);
        }

        [TestMethod]
        public void OneExclusionTemporary_Compute_In2YearsPlusExclusion()
        {
            // T: Temporary date
            // v..v : Vacation
            // P: Permanent Residency
            // ^ : Today
            // o : Projected date
            // |--T-------v-v---------P^-------------------o---|
            var profile = new Profile("Test")
            {
                TemporaryDate = new DateTime(2020, 01, 01),
                PRDate = new DateTime(2022, 01, 01),
                ExclusionPeriods = [
                    new Period(new DateTime(2021, 03, 07), new DateTime(2021, 03, 14), PeriodType.Vacation) // One week vacation
                ]
            };

            CitizenshipResult result = CitizenshipAlgorithm.Compute(profile, new DateTime(2022, 01, 01));

            Assert.AreEqual(362, result.TemporaryDays);
            Assert.AreEqual(1, result.PRDays);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.StartTemporary);
            Assert.AreEqual(new DateTime(2024, 01, 04), result.ProjectedDate);
            Assert.AreEqual(4, result.Periods.Length);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[0].Type);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.Periods[0].Begin);
            Assert.AreEqual(new DateTime(2021, 03, 07), result.Periods[0].End);
            Assert.AreEqual(PeriodType.Vacation, result.Periods[1].Type);
            Assert.AreEqual(new DateTime(2021, 03, 07), result.Periods[1].Begin);
            Assert.AreEqual(new DateTime(2021, 03, 14), result.Periods[1].End);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[2].Type);
            Assert.AreEqual(new DateTime(2021, 03, 14), result.Periods[2].Begin);
            Assert.AreEqual(new DateTime(2021, 12, 31), result.Periods[2].End);
            Assert.AreEqual(PeriodType.PR, result.Periods[3].Type);
            Assert.AreEqual(new DateTime(2022, 01, 01), result.Periods[3].Begin);
            Assert.AreEqual(new DateTime(2024, 01, 03), result.Periods[3].End);
        }

        [TestMethod]
        public void OneExclusionWithNoStatusTemporary_Compute_In2YearsPlusExclusionPeriods()
        {
            // T: Temporary date
            // v..v : Vacation
            // s..s : No Status
            // P: Permanent Residency
            // ^ : Today
            // o : Projected date
            // |--T----s-sv-v---------P^-------------------o---|
            var profile = new Profile("Test")
            {
                TemporaryDate = new DateTime(2020, 01, 01),
                PRDate = new DateTime(2022, 01, 01),
                ExclusionPeriods = [
                    new Period(new DateTime(2021, 03, 01), new DateTime(2021, 03, 07), PeriodType.NoStatus), // Lost status
                    new Period(new DateTime(2021, 03, 07), new DateTime(2021, 03, 14), PeriodType.Vacation) // One week vacation
                ]
            };

            CitizenshipResult result = CitizenshipAlgorithm.Compute(profile, new DateTime(2022, 01, 01));

            Assert.AreEqual(359, result.TemporaryDays);
            Assert.AreEqual(1, result.PRDays);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.StartTemporary);
            Assert.AreEqual(new DateTime(2024, 01, 07), result.ProjectedDate);
            Assert.AreEqual(5, result.Periods.Length);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[0].Type);
            Assert.AreEqual(new DateTime(2020, 01, 01), result.Periods[0].Begin);
            Assert.AreEqual(new DateTime(2021, 03, 01), result.Periods[0].End);
            Assert.AreEqual(PeriodType.NoStatus, result.Periods[1].Type);
            Assert.AreEqual(new DateTime(2021, 03, 01), result.Periods[1].Begin);
            Assert.AreEqual(new DateTime(2021, 03, 07), result.Periods[1].End);
            Assert.AreEqual(PeriodType.Vacation, result.Periods[2].Type);
            Assert.AreEqual(new DateTime(2021, 03, 07), result.Periods[2].Begin);
            Assert.AreEqual(new DateTime(2021, 03, 14), result.Periods[2].End);
            Assert.AreEqual(PeriodType.Temporary, result.Periods[3].Type);
            Assert.AreEqual(new DateTime(2021, 03, 14), result.Periods[3].Begin);
            Assert.AreEqual(new DateTime(2021, 12, 31), result.Periods[3].End);
            Assert.AreEqual(PeriodType.PR, result.Periods[4].Type);
            Assert.AreEqual(new DateTime(2022, 01, 01), result.Periods[4].Begin);
            Assert.AreEqual(new DateTime(2024, 01, 06), result.Periods[4].End);
        }
    }
}