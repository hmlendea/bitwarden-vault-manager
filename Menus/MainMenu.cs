using NuciCLI;
using NuciCLI.Menus;

using BitwardenVaultManager.Services;

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
        }

        void LoadFile()
        {
            string filePath = NuciConsole.ReadLine("Path to the (unencrypted) Bitwarden exported JSON: ");
            vaultManager.Load(filePath);
            vaultManager.PrintItemDetails();
        }
    }
}
