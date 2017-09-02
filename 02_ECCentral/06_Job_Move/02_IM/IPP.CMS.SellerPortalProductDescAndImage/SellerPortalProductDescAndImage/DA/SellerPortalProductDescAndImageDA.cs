using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.DA
{
    public class SellerPortalProductDescAndImageDA
    {
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static string LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
        public static string InUser = ConfigurationManager.AppSettings["InUser"];

        /// <summary>
        /// 获取sender portal的产品图片列表
        /// </summary>
        /// <param name="commonSku"></param>
        /// <param name="productGroupSysno"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ItemVendorProductRequestFileEntity> GetSellerPortalProductRequestImageList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalProductRequestImageList");

            return cmd.ExecuteEntityList<ItemVendorProductRequestFileEntity>();
        }

        public static ItemVendorProductRequestEntity GetSellerPortalProductRequestBySysNo(int sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalProductRequestBySysNo");
            cmd.SetParameterValue("@SysNo", sysno);
            return cmd.ExecuteEntity<ItemVendorProductRequestEntity>();
        }

        public static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["EmailTo"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["EmailCC"]);
            string MailSubject = DateTime.Now + " SenderPicNewProduct JOB 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);

            command.ExecuteNonQuery();
        }

        public static void SendMailIMGProcFail(string mailSubject,string mailbody)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["ProcFail_EmailTo"]);

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", "");
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailbody);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// ProductRequest_Files表count>5的list
        /// </summary>
        /// <returns></returns>
        public static List<ItemVendorProductRequestFileEntity> GetExceedFiveCountList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetExceedFiveCountList");

            return cmd.ExecuteEntityList<ItemVendorProductRequestFileEntity>();
        }

        /// <summary>
        /// count>5，删除ProductRequest_Files表记录，同时在表ProductRequest_ProcessLog增加log日志
        /// </summary>
        /// <param name="sysNo"></param>
        public static void DeleteSellerPortalProductRequestImageFiles(int sysNo)
        {

            DataCommand command = DataCommandManager.GetDataCommand("DeleteSellerPortalProductRequestImageFiles");
            command.SetParameterValue("@SysNo", sysNo);

            command.ExecuteNonQuery();
        }

     
        /// <summary>
        /// 获取三个状态都 BasicInfoStatus = "F",FileStatus = "F",ExInfoStatus = "F"
        /// </summary>
        /// <returns></returns>
        public static List<ItemVendorProductRequestFileEntity> GetSellerPortalThreeFStatusList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalThreeFStatusList");

            return cmd.ExecuteEntityList<ItemVendorProductRequestFileEntity>();
        }

        /// <summary>
        /// 处理三个状态都 BasicInfoStatus = "F",FileStatus = "F",ExInfoStatus = "F"，则Status = "F"
        /// </summary>
        /// <param name="ProductRequestSysno"></param>
        /// <param name="status"></param>
        public static void UpdateSellerPortalProductRequestStatus(int ProductRequestSysno,string status)
        {

            DataCommand command = DataCommandManager.GetDataCommand("UpdateSellerPortalProductRequestStatus");
            command.SetParameterValue("@SysNo", ProductRequestSysno);
            command.SetParameterValue("@Status", status);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询图片每个ProductRequestSysno都为"F"的list
        /// </summary>
        /// <returns></returns>
        public static List<ItemVendorProductRequestFileEntity> GetSellerPortalImageFFilesFStatusList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalImageFFilesFStatusList");

            return cmd.ExecuteEntityList<ItemVendorProductRequestFileEntity>();
        }

        /// <summary>
        /// 修改FileStatus状态
        /// </summary>
        /// <param name="ProductRequestSysno"></param>
        public static void UpdateSellerPortalProductRequestFileStatus(int ProductRequestSysno)
        {

            DataCommand command = DataCommandManager.GetDataCommand("UpdateSellerPortalProductRequestFileStatus");
            command.SetParameterValue("@SysNo", ProductRequestSysno);            

            command.ExecuteNonQuery();
        }

        public static int InsertProductRequestImageProcessLog(string imageUrl, string imageName, int productRequestSysNo, string status,string memo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductRequestImageProcessLog");

            cmd.SetParameterValue("@ImageUrl", imageUrl);
            cmd.SetParameterValue("@ImageName", imageName);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@Memo", memo);
            cmd.SetParameterValue("@ProductRequestSysNo", productRequestSysNo);

            return cmd.ExecuteNonQuery();
        }

        public static void ApproveProductRequest_Ex(int sysno, string productdesclong)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ApproveProductRequest_Ex");

            cmd.SetParameterValue("@SysNo", sysno);
            cmd.SetParameterValue("@ProductDescLong", productdesclong);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取ExInfoStatus为P的，详细信息描述列表
        /// </summary>
        /// <returns></returns>
        public static List<ItemVendorProductRequestEntity> GetSellerPortalProductRequestDescLongList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalProductRequestDescLongList");

            return cmd.ExecuteEntityList<ItemVendorProductRequestEntity>();
        }

        public static List<ProductRequestImage> GetProductRequestImageListLog(int productrequestsysno,string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductRequestImageListLog");
            command.SetParameterValue("@ProductRequestSysNo", productrequestsysno);
            command.SetParameterValue("@Status", status);
            return command.ExecuteEntityList<ProductRequestImage>();
        }

        public static List<ItemVendorProductRequestFileEntity> GetProductRequestImageList(int productrequestsysno, string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductRequestImageList");
            command.SetParameterValue("@ProductRequestSysNo", productrequestsysno);
            command.SetParameterValue("@Status", status);
            return command.ExecuteEntityList<ItemVendorProductRequestFileEntity>();
        }

        /// <summary>
        /// 更新CommonInfo默认图片信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="fileName"></param>
        public static void UpdateProductRequestImageProcessLogStatus(ProductRequestImage entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductRequestImageProcessLogStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Memo", entity.Memo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// ProductRequestImageProcessLog图片处理次数Count + 1
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int SetProductRequestImageProcessLogCount(ProductRequestImage entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetProductRequestImageProcessLogCount");

            cmd.SetParameterValue("@SysNo", entity.SysNo);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询ProductRequestImageProcessLog每个ProductRequestSysno都为"F"的list
        /// </summary>
        /// <returns></returns>
        public static List<ProductRequestImage> GetSellerPortalProductLongDescFStatusList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalProductLongDescFStatusList");

            return cmd.ExecuteEntityList<ProductRequestImage>();
        }

        /// <summary>
        /// 修改表ProductRequest，字段ExInfoStatus状态
        /// </summary>
        /// <param name="ProductRequestSysno"></param>
        public static void UpdateSellerPortalProductRequestExInfoStatus(int productRequestSysNo)
        {

            DataCommand command = DataCommandManager.GetDataCommand("UpdateSellerPortalProductRequestExInfoStatus");
            command.SetParameterValue("@SysNo", productRequestSysNo);

            command.ExecuteNonQuery();
        }

        public static void CallExternalSP(int productRequstSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CallExternalSP");

            cmd.SetParameterValue("@ProductRequstSysNo", productRequstSysNo);
            cmd.ExecuteNonQuery();
          
        }

        /// <summary>
        ///  ProductRequest_Ex处理次数Count + 1
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int SetProductRequestExCount(int sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetProductRequestExCount");

            cmd.SetParameterValue("@SysNo", sysno);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 增加[OverseaContentManagement].[dbo].[ProductRequest_ProcessLog]
        /// </summary>
        /// <param name="ProductRequestSysno"></param>
        public static void InsertProductDescProductRequest_ProcessLog(int productRequestSysNo)
        {

            DataCommand command = DataCommandManager.GetDataCommand("InsertProductDescProductRequest_ProcessLog");
            command.SetParameterValue("@SysNo", productRequestSysNo);

            command.ExecuteNonQuery();
        }

        //******************************  图片处理Strat  ******************************
        public static ProductGroupInfoEntity GetProductGroupInfoByCommonSku(string commonSku, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupInfoByCommonSku");
            cmd.SetParameterValue("@CommonSku", commonSku);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            ProductGroupInfoEntity result = cmd.ExecuteEntity<ProductGroupInfoEntity>();
            return result;
        }

        /// <summary>
        /// 创建图片与商品组关联信息
        /// </summary>
        /// <param name="productImageEntity"></param>
        /// <returns></returns>
        public static ProductGroupInfoImageEntity CreateProductGroupInfoImage(ProductGroupInfoImageEntity productImageEntity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductGroupInfoImage");
            cmd.SetParameterValue("@ProductGroupSysno", productImageEntity.ProductGroupSysno);
            cmd.SetParameterValue("@ResourceUrl", productImageEntity.ResourceUrl);
            cmd.SetParameterValue("@Type", productImageEntity.Type);
            cmd.SetParameterValue("@Status", productImageEntity.Status);
            cmd.SetParameterValue("@InUser", productImageEntity.Header.OperationUser.FullName);
            cmd.SetParameterValue("@CompanyCode", productImageEntity.Header.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", productImageEntity.Header.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", LanguageCode);
            cmd.ExecuteNonQuery();
            productImageEntity.SysNo = (int)cmd.GetParameterValue("@SysNo");

            return productImageEntity;
        }

        /// <summary>
        /// 创建商品与图片关联信息
        /// </summary>
        /// <param name="productImageEntity"></param>
        public static void CreateProductCommonInfoImage(ProductGroupInfoImageEntity productImageEntity, string commonSKUNumber)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductCommonInfoImage");
            cmd.SetParameterValue("@ProductGroupImageSysno", productImageEntity.SysNo);
            cmd.SetParameterValue("@ProductGroupSysNo", productImageEntity.ProductGroupSysno);
            cmd.SetParameterValue("@CommonSKUNumber", commonSKUNumber);
            cmd.SetParameterValue("@InUser", productImageEntity.Header.OperationUser.FullName);
            cmd.SetParameterValue("@CompanyCode", productImageEntity.Header.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", productImageEntity.Header.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", LanguageCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 插入图片修改队列
        /// </summary>
        /// <param name="productGroupInfoSysNo"></param>
        /// <param name="header"></param>
        public static void InsertImageLogByGroupInfo(int productGroupInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductImageProcessLog");
            cmd.SetParameterValue("@ProductGroupInfoSysNo", productGroupInfoSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 设置商品图片状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int SetProductRequestImageStatus(ItemVendorProductRequestFileEntity entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetProductRequestImageStatus");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 图片处理次数Count + 1
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int SetProductRequestImageCount(ItemVendorProductRequestFileEntity entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetProductRequestImageCount");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 取商品图片数
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <returns></returns>
        public static int GetCommonInfoPicNumber(int productCommonInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCommonInfoPicNumber");
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            return cmd.ExecuteScalar<int>();
        }

        //******************************  图片处理End  ******************************

    }
}
