using System;
using System.Collections.Generic;
using System.Linq;

using NuciCLI;
using NuciCLI.Menus;

using BitwardenVaultManager.Service;
using BitwardenVaultManager.Service.Models;
using BitwardenVaultManager.Service.Helpers;

namespace BitwardenVaultManager.Menus
{
    /// <summary>
    /// Main menu.
    /// </summary>
    public sealed class MainMenu : Menu
    {
        private static Func<string, string> DefaultReadLine => NuciConsole.ReadLine;

        private static Action<string> DefaultWriteLine => NuciConsole.WriteLine;

        private static Action<string, NuciConsoleColour> DefaultWriteColouredLine => NuciConsole.WriteLine;

        private static Action<IEnumerable<string>> DefaultWriteLines => NuciConsole.WriteLines;

        private static Action<IEnumerable<string>, NuciConsoleColour> DefaultWriteColouredLines => NuciConsole.WriteLines;

        private readonly IVaultManager vaultManager;
        private Func<string, string> readLine;
        private Action<string> writeLine;
        private Action<string, NuciConsoleColour> writeColouredLine;
        private Action<IEnumerable<string>> writeLines;
        private Action<IEnumerable<string>, NuciConsoleColour> writeColouredLines;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu() : this(new VaultManager(), Program.VaultFilePath)
        {
        }

        public MainMenu(IVaultManager vaultManager) : base("Bitwarden Vault Manager")
        {
            this.vaultManager = vaultManager;
            InitialiseConsoleActions();

            RegisterCommands();
        }

        public MainMenu(IVaultManager vaultManager, string vaultFilePath) : base("Bitwarden Vault Manager")
        {
            this.vaultManager = vaultManager;
            InitialiseConsoleActions();
            this.vaultManager.Load(vaultFilePath);

            RegisterCommands();
        }

        private void InitialiseConsoleActions()
        {
            readLine = DefaultReadLine;
            writeLine = DefaultWriteLine;
            writeColouredLine = DefaultWriteColouredLine;
            writeLines = DefaultWriteLines;
            writeColouredLines = DefaultWriteColouredLines;
        }

        private void RegisterCommands()
        {
            AddCommand("get-email-addresses", "Gets all email addresses", GetEmailAddresses);
            AddCommand("get-email-address-usages", "Gets all the accounts that are associated with a given email address", GetEmailAddressUsages);
            AddCommand("get-phone-numbers", "Gets all phone numbers", GetPhoneNumbers);
            AddCommand("get-phone-number-usages", "Gets all the accounts that are associated with a given phone number", GetPhoneNumberUsages);
            AddCommand("get-items-by-password-length", "Gets the list of items that use passwords of the given length", GetItemsByPasswordLength);
            AddCommand("get-items-without-2fa", "Gets the list of items without 2-factor authentication", GetItemsWithout2FA);
            AddCommand("get-misconfigured-items", "Gets the list of errors for misconfigured items", GetMisconfiguredItems);
            AddCommand("get-password-lengths", "Gets the lengths of the passwords", GetPasswordLengths);
            AddCommand("get-password-usages", "Gets all the accounts that use a given password", GetPasswordUsages);
            AddCommand("get-passwords-containing", "Gets the passwords that contain a given text", GetPasswordsContaining);
            AddCommand("get-reused-passwords", "Gets the passwords that are reused across different accounts", GetReusedPasswords);
            AddCommand("get-totp-urls", "Gets the TOTP association URLs for all the items that have them", GetTotpUrls);
            AddCommand("get-usernames", "Gets all the unique usernames", GetUsernames);
            AddCommand("get-username-usages", "Gets all the accounts that use a given username", GetUsernameUsages);
            AddCommand("get-weak-passwords", "Gets all weak passwords", GetWeakPasswords);
        }

        void GetEmailAddresses()
        {
            IEnumerable<string> emailAddresses = vaultManager.GetEmailAddresses();
            IDictionary<string, int> emailAddressUsages = emailAddresses.ToDictionary(x => x, x => 0);

            if (!emailAddresses.Any())
            {
                writeLine("There are no email addresses associated with any item!");
                return;
            }

            writeLine($"There are {emailAddressUsages.Count} email addresses:");

            foreach (string emailAddress in emailAddresses)
            {
                emailAddressUsages[emailAddress] = vaultManager.GetItemsByEmailAddress(emailAddress).Count();
            }

            foreach (string emailAddress in emailAddressUsages.Keys.OrderByDescending(x => emailAddressUsages[x]).ThenBy(x => x))
            {
                writeLine($"{emailAddress} ({emailAddressUsages[emailAddress]} accounts)");
            }
        }

        void GetEmailAddressUsages()
        {
            string emailAddress = readLine("Email Address: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByEmailAddress(emailAddress);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                writeLine("There are no logins associated with the provided email address!");
                return;
            }

            writeLine($"The '{emailAddress}' email address is associated with {results.Count} items:");
            writeLines(results);
        }

        void GetPhoneNumbers()
        {
            IEnumerable<string> phoneNumbers = vaultManager.GetPhoneNumbers();
            IDictionary<string, int> phoneNumberUsages = phoneNumbers.ToDictionary(x => x, x => 0);

            if (!phoneNumbers.Any())
            {
                writeLine("There are no phone numbers associated with any item!");
                return;
            }

            writeLine($"There are {phoneNumberUsages.Count} phone numbers:");

            foreach (string phoneNumber in phoneNumbers)
            {
                phoneNumberUsages[phoneNumber] = vaultManager.GetItemsByPhoneNumber(phoneNumber).Count();
            }

            foreach (string phoneNumber in phoneNumberUsages.Keys.OrderByDescending(x => phoneNumberUsages[x]).ThenBy(x => x))
            {
                writeLine($"{phoneNumber} ({phoneNumberUsages[phoneNumber]} accounts)");
            }
        }

