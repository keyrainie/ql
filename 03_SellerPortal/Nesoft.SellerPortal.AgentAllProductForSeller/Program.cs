using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Nesoft.SellerPortal.Entity.Store.Vendor;
using Nesoft.SellerPortal.Service.Common;
using Nesoft.SellerPortal.Service.Product;
using Nesoft.SellerPortal.Service.Store;

namespace Nesoft.SellerPortal.AgentAllProductForSeller
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化商家代理商品的数据,商家编号在appsetting的配置中
            //所有的代理级别都是原厂
            //init c3
            Console.WriteLine("init c3...");
            List<Nesoft.SellerPortal.Entity.Product.CategoryInfo> c1 = ProductMaintainService.GetAllCategory1List();
            List<Nesoft.SellerPortal.Entity.Product.CategoryInfo> c2 = new List<Nesoft.SellerPortal.Entity.Product.CategoryInfo>();
            List<Nesoft.SellerPortal.Entity.Product.CategoryInfo> c3 = new List<Nesoft.SellerPortal.Entity.Product.CategoryInfo>();
            c1.ForEach(p => c2.AddRange(ProductMaintainService.GetAllCategory2List(p.SysNo)));
            c2.ForEach(p => c3.AddRange(ProductMaintainService.GetAllCategory3List(p.SysNo)));
            Console.WriteLine("C3 total is : {0}", c3.Count);

            //init brand

            Console.WriteLine("init brand...");
            BrandQueryFilter filter = new BrandQueryFilter();
            filter.Status = ValidStatus.Active;
            filter.PageIndex = 0;
            filter.PageSize = 100000;

            var brands = CommonService.QueryBrandList(filter);
            Console.WriteLine("brand total is : {0}", brands.ResultList.Count);

            brands.ResultList.ForEach(b =>
            {
                c3.ForEach(c =>
                {
                    VendorAgentInfo agent = new VendorAgentInfo();
                    agent.C3SysNo = c.SysNo;
                    agent.C3Name = c.CategoryName;
                    //agent.C2SysNo = 10;
                    agent.AgentLevel = "原厂";
                    agent.BrandInfo = new BrandInfo
                        {
                            SysNo = b.SysNo,
                            BrandNameLocal = b.BrandNameLocal
                        };
                    agent.Status = VendorAgentStatus.Draft;
                    agent.CompanyCode = "8601";
                    
                    AppendToSeller(new List<VendorAgentInfo> { agent });
                });
            });

            Console.ReadKey();

        }

        static void AppendToSeller(List<VendorAgentInfo> agentsInfoes)
        {
            var sellerSysNoes = ConfigurationManager.AppSettings["SellerSysNoes"].Split(new char[] { ',' }).ToList();
            sellerSysNoes.ForEach(p =>
            {
                int sellerSysNo;
                if (int.TryParse(p, out sellerSysNo))
                {
                    Console.WriteLine("SellerSysNo:{0},Brand[{1}]:{2},c3[{3}]:{4}", 
                        p,
                        agentsInfoes[0].BrandInfo.SysNo,
                        agentsInfoes[0].BrandInfo.BrandNameLocal,
                        agentsInfoes[0].C3SysNo,
                        agentsInfoes[0].C3Name);
                    try
                    {
                        StoreService.SaveStoreAgentProduct(agentsInfoes, sellerSysNo, "System", true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }
    }
}
