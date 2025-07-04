using System.Collections.Generic;
using System.Linq;

using NuciCLI;
using NuciCLI.Menus;

using BitwardenVaultManager.Service;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Menus
{
    /// <summary>
    /// Main menu.
    /// </summary>
    public class MainMenu : Menu
    {
        readonly IVaultManager vaultManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu() : base("Bitwarden Vault Manager")
        {
            vaultManager = new VaultManager();
            vaultManager.Load(Program.VaultFilePath);

            AddCommand("get-email-addresses", "Gets all email addresses", GetEmailAddresses);
            AddCommand("get-email-address-usages", "Gets all the accounts that are associated with a given email address", GetEmailAddressUsages);
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
                NuciConsole.WriteLine("There are no email addresses associated with any item!");
                return;
            }

            NuciConsole.WriteLine($"There are {emailAddressUsages.Count} email addresses:");

            foreach (string emailAddress in emailAddresses)
            {
                emailAddressUsages[emailAddress] = vaultManager.GetItemsByEmailAddress(emailAddress).Count();
            }

            foreach (string emailAddress in emailAddressUsages.Keys.OrderByDescending(x => emailAddressUsages[x]).ThenBy(x => x))
            {
                NuciConsole.WriteLine($"{emailAddress} ({emailAddressUsages[emailAddress]} accounts)");
            }
        }

        void GetEmailAddressUsages()
        {
            string emailAddress = NuciConsole.ReadLine("Email Address: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByEmailAddress(emailAddress);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins associated with the provided email address!");
                return;
            }

            NuciConsole.WriteLine($"The '{emailAddress}' email address is associated with {results.Count} items:");
            NuciConsole.WriteLines(results);
        }

        void GetItemsByPasswordLength()
        {
            int length = int.Parse(NuciConsole.ReadLine("Password length: "));
            IEnumerable<string> results = vaultManager
                .GetItemsByPasswordLength(length)
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x);

            if (!results.Any())
            {
                NuciConsole.WriteLine($"There are no items that use {length} character long passwords!");
                return;
            }

            NuciConsole.WriteLine($"There are '{results.Count()}' items that use {length} character long passwords:");
            NuciConsole.WriteLines(results);
        }

        void GetItemsWithout2FA()
        {
            IEnumerable<string> results = vaultManager
                .GetItemsWithoutTotp()
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x);

            if (!results.Any())
            {
                NuciConsole.WriteLine("All items are using 2-factor authentication, good job!", NuciConsoleColour.Green);
                return;
            }

            NuciConsole.WriteLine($"There are '{results.Count()}' misconfigured items:");
            NuciConsole.WriteLines(results);
        }

        void GetMisconfiguredItems()
        {
            IEnumerable<string> errors = vaultManager.GetMisconfiguredItems();

            if (!errors.Any())
            {
                NuciConsole.WriteLine("All items are properly configured, good job!", NuciConsoleColour.Green);
                return;
            }

            NuciConsole.WriteLine($"There are '{errors.Count()}' misconfigured items:");
            NuciConsole.WriteLines(errors, NuciConsoleColour.Red);
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
                NuciConsole.WriteLine("There are no logins!");
                return;
            }

            NuciConsole.WriteLines(results.Select(x => $"{x.Key} ({x.Value} logins)"));
        }

        void GetPasswordUsages()
        {
            string password = NuciConsole.ReadLine("Password: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPassword(password);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins using the provided password!");
                return;
            }

            NuciConsole.WriteLine($"The '{password}' password is associated with {results.Count} items:");
            NuciConsole.WriteLines(results);
        }

        void GetPasswordsContaining()
        {
            string text = NuciConsole.ReadLine("Text: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPasswordContaining(text);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins that use passwords containing the provided text!");
                return;
            }

            NuciConsole.WriteLine($"The text '{text}' is used in {results.Count} passwords:");
            NuciConsole.WriteLines(results);
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

                NuciConsole.WriteLine("The password '" + password + "' is reused accross " + items.Count + " accounts:", NuciConsoleColour.Red);
                NuciConsole.WriteLines(items.Select(GetItemDescription));
            }
        }

        void GetTotpUrls()
        {
            IEnumerable<string> urls = vaultManager.GetTotpUrls();
            NuciConsole.WriteLines(urls);
        }

        void GetUsernames()
        {
            IEnumerable<string> usernames = vaultManager.GetUsernames();
            IDictionary<string, int> usernameUsageCounts = usernames.ToDictionary(x => x, x => 0);

            if (!usernames.Any())
            {
                NuciConsole.WriteLine("There are no usernames associated with any item!");
                return;
            }

            NuciConsole.WriteLine($"There are {usernameUsageCounts.Count} usernames:");

            foreach (string username in usernames)
            {
                usernameUsageCounts[username] = vaultManager.GetItemsByUsername(username).Count();
            }

            foreach (string username in usernameUsageCounts.Keys.OrderByDescending(x => usernameUsageCounts[x]).ThenBy(x => x))
            {
                NuciConsole.WriteLine($"{username} ({usernameUsageCounts[username]} accounts)");
            }
        }

        void GetUsernameUsages()
        {
            string username = NuciConsole.ReadLine("Username: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByUsername(username);
            IList<string> results = items
                .Select(item => $" - {GetItemDescription(item)}")
                .OrderBy(x => x)
                .ToList();

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins using the provided username!");
                return;
            }

            NuciConsole.WriteLine($"The '{username}' username is associated with {results.Count} items:");
            NuciConsole.WriteLines(results);
        }

        void GetWeakPasswords()
        {
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsWithWeakPasswords();
            IEnumerable<string> lines = items
                .Select(GetItemDescription)
                .OrderBy(x => x);

            NuciConsole.WriteLines(lines, NuciConsoleColour.Red);
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