        void GetPhoneNumberUsages()
        {
            string phoneNumber = PhoneNumberHelper.Normalise(readLine("Phone Number: "));
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPhoneNumber(phoneNumber);
            List<string> results = [.. items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)];

            if (results.Count.Equals(0))
            {
                writeLine("There are no logins associated with the provided phone number!");
                return;
            }

            writeLine($"The '{phoneNumber}' phone number is associated with {results.Count} items:");
            writeLines(results);
        }

        void GetItemsByPasswordLength()
        {
            int length = int.Parse(readLine("Password length: "));
            IEnumerable<string> results = vaultManager
                .GetItemsByPasswordLength(length)
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x);

            if (!results.Any())
            {
                writeLine($"There are no items that use {length} character long passwords!");
                return;
            }

            writeLine($"There are '{results.Count()}' items that use {length} character long passwords:");
            writeLines(results);
        }

        void GetItemsWithout2FA()
        {
            IEnumerable<string> results = vaultManager
                .GetItemsWithoutTotp()
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x);

            if (!results.Any())
            {
                writeColouredLine("All items are using 2-factor authentication, good job!", NuciConsoleColour.Green);
                return;
            }

            writeLine($"There are '{results.Count()}' misconfigured items:");
            writeLines(results);
        }

        void GetMisconfiguredItems()
        {
            IEnumerable<string> errors = vaultManager.GetMisconfiguredItems();

            if (!errors.Any())
            {
                writeColouredLine("All items are properly configured, good job!", NuciConsoleColour.Green);
                return;
            }

            writeLine($"There are '{errors.Count()}' misconfigured items:");
            writeColouredLines(errors, NuciConsoleColour.Red);
        }

        void GetPasswordLengths()
        {
            IEnumerable<string> passwords = vaultManager.GetPasswords();
            IDictionary<int, int> results = passwords
                .GroupBy(password => password.Length)
                .OrderByDescending(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count());

            if (!results.Any())
            {
                writeLine("There are no logins!");
                return;
            }

            writeLines(results.Select(x => $"{x.Key} ({x.Value} logins)"));
        }

        void GetPasswordUsages()
        {
            string password = readLine("Password: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPassword(password);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                writeLine("There are no logins using the provided password!");
                return;
            }

            writeLine($"The '{password}' password is associated with {results.Count} items:");
            writeLines(results);
        }

        void GetPasswordsContaining()
        {
            string text = readLine("Text: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPasswordContaining(text);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                writeLine("There are no logins that use passwords containing the provided text!");
                return;
            }

            writeLine($"The text '{text}' is used in {results.Count} passwords:");
            writeLines(results);
        }

        void GetReusedPasswords()
        {
            IEnumerable<string> passwords = vaultManager.GetPasswords();

            foreach (string password in passwords)
            {
                IList<BitwardenItem> items = vaultManager.GetItemsByPassword(password).ToList();

                if (items.Count <= 1)
                {
                    continue;
                }

                writeColouredLine("The password '" + password + "' is reused accross " + items.Count + " accounts:", NuciConsoleColour.Red);
                writeLines(items.Select(GetItemDescription));
            }
        }

        void GetTotpUrls()
        {
            IEnumerable<string> urls = vaultManager.GetTotpUrls();
            writeLines(urls);
        }

        void GetUsernames()
        {
            IEnumerable<string> usernames = vaultManager.GetUsernames();
            IDictionary<string, int> usernameUsageCounts = usernames.ToDictionary(x => x, x => 0);

            if (!usernames.Any())
            {
                writeLine("There are no usernames associated with any item!");
                return;
            }

            writeLine($"There are {usernameUsageCounts.Count} usernames:");

            foreach (string username in usernames)
            {
                usernameUsageCounts[username] = vaultManager.GetItemsByUsername(username).Count();
            }

            foreach (string username in usernameUsageCounts.Keys.OrderByDescending(x => usernameUsageCounts[x]).ThenBy(x => x))
            {
                writeLine($"{username} ({usernameUsageCounts[username]} accounts)");
            }
        }

        void GetUsernameUsages()
        {
            string username = readLine("Username: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByUsername(username);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                writeLine("There are no logins using the provided username!");
                return;
            }

            writeLine($"The '{username}' username is associated with {results.Count} items:");
            writeLines(results);
        }

        void GetWeakPasswords()
        {
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsWithWeakPasswords();
            IEnumerable<string> lines = items
                .Select(GetItemDescription)
                .OrderBy(x => x);

            writeColouredLines(lines, NuciConsoleColour.Red);
        }

        string GetItemDescription(BitwardenItem item)
        {
            string folderName = vaultManager.GetFolderName(item.FolderId);
            string itemDescription = string.Empty;

            if (!string.IsNullOrWhiteSpace(folderName))
            {
                itemDescription += $"{folderName}/";
            }

            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                itemDescription += item.Name;
            }

            if (!(item.Login is null) && !string.IsNullOrWhiteSpace(item.Login.Username))
            {
                itemDescription += $" - {item.Login.Username}";
            }

            return itemDescription;
        }
    }
}
