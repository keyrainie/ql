using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.Utility
{
    public interface IFileExport
    {
        byte[] CreateFile(List<DataTable> data, List<List<ColumnData>> columnSetting, List<TextInfo> textInfoSetting, out string fileName, string FileTitle);
    }
}
