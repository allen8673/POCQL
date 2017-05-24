using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    /// <summary>
    /// Property 同時對應多個欄位的model
    /// </summary>
    [EntityMapper("")]
    public class MultiColumnModel
    {
        // *** 如果在Attribute上給予'{:Data Source:}'字串，則在INSER/UPDATE時會固定以指定的Data Source給予對應欄位值 ***
        // *** Ex: 如果設定{:GETDATE():}，則每次在INSER/UPDATE Property_1對應到的欄位時都會固定給值為GETDATE() ***
        [MultiColumnMapper("{:@DataSource:}", "Mat1_Column_1", "Mat2_Column_1", "Mat3_Column_1", "Mat4_Column_1")]
        public string Property_1 { get; set; }

        // *** 可以只針對特定的Column設定資料來源，字串格式為{Column}:{Data Source}，如此在INSER/UPDATE時，該欄位的值都會固定從指定的Data Source來 ***
        // *** Ex: 如果設定Mat1_Column_2 : GETDATE()，則每次在INSER/UPDATE Mat1_Column_2時都會忽略Property_2的值，直接給Mat1_Column_2 GETDATE() ***
        [MultiColumnMapper("Mat1_Column_2 : DataSource", "Mat2_Column_2", "Mat3_Column_2", "Mat4_Column_2")]
        public string Property_2 { get; set; }

        // !!! 上述DataSource的設定都不會在SELECT時作用，請安心服用 !!!

        [MultiColumnMapper("Mat1_Column_3", "Mat2_Column_3", "Mat3_Column_3", "Mat4_Column_3")]
        public string Property_3 { get; set; }

        [MultiColumnMapper("Mat1_Column_4", "Mat2_Column_4", "Mat3_Column_4", "Mat4_Column_4")]
        public string Property_4 { get; set; }

        [MultiColumnMapper("Mat1_Column_5", "Mat2_Column_5", "Mat3_Column_5", "Mat4_Column_5")]
        public string Property_5 { get; set; }

        [MultiColumnMapper("Mat1_Column_6", "Mat2_Column_6", "Mat3_Column_6", "Mat4_Column_6")]
        public string Property_6 { get; set; }
    }
}
