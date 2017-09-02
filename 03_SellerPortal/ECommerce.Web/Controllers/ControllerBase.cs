using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Entity.Common;
using System.Data;
using System.Text;
using ECommerce.Entity;


namespace ECommerce.Web.Controllers
{
    public class ControllerBase : Controller
    {
        public UserAuthVM CurrUser;



        public ControllerBase()
        {
            this.ViewBag.IsUserLogin = UserAuthHelper.HasLogin();
            //加载登录信息
            var userInfo = UserAuthHelper.GetCurrentUser();
            if (userInfo != null)
            {
                this.ViewBag.CurrUser = userInfo;
                CurrUser = userInfo;
            }
        }

        /// <summary>
        /// 构建Ajax请求的错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected object BuildAjaxErrorObject(string msg)
        {
            return new
            {
                error = true,
                message = msg
            };
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.View(actionName).ExecuteResult(this.ControllerContext);
            //base.HandleUnknownAction(actionName);
        }

        protected T BuildQueryFilterEntity<T>() where T : class
        {
            return BuildQueryFilterEntity<T>(null);

        }

        protected T BuildQueryFilterEntity<T>(Action<T> manualMapping) where T : class
        {
            object t = Activator.CreateInstance(typeof(T));
            if (!string.IsNullOrEmpty(this.Request["queryfilter[]"]))
            {
                t = SerializationUtility.JsonDeserialize2<T>(this.Request["queryfilter[]"]);
            }
            if (t is QueryFilter)
            {
                //每页显示条数:
                int pageSize = Convert.ToInt32(Request["length"]);
                //当前页码:
                int pageIndex = Convert.ToInt32(Request["start"]) % pageSize == 0 ? Convert.ToInt32(Request["start"]) / pageSize : Convert.ToInt32(Request["start"]) / pageSize + 1;
                //排序:
                string sortBy = null;
                if (!string.IsNullOrEmpty(Request["order[0][column]"]))
                {
                    string colIndex = Request["order[0][column]"];
                    string sortByField = string.IsNullOrEmpty(Request[string.Format("columns[{0}][name]", colIndex)]) ? Request[string.Format("columns[{0}][data]", colIndex)] : Request[string.Format("columns[{0}][name]", colIndex)];
                    string sortDir = Request["order[0][dir]"];
                    sortBy = string.Format("{0} {1}", sortByField, sortDir.ToUpper());
                }
                ((QueryFilter)t).PageSize = pageSize;
                ((QueryFilter)t).PageIndex = pageIndex;
                ((QueryFilter)t).SortFields = sortBy;
            }

            if (manualMapping != null)
            {
                manualMapping((T)t);
            }
            return (T)t;

        }

        protected string GetQueryFilterParam(string paramName)
        {
            return Request[string.Format("{0}[]", paramName)];
        }

        protected ContentResult AjaxGridJson(QueryResult result)
        {
            string data = SerializeDataTable(result.ResultList);
            data = data.Replace("\r\n", "<br/>");
            ContentResult content = Content("{\"sEcho\":" + Request["draw"] + ",\"iTotalRecords\":" + result.PageInfo.TotalCount + ",\"iTotalDisplayRecords\":" + result.PageInfo.TotalCount + ",\"aaData\":" + (string.IsNullOrEmpty(data) ? "[]" : data) + "}");
            content.ContentType = "application/json; charset=utf-8";
            return content;
        }


        private string SerializeDataTable(DataTable dt)
        {
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return string.Empty;
            }
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName.EndsWith("_ECCentral_Auto_Removed_820319"))
                    {
                        continue;
                    }
                    Type realType;
                    object v = dt.Rows[i][j];
                    realType = dt.Columns[j].DataType;
                    string realValue;
                    string typeStr;
                    if (realType.IsEnum)
                    {
                        realValue = ECommerce.Utility.EnumHelper.GetDescription((Enum)Enum.Parse(realType, v.ToString()));
                        typeStr = realType.AssemblyQualifiedName;
                    }
                    else
                    {
                        TypeCode code = Type.GetTypeCode(realType);
                        switch (code)
                        {
                            case TypeCode.Boolean:
                                realValue = v.ToString().ToLower();
                                break;
                            case TypeCode.DBNull:
                            case TypeCode.Empty:
                                realValue = string.Empty;
                                break;
                            case TypeCode.DateTime:
                                realValue = v is DBNull ? string.Empty : ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss");
                                break;
                            default:
                                realValue = v.ToString();
                                break;
                        }
                        if (code == TypeCode.Object)
                        {
                            code = TypeCode.String;
                        }
                        typeStr = code.ToString();
                    }
                    if (realType == typeof(string))
                    {
                        realValue = HttpUtility.HtmlEncode(realValue.Replace("\n",""));
                    }
                    jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + realValue.Trim().Replace(@"\", @"\\") + "\"");
                    if (j != dt.Columns.Count - 1)
                    {
                        jsonBuilder.Append(",");
                    }
                }
                if (i == dt.Rows.Count - 1)
                {
                    jsonBuilder.Append("}");
                }
                else
                {
                    jsonBuilder.Append("},");
                }

            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        protected JsonResult AjaxGridJson<T>(QueryResult<T> result) where T : class
        {
            return Json(new { sEcho = Request["draw"], iTotalRecords = result.PageInfo.TotalCount, iTotalDisplayRecords = result.PageInfo.TotalCount, aaData = result.ResultList });
        }

        protected void SetBizEntityUserInfo(EntityBase bizEntity, bool isCreate)
        {
            var user = UserAuthHelper.GetCurrentUser();
            if (isCreate)
            {
                bizEntity.InUserSysNo = user.UserSysNo;
                bizEntity.InUserName = user.UserDisplayName;
                bizEntity.InDate = DateTime.Now;
            }
            else
            {
                bizEntity.EditUserSysNo = user.UserSysNo;
                bizEntity.EditUserName = user.UserDisplayName;
                bizEntity.EditDate = DateTime.Now;
            }
            bizEntity.SellerSysNo = user.SellerSysNo;
        }
    }

    /// <summary>
    /// 需要登录验证的Controller从此基类继承
    /// </summary>
    [Auth(NeedAuth = true)]
    [UserAuthorize]
    public class SSLControllerBase : ControllerBase
    {
        public SSLControllerBase()
        {
        }
    }

    /// <summary>
    /// 无需登录验证的Controller从此基类继承
    /// </summary>
    [Auth(NeedAuth = false)]
    public class WWWControllerBase : ControllerBase
    {
    }
}




