namespace BitwardenVaultManager.DataAccess.DataObjects
{
    public sealed class BitwardenItemEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string FolderId { get; set; }
    }
}
