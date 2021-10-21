using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service.Mapping
{
    static class BitwardenFieldMappings
    {
        internal static BitwardenField ToServiceModel(this BitwardenFieldEntity dataObject)
        {
            BitwardenField serviceModel = new BitwardenField();
            serviceModel.Name = dataObject.Name;
            serviceModel.Value = dataObject.Value;

            return serviceModel;
        }

        internal static BitwardenFieldEntity ToDataObject(this BitwardenField serviceModel)
        {
            BitwardenFieldEntity dataObject = new BitwardenFieldEntity();
            dataObject.Name = serviceModel.Name;
            dataObject.Value = serviceModel.Value;

            return dataObject;
        }

        internal static IEnumerable<BitwardenField> ToServiceModels(this IEnumerable<BitwardenFieldEntity> dataObjects)
        {
            IEnumerable<BitwardenField> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<BitwardenFieldEntity> ToDataObjects(this IEnumerable<BitwardenField> serviceModels)
        {
            IEnumerable<BitwardenFieldEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
