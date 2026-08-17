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
            BitwardenItem serviceModel = new()
            {
                Id = Guid.Parse(dataObject.Id),
                Type = (BitwardenItemType)dataObject.Type,
                Name = dataObject.Name
            };

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

        internal static BitwardenItemEntity ToDataObject(this BitwardenItem serviceModel) => new()
        {
            Id = serviceModel.Id.ToString(),
            Type = (int)serviceModel.Type,
            Name = serviceModel.Name,
            FolderId = serviceModel.FolderId.ToString(),
            Favourite = serviceModel.IsFavourite,
            Notes = serviceModel.Notes,
            Login = serviceModel.Login.ToDataObject(),
            Fields = serviceModel.Fields.ToDataObjects()
        };

        internal static IEnumerable<BitwardenItem> ToServiceModels(this IEnumerable<BitwardenItemEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<BitwardenItemEntity> ToDataObjects(this IEnumerable<BitwardenItem> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());
    }
}
