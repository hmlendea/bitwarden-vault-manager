using System.Collections.Generic;

namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenVault
    {
        public bool Encrypted { get; set; }

        public IList<BitwardenFolder> Folders { get; set; }

        public IList<BitwardenItem> Items { get; set; }
    }
}
