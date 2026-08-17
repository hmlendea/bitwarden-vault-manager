using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NuciCLI;

using BitwardenVaultManager.Menus;
using BitwardenVaultManager.Service;
using BitwardenVaultManager.Service.Models;

using Moq;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class MainMenuTests
    {
        private Mock<IVaultManager> vaultManagerMock;

        [SetUp]
        public void SetUp()
        {
            vaultManagerMock = new();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(It.IsAny<Guid>()))
                .Returns((string)null!);
        }

        [Test]
        public void GivenAVaultFilePath_WhenCreatingTheDefaultMenu_ThenTheVaultManagerLoadsThatVault()
        {
            string filePath = Path.GetTempFileName();
            Type programType = typeof(MainMenu).Assembly.GetType("BitwardenVaultManager.Program")!;
            PropertyInfo vaultFilePathProperty = programType.GetProperty("VaultFilePath", BindingFlags.Static | BindingFlags.Public)!;

            try
            {
                File.WriteAllText(filePath, "{\"encrypted\":false,\"folders\":[],\"items\":[]}");
                vaultFilePathProperty.SetValue(null, filePath);

                MainMenu menu = new();

                Assert.That(menu, Is.Not.Null);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void GivenALoadingConstructor_WhenCreatingTheMenu_ThenTheVaultIsLoaded()
        {
            MainMenu menu = new(vaultManagerMock.Object, "vault.json");

            Assert.That(menu, Is.Not.Null);
            vaultManagerMock.Verify(vaultManager => vaultManager.Load("vault.json"), Times.Once);
        }

        [Test]
        public void GivenNoEmailAddresses_WhenExecutingGetEmailAddresses_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetEmailAddresses())
                .Returns([]);

            string output = InvokeMenuMethod("GetEmailAddresses");

            Assert.That(output, Does.Contain("There are no email addresses associated with any item!"));
        }

        [Test]
        public void GivenEmailAddresses_WhenExecutingGetEmailAddresses_ThenTheyAreWrittenInUsageOrder()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetEmailAddresses())
                .Returns(new[] { "beta@example.com", "alpha@example.com" });
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByEmailAddress("beta@example.com"))
                .Returns([BuildItem(Guid.Empty, "Beta", "beta@example.com")]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByEmailAddress("alpha@example.com"))
                .Returns(
                [
                    BuildItem(Guid.Empty, "Alpha One", "alpha@example.com"),
                    BuildItem(Guid.Empty, "Alpha Two", "alpha@example.com")
                ]);

            string output = InvokeMenuMethod("GetEmailAddresses");

            Assert.That(output, Does.Contain("There are 2 email addresses:"));
            AssertThatTextAppearsInOrder(output, "alpha@example.com (2 accounts)", "beta@example.com (1 accounts)");
        }

        [Test]
        public void GivenNoEmailAddressUsages_WhenExecutingGetEmailAddressUsages_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByEmailAddress("person@example.com"))
                .Returns([]);

            string output = InvokeMenuMethod("GetEmailAddressUsages", "person@example.com");

            Assert.That(output, Does.Contain("There are no logins associated with the provided email address!"));
        }

        [Test]
        public void GivenEmailAddressUsages_WhenExecutingGetEmailAddressUsages_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByEmailAddress("person@example.com"))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetEmailAddressUsages", "person@example.com");

            Assert.That(output, Does.Contain("The 'person@example.com' email address is associated with 2 items:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenNoPhoneNumbers_WhenExecutingGetPhoneNumbers_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetPhoneNumbers())
                .Returns([]);

            string output = InvokeMenuMethod("GetPhoneNumbers");

            Assert.That(output, Does.Contain("There are no phone numbers associated with any item!"));
        }

        [Test]
        public void GivenPhoneNumbers_WhenExecutingGetPhoneNumbers_ThenTheyAreWrittenInUsageOrder()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetPhoneNumbers())
                .Returns(new[] { "+44123456789", "+40712345678" });
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPhoneNumber("+44123456789"))
                .Returns([BuildItem(Guid.Empty, "One", "+44123456789")]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPhoneNumber("+40712345678"))
                .Returns(
                [
                    BuildItem(Guid.Empty, "Two", "+40712345678"),
                    BuildItem(Guid.Empty, "Three", "+40712345678")
                ]);

            string output = InvokeMenuMethod("GetPhoneNumbers");

            Assert.That(output, Does.Contain("There are 2 phone numbers:"));
            AssertThatTextAppearsInOrder(output, "+40712345678 (2 accounts)", "+44123456789 (1 accounts)");
        }

        [Test]
        public void GivenNoPhoneNumberUsages_WhenExecutingGetPhoneNumberUsages_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPhoneNumber("+40712345678"))
                .Returns([]);

            string output = InvokeMenuMethod("GetPhoneNumberUsages", "(0712) 345-678");

            Assert.That(output, Does.Contain("There are no logins associated with the provided phone number!"));
        }

        [Test]
        public void GivenPhoneNumberUsages_WhenExecutingGetPhoneNumberUsages_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPhoneNumber("+40712345678"))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetPhoneNumberUsages", "(0712) 345-678");

            Assert.That(output, Does.Contain("The '+40712345678' phone number is associated with 2 items:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenNoPasswordLengthMatches_WhenExecutingGetItemsByPasswordLength_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPasswordLength(12))
                .Returns([]);

            string output = InvokeMenuMethod("GetItemsByPasswordLength", "12");

            Assert.That(output, Does.Contain("There are no items that use 12 character long passwords!"));
        }

        [Test]
        public void GivenPasswordLengthMatches_WhenExecutingGetItemsByPasswordLength_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPasswordLength(12))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetItemsByPasswordLength", "12");

            Assert.That(output, Does.Contain("There are '2' items that use 12 character long passwords:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenNoItemsWithoutTotp_WhenExecutingGetItemsWithout2Fa_ThenTheSuccessMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsWithoutTotp())
                .Returns([]);

            string output = InvokeMenuMethod("GetItemsWithout2FA");

            Assert.That(output, Does.Contain("All items are using 2-factor authentication, good job!"));
        }

        [Test]
        public void GivenItemsWithoutTotp_WhenExecutingGetItemsWithout2Fa_ThenTheDescriptionsAreWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsWithoutTotp())
                .Returns([BuildItem(Guid.Empty, "Alpha", "alpha-user")]);

            string output = InvokeMenuMethod("GetItemsWithout2FA");

            Assert.That(output, Does.Contain("There are '1' misconfigured items:"));
            Assert.That(output, Does.Contain(" - Alpha - alpha-user"));
        }

        [Test]
        public void GivenNoMisconfiguredItems_WhenExecutingGetMisconfiguredItems_ThenTheSuccessMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetMisconfiguredItems())
                .Returns([]);

            string output = InvokeMenuMethod("GetMisconfiguredItems");

            Assert.That(output, Does.Contain("All items are properly configured, good job!"));
        }

        [Test]
        public void GivenMisconfiguredItems_WhenExecutingGetMisconfiguredItems_ThenTheErrorsAreWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetMisconfiguredItems())
                .Returns(new[] { "Error one", "Error two" });

            string output = InvokeMenuMethod("GetMisconfiguredItems");

            Assert.That(output, Does.Contain("There are '2' misconfigured items:"));
            Assert.That(output, Does.Contain("Error one"));
            Assert.That(output, Does.Contain("Error two"));
        }

        [Test]
        public void GivenNoPasswords_WhenExecutingGetPasswordLengths_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetPasswords())
                .Returns([]);

            string output = InvokeMenuMethod("GetPasswordLengths");

            Assert.That(output, Does.Contain("There are no logins!"));
        }

        [Test]
        public void GivenPasswords_WhenExecutingGetPasswordLengths_ThenGroupedLengthsAreWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetPasswords())
                .Returns(new[] { "abcd", "efgh", "abcde" });

            string output = InvokeMenuMethod("GetPasswordLengths");

            AssertThatTextAppearsInOrder(output, "5 (1 logins)", "4 (2 logins)");
        }

        [Test]
        public void GivenNoPasswordUsages_WhenExecutingGetPasswordUsages_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPassword("Password123!"))
                .Returns([]);

            string output = InvokeMenuMethod("GetPasswordUsages", "Password123!");

            Assert.That(output, Does.Contain("There are no logins using the provided password!"));
        }

        [Test]
        public void GivenPasswordUsages_WhenExecutingGetPasswordUsages_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPassword("Password123!"))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetPasswordUsages", "Password123!");

            Assert.That(output, Does.Contain("The 'Password123!' password is associated with 2 items:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenNoPasswordContainingMatches_WhenExecutingGetPasswordsContaining_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPasswordContaining("needle"))
                .Returns([]);

            string output = InvokeMenuMethod("GetPasswordsContaining", "needle");

            Assert.That(output, Does.Contain("There are no logins that use passwords containing the provided text!"));
        }

        [Test]
        public void GivenPasswordContainingMatches_WhenExecutingGetPasswordsContaining_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPasswordContaining("needle"))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetPasswordsContaining", "needle");

            Assert.That(output, Does.Contain("The text 'needle' is used in 2 passwords:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenReusedAndUniquePasswords_WhenExecutingGetReusedPasswords_ThenOnlyTheReusedPasswordsAreWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetPasswords())
                .Returns(new[] { "unique", "shared" });
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPassword("unique"))
                .Returns([BuildItem(Guid.Empty, "Solo", "solo-user")]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByPassword("shared"))
                .Returns(
                [
                    BuildItem(Guid.Empty, "Alpha", "alpha-user"),
                    BuildItem(Guid.Empty, "Beta", "beta-user")
                ]);

            string output = InvokeMenuMethod("GetReusedPasswords");

            Assert.That(output, Does.Not.Contain("unique"));
            Assert.That(output, Does.Contain("The password 'shared' is reused accross 2 accounts:"));
            Assert.That(output, Does.Contain("Alpha - alpha-user"));
            Assert.That(output, Does.Contain("Beta - beta-user"));
        }

        [Test]
        public void GivenNoUsernames_WhenExecutingGetUsernames_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetUsernames())
                .Returns([]);

            string output = InvokeMenuMethod("GetUsernames");

            Assert.That(output, Does.Contain("There are no usernames associated with any item!"));
        }

        [Test]
        public void GivenUsernames_WhenExecutingGetUsernames_ThenTheyAreWrittenInUsageOrder()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetUsernames())
                .Returns(new[] { "beta-user", "alpha-user" });
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByUsername("beta-user"))
                .Returns([BuildItem(Guid.Empty, "Beta", "beta-user")]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByUsername("alpha-user"))
                .Returns(
                [
                    BuildItem(Guid.Empty, "Alpha One", "alpha-user"),
                    BuildItem(Guid.Empty, "Alpha Two", "alpha-user")
                ]);

            string output = InvokeMenuMethod("GetUsernames");

            Assert.That(output, Does.Contain("There are 2 usernames:"));
            AssertThatTextAppearsInOrder(output, "alpha-user (2 accounts)", "beta-user (1 accounts)");
        }

        [Test]
        public void GivenNoUsernameUsages_WhenExecutingGetUsernameUsages_ThenTheEmptyMessageIsWritten()
        {
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByUsername("alpha-user"))
                .Returns([]);

            string output = InvokeMenuMethod("GetUsernameUsages", "alpha-user");

            Assert.That(output, Does.Contain("There are no logins using the provided username!"));
        }

        [Test]
        public void GivenUsernameUsages_WhenExecutingGetUsernameUsages_ThenTheSortedDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsByUsername("alpha-user"))
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetUsernameUsages", "alpha-user");

            Assert.That(output, Does.Contain("The 'alpha-user' username is associated with 2 items:"));
            AssertThatTextAppearsInOrder(output, " - Personal/Alpha - alpha-user", " - Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenWeakPasswords_WhenExecutingGetWeakPasswords_ThenTheDescriptionsAreWritten()
        {
            Guid folderId = Guid.NewGuid();
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetItemsWithWeakPasswords())
                .Returns(
                [
                    BuildItem(folderId, "Zulu", "zulu-user"),
                    BuildItem(folderId, "Alpha", "alpha-user")
                ]);
            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string output = InvokeMenuMethod("GetWeakPasswords");

            AssertThatTextAppearsInOrder(output, "Personal/Alpha - alpha-user", "Personal/Zulu - zulu-user");
        }

        [Test]
        public void GivenAnItemWithFolderNameAndUsername_WhenGettingTheDescription_ThenAllSegmentsAreIncluded()
        {
            Guid folderId = Guid.NewGuid();
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo getItemDescriptionMethod = typeof(MainMenu).GetMethod("GetItemDescription", BindingFlags.Instance | BindingFlags.NonPublic)!;
            BitwardenItem item = BuildItem(folderId, "Entry", "user-name");

            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetFolderName(folderId))
                .Returns("Personal");

            string description = (string)getItemDescriptionMethod.Invoke(menu, [item])!;

            Assert.That(description, Is.EqualTo("Personal/Entry - user-name"));
        }

        [Test]
        public void GivenAnItemWithoutFolderNameOrUsername_WhenGettingTheDescription_ThenOnlyTheAvailableSegmentsAreIncluded()
        {
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo getItemDescriptionMethod = typeof(MainMenu).GetMethod("GetItemDescription", BindingFlags.Instance | BindingFlags.NonPublic)!;
            BitwardenItem item = new()
            {
                Name = "Entry",
                Login = new()
                {
                    Username = " "
                }
            };

            string description = (string)getItemDescriptionMethod.Invoke(menu, [item])!;

            Assert.That(description, Is.EqualTo("Entry"));
        }

        [Test]
        public void GivenAnItemWithoutALogin_WhenGettingTheDescription_ThenOnlyTheNameIsReturned()
        {
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo getItemDescriptionMethod = typeof(MainMenu).GetMethod("GetItemDescription", BindingFlags.Instance | BindingFlags.NonPublic)!;
            BitwardenItem item = new()
            {
                Name = "Entry"
            };

            string description = (string)getItemDescriptionMethod.Invoke(menu, [item])!;

            Assert.That(description, Is.EqualTo("Entry"));
        }

        private string InvokeMenuMethod(string methodName)
        {
            StringWriter outputWriter = new();
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo method = typeof(MainMenu).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;

            ConfigureMenuIo(menu, [], outputWriter);
            method.Invoke(menu, []);

            return outputWriter.ToString();
        }

        private string InvokeMenuMethod(string methodName, string input)
        {
            StringWriter outputWriter = new();
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo method = typeof(MainMenu).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;

            ConfigureMenuIo(menu, [input], outputWriter);
            method.Invoke(menu, []);

            return outputWriter.ToString();
        }

        private static void ConfigureMenuIo(MainMenu menu, IEnumerable<string> inputs, StringWriter outputWriter)
        {
            Queue<string> queuedInputs = new(inputs);

            SetInstanceField(menu, "readLine", (Func<string, string>)(_ => queuedInputs.Dequeue()));
            SetInstanceField(menu, "writeLine", (Action<string>)(message => outputWriter.WriteLine(message)));
            SetInstanceField(menu, "writeColouredLine", (Action<string, NuciConsoleColour>)((message, _) => outputWriter.WriteLine(message)));
            SetInstanceField(menu, "writeLines", (Action<IEnumerable<string>>)(messages => WriteMessages(outputWriter, messages)));
            SetInstanceField(menu, "writeColouredLines", (Action<IEnumerable<string>, NuciConsoleColour>)((messages, _) => WriteMessages(outputWriter, messages)));
        }

        private static void SetInstanceField(MainMenu menu, string fieldName, object value)
        {
            FieldInfo field = typeof(MainMenu).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;

            field.SetValue(menu, value);
        }

        private static void WriteMessages(StringWriter outputWriter, IEnumerable<string> messages)
        {
            foreach (string message in messages)
            {
                outputWriter.WriteLine(message);
            }
        }

        private static void AssertThatTextAppearsInOrder(string text, string firstValue, string secondValue)
        {
            int firstIndex = text.IndexOf(firstValue, StringComparison.InvariantCulture);
            int secondIndex = text.IndexOf(secondValue, StringComparison.InvariantCulture);

            Assert.That(firstIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(secondIndex, Is.GreaterThan(firstIndex));
        }

        private static BitwardenItem BuildItem(Guid folderId, string name, string username) => new()
        {
            FolderId = folderId,
            Name = name,
            Login = new()
            {
                Username = username
            }
        };
    }
}