using System;

namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public BitwardenItemType Type { get; set; }

        public Guid FolderId { get; set; }
    }
}
