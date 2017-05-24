using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    [EntityMapper("Table_1")]
    public class EditorModel
    {
        // *** 如果給予Property PrimaryKeyAttribute，
        //     則在UPDATE的時候會略過該Property對應的Column不進行資料異動 ***
        [PrimaryKey]
        [ColumnMapper("Column_1")]
        public string Property_1 { get; set; }

        // *** 預設情況下，如果該Property Value是NULL，
        //     則在INSER/UPDATE時會略過不處理 ***
        [ColumnMapper("Column_2")]
        public string Property_2 { get; set; }

        // *** 如果指定該Property對應的欄位是Nullable，
        //     則當該Property Value是NULL的情況下時，INSER/UPDATE還是會新增/異動資料(為NULL) ***
        [Nullable]
        [ColumnMapper("Column_3")]
        public string Property_3 { get; set; }

        // *** 如果有指定資料來源，設定字串為{Column}:{Data Source}
        //     則在每次的INSER/UPDATE都會忽略Property Value，直接從指定DataSource取得資料，
        //     新增/異動對應的欄位 ***
        [ColumnMapper("Column_4 : DataSource")]
        public DateTime? Property_4 { get; set; }

        [ReadOnly]
        [ColumnMapper("Column_5")]
        public string Property_5 { get; set; }

        [ColumnMapper("Column_6", "Table_2")]
        public string Property_6 { get; set; }
    }
}
