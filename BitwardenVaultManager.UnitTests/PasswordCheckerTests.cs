using BitwardenVaultManager.Service;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class PasswordCheckerTests
    {
        private PasswordChecker subject;

        [SetUp]
        public void SetUp()
            => subject = new();

        [TestCase(null, PasswordStrength.Terrible)]
        [TestCase("", PasswordStrength.Terrible)]
        [TestCase("   ", PasswordStrength.Terrible)]
        [TestCase("abcdefghi", PasswordStrength.VeryWeak)]
        [TestCase("abcdefghij", PasswordStrength.Weak)]
        [TestCase("abcdefghijklmnopqrst", PasswordStrength.Poor)]
        [TestCase("abcdefghijklmnopqrstuvwxyz1234", PasswordStrength.Good)]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#", PasswordStrength.Ultimate)]
        public void GivenVariousPasswords_WhenGettingPasswordStrength_ThenTheExpectedStrengthIsReturned(
            string? password,
            PasswordStrength expectedStrength)
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength(password!);

            Assert.That(actualStrength, Is.EqualTo(expectedStrength));
        }

        [Test]
        public void GivenALongPasswordContainingOnlyLowercaseLetters_WhenGettingPasswordStrength_ThenOnlyLengthBonusesAreApplied()
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefgh");

            Assert.That(actualStrength, Is.EqualTo(PasswordStrength.Strong));
        }

        [Test]
        public void GivenAPasswordContainingMixedCaseLetters_WhenGettingPasswordStrength_ThenTheMixedCaseBonusIsApplied()
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength("Abcdefghij");

            Assert.That(actualStrength, Is.EqualTo(PasswordStrength.Poor));
        }

        [Test]
        public void GivenAPasswordContainingRecommendedCountsOfMixedCaseLetters_WhenGettingPasswordStrength_ThenTheRecommendedLetterBonusIsApplied()
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength("ABCdefghij");

            Assert.That(actualStrength, Is.EqualTo(PasswordStrength.Acceptable));
        }

        [Test]
        public void GivenAPasswordContainingDigits_WhenGettingPasswordStrength_ThenTheDigitBonusesAreApplied()
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength("abc1234567!!!");

            Assert.That(actualStrength, Is.EqualTo(PasswordStrength.Good));
        }

        [Test]
        public void GivenAPasswordContainingSymbols_WhenGettingPasswordStrength_ThenTheSymbolBonusesAreApplied()
        {
            PasswordStrength actualStrength = subject.GetPasswordStrength("abcdefghij!!!");

            Assert.That(actualStrength, Is.EqualTo(PasswordStrength.Acceptable));
        }
    }
}