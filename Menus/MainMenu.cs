using System;
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

            AddCommand("load", "Load a Bitwarden vault", () => LoadFile());
            AddCommand("get-email-addresses", "Gets the list of all email addresses used", () => GetEmailAddresses());
            AddCommand("get-email-address-usages", "Gets the list of all the accounts that use a given email address", () => GetEmailAddresseUsages());
            AddCommand("get-misconfigured-items", "Gets the list of errors for misconfigured items", () => GetMisconfiguredItems());
            AddCommand("get-reused-passwords", "Gets the list of passwords that are reused across different accounts", () => GetReusedPasswords());
            AddCommand("get-weak-passwords", "Gets the list of all weak passwords", () => GetWeakPasswords());
        }

        void LoadFile()
        {
            string filePath = NuciConsole.ReadLine("Path to the (unencrypted) Bitwarden exported JSON: ");
            vaultManager.Load(filePath);
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
                string folderName = vaultManager.GetFolderName(item.FolderId);
                results.Add($"{folderName}/{item.Name}");
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
                    string folderName = vaultManager.GetFolderName(item.FolderId);
                    NuciConsole.WriteLine($" - {folderName}/{item.Name}");
                }
            }
        }

        void GetWeakPasswords()
        {
            IEnumerable<string> passwords = vaultManager.GetWeakPasswords();

            foreach (string password in passwords)
            {
                IList<BitwardenItem> items = vaultManager.GetItemsByPassword(password).ToList();

                NuciConsole.WriteLine("The password '" + password + "' is weak. Accounts that use it (" + items.Count + "):", NuciConsoleColour.Red);

                foreach (BitwardenItem item in items)
                {
                    string folderName = vaultManager.GetFolderName(item.FolderId);
                    NuciConsole.WriteLine($" - {folderName}/{item.Name}");
                }
            }
        }
    }
}
