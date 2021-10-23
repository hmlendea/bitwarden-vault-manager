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
        readonly VaultManager vaultManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu() : base("Bitwarden Vault Manager")
        {
            vaultManager = new VaultManager();
            vaultManager.Load(Program.VaultFilePath);

            AddCommand("get-email-addresses", "Gets all email addresses", () => GetEmailAddresses());
            AddCommand("get-email-address-usages", "Gets all the accounts that are associated with a given email address", () => GetEmailAddresseUsages());
            AddCommand("get-misconfigured-items", "Gets the list of errors for misconfigured items", () => GetMisconfiguredItems());
            AddCommand("get-password-usages", "Gets all the accounts that use a given password", () => GetPasswordUsages());
            AddCommand("get-reused-passwords", "Gets the passwords that are reused across different accounts", () => GetReusedPasswords());
            AddCommand("get-totp-urls", "Gets the TOTP association URLs for all the items that have them", () => GetTotpUrls());
            AddCommand("get-weak-passwords", "Gets all weak passwords", () => GetWeakPasswords());
        }

        void GetEmailAddresses()
        {
            IEnumerable<string> emailAddresses = vaultManager.GetEmailAddresses();
            IDictionary<string, int> emailAddressUsages = emailAddresses.ToDictionary(x => x, x => 0);

            foreach (string emailAddress in emailAddresses)
            {
                emailAddressUsages[emailAddress] = vaultManager.GetItemsByEmailAddress(emailAddress).Count();
            }

            foreach (string emailAddress in emailAddressUsages.Keys.OrderByDescending(x => emailAddressUsages[x]).ThenBy(x => x))
            {
                NuciConsole.WriteLine($"{emailAddress} ({emailAddressUsages[emailAddress]} accounts)");
            }
        }

        void GetEmailAddresseUsages()
        {
            string emailAddress = NuciConsole.ReadLine("Email Address: ");
            IEnumerable<BitwardenItem> items = vaultManager.GetItemsByEmailAddress(emailAddress);
            IList<string> results = new List<string>();

            foreach (BitwardenItem item in items)
            {
                results.Add(GetItemDescription(item));
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine(result);
            }
        }

        void GetMisconfiguredItems()
        {
            IEnumerable<string> errors = vaultManager.GetMisconfiguredItems();

            if (errors.Count() == 0)
            {
                NuciConsole.WriteLine("All items are properly configured, good job!", NuciConsoleColour.Green);
                return;
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
            }

            foreach (string result in results.OrderBy(x => x))
            {
                NuciConsole.WriteLine(result);
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

            foreach (string url in items.OrderBy(x => x))
            {
                NuciConsole.WriteLine(url);
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
