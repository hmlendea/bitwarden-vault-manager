using System;
using System.Text;

using NuciCLI.Menus;

using BitwardenVaultManager.Menus;

namespace BitwardenVaultManager
{
    class Program
    {
        static string applicationDirectory;

        /// <summary>
        /// Gets the application directory.
        /// </summary>
        /// <value>The application directory.</value>
        public static string ApplicationDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(applicationDirectory))
                {
                    applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                return applicationDirectory;
            }
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            MenuManager.Instance.AreStatisticsEnabled = true;
            MenuManager.Instance.Start<MainMenu>();
        }
    }
}
