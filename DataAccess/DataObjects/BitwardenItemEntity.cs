using System.Collections.Generic;

namespace BitwardenVaultManager.DataAccess.DataObjects
{
    public sealed class BitwardenItemEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string FolderId { get; set; }

        public bool Favourite { get; set; }

        public string Notes { get; set; }

        public BitwardenLoginEntity Login { get; set; }

        public IEnumerable<BitwardenFieldEntity> Fields { get; set; }
    }
}
