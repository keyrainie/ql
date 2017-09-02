using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Net;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ComputerConfigProcessor))]
    public class ComputerConfigProcessor
    {
        private IComputerConfigDA _configDA = ObjectFactory<IComputerConfigDA>.Instance;

        //加载
        public ComputerConfigMaster LoadComputerConfig(int sysNo)
        {
            var configItems = _configDA.GetComputerConfigItems(sysNo);
            var categories = _configDA.GetAllComputerPartsCategory();
            //调用IM接口取得商品详细
            var productSysNoList = configItems.Where(item => item.ProductSysNo.HasValue).Select(item => item.ProductSysNo.Value).ToList();
            var productInfoList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);
            //调用Inventory接口取得库存信息
            var inventoryInfoList = ExternalDomainBroker.GetProductInventoryInfoByProductSysNoList(productSysNoList);
            foreach (var item in configItems)
            {
                //填充商品详细
                var foundProduct = productInfoList.FirstOrDefault(product => product.SysNo == item.ProductSysNo);
                if (foundProduct != null)
                {
                    item.ProductID = foundProduct.ProductID;
                    item.ProductName = foundProduct.ProductName;
                    item.UnitCost = foundProduct.ProductPriceInfo.UnitCost;
                    item.CurrentPrice = foundProduct.ProductPriceInfo.CurrentPrice;
                }
                //填充库存信息
                var foundInventory = inventoryInfoList.FirstOrDefault(inventory => inventory.ProductSysNo.Value == item.ProductSysNo);
                if (foundInventory != null)
                {
                    item.OnlineQty = foundInventory.OnlineQty;
                }
                item.PartsCategories = categories.FindAll(c => c.ComputerPartSysNo == item.ComputerPartSysNo);
            }
            //按优先级升序
            configItems.OrderBy(item => item.Priority);


            var master = _configDA.LoadMaster(sysNo);
            master.ConfigItemList = configItems;

            return master;
        }


        public List<ComputerConfigItem> GetComputerConfigProductListByProductSysNos(List<int> sysNos)
        {
            List<ComputerConfigItem> result = new List<ComputerConfigItem>();
            foreach (var sysno in sysNos)
            {
                result.AddRange(_configDA.GetComputerConfigProductListByProductSysNo(sysno));
            }
            return result;
        }

        /// <summary>
        /// 根据用户输入的商品信息构造一个ConfigItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ComputerConfigItem BuildConfigItem(ComputerConfigItem item)
        {
            var foundProduct = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
            if (foundProduct == null)
            {
                throw new BizException("商品不存在。");
            }
            var partsCategoryList = _configDA.GetComputerPartsCategory(item.ComputerPartSysNo);
            if (!foundProduct.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue
                || !partsCategoryList.Exists(c => c == foundProduct.ProductBasicInfo.ProductCategoryInfo.SysNo.Value))
            {
                throw new BizException(string.Format("商品{0}不在该组件的可选分类中,不能添加。", foundProduct.ProductID));
            }
            if (foundProduct.ProductStatus != BizEntity.IM.ProductStatus.Active)
            {
                throw new BizException(string.Format("商品{0}不是上架状态,不能添加!。", foundProduct.ProductID));
            }
            //填充商品详细
            item.ProductID = foundProduct.ProductID;
            item.ProductName = foundProduct.ProductName;
            item.UnitCost = foundProduct.ProductPriceInfo.UnitCost;
            item.CurrentPrice = foundProduct.ProductPriceInfo.CurrentPrice;

            //填充库存信息
            var foundInventory = ExternalDomainBroker.GetProductTotalInventoryInfo(item.ProductSysNo.Value);
            if (foundInventory != null)
            {
                if (foundInventory.OnlineQty <= 0)
                {
                    throw new BizException(string.Format("商品{0}库存为零,不能添加!。", foundProduct.ProductID));
                }
                item.OnlineQty = foundInventory.OnlineQty;
            }
            item.ProductQty = 1;
            item.Discount = 0;

            return item;
        }

        /// <summary>
        /// 获取所有组件列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerParts> GetAllComputerParts()
        {
            var parts = _configDA.GetAllComputerParts();
            var categories = _configDA.GetAllComputerPartsCategory();
            foreach (var p in parts)
            {
                p.PartsCategories = categories.FindAll(c => c.ComputerPartSysNo == p.SysNo);
            }
            //按优先级升序
            parts.OrderBy(item => item.Priority);
            return parts;
        }

        /// <summary>
        /// 获取配置单类型列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerConfigType> LoadAllConfigType()
        {
            return _configDA.LoadAllConfigType();
        }

        /// <summary>
        /// 获取配置单最后更新人列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public List<UserInfo> GetEditUsers(string companyCode, string channelID)
        {
            return _configDA.GetEditUsers(companyCode, channelID);
        }

        /// <summary>
        /// 创建配置单
        /// </summary>
        /// <param name="msg"></param>
        public void CreateComputerConfigMaster(ComputerConfigMaster msg)
        {
            ValidateCommon(msg);
            using (TransactionScope scope = new TransactionScope())
            {
                _configDA.CreateComputerConfigMaster(msg);
                foreach (var configItem in msg.ConfigItemList)
                {
                    configItem.ComputerConfigSysNo = msg.SysNo.Value;
                    _configDA.CreateComputerConfigInfo(configItem);
                }
                //操作Combo
                InteractWithCombo(msg);
                ExternalDomainBroker.CreateOperationLog(
                String.Format("{0}{1}SysNo:{2}",
                DateTime.Now.ToString(), "新建配置单"
                , msg.SysNo)
                , BizEntity.Common.BizLogType.ComputerConfig_Add
                , msg.SysNo.Value, msg.CompanyCode);
                scope.Complete();
            }
        }

        /// <summary>
        /// 更新配置单
        /// </summary>
        /// <param name="msg"></param>
        public void UpdateComputerConfigMaster(ComputerConfigMaster msg)
        {
            ValidateCommon(msg);
            using (TransactionScope scope = new TransactionScope())
            {
                _configDA.UpdateComputerConfigMaster(msg);
                _configDA.DeleteComputerConfigInfo(msg.SysNo.Value);
                foreach (var configItem in msg.ConfigItemList)
                {
                    configItem.ComputerConfigSysNo = msg.SysNo.Value;
                    _configDA.CreateComputerConfigInfo(configItem);
                }
                //操作Combo
                InteractWithCombo(msg);

             ExternalDomainBroker.CreateOperationLog(
             String.Format("{0}{1}SysNo:{2}| 配置单名称:{3}| 配置单类型:{4} |配置单说明:{5}|所属渠道:{6}|优先级:{7}",
             DateTime.Now.ToString(), "编辑配置单"
             , msg.SysNo, msg.ComputerConfigName, msg.ComputerConfigTypeSysNo
              ,msg.Note , msg.WebChannel.ChannelID, msg.Priority)
             , BizEntity.Common.BizLogType.ComputerConfig_Update
             , msg.SysNo.Value, msg.CompanyCode);
             scope.Complete();
            }
        }

        public void AuditComputerConfigMaster(int masterSysNo)
        {
            var master = this.LoadComputerConfig(masterSysNo);
            #region Check 审核人与创建人不能相同
            string currentUser = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo).UserName;
            if (master.Status == ComputerConfigStatus.Pending && master.InUser.ToLower() == currentUser.ToLower())
            {
                throw new BizException(string.Format("{0} 创建人与审核人不能相同", master.ComputerConfigName));
            }
            #endregion
            if (master.Status == ComputerConfigStatus.Pending)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    master.Status = ComputerConfigStatus.Running;
                    ValidateMustParts(master);
                    _configDA.AuditComputerConfigMaster(masterSysNo, master.Status);
                    //操作Combo
                    InteractWithCombo(master);
                    scope.Complete();
                }
            }
            //[Mark][Alan.X.Luo 硬编码]
            ExternalDomainBroker.CreateOperationLog(string.Format("DIY自助装机{0}从待审核变审核通过", masterSysNo), BizEntity.Common.BizLogType.ComputerConfig_Aduit, masterSysNo, "8601");
        }

        public void DeclineComputerConfigMaster(int masterSysNo)
        {
            var master = this.LoadComputerConfig(masterSysNo);
            if (master.Status == ComputerConfigStatus.Pending)
            {
                master.Status = ComputerConfigStatus.Void;
                _configDA.AuditComputerConfigMaster(masterSysNo, master.Status);
                //操作Combo
                InteractWithCombo(master);
            }
            //[Mark][Alan.X.Luo 硬编码]
            ExternalDomainBroker.CreateOperationLog(string.Format("DIY自助装机{0}从待审核变审核拒绝", masterSysNo), BizEntity.Common.BizLogType.ComputerConfig_Aduit, masterSysNo, "8601");
        }

        public void VoidComputerConfigMaster(int masterSysNo)
        {
            var master = this.LoadComputerConfig(masterSysNo);
            if (master.Status != ComputerConfigStatus.Void)
            {
                var status = master.Status;
                master.Status = ComputerConfigStatus.Void;
                _configDA.AuditComputerConfigMaster(masterSysNo, master.Status);
                //操作Combo
                InteractWithCombo(master);
            }
            //[Mark][Alan.X.Luo 硬编码]
            ExternalDomainBroker.CreateOperationLog(string.Format("DIY自助装机{0}从 有效变无效", masterSysNo), BizEntity.Common.BizLogType.ComputerConfig_Void, masterSysNo, "8601");
        }

        private void InteractWithCombo(ComputerConfigMaster msg)
        {
            ComputerConfigMaster oldEntity = _configDA.LoadMaster(msg.SysNo.Value);
            //1.只有在配置单状态变为运行或作废状态时才处理和Combo的交互
            //2.用户配置单不处理Combo
            if ((msg.Status != ComputerConfigStatus.Running && msg.Status != ComputerConfigStatus.Void)
                || oldEntity.CustomerSysNo > 0)
            {
                return;
            }
            ComboStatus comboTargetStatus = ComboStatus.Deactive;
            if (msg.Status == ComputerConfigStatus.Running)
            {
                comboTargetStatus = ComboStatus.Active;
            }
            var comboBizProcessor = ObjectFactory<ComboProcessor>.Instance;
            var comboSysNo = _configDA.GetComboSysNo(msg.SysNo.Value);
            if (comboSysNo > 0)
            {
                //Combo已存在，更新Combo状态
                comboBizProcessor.UpdateStatus(comboSysNo, comboTargetStatus);
            }
            else if (msg.Status != ComputerConfigStatus.Void)//作废时候同步状态不新增
            {
                //创建Combo
                ComboInfo comboInfo = new ComboInfo();
                comboInfo.CompanyCode = msg.CompanyCode;
                comboInfo.WebChannel = msg.WebChannel;
                comboInfo.IsShowName = false;
                comboInfo.Name = new LanguageContent(msg.ComputerConfigName.Length > 12 ? msg.ComputerConfigName.Substring(0, 12) : msg.ComputerConfigName);
                comboInfo.ReferenceSysNo = msg.SysNo;
                comboInfo.ReferenceType = 2;
                comboInfo.SaleRuleType = ComboType.Common;
                comboInfo.Status = ComboStatus.Active;
                comboInfo.Priority = msg.Priority;
                comboInfo.Items = new List<ComboItem>();
                foreach (var c in msg.ConfigItemList)
                {

                    if (c.ProductSysNo > 0) //bug 95303 一个配置单有很多配件 不是每一个配件都必选
                    {
                        ComboItem item = new ComboItem();
                        item.ProductSysNo = c.ProductSysNo;
                        item.Quantity = c.ProductQty;
                        item.ProductUnitCost = c.UnitCost;
                        item.ProductID = c.ProductID;
                        item.ProductName = c.ProductName;
                        item.Discount = c.Discount;
                        item.IsMasterItemB = msg.ComputerConfigName == "CPU";
                        comboInfo.Items.Add(item);
                    }
                }
                comboBizProcessor.CreateCombo(comboInfo);
            }
        }

        /// <summary>
        /// Check Items 是否存在于随心配与销售规则中
        /// </summary>
        /// <param name="sysNos"></param>
        /// <returns></returns>
        public virtual List<string> CheckOptionalAccessoriesItemAndCombos(List<int> sysNos)
        {
            List<string> resultMsg = new List<string>();

            List<ComboInfo> comboList = ObjectFactory<ComboProcessor>.Instance.GetActiveAndWaitingComboListByProductSysNo(sysNos);
            List<OptionalAccessoriesItem> oaItemList = ObjectFactory<OptionalAccessoriesProcessor>.Instance.GetActiveAndWaitingItemListByProductSysNo(sysNos);

            if (oaItemList.Count() > 0)
            {
                string masterItemID = string.Empty;
                foreach (var item in oaItemList)
                {
                    masterItemID = ObjectFactory<IOptionalAccessoriesDA>.Instance.Load(item.OptionalAccessoriesSysNo.Value).Items
                                        .Where(o => o.IsMasterItemB.Value).Select(i => i.ProductID).Join(",");
                    resultMsg.Add(string.Format("{2}已存在于随心配编号{0}中，折扣为{1}元，主商品为{3} ",
                            item.OptionalAccessoriesSysNo, item.Discount, item.ProductID, masterItemID));
                }
            }

            if (comboList.Count() > 0)
            {
                string masterItemID = string.Empty;
                foreach (var combo in comboList)
                {
                    masterItemID = combo.Items.Where(i => i.IsMasterItemB.Value).Select(i => i.ProductID).Join(",");
                    foreach (var item in combo.Items.Where(i => !i.IsMasterItemB.Value))
                    {
                        if (sysNos.Contains(item.ProductSysNo.Value))
                        {
                            resultMsg.Add(string.Format("{2}已存在于销售规则编号{0}中，折扣为{1}元，主商品为{3} ",
                                combo.SysNo, item.Discount, item.ProductID, masterItemID));
                        }
                    }
                }
            }

            if (resultMsg.Count() > 0) { resultMsg.Add("请确认是否继续。"); }
            return resultMsg;
        }


        private void ValidateCommon(ComputerConfigMaster entity)
        {
            entity.SysNo = entity.SysNo ?? 0;
            if (entity.ComputerConfigTypeSysNo <= 0)
            {
                throw new BizException("请选择配置单类型!");
            }
            if (string.IsNullOrEmpty(entity.ComputerConfigName))
            {
                throw new BizException("配置单名称不能为空!");
            }
            if (string.IsNullOrEmpty(entity.Note))
            {
                throw new BizException("配置单说明不能为空!");
            }
            if (entity.Priority < 0)
            {
                throw new BizException("配置单优先级必须>=0!");
            }
            if (entity.ConfigItemList == null || entity.ConfigItemList.Count == 0)
            {
                throw new BizException("配置单商品信息列表不能为空!");
            }

            int duplicateCount = _configDA.CountComputerConfigName(entity.SysNo.Value, entity.ComputerConfigName, entity.CompanyCode, entity.WebChannel.ChannelID);
            if (duplicateCount > 0)
            {
                throw new BizException("配置单名称已存在，请验证。");
            }

            decimal totalDiscountPrice = 0M;
            foreach (ComputerConfigItem configItem in entity.ConfigItemList)
            {
                var product = ExternalDomainBroker.GetProductInfo(configItem.ProductSysNo.Value);
                if (product == null)
                {
                    throw new BizException(string.Format("商品{0}不存在。", configItem.ProductSysNo));
                }

                if (configItem.ProductQty <= 0)
                {
                    throw new BizException("商品购买数量未填写或输入有误!");
                }

                if (configItem.Discount > 0)
                {
                    throw new BizException("商品折扣未填写或输入有误!");
                }

                if (configItem.OnlineQty <= 0 || configItem.ProductQty > configItem.OnlineQty)
                {
                    throw new BizException(configItem.ComputerPartName + "商品库存不足!");
                }

                totalDiscountPrice += ObjectFactory<IIMBizInteract>.Instance.GetProductMarginAmount(
                    configItem.CurrentPrice.Value + configItem.Discount, 0, configItem.UnitCost) * configItem.ProductQty;
            }

            if (totalDiscountPrice < 0)
            {
                if (entity.Status != ComputerConfigStatus.Pending)
                {
                    throw new BizException("配置单折扣价毛利率低于基准毛利率，请提交审核!");
                }
            }
            else if (entity.Status == ComputerConfigStatus.Pending)
            {
                throw new BizException("不需要审核,请直接保存!");
            }

            ValidateMustParts(entity);

            //如果通过所有Check， 且提交状态为初始‘O’，则直接运行‘A’
            if (entity.Status == ComputerConfigStatus.Origin)
            {
                entity.Status = ComputerConfigStatus.Running;
            }
        }

        //通过必填部件验证是否存在相同的配置单
        private void ValidateMustParts(ComputerConfigMaster entity)
        {
            if (entity.ConfigItemList == null || entity.ConfigItemList.Count < 1)
            {
                throw new BizException("此配置单商品配置信息有误，请验证。");
            }

            List<int> mustProductSysNoList = new List<int>();
            var _parts = _configDA.GetAllComputerParts().Where(p => p.IsMust == YNStatus.Yes);
            foreach (var p in _parts)
            {
                if (entity.ConfigItemList.Where(i => i.ComputerPartSysNo == p.SysNo).Count() < 1)
                {
                    throw new BizException(string.Format("配置项”{0}“为必填，请先选择对应类型商品。", p.ComputerPartName));
                }
            }

            foreach (ComputerConfigItem configItem in entity.ConfigItemList)
            {
                if (configItem.IsMust == YNStatus.Yes)
                {
                    if (configItem.ProductSysNo == null || configItem.ProductSysNo.Value <= 0)
                    {
                        throw new BizException(string.Format("{0}为必选，请选择有效商品!", configItem.ComputerPartName));
                    }
                    mustProductSysNoList.Add(configItem.ProductSysNo.Value);
                }
            }
            if (mustProductSysNoList.Count == 0)
            {
                throw new BizException("配置单必选商品信息列表不能为空!");
            }
            //通过必填部件验证是否存在相同的配置单
            mustProductSysNoList.Sort();
            entity.UniqueValidation = mustProductSysNoList.Join("-");
            int uCount = _configDA.CountUniqueValidation(entity.SysNo.Value, entity.UniqueValidation, entity.CompanyCode, entity.WebChannel.ChannelID);
            if (uCount > 0)
            {
                throw new BizException("已存在相同配置单，请选择其他必选商品!");
            }
        }

        #region DIY装机调度JOB
        public void CheckComputerConfigInfo()
        {
            var computerConfigList = ObjectFactory<IComputerConfigQueryDA>.Instance.GetComputerConfigMasterList();
            var masterSysNoTmp = new List<int>();
            //分 Customer 和 NewEgg 配置单进行处理
            foreach (var computerConfig in computerConfigList)
            {
                foreach (var configItem in computerConfig.ConfigItemList)
                {
                    //邮件内容
                    string recordTmp = string.Empty;
                    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    //判断是否是NewEgg配置单
                    if (computerConfig.CustomerSysNo < 0 && computerConfig.CreateUserSysNo == 0)
                    {
                        //是否库存不足或商品状态无效
                        if ((configItem.OnlineQty <= 0 || configItem.OnlineQty < configItem.ProductQty || configItem.ProductStatus != ProductStatus.Active) && !masterSysNoTmp.Contains(computerConfig.SysNo.Value))
                        {
                            using (var ts = new TransactionScope())
                            {
                                computerConfig.Status = ComputerConfigStatus.Void;
                                ObjectFactory<IComputerConfigDA>.Instance.SetComputerConfigMasterStatus(computerConfig);
                                ObjectFactory<IComputerConfigDA>.Instance.UpdateSaleRuleStatus(computerConfig);
                                masterSysNoTmp.Add(computerConfig.SysNo.Value);
                                recordTmp = (string.Format("\r\n由于商品\"{1}\"的库存或者状态有变化，配置单\"{0}\"状态已被修改为无效。" + "库存：{2} 商品状态：{3}"
                                                            , computerConfig.ComputerConfigName, configItem.ProductID, configItem.OnlineQty, GetProductStatusStr((int)configItem.ProductStatus)));
                                //写入日志
                                ExternalDomainBroker.CreateOperationLog(recordTmp, BizLogType.ComputerConfig_Void, computerConfig.SysNo.Value, computerConfig.CompanyCode);
                                //发送邮件
                                SendMail("配置单编辑", computerConfig.SysNo.Value, recordTmp, computerConfig.CompanyCode, computerConfig.CreateUserSysNo);
                                ts.Complete();
                            }
                        }

                    }
                    else
                    {
                        if (configItem.ProductStatus != 0 && configItem.ProductStatus != ProductStatus.Active && !masterSysNoTmp.Contains(computerConfig.SysNo.Value))
                        {
                            using (var ts = new TransactionScope())
                            {
                                computerConfig.Status = ComputerConfigStatus.Void;
                                ObjectFactory<IComputerConfigDA>.Instance.SetComputerConfigMasterStatus(computerConfig);
                                ObjectFactory<IComputerConfigDA>.Instance.UpdateSaleRuleStatus(computerConfig);
                                masterSysNoTmp.Add(computerConfig.SysNo.Value);
                                recordTmp = (string.Format("\r\n由于商品\"{1}\"的库存或者状态有变化，配置单\"{0}\"状态已被修改为无效。" + "状态：{3}"
                                                    , computerConfig.ComputerConfigName, configItem.ProductID, configItem.OnlineQty, GetProductStatusStr((int)configItem.ProductStatus)));
                                //写入日志
                                ExternalDomainBroker.CreateOperationLog(recordTmp, BizLogType.ComputerConfig_Void, computerConfig.SysNo.Value, computerConfig.CompanyCode);
                                //发送邮件
                                SendMail("配置单编辑", computerConfig.SysNo.Value, recordTmp, computerConfig.CompanyCode, computerConfig.CreateUserSysNo);
                                ts.Complete();
                            }
                        }
                    }
                }
            }
        }

        private static void SendMail(string mailSubject, int computerConfigSysNo, string operationNote, string companyCode, int createUserSysNo)
        {
            var userInfo = ExternalDomainBroker.GetUserInfoBySysNo(createUserSysNo);
            var param = new KeyValueVariables();
            param.Add("Subject", mailSubject);
            param.Add("LogSysNo", computerConfigSysNo);
            param.Add("OperationNote", operationNote);
            param.Add("CompanyCode", companyCode);
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(userInfo.EmailAddress, "MKT_ComputerConfig_Edit", param, true);
        }

        private static string GetProductStatusStr(int productStatus)
        {
            string returnVal = string.Empty;
            // -1 作废 0 仅展示 1 上架 2 不展示
            switch (productStatus)
            {
                case -1:
                    returnVal = "作废";
                    break;
                case 0:
                    returnVal = "仅展示";
                    break;
                case 1:
                    returnVal = "上架";
                    break;
                case 2:
                    returnVal = "不展示";
                    break;
                default:
                    break;
            }
            return returnVal;
        }
        #endregion
    }
}
