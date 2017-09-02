using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Product;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Utility;

namespace ECommerce.Service.Product
{
    public class ProductService
    {
        public static QueryResult<ProductInfo> QueryProductList(ProductListQueryFilter queryFilter)
        {
            return ProductDA.QueryProductList(queryFilter);
        }

        /// <summary>
        /// 分页查询商品
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ProductCommonInfo> QueryCommonProduct(ProducCommonQueryFilter queryFilter)
        {
            if (queryFilter == null || string.IsNullOrEmpty(queryFilter.VendorSysNo))
            {
                throw new BusinessException("商家编号不能为空");
            }

            return ProductDA.QueryCommonProduct(queryFilter);
        }
        /// <summary>
        /// 根据供应商编号查询品牌
        /// </summary>
        /// <returns></returns>
        public static List<BrandInfo> GetBrandByVendorSysNo(int vendorSysNo)
        {
            return ProductDA.GetBrandByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 分页查询商品
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ProductQueryInfo> QueryProduct(ProductQueryFilter queryFilter)
        {
            if (queryFilter == null || string.IsNullOrEmpty(queryFilter.VendorSysNo))
            {
                throw new BusinessException("商家编号不能为空");
            }
            return ProductDA.QueryProduct(queryFilter);
        }

        /// <summary>
        /// 使用商家编号及ProductID查询商品
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="ProductID">商品编号</param>
        /// <returns></returns>
        public static ProductQueryInfo GetProduct(int sellerSysNo, string productID)
        {
            ProductQueryFilter filter = new ProductQueryFilter();
            filter.PageIndex = 0;
            filter.PageSize = int.MaxValue;
            filter.VendorSysNo = sellerSysNo.ToString();
            filter.ProductID = productID;
            var result = ProductDA.QueryProduct(filter);
            if (result.ResultList.Count == 1)
            {
                return result.ResultList[0];
            }
            else if (result.ResultList.Count > 1)
            {
                string msg = string.Format("商品{0}存在重复记录，请确认。", productID);
                throw new BusinessException(msg);
            }
            string notFoundMsg = string.Format("商品{0}不存在。", productID);
            throw new BusinessException(notFoundMsg);
        }


        public static ProductQueryInfo GetProductByID(string productID)
        {
            ProductQueryFilter filter = new ProductQueryFilter();
            filter.PageIndex = 0;
            filter.PageSize = int.MaxValue;
            filter.VendorSysNo = null;
            filter.ProductID = productID;
            var result = ProductDA.QueryProduct(filter);
            if (result.ResultList.Count == 1)
            {
                return result.ResultList[0];
            }
            else if (result.ResultList.Count > 1)
            {
                string msg = string.Format("商品{0}存在重复记录，请确认。", productID);
                throw new BusinessException(msg);
            }
            string notFoundMsg = string.Format("商品{0}不存在。", productID);
            throw new BusinessException(notFoundMsg);
        }
        /// <summary>
        /// 使用商家编号及ProductSysNo查询商品
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductQueryInfo GetProductBySysNo(int productSysNo)
        {
            ProductQueryFilter filter = new ProductQueryFilter();
            filter.PageIndex = 0;
            filter.PageSize = int.MaxValue;
            filter.ProductSysNo = productSysNo;
            var result = ProductDA.QueryProduct(filter);
            if (result.ResultList.Count == 1)
            {
                return result.ResultList[0];
            }
            else if (result.ResultList.Count > 1)
            {
                string msg = string.Format("商品#{0}存在重复记录，请确认。", productSysNo);
                throw new BusinessException(msg);
            }
            string notFoundMsg = string.Format("商品#{0}不存在。", productSysNo);
            throw new BusinessException(notFoundMsg);
        }

        /// <summary>
        /// 使用商家编号及ProductSysNo查询商品
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductQueryInfo GetProductBySysNo(int sellerSysNo, int productSysNo)
        {
            ProductQueryFilter filter = new ProductQueryFilter();
            filter.PageIndex = 0;
            filter.PageSize = int.MaxValue;
            filter.VendorSysNo = sellerSysNo.ToString();
            filter.ProductSysNo = productSysNo;
            var result = ProductDA.QueryProduct(filter);
            if (result.ResultList.Count == 1)
            {
                return result.ResultList[0];
            }
            else if (result.ResultList.Count > 1)
            {
                string msg = string.Format("商品#{0}存在重复记录，请确认。", productSysNo);
                throw new BusinessException(msg);
            }
            string notFoundMsg = string.Format("商品#{0}不存在。", productSysNo);
            throw new BusinessException(notFoundMsg);
        }

        /// <summary>
        /// 获取TreeInfo
        /// </summary>
        /// <param name="selectedSysNo">选中的节点SysNo</param>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static List<TreeInfo> GetTreeInfo(int selectedSysNo, int sellerSysNo)
        {
            List<TreeInfo> treeList = new List<TreeInfo>();
            List<FrontProductCategoryInfo> list = ProductDA.GetFrontProductCategory(sellerSysNo);
            if (list != null && list.Count > 0)
            {
                List<FrontProductCategoryInfo> firstList = list.FindAll(item => string.IsNullOrEmpty(item.ParentCategoryCode));
                if (firstList != null && firstList.Count > 0)
                {
                    foreach (var item in firstList)
                    {
                        TreeInfo treeInfo = MappingTreeInfo(item, selectedSysNo, list);
                        treeList.Add(treeInfo);
                        SetChildTreeInfo(list, treeInfo, item.CategoryCode, selectedSysNo);
                    }
                }
            }

            return treeList;
        }

        /// <summary>
        /// 根据SysNo获取商家前台类别
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static FrontProductCategoryInfo GetFrontProductCategoryBySysNo(int sysNo, int sellerSysNo)
        {
            return ProductMaintainDA.GetFrontProductCategoryBySysNo(sellerSysNo, sysNo);
        }

        /// <summary>
        /// 获取当前商家下的所有分类信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategoryByVendorSysNo(int vendorSysNo)
        {
            return ProductDA.GetFrontProductCategory(vendorSysNo);
        }

        /// <summary>
        /// 保存前台类别
        /// </summary>
        /// <param name="info"></param>
        public static FrontProductCategoryInfo SaveFrontProductCategory(FrontProductCategoryInfo info)
        {
            if (info != null)
            {
                #region 验证
                if (!info.SellerSysNo.HasValue)
                {
                    throw new BusinessException("商家编号不能为空");
                }
                if (string.IsNullOrEmpty(info.CategoryName))
                {
                    throw new BusinessException("名称不能为空");
                }
                if (info.FPCLinkUrlMode == Enums.FPCLinkUrlModeType.Define && string.IsNullOrEmpty(info.FPCLinkUrl))
                {
                    throw new BusinessException("链接地址不能为空");
                }
                #endregion

                if (string.IsNullOrEmpty(info.CategoryCode) && string.IsNullOrEmpty(info.ParentCategoryCode))//新增Root节点
                {
                    info.IsLeaf = Enums.CommonYesOrNo.No;
                    return ProductDA.InsertFrontProductCategoryRoot(info);
                }
                else if (string.IsNullOrEmpty(info.CategoryCode) && !string.IsNullOrEmpty(info.ParentCategoryCode))//新增子节点
                {
                    info.IsLeaf = Enums.CommonYesOrNo.Yes;
                    return ProductDA.InsertFrontProductCategory(info);
                }
                else//编辑
                {
                    return ProductDA.UpdateFrontProductCategory(info);
                }
            }
            else
            {
                throw new BusinessException("不能添加空类");
            }
        }

        /// <summary>
        /// 设置子节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="treeInfo"></param>
        /// <param name="parentCategoryCode">父节点CategoryCode</param>
        /// <param name="selectedSysNo">选中的节点SysNo</param>
        private static void SetChildTreeInfo(List<FrontProductCategoryInfo> list, TreeInfo treeInfo, string parentCategoryCode, int selectedSysNo)
        {
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(parentCategoryCode))
            {
                List<FrontProductCategoryInfo> childList = list.FindAll(child => child.ParentCategoryCode == parentCategoryCode);
                if (childList != null && childList.Count > 0)
                {
                    List<TreeInfo> treeList = new List<TreeInfo>();
                    foreach (var child in childList)
                    {
                        TreeInfo tree = MappingTreeInfo(child, selectedSysNo, list);
                        treeList.Add(tree);
                        SetChildTreeInfo(list, tree, child.CategoryCode, selectedSysNo);
                    }
                    treeInfo.children = treeList;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info">前台类别</param>
        /// <param name="selectedSysNo">选中的节点SysNo</param>
        /// <param name="list">所有类别</param>
        /// <returns></returns>
        private static TreeInfo MappingTreeInfo(FrontProductCategoryInfo info, int selectedSysNo, List<FrontProductCategoryInfo> list)
        {
            TreeInfo treeInfo = new TreeInfo();
            treeInfo.id = info.SysNo;
            if (info.Status == Enums.CommonStatus.Actived)
            {
                treeInfo.text = info.CategoryName;
            }
            else
            {
                treeInfo.text = info.CategoryName+"(*)";
            }
            treeInfo.state = new TreeState();
            if (info.SysNo == selectedSysNo)
            {
                treeInfo.state.selected = true;
            }
            else
            {
                treeInfo.state.selected = false;
            }

            if (list.Exists(item => item.ParentCategoryCode == info.CategoryCode))
            {
                treeInfo.type = "default";
                treeInfo.icon = "fa fa-folder icon-state-warning icon-lg";
                treeInfo.state.opened = true;
            }
            else
            {
                treeInfo.type = "file";
                treeInfo.icon = "fa fa-file icon-state-warning";
                treeInfo.state.opened = false;
            }

            return treeInfo;
        }

        #region [WMS API接口调用相关方法:]
        public static int SaveProductInspectionInfo(ProductInspectionInfo info)
        {
            return ProductDA.SaveProductInspectionInfo(info);
        }

        public static bool ExistsProductInspectionInfo(int productSysNo)
        {
            return ProductDA.ExistsProductInspectionInfo(productSysNo);
        }
        #endregion
    }
}
