using BitwardenVaultManager.DataAccess.DataObjects;

namespace BitwardenVaultManager.DataAccess
{
    public interface IBitwardenVaultFileHandler
    {
        BitwardenVaultEntity Load(string filePath);
    }
}
