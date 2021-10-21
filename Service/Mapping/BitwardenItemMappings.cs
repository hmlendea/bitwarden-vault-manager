using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenItemMappings
    {
        internal static BitwardenItem ToServiceModel(this BitwardenItemEntity dataObject)
        {
            BitwardenItem serviceModel = new BitwardenItem();
            serviceModel.Id = Guid.Parse(dataObject.Id);
            serviceModel.Type = (BitwardenItemType)dataObject.Type;
            serviceModel.Name = dataObject.Name;

            if (!string.IsNullOrWhiteSpace(dataObject.FolderId))
            {
                serviceModel.FolderId = Guid.Parse(dataObject.FolderId);
            }

            serviceModel.IsFavourite = dataObject.Favourite;
            serviceModel.Notes = dataObject.Notes;

            if (dataObject.Login != null)
            {
                serviceModel.Login = dataObject.Login.ToServiceModel();
            }

            if (dataObject.Fields != null)
            {
                serviceModel.Fields = dataObject.Fields.ToServiceModels();
            }

            return serviceModel;
        }

        internal static BitwardenItemEntity ToDataObject(this BitwardenItem serviceModel)
        {
            BitwardenItemEntity dataObject = new BitwardenItemEntity();
            dataObject.Id = serviceModel.Id.ToString();
            dataObject.Type = (int)serviceModel.Type;
            dataObject.Name = serviceModel.Name;
            dataObject.FolderId = serviceModel.FolderId.ToString();
            dataObject.Favourite = serviceModel.IsFavourite;
            dataObject.Notes = serviceModel.Notes;
            dataObject.Login = serviceModel.Login.ToDataObject();
            dataObject.Fields = serviceModel.Fields.ToDataObjects();

            return dataObject;
        }

        internal static IEnumerable<BitwardenItem> ToServiceModels(this IEnumerable<BitwardenItemEntity> dataObjects)
        {
            IEnumerable<BitwardenItem> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<BitwardenItemEntity> ToDataObjects(this IEnumerable<BitwardenItem> serviceModels)
        {
            IEnumerable<BitwardenItemEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
