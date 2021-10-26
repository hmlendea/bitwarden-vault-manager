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

            AddCommand("get-email-addresses", "Gets all email addresses", () => GetEmailAddresses());
            AddCommand("get-email-address-usages", "Gets all the accounts that are associated with a given email address", () => GetEmailAddressUsages());
            AddCommand("get-misconfigured-items", "Gets the list of errors for misconfigured items", () => GetMisconfiguredItems());
            AddCommand("get-password-usages", "Gets all the accounts that use a given password", () => GetPasswordUsages());
            AddCommand("get-reused-passwords", "Gets the passwords that are reused across different accounts", () => GetReusedPasswords());
            AddCommand("get-totp-urls", "Gets the TOTP association URLs for all the items that have them", () => GetTotpUrls());
            AddCommand("get-username-usages", "Gets all the accounts that use a given username", () => GetUsernameUsages());
            AddCommand("get-weak-passwords", "Gets all weak passwords", () => GetWeakPasswords());
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
            else
            {
                NuciConsole.WriteLine($"There are {emailAddressUsages.Count} email addresses:");
            }

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
            IList<string> results = new List<string>();

            foreach (BitwardenItem item in items)
            {
                results.Add(GetItemDescription(item));
            }

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins associated with the provided email address!");
                return;
            }
            else
            {
                NuciConsole.WriteLine($"The '{emailAddress}' email address is associated with {results.Count} items:");
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine($" - {result}");
            }
        }

        void GetMisconfiguredItems()
        {
            IEnumerable<string> errors = vaultManager.GetMisconfiguredItems();

            if (!errors.Any())
            {
                NuciConsole.WriteLine("All items are properly configured, good job!", NuciConsoleColour.Green);
                return;
            }
            else
            {
                NuciConsole.WriteLine($"There are '{errors.Count()}' misconfigured items:");
            }

            foreach (string error in errors)
            {
                NuciConsole.WriteLine(error, NuciConsoleColour.Red);
            }
        }

        void GetPasswordUsages()
        {
            string password = NuciConsole.ReadLine("Password: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByPassword(password);
            IList<string> results = new List<string>();

            foreach (BitwardenItem item in items)
            {
                results.Add(GetItemDescription(item));
            }

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins using the provided password!");
                return;
            }
            else
            {
                NuciConsole.WriteLine($"The '{password}' password is associated with {results.Count} items:");
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine($" - {result}");
            }
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

                foreach (BitwardenItem item in items)
                {
                    NuciConsole.WriteLine(GetItemDescription(item));
                }
            }
        }

        void GetTotpUrls()
        {
            IEnumerable<string> items = vaultManager.GetTotpUrls();

            foreach (string url in items)
            {
                NuciConsole.WriteLine(url);
            }
        }

        void GetUsernameUsages()
        {
            string username = NuciConsole.ReadLine("Username: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByUsername(username);
            IList<string> results = new List<string>();

            foreach (BitwardenItem item in items)
            {
                results.Add(GetItemDescription(item));
            }

            if (!results.Any())
            {
                NuciConsole.WriteLine("There are no logins using the provided username!");
                return;
            }
            else
            {
                NuciConsole.WriteLine($"The '{username}' username is associated with {results.Count} items:");
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine($" - {result}");
            }
        }

        void GetWeakPasswords()
        {
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsWithWeakPasswords();
            IList<string> results = new List<string>();

            foreach (BitwardenItem item in items)
            {
                results.Add(GetItemDescription(item));
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine(result, NuciConsoleColour.Red);
            }
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
