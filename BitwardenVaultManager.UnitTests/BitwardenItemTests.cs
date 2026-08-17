using BitwardenVaultManager.Service.Models;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class BitwardenItemTests
    {
        [Test]
        public void GivenAnEmailAddressField_WhenGettingTheEmailAddress_ThenTheFieldValueIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Email Address", "person@example.com")
                ],
                Login = BuildLogin("fallback")
            };

            Assert.That(item.EmailAddress, Is.EqualTo("person@example.com"));
        }

        [Test]
        public void GivenALaterMatchingEmailAddressField_WhenGettingTheEmailAddress_ThenTheFirstValidMatchingValueIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Email Address", "not-an-email"),
                    BuildField("Email", "secondary@example.com")
                ],
                Login = BuildLogin("fallback@example.com")
            };

            Assert.That(item.EmailAddress, Is.EqualTo("secondary@example.com"));
        }

        [Test]
        public void GivenNoValidEmailAddressFieldAndAnEmailLogin_WhenGettingTheEmailAddress_ThenTheLoginUsernameIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Email Address", " "),
                    BuildField("Email", "invalid")
                ],
                Login = BuildLogin("fallback@example.com")
            };

            Assert.That(item.EmailAddress, Is.EqualTo("fallback@example.com"));
        }

        [Test]
        public void GivenNoEmailData_WhenGettingTheEmailAddress_ThenNullIsReturned()
        {
            BitwardenItem item = new()
            {
                Login = BuildLogin("plain-user")
            };

            Assert.That(item.EmailAddress, Is.Null);
        }

        [Test]
        public void GivenNoEmailDataAndNoLogin_WhenGettingTheEmailAddress_ThenNullIsReturned()
        {
            BitwardenItem item = new();

            Assert.That(item.EmailAddress, Is.Null);
        }

        [Test]
        public void GivenNoEmailDataAndAWhitespaceLoginUsername_WhenGettingTheEmailAddress_ThenNullIsReturned()
        {
            BitwardenItem item = new()
            {
                Login = BuildLogin(" ")
            };

            Assert.That(item.EmailAddress, Is.Null);
        }

        [Test]
        public void GivenAPhoneNumberField_WhenGettingThePhoneNumber_ThenTheNormalisedFieldValueIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Phone Number", "(0712) 345-678")
                ]
            };

            Assert.That(item.PhoneNumber, Is.EqualTo("+40712345678"));
        }

        [Test]
        public void GivenALaterMatchingPhoneNumberField_WhenGettingThePhoneNumber_ThenTheFirstValidMatchingValueIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Phone Number", "invalid"),
                    BuildField("phone", "0040 712 345 678")
                ],
                Login = BuildLogin("+44 123 456 789")
            };

            Assert.That(item.PhoneNumber, Is.EqualTo("+40712345678"));
        }

        [Test]
        public void GivenNoValidPhoneFieldAndAValidLoginPhoneNumber_WhenGettingThePhoneNumber_ThenTheNormalisedLoginUsernameIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Phone Number", "invalid")
                ],
                Login = BuildLogin("+44 (123) 456-789")
            };

            Assert.That(item.PhoneNumber, Is.EqualTo("+44123456789"));
        }

        [Test]
        public void GivenNoPhoneData_WhenGettingThePhoneNumber_ThenNullIsReturned()
        {
            BitwardenItem item = new()
            {
                Login = BuildLogin("plain-user")
            };

            Assert.That(item.PhoneNumber, Is.Null);
        }

        [Test]
        public void GivenNoPhoneDataAndNoLogin_WhenGettingThePhoneNumber_ThenNullIsReturned()
        {
            BitwardenItem item = new();

            Assert.That(item.PhoneNumber, Is.Null);
        }

        [Test]
        public void GivenAUsernameField_WhenGettingTheUsername_ThenTheFieldValueIsReturned()
        {
            BitwardenItem item = new()
            {
                Fields =
                [
                    BuildField("Username", "field-user")
                ],
                Login = BuildLogin("login-user")
            };

            Assert.That(item.Username, Is.EqualTo("field-user"));
        }

        [Test]
        public void GivenNoUsernameFieldAndALoginUsername_WhenGettingTheUsername_ThenTheLoginUsernameIsReturned()
        {
            BitwardenItem item = new()
            {
                Login = BuildLogin("login-user")
            };

            Assert.That(item.Username, Is.EqualTo("login-user"));
        }

        [Test]
        public void GivenNoUsernameData_WhenGettingTheUsername_ThenNullIsReturned()
        {
            BitwardenItem item = new();

            Assert.That(item.Username, Is.Null);
        }

        private static BitwardenField BuildField(string name, string value) => new()
        {
            Name = name,
            Value = value
        };

        private static BitwardenLogin BuildLogin(string username) => new()
        {
            Username = username
        };
    }
}