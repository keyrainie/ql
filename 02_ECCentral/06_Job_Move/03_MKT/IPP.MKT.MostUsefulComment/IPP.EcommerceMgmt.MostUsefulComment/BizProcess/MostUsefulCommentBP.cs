using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IPP.EcommerceMgmt.MostUsefulComment.DataAccess;
using IPP.EcommerceMgmt.MostUsefulComment.Entities;

namespace IPP.EcommerceMgmt.MostUsefulComment.BizProcess
{
    public class MostUsefulCommentBP
    {
        public void Process()
        {
            string companyCode = ConfigurationManager.AppSettings["CompanyCode"] ?? "8601";
            string editUser = ConfigurationManager.AppSettings["EditUser"] ?? "Job";
            int productSysNo = 0;
            int.TryParse(ConfigurationManager.AppSettings["TestProductSysNo"], out productSysNo);
            List<CommentEntity> commentList = MostUsefulCommentDA.GetCommentList(companyCode);
            CommentEntity testEntity = commentList.FirstOrDefault(p => p.ProductSysNo == productSysNo);
            if (testEntity != null)
            {
                MostUsefulCommentDA.UpdateProductReview(testEntity.ProductSysNo, companyCode, editUser);
                return;
            }
            foreach (CommentEntity entity in commentList)
            {
                MostUsefulCommentDA.UpdateProductReview(entity.ProductSysNo, companyCode, editUser);
            }
        }
    }
}
