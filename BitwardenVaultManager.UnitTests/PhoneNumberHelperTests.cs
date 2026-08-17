using System;

using BitwardenVaultManager.Service.Helpers;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class PhoneNumberHelperTests
    {
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("   ", false)]
        [TestCase("07123", false)]
        [TestCase("0712abc345", false)]
        [TestCase("+40 712 345 678", true)]
        [TestCase("(0712) 345-678", true)]
        public void GivenVariousPhoneNumbers_WhenCheckingValidity_ThenTheExpectedResultIsReturned(string? value, bool expectedResult)
        {
            bool actualResult = PhoneNumberHelper.IsValid(value!);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase("  +40 712 345 678  ", "+40712345678")]
        [TestCase("0040 712-345-678", "+40712345678")]
        [TestCase("(0712) 345-678", "+40712345678")]
        [TestCase("+44 (123) 456-789", "+44123456789")]
        public void GivenVariousPhoneNumbers_WhenNormalising_ThenTheExpectedValueIsReturned(string phoneNumber, string expectedPhoneNumber)
        {
            string actualPhoneNumber = PhoneNumberHelper.Normalise(phoneNumber);

            Assert.That(actualPhoneNumber, Is.EqualTo(expectedPhoneNumber));
        }

        [Test]
        public void GivenANullPhoneNumber_WhenNormalising_ThenANullReferenceExceptionIsThrown()
        {
            TestDelegate action = () => PhoneNumberHelper.Normalise(null!);

            Assert.That(action, Throws.TypeOf<NullReferenceException>());
        }
    }
}