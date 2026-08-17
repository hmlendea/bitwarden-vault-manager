using System;
using System.Text;

using NuciCLI.Menus;

using BitwardenVaultManager.Menus;

namespace BitwardenVaultManager
{
    class Program
    {
        public static string VaultFilePath { get; private set; }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            VaultFilePath = string.Concat(args);

            MenuManager.Instance.AreStatisticsEnabled = true;
            MenuManager.Instance.Start<MainMenu>();
        }
    }
}
