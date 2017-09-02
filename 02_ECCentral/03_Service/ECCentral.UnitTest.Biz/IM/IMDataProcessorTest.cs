using System;
using System.IO;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.IM
{
    [TestClass]
    public class IMDataProcessorTest
    {
        [TestMethod]
        public void Test_IM()
        {
            //ProductGroupAppService service = new ProductGroupAppService();
            //ProductGroup group = service.GetProductGroup(52823);


        }

        [TestMethod]
        public void Test_IM_QueryCategoryProperty()
        {
            //ICategoryPropertyQueryDA d = ObjectFactory<ICategoryPropertyQueryDA>.Instance;
            //d.QueryCategoryProperty(new CategoryPropertyQueryFilter()
            //{
            //    CategorySysNo = 10,
            //    PagingInfo = new PagingInfo
            //    {
            //        PageIndex = 1,
            //        PageSize =10,
            //    }
            //});
        }

        [TestMethod]
        public void Test_IM_QueryAccessory()
        {
            //int t;
            //IAccessoryQueryDA d = ObjectFactory<IAccessoryQueryDA>.Instance;
            //d.QueryAccessory(new AccessoryQueryFilter
            //{
            //    AccessoryName = "44",
            //    PagingInfo = new PagingInfo
            //    {
            //        PageIndex = 1,
            //        PageSize = 10,
            //    }
            //}, out t);
        }

        [TestMethod]
        public void Test_IM_ProductCreateDataFeed()
        {
            //try
            //{
            //    XmlSerializer xml = new XmlSerializer(typeof(ProductInitData));

            //    ProductInitData data = new ProductInitData();
            //    StreamReader writer = new StreamReader(@"d:\7.txt", Encoding.UTF8);

            //    data = (ProductInitData)xml.Deserialize(writer);

            //    ProductCreateDataFeedAppService service = new ProductCreateDataFeedAppService();
            //    service.CreateDataFeedProduct(data);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

        }

        [TestMethod]
        public void Test_IM_GetPriceChangeLogInfoByProductSysNo()
        {
            //try
            //{

            //    BizInteractAppService service = new BizInteractAppService();
            //    service.GetPriceChangeLogInfoByProductSysNo(83013, new DateTime(2010, 1, 1), new DateTime(2011, 1, 1));
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

        }

        [TestMethod]
        public void Test_IM_GetCategorySettingBySysNo()
        {
            try
            {

                CategorySettingProcessor service = new CategorySettingProcessor();
                service.GetCategorySettingBySysNo(709, 15775);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
