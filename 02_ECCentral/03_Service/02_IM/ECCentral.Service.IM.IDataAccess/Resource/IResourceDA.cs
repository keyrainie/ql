using System;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IResourceDA
    {
        Resource InsertResource(Resource resource);

        void DeleteResource(Int32? resourceSysNo);

        void SaveResource(Resource resource);

    }
}
