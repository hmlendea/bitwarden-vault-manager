using BitwardenVaultManager.Service.Models;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class BitwardenLoginTests
    {
        [Test]
        public void GivenAnUnsetTotpValue_WhenReadingTheProperty_ThenNullIsReturned()
        {
            BitwardenLogin login = new();

            Assert.That(login.TOTP, Is.Null);
        }

        [Test]
        public void GivenATotpValueContainingSpaces_WhenReadingTheProperty_ThenTheSpacesAreRemoved()
        {
            BitwardenLogin login = new()
            {
                TOTP = " ab cd ef 12 34 "
            };

            Assert.That(login.TOTP, Is.EqualTo("abcdef1234"));
        }
    }
}