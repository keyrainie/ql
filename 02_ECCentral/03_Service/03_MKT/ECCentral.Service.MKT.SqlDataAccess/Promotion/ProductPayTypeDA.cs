using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductPayTypeDA))]
    public class ProductPayTypeDA : IProductPayTypeDA
    {
        public int CreateProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            var cmd = DataCommandManager.GetDataCommand("InsertDisAllowProductPayTypeList");
            cmd.SetParameterValue("@ProductID", productPayTypeInfo.ProductID);
            cmd.SetParameterValue("@BeginDate", productPayTypeInfo.BeginDate);
            cmd.SetParameterValue("@EndDate", productPayTypeInfo.EndDate);
            cmd.SetParameterValue("@PayType", productPayTypeInfo.PayTypeSysNo);
            cmd.SetParameterValue("@EditUser", productPayTypeInfo.EditUser);
            cmd.SetParameterValue("@InUser", productPayTypeInfo.EditUser);
            cmd.SetParameterValue("@Status", 'A');

            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@result");
        }


        public int AbortProductPayType(string sysNo, string editUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbortPayType");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@EditUser", editUser);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@result");
        }
    }
}
