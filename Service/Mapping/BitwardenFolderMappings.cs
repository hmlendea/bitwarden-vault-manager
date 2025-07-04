using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenFolderMappings
    {
        internal static BitwardenFolder ToServiceModel(this BitwardenFolderEntity dataObject) => new()
        {
            Id = Guid.Parse(dataObject.Id),
            Name = dataObject.Name
        };

        internal static BitwardenFolderEntity ToDataObject(this BitwardenFolder serviceModel) => new()
        {
            Id = serviceModel.Id.ToString(),
            Name = serviceModel.Name
        };

        internal static IEnumerable<BitwardenFolder> ToServiceModels(this IEnumerable<BitwardenFolderEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<BitwardenFolderEntity> ToDataObjects(this IEnumerable<BitwardenFolder> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());
    }
}
