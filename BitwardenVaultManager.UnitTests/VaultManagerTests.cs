using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess;
using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service;
using BitwardenVaultManager.Service.Models;

using Moq;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class VaultManagerTests
    {
        private static string VaultFilePath => "vault.json";

        private Mock<IBitwardenVaultFileHandler> vaultFileHandlerMock;
        private Mock<IPasswordChecker> passwordCheckerMock;
        private VaultManager subject;

        [SetUp]
        public void SetUp()
        {
            vaultFileHandlerMock = new();
            passwordCheckerMock = new();
            passwordCheckerMock
                .Setup(passwordChecker => passwordChecker.GetPasswordStrength(It.IsAny<string>()))
                .Returns(PasswordStrength.Strong);
            subject = new(vaultFileHandlerMock.Object, passwordCheckerMock.Object);
        }

        [Test]
        public void GivenTheDefaultConstructor_WhenCreatingTheVaultManager_ThenAnInstanceIsCreated()
            => Assert.That(new VaultManager(), Is.Not.Null);

        [Test]
        public void GivenALoadedVault_WhenGettingMisconfiguredItems_ThenOnlyLoginItemsWithoutEmailAddressesAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("Missing Email", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", []),
                    BuildItemEntity("Valid Email", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", []),
                    BuildItemEntity("Credit Card", BitwardenItemType.CreditCard, folderId, "plain-user", "Password123!", "", [])
                ],
                [BuildFolderEntity(folderId, "Personal")]));

            List<string> errors = subject.GetMisconfiguredItems().ToList();

            Assert.That(errors, Has.Count.EqualTo(1));
            Assert.That(errors.Single(), Is.EqualTo("The 'Missing Email' login does not have an 'Email Address' field"));
        }

        [Test]
        public void GivenACompletelyConfiguredVault_WhenGettingMisconfiguredItems_ThenNoErrorsAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("Valid Email", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", [])
                ],
                [BuildFolderEntity(folderId, "Personal")]));

            IEnumerable<string> errors = subject.GetMisconfiguredItems();

            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void GivenAnExistingFolderIdentifier_WhenGettingTheFolderName_ThenTheFolderNameIsReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity([], [BuildFolderEntity(folderId, "Personal")]));

            string folderName = subject.GetFolderName(folderId);

            Assert.That(folderName, Is.EqualTo("Personal"));
        }

        [Test]
        public void GivenAnUnknownFolderIdentifier_WhenGettingTheFolderName_ThenNullIsReturned()
        {
            LoadVault(BuildVaultEntity([], []));

            string folderName = subject.GetFolderName(Guid.NewGuid());

            Assert.That(folderName, Is.Null);
        }

        [Test]
        public void GivenVariousItems_WhenGettingUsernames_ThenDistinctCaseInsensitiveUsernamesAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "Person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", []),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "", "Password123!", "", [BuildFieldEntity("Username", "FieldUser")]),
                    BuildItemEntity("Four", BitwardenItemType.Login, folderId, "", "Password123!", "", []),
                    BuildItemEntity("Five", BitwardenItemType.Login, folderId, "   ", "Password123!", "", [])
                ],
                []));

            List<string> usernames = subject.GetUsernames().ToList();

            Assert.That(usernames, Is.EqualTo(new[] { "Person@example.com", "FieldUser" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingEmailAddresses_ThenDistinctLowercaseAddressesAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "Person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", []),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", []),
                    BuildItemEntity("Four", BitwardenItemType.Login, folderId, "", "Password123!", "", [BuildFieldEntity("Email", "Second@Example.Com")])
                ],
                []));

            List<string> emailAddresses = subject.GetEmailAddresses().ToList();

            Assert.That(emailAddresses, Is.EqualTo(new[] { "person@example.com", "second@example.com" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingPhoneNumbers_ThenDistinctNormalisedPhoneNumbersAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "+40 712 345 678", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "0040 712 345 678", "Password123!", "", []),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [BuildFieldEntity("phone", "+44 (123) 456-789")])
                ],
                []));

            List<string> phoneNumbers = subject.GetPhoneNumbers().ToList();

            Assert.That(phoneNumbers, Is.EqualTo(new[] { "+40712345678", "+44123456789" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingPasswords_ThenDistinctLoginPasswordsAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "other@example.com", "Password123!", "", []),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "third@example.com", "Different456!", "", []),
                    BuildItemEntity("Four", BitwardenItemType.CreditCard, folderId, "plain-user", "CardPassword", "", []),
                    BuildItemEntity("Five", BitwardenItemType.Login, folderId, "blank@example.com", "   ", "", [])
                ],
                []));

            List<string> passwords = subject.GetPasswords().ToList();

            Assert.That(passwords, Is.EqualTo(new[] { "Password123!", "Different456!" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByEmailAddress_ThenMatchingItemsAreReturnedCaseInsensitively()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "Person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [BuildFieldEntity("Email", "person@example.com")]),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "another@example.com", "Password123!", "", []),
                    BuildItemEntity("Four", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByEmailAddress("PERSON@example.com").Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "One", "Two" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByPhoneNumber_ThenMatchingItemsAreReturnedUsingTheNormalisedPhoneNumber()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "+40 712 345 678", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [BuildFieldEntity("phone", "0040 712 345 678")]),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "+44 123 456 789", "Password123!", "", []),
                    BuildItemEntity("Four", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByPhoneNumber("(0712) 345-678").Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "One", "Two" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByPassword_ThenOnlyExactPasswordMatchesAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "other@example.com", "password123!", "", []),
                    BuildItemEntity("Three", BitwardenItemType.CreditCard, folderId, "plain-user", "Password123!", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByPassword("Password123!").Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "One" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByPasswordContaining_ThenOnlyExactCaseMatchesAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "person@example.com", "AlphaCasePassword", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "other@example.com", "alphacasepassword", "", []),
                    BuildItemEntity("Three", BitwardenItemType.CreditCard, folderId, "plain-user", "AlphaCasePassword", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByPasswordContaining("Case").Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "One" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByPasswordLength_ThenOnlyPasswordsOfTheRequestedLengthAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "person@example.com", "abcd", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "other@example.com", "abcde", "", []),
                    BuildItemEntity("Three", BitwardenItemType.CreditCard, folderId, "plain-user", "abcd", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByPasswordLength(4).Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "One" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsByUsername_ThenMatchingItemsAreReturnedCaseInsensitively()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("One", BitwardenItemType.Login, folderId, "Person@example.com", "Password123!", "", []),
                    BuildItemEntity("Two", BitwardenItemType.Login, folderId, "plain-user", "Password123!", "", [BuildFieldEntity("Username", "FieldUser")]),
                    BuildItemEntity("Three", BitwardenItemType.Login, folderId, "different", "Password123!", "", []),
                    BuildItemEntity("Four", BitwardenItemType.Login, folderId, "", "Password123!", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsByUsername("fielduser").Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "Two" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsWithWeakPasswords_ThenOnlyWeakLoginsWithoutAnEnabledWeakPasswordMarkerAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            ConfigurePasswordStrengths(
                new Dictionary<string, PasswordStrength>
                {
                    ["WeakOne"] = PasswordStrength.Weak,
                    ["WeakTwo"] = PasswordStrength.Poor,
                    ["WeakThree"] = PasswordStrength.Acceptable,
                    ["WeakIgnored"] = PasswordStrength.Weak,
                    ["StrongPassword"] = PasswordStrength.VeryStrong
                });
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("Null Fields", BitwardenItemType.Login, folderId, "one@example.com", "WeakOne", "", null),
                    BuildItemEntity("No Marker", BitwardenItemType.Login, folderId, "two@example.com", "WeakTwo", "", []),
                    BuildItemEntity("False Marker", BitwardenItemType.Login, folderId, "three@example.com", "WeakThree", "", [BuildFieldEntity("Weak Password", "false")]),
                    BuildItemEntity("True Marker", BitwardenItemType.Login, folderId, "four@example.com", "WeakIgnored", "", [BuildFieldEntity("Weak Password", "true")]),
                    BuildItemEntity("Strong Password", BitwardenItemType.Login, folderId, "five@example.com", "StrongPassword", "", []),
                    BuildItemEntity("Credit Card", BitwardenItemType.CreditCard, folderId, "plain-user", "WeakOne", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsWithWeakPasswords().Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "Null Fields", "No Marker", "False Marker" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingItemsWithoutTotp_ThenOnlyWeakLoginsWithoutTotpAreReturned()
        {
            Guid folderId = Guid.NewGuid();
            ConfigurePasswordStrengths(
                new Dictionary<string, PasswordStrength>
                {
                    ["WeakOne"] = PasswordStrength.Weak,
                    ["WeakTwo"] = PasswordStrength.Poor,
                    ["StrongPassword"] = PasswordStrength.VeryStrong
                });
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("Weak Without Totp", BitwardenItemType.Login, folderId, "one@example.com", "WeakOne", "", []),
                    BuildItemEntity("Weak With Whitespace Totp", BitwardenItemType.Login, folderId, "two@example.com", "WeakTwo", "   ", []),
                    BuildItemEntity("Weak With Totp", BitwardenItemType.Login, folderId, "three@example.com", "WeakTwo", "ABCDEF", []),
                    BuildItemEntity("Strong Without Totp", BitwardenItemType.Login, folderId, "four@example.com", "StrongPassword", "", []),
                    BuildItemEntity("Credit Card", BitwardenItemType.CreditCard, folderId, "plain-user", "WeakOne", "", [])
                ],
                []));

            List<string> itemNames = subject.GetItemsWithoutTotp().Select(item => item.Name).ToList();

            Assert.That(itemNames, Is.EqualTo(new[] { "Weak Without Totp", "Weak With Whitespace Totp" }));
        }

        [Test]
        public void GivenVariousItems_WhenGettingTotpUrls_ThenTheUrlsAreOrderedAndUseTheExpectedParameters()
        {
            Guid folderId = Guid.NewGuid();
            LoadVault(BuildVaultEntity(
                [
                    BuildItemEntity("Steam", BitwardenItemType.Login, folderId, "steam-user", "Password123!", "STEAM", [], true),
                    BuildItemEntity("Blizzard", BitwardenItemType.Login, folderId, "blizzard-user", "Password123!", "BLIZZARD", []),
                    BuildItemEntity("Battle.net", BitwardenItemType.Login, folderId, "battle-user", "Password123!", "BATTLE", []),
                    BuildItemEntity("Gemini", BitwardenItemType.Login, folderId, "gemini-user", "Password123!", "GEMINI", []),
                    BuildItemEntity("Alpha", BitwardenItemType.Login, folderId, "alpha-user", "Password123!", "ALPHA", []),
                    BuildItemEntity("No Totp", BitwardenItemType.Login, folderId, "no-totp-user", "Password123!", "", []),
                    BuildItemEntity("Credit Card", BitwardenItemType.CreditCard, folderId, "plain-user", "Password123!", "IGNORED", [])
                ],
                []));

            List<string> totpUrls = subject.GetTotpUrls().ToList();

            Assert.That(
                totpUrls,
                Is.EqualTo(
                [
                    "otpauth://totp/Alpha:alpha-user:?secret=ALPHA&digits=6&period=30&issuer=Alpha",
                    "otpauth://totp/Battle.net:battle-user:?secret=BATTLE&digits=8&period=30&issuer=Battle.net",
                    "otpauth://totp/Blizzard:blizzard-user:?secret=BLIZZARD&digits=8&period=30&issuer=Blizzard",
                    "otpauth://totp/Gemini:gemini-user:?secret=GEMINI&digits=7&period=10&issuer=Gemini",
                    "otpauth://steam/Steam:steam-user:?secret=STEAM&digits=5&period=30&issuer=Steam"
                ]));
        }

        private void ConfigurePasswordStrengths(Dictionary<string, PasswordStrength> strengthByPassword)
        {
            passwordCheckerMock
                .Setup(passwordChecker => passwordChecker.GetPasswordStrength(It.IsAny<string>()))
                .Returns((string providedPassword) => strengthByPassword[providedPassword]);
        }

        private void LoadVault(BitwardenVaultEntity vaultEntity)
        {
            vaultFileHandlerMock
                .Setup(fileHandler => fileHandler.Load(VaultFilePath))
                .Returns(vaultEntity);

            subject.Load(VaultFilePath);
        }

        private static BitwardenFolderEntity BuildFolderEntity(Guid folderId, string name) => new()
        {
            Id = folderId.ToString(),
            Name = name
        };

        private static BitwardenFieldEntity BuildFieldEntity(string name, string value) => new()
        {
            Name = name,
            Value = value
        };

        private static BitwardenItemEntity BuildItemEntity(
            string name,
            BitwardenItemType itemType,
            Guid folderId,
            string username,
            string password,
            string totp,
            IEnumerable<BitwardenFieldEntity>? fields)
            => BuildItemEntity(name, itemType, folderId, username, password, totp, fields, false);

        private static BitwardenItemEntity BuildItemEntity(
            string name,
            BitwardenItemType itemType,
            Guid folderId,
            string username,
            string password,
            string totp,
            IEnumerable<BitwardenFieldEntity>? fields,
            bool isFavourite) => new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Type = (int)itemType,
            FolderId = folderId.ToString(),
            Favourite = isFavourite,
            Notes = $"Notes for {name}",
            Login = new()
            {
                Username = username,
                Password = password,
                TOTP = totp
            },
            Fields = fields
        };

        private static BitwardenVaultEntity BuildVaultEntity(
            IEnumerable<BitwardenItemEntity> items,
            IEnumerable<BitwardenFolderEntity> folders) => new()
        {
            Encrypted = false,
            Folders = folders.ToList(),
            Items = items.ToList()
        };
    }
}