using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenLoginMappings
    {
        internal static BitwardenLogin ToServiceModel(this BitwardenLoginEntity dataObject) => new()
        {
            Username = dataObject.Username,
            Password = dataObject.Password,
            TOTP = dataObject.TOTP
        };

        internal static BitwardenLoginEntity ToDataObject(this BitwardenLogin serviceModel) => new()
        {
            Username = serviceModel.Username,
            Password = serviceModel.Password,
            TOTP = serviceModel.TOTP
        };

        internal static IEnumerable<BitwardenLogin> ToServiceModels(this IEnumerable<BitwardenLoginEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<BitwardenLoginEntity> ToDataObjects(this IEnumerable<BitwardenLogin> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());
    }
}
