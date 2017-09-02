using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(VendorAppService))]
    public class VendorAppService
    {
        #region [Fields]
        private VendorProcessor m_VendorProcessor;
        private IInventoryBizInteract m_InventoryBizInteract;

        public VendorProcessor VendorProcessor
        {
            get
            {
                if (null == m_VendorProcessor)
                {
                    m_VendorProcessor = ObjectFactory<VendorProcessor>.Instance;
                }
                return m_VendorProcessor;
            }
        }

        public IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                if (null == m_InventoryBizInteract)
                {
                    m_InventoryBizInteract = ObjectFactory<IInventoryBizInteract>.Instance;
                }
                return m_InventoryBizInteract;
            }
        }
        #endregion

        public VendorInfo LoadVendorInfoBySysNo(int vendorSysNo)
        {
            return VendorProcessor.LoadVendorInfo(vendorSysNo);
        }

        public List<VendorHistoryLog> LoadVendorHistoryLogBySysNo(int vendorSysNo)
        {
            VendorInfo newVendorInfo = new VendorInfo()
            {
                SysNo = vendorSysNo
            };
            return VendorProcessor.LoadVendorHistoryLogInfo(newVendorInfo);
        }

        public VendorInfo CreateNewVendor(VendorInfo newVendorInfo)
        {
            return VendorProcessor.CreateVendor(newVendorInfo);
        }

        public VendorModifyRequestInfo CancelVendorManufacturer(VendorModifyRequestInfo info)
        {
            return VendorProcessor.CancelVendorManufacturer(info);
        }

        public VendorModifyRequestInfo PassVendorManufacturer(VendorModifyRequestInfo info)
        {
            return VendorProcessor.PassVendorManufacturer(info);
        }

        public void ApproveVendor(VendorApproveInfo vendorApproveInfo)
        {
            VendorProcessor.ApproveVendor(vendorApproveInfo);
        }

        public VendorModifyRequestInfo RequestForApprovalVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            return VendorProcessor.RequestForApprovalVendorFinanceInfo(info);
        }

        public VendorModifyRequestInfo ApproveVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            return VendorProcessor.ApproveVendorFinanceInfo(info);
        }

        public VendorModifyRequestInfo DeclineVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            return VendorProcessor.DeclineVendorFinanceInfo(info);
        }

        public VendorModifyRequestInfo WithDrawVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            return VendorProcessor.WithDrawVendorFinanceInfo(info);
        }

        public int HoldVendor(VendorInfo vendorInfo)
        {
            return VendorProcessor.HoldVendor(vendorInfo);
        }

        public int UnHoldVendor(VendorInfo vendorInfo)
        {
            return VendorProcessor.UnHoldVendor(vendorInfo);
        }

        public VendorInfo EditVendorInfo(VendorInfo info)
        {
            return VendorProcessor.EditVendor(info);
        }

        public VendorHistoryLog CreateVendorHistoryLog(VendorHistoryLog logInfo)
        {
            return VendorProcessor.CreateVendorHistoryLog(logInfo);
        }

        public void UpdateVendorEmailAddress(VendorInfo vendorInfo)
        {
            VendorProcessor.UpdateVendorEmail(vendorInfo);
        }

        public string MoveVendorFileAttachments(string fileIdentity)
        {
            string getConfigPath = AppSettingManager.GetSetting("PO", "VendorAttachmentFilesPath");
            if (!Path.IsPathRooted(getConfigPath))
            {
                //是相对路径:
                getConfigPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, getConfigPath);
            }
            string fileName = Guid.NewGuid().ToString() + FileUploadManager.GetFileExtensionName(fileIdentity);
            string getDestinationPath = Path.Combine(getConfigPath, fileName);
            string getFolder = Path.GetDirectoryName(getDestinationPath);
            if (!Directory.Exists(getFolder))
            {
                Directory.CreateDirectory(getFolder);
            }
            //将上传的文件从临时文件夹剪切到目标文件夹:
            FileUploadManager.MoveFile(fileIdentity, getDestinationPath);
            return fileName;
        }

        public List<WarehouseInfo> LoadWareHouseInfo(string companyCode)
        {
            return InventoryBizInteract.GetWarehouseList(companyCode);

        }

        public List<int> HoldOrUnholdVendorPM(int vendorSysNo, int holdUserSysNo, string reason, List<int> holdSysNoList, List<int> unHoldSysNoList, string companyCode)
        {
            return VendorProcessor.HoldOrUnholdVendorPM(vendorSysNo, holdUserSysNo, reason, holdSysNoList, unHoldSysNoList, companyCode);
        }
    }
}