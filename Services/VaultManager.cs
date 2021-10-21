using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using BitwardenVaultManager.Models;

namespace BitwardenVaultManager.Services
{
    public class VaultManager
    {
        BitwardenVault vault;

        public void Load(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            vault = JsonConvert.DeserializeObject<BitwardenVault>(fileContent);
        }

        public void PrintItemDetails()
        {
            BitwardenItem item = vault.Items.First(x => !string.IsNullOrWhiteSpace(x.FolderId));
            BitwardenFolder folder = vault.Folders.First(x => x.Id == item.FolderId);

            Console.WriteLine($"Id: {item.Name}");
            Console.WriteLine($"Name: {item.Name}");
            Console.WriteLine($"Folder: {folder.Name}");
        }
    }
}
