using System.Collections.Generic;
using System.Linq;

using NuciCLI;
using NuciCLI.Menus;

using BitwardenVaultManager.Service;

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
            AddCommand("get-misconfigured-items", "Gets the list of errors for misconfigured items", () => GetMisconfiguredItems());
            AddCommand("get-email-addresses", "Gets the list of all email addresses used", () => GetEmailAddresses());
        }

        void LoadFile()
        {
            string filePath = NuciConsole.ReadLine("Path to the (unencrypted) Bitwarden exported JSON: ");
            vaultManager.Load(filePath);
            vaultManager.PrintItemDetails();
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

        void GetEmailAddresses()
        {
            IDictionary<string, int> emailAddressUsages = vaultManager.GetEmailAddressUsageCounts();

            foreach (string emailAddress in emailAddressUsages.Keys.OrderByDescending(x => emailAddressUsages[x]).ThenBy(x => x))
            {
                NuciConsole.WriteLine($"{emailAddress} ({emailAddressUsages[emailAddress]} accounts)");
            }
        }
    }
}
