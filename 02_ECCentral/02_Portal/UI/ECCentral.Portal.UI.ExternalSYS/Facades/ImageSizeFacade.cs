using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class ImageSizeFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public ImageSizeFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public ImageSizeFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 获取所有尺寸
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetAllImageSize(int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAllImageSizeUrl = "ExternalSYSService/ImageSize/GetAllImageSize";
            ImageSizeQueryFilter query = new ImageSizeQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo() 
                {
                    PageIndex=PageIndex,
                    PageSize=PageSize,
                    SortBy=SortField
                }
            };
            restClient.QueryDynamicData(GetAllImageSizeUrl, query, callback);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateImageSize(ImageSizeVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string CreateImageSizeUrl = "ExternalSYSService/ImageSize/CreateImageSize";
            ImageSizeInfo info = new ImageSizeInfo() 
            {
                ImageHeight=Convert.ToInt32(model.ImageHeight),
                ImageWidth=Convert.ToInt32(model.ImageWidth)
            };
            restClient.Create(CreateImageSizeUrl, info, callback);
         }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void DeleteImageSize(int SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string DeleteImageSizeUrl = "ExternalSYSService/ImageSize/DeleteImageSize";
            restClient.Delete(DeleteImageSizeUrl, SysNo, callback);
        }
    }
}
