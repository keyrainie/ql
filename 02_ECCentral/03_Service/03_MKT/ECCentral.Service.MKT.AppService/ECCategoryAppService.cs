using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ECCategoryAppService))]
    public class ECCategoryAppService
    {
        private ECCategoryProcessor _ecCategoryProcessor = ObjectFactory<ECCategoryProcessor>.Instance;

        /// <summary>
        /// 插入前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public ECCategory Insert(ECCategory entity)
        {
            return _ecCategoryProcessor.Insert(entity);
        }

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public void Update(ECCategory entity)
        {
            _ecCategoryProcessor.Update(entity);
        }

        /// <summary>
        /// 删除前台显示分类
        /// </summary>
        /// <param name="sysNo">前台显示分类系统编号</param>
        public void Delete(int sysNo)
        {
            _ecCategoryProcessor.Delete(sysNo);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>ECCategory对象</returns>
        public ECCategory Load(int sysNo)
        {
            return _ecCategoryProcessor.Load(sysNo);
        }

        public void InsertCategoryProductMapping(int ecCategorySysNo, List<int> productSysNoList)
        {
            _ecCategoryProcessor.InsertCategoryProductMapping(ecCategorySysNo, productSysNoList);
        }

        public void DeleteCategoryProductMapping(int ecCategorySysNo, List<int> productSysNoList)
        {
            _ecCategoryProcessor.DeleteCategoryProductMapping(ecCategorySysNo, productSysNoList);
        }
    }
}
