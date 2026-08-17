using System.Collections.Generic;

namespace BitwardenVaultManager.DataAccess.DataObjects
{
    public sealed class BitwardenVaultEntity
    {
        public bool Encrypted { get; set; }

        public IList<BitwardenFolderEntity> Folders { get; set; }

        public IList<BitwardenItemEntity> Items { get; set; }
    }
}
