using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using ProductSaleInfoBiz.Model;
using ProductSaleInfoBiz.Compoents;

namespace ProductSaleInfoBiz.DAL
{
    public class ProductSaleInoDal
    {
        public static List<ProductSaleInfo> GetProductSaleInoInfo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductSaleInoInfo");
            command.SetParameterValue("@BeginTimeWeek",DateTime.Now.Date.AddDays(-7));
            command.SetParameterValue("@EndTimeWeek",DateTime.Now.Date);
            command.SetParameterValue("@BeginTimeMonth", DateTime.Now.Date.AddDays(-30));
            command.SetParameterValue("@EndTimeMonth", DateTime.Now.Date);
            command.SetParameterValue("@BeginTime14day", DateTime.Now.Date.AddDays(-14));
            command.SetParameterValue("@BeginTime60day", DateTime.Now.Date.AddDays(-60));
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            return command.ExecuteEntityList<ProductSaleInfo>();
        }
    }
}
