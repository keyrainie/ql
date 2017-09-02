using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRequestDA
    {
        int CreateSysNo();

        RMARequestInfo Create(RMARequestInfo entity);

        void Update(RMARequestInfo entity);

        void UpdateStatus(int soSysNo, RMARequestStatus status);

        DataSet LoadForCheckCancelReceive(int requestSysNo);

        RMARequestInfo LoadBySysNo(int sysNo);

        RMARequestInfo LoadWithRegistersBySysNo(int sysNo);

        RMARequestInfo LoadByRegisterSysNo(int registerSysNo);

        List<RMARequestInfo> LoadRequestBySOSysNo(int soSysNo);

        string GetInventoryMemo(int? whNo, int? productSysNo, string companyCode);

        void InsertRMAInventoryLog(RMAInventoryLog entity);

        bool PrintLabels(int sysNo);

        bool IsRMARequestExists(int soSysNo);

        string CreateServiceCode();
    }
}
