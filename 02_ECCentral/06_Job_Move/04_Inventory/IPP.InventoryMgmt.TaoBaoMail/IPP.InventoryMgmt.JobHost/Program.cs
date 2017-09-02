using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using System.Xml.Serialization;
using System.IO;
using IPP.InventoryMgmt.JobV31.Common;
using IPP.InventoryMgmt.JobV31.Biz;
using IPP.InventoryMgmt.JobV31.Provider;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.InventoryMgmt.JobHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = string.Empty;
            Console.WriteLine("请选择操作：1--运行job，0--退出job");
            input = Console.ReadLine();
            while (input != "0")
            {
                JobAutoRun job = new JobAutoRun();
                JobContext context = new JobContext();
                job.Run(context);
                Console.WriteLine("请选择操作：1--运行job，0--退出job");
                input = Console.ReadLine();
            }
        }

        private static void Test()
        {
            TaobaoResponse response = new TaobaoResponse();
            TaobaoProductCollection collection = new TaobaoProductCollection();
            collection.ProductCollection = new List<TaobaoProduct>();
            collection.ProductCollection.Add(new TaobaoProduct
            {
                ProductID = "123456",
                Qty = 15,
                NumberID = "456789"
            });
            response.Response = collection;
            response.IsList = true;

            XmlSerializer ser = new XmlSerializer(typeof(TaobaoResponse));
            MemoryStream stream = new MemoryStream();
            ser.Serialize(stream, response);
            string result = Encoding.UTF8.GetString(stream.GetBuffer());
            Console.WriteLine(result);

            //Encoding encoding = CommonConst.taobao_response_encoding;
            //Console.WriteLine(encoding.ToString());

            //TaobaoResponse taobaoResponse = XmlSerializerHelper.Deserializer<TaobaoResponse>(result, CommonConst.taobao_response_encoding);
            List<TaobaoProduct> list = SynInventoryQtyBiz.QuerySynProduct();
            Console.WriteLine(list.Count);
            Console.ReadLine();
        }
        private static string temp1 = "<?xml version=\"1.0\"?>"+
"<items_inventory_get_response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instan"+
"ce\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" list=\"true\">"+
"  <items>"+
"    <item>"+
"      <num_iid>456789</num_iid>"+
"      <outer_id>123456</outer_id>"+
"      <num>15</num>"+
"    </item>"+
" </items>"+
"  <total_results>0</total_results>"+
"</items_inventory_get_response>";

        private static string temp = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>"+
"<items_inventory_get_response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instan"+
"ce\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"+
"	<items list=\"true\">"+
"		<item>"+
"			<num>166</num>"+
"			<num_iid>13334455990</num_iid>"+
"			<outer_id>A18-049-2M8</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>91</num>"+
"			<num_iid>13922888689</num_iid>"+
"			<outer_id>99-c01-732</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>11</num>"+
"			<num_iid>13305223703</num_iid>"+
"			<outer_id>26-c10-572</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>20</num>"+
"			<num_iid>13905152506</num_iid>"+
"			<outer_id>90-c12-900</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>1</num>"+
"			<num_iid>13314475511</num_iid>"+
"			<outer_id>A24-053-2V6</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>1739</num>"+
"			<num_iid>13034525753</num_iid>"+
"			<outer_id>21-c21-026</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>116</num>"+
"			<num_iid>13033533627</num_iid>"+
"			<outer_id>75-c13-306</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>10</num>"+
"			<num_iid>13317503393</num_iid>"+
"			<outer_id>21-c02-160</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>3</num>"+
"			<num_iid>13316047100</num_iid>"+
"			<outer_id>A41-09Z-1RU</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>12</num>"+
"			<num_iid>13031482175</num_iid>"+
"			<outer_id>A42-026-1DL</outer_id>"+
"		</item>"+
"		<item>"+
"			<num>13</num>"+
"			<num_iid>13902136467</num_iid>"+
"			<outer_id>A22-0A4-4CW</outer_id>"+
"		</item>"+		
"	</items>"+
"	<total_results>105</total_results>"+
"</items_inventory_get_response>";
    }
}
