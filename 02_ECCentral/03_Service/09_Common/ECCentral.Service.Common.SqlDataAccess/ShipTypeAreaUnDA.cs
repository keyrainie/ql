using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Common.IDataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IShipTypeAreaUnDA))]
    public class ShipTypeAreaUnDA:IShipTypeAreaUnDA
    {
        /// <summary>
        /// 删除配送方式-地区（非）
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public void VoidShipTypeAreaUn(List<int> sysNoList)
        {
            if (sysNoList != null && sysNoList.Count > 0)
            {
                CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeAreaUn_Void");
                cmd.CommandText = cmd.CommandText.Replace("@SysNo", String.Join(",", sysNoList));
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 新增配送方式-地区（非）
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public ErroDetail CreateShipTypeAreaUn(ShipTypeAreaUnInfo entity)
        {
            List<ShipTypeAreaUnInfo> _listShipTypeAreaUn;
            CommonDA common = new CommonDA();
            _listShipTypeAreaUn = common.GetShipTypeAreaUnList(entity.CompanyCode);

            ErroDetail _erro = new ErroDetail();
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeAreaUn_Create");
            StringBuilder sqlbuiler = new StringBuilder();
            if (entity.AreaSysNoList != null && entity.AreaSysNoList.Count > 0)
            {
                foreach (int sysno in entity.AreaSysNoList)
                {
                    ShipTypeAreaUnInfo item;
                    //ErroDetail erro = new ErroDetail();
                    item=_listShipTypeAreaUn.Where(f => f.AreaSysNo == sysno && f.ShipTypeSysNo == entity.ShipTypeSysNo).FirstOrDefault();
                    if (item == null)
                    {
                        cmd.SetParameterValue("@ShipTypeSysNo", entity.ShipTypeSysNo);
                        cmd.SetParameterValue("@CompanyCode", "8601");
                        string sql = cmd.CommandText.Replace("#AreaSysNo#", sysno.ToString());
                        sqlbuiler.AppendLine(sql);
                        _erro.SucceedList.Add(item);
                    }
                    else
                    {
                        _erro.ErroList.Add(item); ;

                    }
                }
                if (!string.IsNullOrEmpty(sqlbuiler.ToString()))
                {
                    cmd.CommandText = sqlbuiler.ToString();
                    cmd.ExecuteNonQuery();
                }
            }
            return _erro;
        }


        public List<ShipTypeAreaUnInfo> QueryShipAreaUnByAreaSysNo(IEnumerable<int> shipTypeSysNoS, int areaSysNo)
        {
            var shipTypeArray = shipTypeSysNoS.Select(p => p.ToString()).ToArray();
            var dataCommand = DataCommandManager.GetDataCommand("QueryShipAreaUnByAreaSysNo");
            dataCommand.SetParameterValue("@CompanyCode", "8601");
            dataCommand.SetParameterValue("@AreaSysNo", areaSysNo);

            dataCommand.ReplaceParameterValue("#ShipTypeSysNos", string.Join(",", shipTypeArray));

            return dataCommand.ExecuteEntityList<ShipTypeAreaUnInfo>();
        }
    }
}
