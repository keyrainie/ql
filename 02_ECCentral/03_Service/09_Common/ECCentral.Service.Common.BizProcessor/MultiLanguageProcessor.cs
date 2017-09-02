using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using System.IO;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(MultiLanguageProcessor))]
    public class MultiLanguageProcessor
    {
        private IMultiLanguageDA m_DataAccess = ObjectFactory<IMultiLanguageDA>.Instance;


        public List<MultiLanguageBizEntity> GetMultiLanguageBizEntityList(MultiLanguageDataContract dataContract)
        {
            if (dataContract == null || string.IsNullOrWhiteSpace(dataContract.BizEntityType) || dataContract.SysNo == 0)
            {
                throw new BizException("请输入有效的业务对象类型和系统编号！");
            }
            List<MultiLanguageBizEntity> resourceList = MultiLanguageResourceManager.LoadResouce();
            
            MultiLanguageBizEntity curEntityTemplate = resourceList.Find(f => f.BizEntityType.Trim().ToUpper() == dataContract.BizEntityType.Trim().ToUpper());
            if (curEntityTemplate == null || curEntityTemplate.PropertyItemList == null || curEntityTemplate.PropertyItemList.Count==0)
            {
                throw new BizException(string.Format("请为{0}配置正确的属性！", dataContract.BizEntityType));
            }
            curEntityTemplate.BizEntityType = dataContract.BizEntityType.Trim();
            curEntityTemplate.SysNo = dataContract.SysNo;

            List<MultiLanguageBizEntity> list = m_DataAccess.GetMultiLanguageBizEntityList(curEntityTemplate);

            List<Language> languageList = m_DataAccess.GetAllLanguageList();
            languageList.RemoveAll(f => f.IsDefault == 1);

            List<MultiLanguageBizEntity> result = new List<MultiLanguageBizEntity>();
            foreach (Language lang in languageList)
            {
                MultiLanguageBizEntity curEntity = list.Find(f => f.LanguageCode.Trim().ToUpper() == lang.LanguageCode.Trim().ToUpper());
                if (curEntity == null)
                {
                    MultiLanguageBizEntity entityRes = curEntityTemplate.DeepCopy();
                    entityRes.PropertyItemList.ForEach(item=>item.Value=null);
                    entityRes.LanguageCode = lang.LanguageCode;
                    entityRes.LanguageName = lang.LanguageName;
                    result.Add(entityRes);
                }
                else
                {
                    curEntity.LanguageName = lang.LanguageName;
                    result.Add(curEntity);
                }
            } 
            return result;
        }

        public void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            if (entity == null)
            {
                throw new BizException("请输入有效的业务对象！");
            }
            if (string.IsNullOrWhiteSpace(entity.LanguageCode) || string.IsNullOrWhiteSpace(entity.MappingTable)
                || entity.SysNo == 0 || string.IsNullOrWhiteSpace(entity.BizEntityType))
            {
                throw new BizException("业务对象的语言编码，系统编号，业务类型，映射表名不能为空！");
            }
            if (entity.PropertyItemList == null || entity.PropertyItemList.Count == 0)
            {
                throw new BizException(string.Format("请为{0}配置正确的属性！", entity.BizEntityType));
            }

            m_DataAccess.SetMultiLanguageBizEntity(entity);
        }
    }

    internal static class MultiLanguageResourceManager
    {
        private static object s_SyncObj = new object();
        private static List<MultiLanguageBizEntity> _entityList = null;

        internal static List<MultiLanguageBizEntity> LoadResouce()
        {
            if (_entityList == null)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Configuration/MultiLanguageBizEntity.Config");
                lock (s_SyncObj)
                {
                    _entityList = ECCentral.Service.Utility.SerializationUtility.LoadFromXml<List<MultiLanguageBizEntity>>(path);
                    if (_entityList == null)
                    {
                        _entityList = new List<MultiLanguageBizEntity>();
                    }
                }
            }
            return _entityList;

        }
    }
}