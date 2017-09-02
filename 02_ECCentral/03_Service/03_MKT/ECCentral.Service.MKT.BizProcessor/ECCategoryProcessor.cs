using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using System.Transactions;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ECCategoryProcessor))]
    public class ECCategoryProcessor
    {
        private IECCategoryDA _ecCategoryDA = ObjectFactory<IECCategoryDA>.Instance;
        private IECCategoryQueryDA _ecCategoryQueryDA = ObjectFactory<IECCategoryQueryDA>.Instance;
        /// <summary>
        /// 通过前台3级类别找到对应的后台3级类别，
        /// 在把与后台3级类别对用的所有前台3级类别找出来
        /// 不包含传入的c3SysNo
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public List<ECCategory> GetRelatedECCategory3SysNo(int c3SysNo)
        {
            return _ecCategoryDA.GetRelatedECCategory3SysNo(c3SysNo);
        }

        /// <summary>
        /// 插入前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public ECCategory Insert(ECCategory entity)
        {
            Validate(entity);
            using (TransactionScope ts = new TransactionScope())
            {
                _ecCategoryDA.Insert(entity);
                if (entity.Level == ECCategoryLevel.Category1)
                {
                    ECCategoryRelation r = new ECCategoryRelation();
                    r.ECCategorySysNo = entity.SysNo.Value;
                    r.ParentSysNo = null;
                    _ecCategoryDA.InsertRelation(r);
                }
                else
                {
                    if (entity.ParentList != null)
                    {
                        foreach (var p in entity.ParentList)
                        {
                            if (p.RSysNo > 0)
                            {
                                ECCategoryRelation r = new ECCategoryRelation();
                                r.ECCategorySysNo = entity.SysNo.Value;
                                r.ParentSysNo = p.RSysNo;
                                _ecCategoryDA.InsertRelation(r);
                            }
                            else
                            {
                                ECCategory parentItem= _ecCategoryDA.Load(p.SysNo.Value);
                                if(parentItem==null||parentItem.RSysNo<=0)
                                {
                                    throw new BizException(string.Format("节点{1}为无效节点！",p.Name));
                                }
                                ECCategoryRelation r = new ECCategoryRelation();
                                r.ECCategorySysNo = entity.SysNo.Value;
                                r.ParentSysNo = parentItem.RSysNo;
                                _ecCategoryDA.InsertRelation(r);
                            }

                        }
                    }
                }


                ts.Complete();
            }

            return entity;

        }

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public void Update(ECCategory entity)
        {
            Validate(entity);
            using (TransactionScope ts = new TransactionScope())
            {
                _ecCategoryDA.Update(entity);
                //2.删除不在当前父类列表中的层级关系
                List<int> rParentSysNoList = entity.ParentList.ConvertAll(p => p.RSysNo);
                _ecCategoryDA.DeleteOldRelation(entity.SysNo.Value, rParentSysNoList);
                //3.重新插入层级关系
                foreach (var rSysNo in rParentSysNoList)
                {
                    if (!_ecCategoryDA.ExistsRelation(entity.SysNo.Value, rSysNo))
                    {
                        ECCategoryRelation r = new ECCategoryRelation();
                        r.ECCategorySysNo = entity.SysNo.Value;
                        r.ParentSysNo = rSysNo;
                        _ecCategoryDA.InsertRelation(r);
                    }
                }

                ts.Complete();
            }
        }

        /// <summary>
        /// 删除前台显示分类
        /// </summary>
        /// <param name="sysNo">前台显示分类系统编号</param>
        public void Delete(int sysNo)
        {
            var c = _ecCategoryDA.Load(sysNo);
            var childList = _ecCategoryQueryDA.GetECCategoryCurrentChildSysNos(c.SysNo.Value, c.RSysNo);
            if (childList != null && childList.Count > 0)
            {
                //throw new BizException("当前节点存在有效或无效的子节点，不允许删除。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_CannotDelete"));
            }
            using (TransactionScope ts = new TransactionScope())
            {
                //1.删除层级关系
                _ecCategoryDA.DeleteRelation(sysNo);
                //2.删除前台分类记录
                _ecCategoryDA.Delete(sysNo);

                ts.Complete();
            }
        }

        private void Validate(ECCategory entity)
        {
            entity.CompanyCode = "8601";
            string parentSysnoList="-1";
            if (entity.ParentList != null && entity.ParentList.Count>0)
            {
                foreach (var item in entity.ParentList)
                {
                    parentSysnoList +=","+ item.RSysNo;
                }
            }
            if (_ecCategoryDA.CheckNameDuplicate(entity.Name, entity.SysNo ?? 0, entity.Level, entity.CompanyCode, entity.WebChannel.ChannelID, parentSysnoList))
            {
                //throw new BizException("类别名称已存在，请检查。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_ExistsSameName"));
            }
            if (entity.Level != ECCategoryLevel.Category1)
            {
                if (entity.ParentList == null || entity.ParentList.Count == 0)
                {
                    //throw new BizException("父类数量不能为0。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_MustHaveParents"));
                }
            }
            
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>ECCategory对象</returns>
        public ECCategory Load(int sysNo)
        {
            //加载父类列表

            //加载子类列表

            return _ecCategoryDA.Load(sysNo);
        }

        public void InsertCategoryProductMapping(int ecCategorySysNo, List<int> productSysNoList)
        {
            Dictionary<int, string> isMapped = new Dictionary<int, string>();

            if (productSysNoList != null)
            {
                productSysNoList.ForEach(x =>
                {
                    string name = _ecCategoryDA.IsProductMapped(x);
                    if (!string.IsNullOrEmpty(name))
                    {
                        isMapped.Add(x, name);
                    }
                });

                if (isMapped.Count > 0)
                {
                    string message = string.Empty;
                    isMapped.ForEach(kv =>
                    {
                       // message += string.Format("商品:{0}，已添加到 {1} 分类中，请先删除再添加 ", kv.Key, kv.Value) + "\n";
                        message += string.Format(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_ExsistGoodInCategory"), kv.Key, kv.Value) + "\n";
                    });

                    throw new BizException(message);
                }
            }

            using (TransactionScope scope = new TransactionScope())
            {
                if (productSysNoList != null)
                {
                    productSysNoList.ForEach(p =>
                    {
                        _ecCategoryDA.InsertCategoryProductMapping(ecCategorySysNo, p);
                    });
                }
                scope.Complete();
            }
        }

        public void DeleteCategoryProductMapping(int ecCategorySysNo, List<int> productSysNoList)
        {
            string error = string.Empty;
            using (TransactionScope scope = new TransactionScope())
            {
                if (productSysNoList != null)
                {
                    productSysNoList.ForEach(p =>
                    {
                        bool succ = _ecCategoryDA.DeleteCategoryProductMapping(ecCategorySysNo, p);
                        if (!succ)
                        {
                            //error += string.Format("商品：{0} 不在当前选择的分类中,删除不成功\n", p);
                            error += string.Format(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_GoodsNotInCurrentCategory"), p);
                        }
                    });
                }
                scope.Complete();
            }

            if (!string.IsNullOrEmpty(error))
            {
                throw new BizException(error);
            }
        }
    }
}
