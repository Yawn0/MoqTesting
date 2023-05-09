using Moq;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object); //system under test
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>
            {
                DefaultValue = DefaultValue.Mock
            };
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object); //system under test
            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(x => x.StartsWith("y")))).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Exclusive))).Returns(true);
            mockValidator.Setup(x => x.IsValid(It.IsIn("z", "b", "x"))).Returns(true);

            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object); //system under test
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "z"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferValidRequestFlyerApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);   // "strict" checks if properties and methods of the interface have been setup
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object); //system under test
            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    bool isValid = true;
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object); //system under test
        //    var application = new CreditCardApplication()
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42,
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns(CreditCardApplicationEvaluator.EXPIRED);

            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.LicenseData).Returns(mockLicenseData.Object);

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns(CreditCardApplicationEvaluator.EXPIRED);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidatinMode);
            mockValidator.SetupAllProperties();

            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidatinMode.Detailed, mockValidator.Object.ValidatinMode);
        }

        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication()
            {
                FrequentFlyerNumber = "1"
            };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "FrequestFlyerNumber should be validated");
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.LicenseData.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication()
            {
                GrossAnnualIncome = 100_000
            };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }
    }
}