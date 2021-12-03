using System;
using System.Collections.Generic;

using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service
{
    public interface IVaultManager
    {
        void Load(string filePath);

        IEnumerable<string> GetMisconfiguredItems();

        string GetFolderName(Guid folderId);

        IEnumerable<string> GetEmailAddresses();

        IEnumerable<string> GetPasswords();

        IEnumerable<BitwardenItem> GetItemsByEmailAddress(string emailAddress);

        IEnumerable<BitwardenItem> GetItemsByPassword(string password);

        IEnumerable<BitwardenItem> GetItemsByPasswordLength(int length);

        IEnumerable<BitwardenItem> GetItemsByUsername(string username);

        IEnumerable<BitwardenItem> GetItemsWithWeakPasswords();

        IEnumerable<BitwardenItem> GetItemsWithoutTotp();

        IEnumerable<string> GetTotpUrls();
    }
}
