using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenVaultMappings
    {
        internal static BitwardenVault ToServiceModel(this BitwardenVaultEntity dataObject) => new()
        {
            IsEncrypted = dataObject.Encrypted,
            Folders = dataObject.Folders.ToServiceModels().ToList(),
            Items = dataObject.Items.ToServiceModels().ToList()
        };

        internal static BitwardenVaultEntity ToDataObject(this BitwardenVault serviceModel) => new()
        {
            Encrypted = serviceModel.IsEncrypted,
            Folders = serviceModel.Folders.ToDataObjects().ToList(),
            Items = serviceModel.Items.ToDataObjects().ToList()
        };

        internal static IEnumerable<BitwardenVault> ToServiceModels(this IEnumerable<BitwardenVaultEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<BitwardenVaultEntity> ToDataObjects(this IEnumerable<BitwardenVault> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());
    }
}
