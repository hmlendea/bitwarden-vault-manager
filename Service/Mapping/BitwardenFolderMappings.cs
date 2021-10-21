using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenFolderMappings
    {
        internal static BitwardenFolder ToServiceModel(this BitwardenFolderEntity dataObject)
        {
            BitwardenFolder serviceModel = new BitwardenFolder();
            serviceModel.Id = Guid.Parse(dataObject.Id);
            serviceModel.Name = dataObject.Name;

            return serviceModel;
        }

        internal static BitwardenFolderEntity ToDataObject(this BitwardenFolder serviceModel)
        {
            BitwardenFolderEntity dataObject = new BitwardenFolderEntity();
            dataObject.Id = serviceModel.Id.ToString();
            dataObject.Name = serviceModel.Name;

            return dataObject;
        }

        internal static IEnumerable<BitwardenFolder> ToServiceModels(this IEnumerable<BitwardenFolderEntity> dataObjects)
        {
            IEnumerable<BitwardenFolder> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<BitwardenFolderEntity> ToDataObjects(this IEnumerable<BitwardenFolder> serviceModels)
        {
            IEnumerable<BitwardenFolderEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
