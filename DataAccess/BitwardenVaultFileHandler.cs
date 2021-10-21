using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using BitwardenVaultManager.DataAccess.DataObjects;

namespace BitwardenVaultManager.DataAccess
{
    public class BitwardenVaultFileHandler : IBitwardenVaultFileHandler
    {
        public BitwardenVaultEntity Load(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<BitwardenVaultEntity>(fileContent);
        }
    }
}
