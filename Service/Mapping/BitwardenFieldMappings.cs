using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenFieldMappings
    {
        internal static BitwardenField ToServiceModel(this BitwardenFieldEntity dataObject) => new()
        {
            Name = dataObject.Name,
            Value = dataObject.Value
        };

        internal static BitwardenFieldEntity ToDataObject(this BitwardenField serviceModel) => new()
        {
            Name = serviceModel.Name,
            Value = serviceModel.Value
        };

        internal static IEnumerable<BitwardenField> ToServiceModels(this IEnumerable<BitwardenFieldEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<BitwardenFieldEntity> ToDataObjects(this IEnumerable<BitwardenField> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());
    }
}
