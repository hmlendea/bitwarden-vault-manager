using System.IO;
using System.Text.Json;

using BitwardenVaultManager.DataAccess.DataObjects;

namespace BitwardenVaultManager.DataAccess
{
    public class BitwardenVaultFileHandler : IBitwardenVaultFileHandler
    {
        static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BitwardenVaultEntity Load(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            BitwardenVaultEntity vault = JsonSerializer.Deserialize<BitwardenVaultEntity>(fileContent, JsonOptions);

            if (vault is null)
            {
                throw new InvalidDataException($"Failed to deserialise the Bitwarden vault from '{filePath}'.");
            }

            return vault;
        }
    }
}
