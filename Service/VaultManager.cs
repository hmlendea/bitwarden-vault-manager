using System;
using System.Linq;

using BitwardenVaultManager.DataAccess;
using BitwardenVaultManager.Service.Mapping;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service
{
    public class VaultManager
    {
        readonly IBitwardenVaultFileHandler vaultFileHandler;

        BitwardenVault vault;

        public VaultManager()
            : this(new BitwardenVaultFileHandler())
        {
            
        }

        public VaultManager(IBitwardenVaultFileHandler vaultFileHandler)
        {
            this.vaultFileHandler = vaultFileHandler;
        }

        public void Load(string filePath)
        {
            vault = vaultFileHandler.Load(filePath).ToServiceModel();
        }

        public void PrintItemDetails()
        {
            BitwardenItem item = vault.Items.First(x => x.FolderId != Guid.Empty);
            BitwardenFolder folder = vault.Folders.First(x => x.Id == item.FolderId);

            Console.WriteLine($"Id: {item.Name}");
            Console.WriteLine($"Name: {item.Name}");
            Console.WriteLine($"Type: {item.Type}");
            Console.WriteLine($"Folder: {folder.Name}");
        }
    }
}
