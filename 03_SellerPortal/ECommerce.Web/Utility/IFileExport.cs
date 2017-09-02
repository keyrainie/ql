using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.Web.Utility
{
    public interface IFileExport
    {
        byte[] CreateFile(List<DataTable> data, List<List<ColumnData>> columnSetting, List<TextInfo> textInfoSetting, out string fileName, string FileTitle);
    }
}
