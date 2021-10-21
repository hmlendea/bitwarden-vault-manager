using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenLoginMappings
    {
        internal static BitwardenLogin ToServiceModel(this BitwardenLoginEntity dataObject)
        {
            BitwardenLogin serviceModel = new BitwardenLogin();
            serviceModel.Username = dataObject.Username;
            serviceModel.Password = dataObject.Password;
            serviceModel.TOTP = dataObject.TOTP;

            return serviceModel;
        }

        internal static BitwardenLoginEntity ToDataObject(this BitwardenLogin serviceModel)
        {
            BitwardenLoginEntity dataObject = new BitwardenLoginEntity();
            dataObject.Username = serviceModel.Username;
            dataObject.Password = serviceModel.Password;
            dataObject.TOTP = serviceModel.TOTP;

            return dataObject;
        }

        internal static IEnumerable<BitwardenLogin> ToServiceModels(this IEnumerable<BitwardenLoginEntity> dataObjects)
        {
            IEnumerable<BitwardenLogin> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<BitwardenLoginEntity> ToDataObjects(this IEnumerable<BitwardenLogin> serviceModels)
        {
            IEnumerable<BitwardenLoginEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
