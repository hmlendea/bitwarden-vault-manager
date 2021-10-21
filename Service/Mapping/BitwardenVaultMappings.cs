using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenVaultMappings
    {
        internal static BitwardenVault ToServiceModel(this BitwardenVaultEntity dataObject)
        {
            BitwardenVault serviceModel = new BitwardenVault();
            serviceModel.IsEncrypted = dataObject.Encrypted;
            serviceModel.Folders = dataObject.Folders.ToServiceModels().ToList();
            serviceModel.Items = dataObject.Items.ToServiceModels().ToList();

            return serviceModel;
        }

        internal static BitwardenVaultEntity ToDataObject(this BitwardenVault serviceModel)
        {
            BitwardenVaultEntity dataObject = new BitwardenVaultEntity();
            dataObject.Encrypted = serviceModel.IsEncrypted;
            dataObject.Folders = serviceModel.Folders.ToDataObjects().ToList();
            dataObject.Items = serviceModel.Items.ToDataObjects().ToList();

            return dataObject;
        }

        internal static IEnumerable<BitwardenVault> ToServiceModels(this IEnumerable<BitwardenVaultEntity> dataObjects)
        {
            IEnumerable<BitwardenVault> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<BitwardenVaultEntity> ToDataObjects(this IEnumerable<BitwardenVault> serviceModels)
        {
            IEnumerable<BitwardenVaultEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
