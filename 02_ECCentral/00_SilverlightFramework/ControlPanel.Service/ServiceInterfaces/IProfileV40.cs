using System.Collections.Generic;
using System.ServiceModel;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface IProfileV40
    {
        [OperationContract]
        SimpleTypeDataContract<List<UserProfile>> Query(ProfileQueryV40 contract);

        [OperationContract]
        DefaultDataContract Save(SimpleTypeDataContract<List<UserProfile>> contract);

        [OperationContract]
        DataGridProfileDataV40 GetDataGridProfileItems(SimpleTypeDataContract<string> guid);
    }
}
