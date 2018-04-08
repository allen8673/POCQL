using Microsoft.VisualStudio.TestTools.UnitTesting;
using POCQL.DAO.Model;
using POCQL.Model;
using POCQL.Model.MapAttribute;
using POCQL.Test.Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example
{
    /// <summary>
    /// SQLTool INSERT/UPDATE用法
    /// </summary>
    [TestClass]
    public class EditorExample
    {

        [TestMethod]
        public void InsertExp()
        {
            #region 範例資料
            EditorModel edModel = new EditorModel
            {
                // *** 設定PrimaryKey的Property在INSERT時不會作用，程式還是會處理 ***
                Property_1 = "value1",
                // *** 這裡讓Property_2 = NULL，讓程式自己略過它不處理 ***
                Property_2 = null,
                // *** 這裡也讓Property_3 = NULL，但是因為已經設定成Nullable，所以程式還是會處理 ***
                Property_3 = null,
                Property_4 = null,
                Property_5 = "value2",
                Property_6 = "value3"
            };

            // *** 額外再加一個MultiColumnModel，測試MatcheColumns Method ***
            MultiColumnModel mcModel = new MultiColumnModel
            {
                // *** Property_1有設定DataSource，所以不管Property Value為何，
                //     程式都只會從DataSource取得資料 ***
                Property_1 = "value1",
                // *** Property_2只有'Mat1_Column_2'設定DataSource，
                //     所以當MatcheColumns Method指定'Mat1_Column_2'時，
                //     資料一定都是從DataSource來，其他欄位則是取決於Property Value ***
                Property_2 = "value2",
                Property_3 = "value3",
                Property_4 = null,
                Property_5 = "value4",
                Property_6 = null,
            };
            #endregion

            // Common Insert:
            SqlSet set1 = Insert.Table("Tabel")       // *** 只是單純指定Table ***
                                .Columns(edModel)     // *** 從物件決定要INSERT的欄位
                                .MatcheColumns(mcModel, "Mat1", "Mat2")   // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .Output();

            // Insert From:
            SqlSet set2 = Insert.Table("Tabel1")
                                .Columns(edModel)                           // *** 從物件決定要INSERT的欄位
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .ColumnsFrom("Tabel2",                      // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn1",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn2 : SColumn2")
                                .Where("{Condition}")
                                .Output(new { param = "value" });

            // Insert From:
            SqlSet set3 = Insert.Table("Tabel1")
                                .Columns(edModel)                           // *** 從物件決定要INSERT的欄位
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .ColumnsFrom("Tabel2 T2",                   // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn1",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn2 : SColumn2")
                                .Where("{Condition}")
                                .Output(new { param = "value" });

            // Insert From:
            SqlSet set4 = Insert.Table("Table_1")
                                .Columns(edModel)                           // *** 從物件決定要INSERT的欄位
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .ColumnsFrom("Table_2 T2",                   // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn1",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn2 : SColumn2")
                                .Where(new ConditionModel
                                {
                                    //Property_1 = "value_1",
                                    Property_2 = new IntervalVal("pre_value", "post_value"),
                                    Property_4 = DateTime.Now,
                                    Property_5 = "value_2"
                                })
                                .Output(new { param = "value" });

            SqlSet set5 = Insert.Table("Tabel1")
                                .Columns("Column_1: Data_1",
                                         "Column_2: Data_2",
                                         "Column_3: Data_3",
                                         "Column_4: Data_4")
                                .MatcheColumns(mcModel, "Mat1", "Mat2")
                                .ColumnsFrom("Tabel2 T2",
                                             "OtherColumn1",
                                             "OtherColumn2 : SColumn2")
                                .Where("{Condition}")
                                .Output(new { param = "value" });

        }

        [TestMethod]
        public void Insert_Demo_1()
        {
            string sql = Insert.Table("EMPLOYEE")
                               .Columns("NAME : '王O明'",
                                        "SEX : 'M'",
                                        "DEPT : dbo.GETDEPT()",
                                        "TITLE : dbo.GETDEF('title')")
                               .ToString();
        }

        [TestMethod]
        public void Insert_Demo_2()
        {

            Employee employee = new Employee
            {
                Name = "王O明",
                Sex = "M",
                Department = "100"
            };

            string sql = Insert.Table("EMPLOYEE")
                               .Columns(employee,                                        
                                        "TITLE : dbo.GETDEF('title')")
                               .Columns(new ValueBinder<Employee>(employee)
                               {
                                   { "DEPT_NAME : dbo.GETNAME('dept', @Department)", i=>i.Department}
                               })
                               .ToString();
        }

        [TestMethod]
        public void Insert_Demo_3()
        {
            string sql = Insert.Table("EMPLOYEE")
                               .ColumnsFrom("USER", 
                                            "NAME", "SEX",
                                            "DEPT : @DEPT", "TITLE : @TITLE",
                                            "DEPT_NAME : dbo.GETNAME('dept', @Department)")
                               .Where("ID = @id")
                               .ToString();
        }

        [TestMethod]
        public void Insert_Demo_4()
        {
            Employee employee = new Employee
            {
                Name = "王O明",
                Sex = "M",
                Department = "100"
            };

            UserInfo userinfo = new UserInfo
            {
                UserID = "123",
                UserName = "系O員",
                DeptID = "001",
                DeptName = "系統管理部"
            };

            string sql = Insert.Table("EMPLOYEE")
                               .Columns(employee)
                               .MatcheColumns(userinfo, "CRT")
                               .Where("ID = @id")
                               .ToString();
        }

        [TestMethod]
        public void Update_Demo_1()
        {
            string sql = Update.Table("EMPLOYEE")
                               .Columns("DEPT : '100'",
                                        "TITLE : 'OO專員'")
                               .Where("EMPLOYEE_NO = @no")
                               .ToString();
        }

        [TestMethod]
        public void Update_Demo_2()
        {
            Employee employee = new Employee
            {
                Department = "100",
                Title = "OO專員"
            };

            string sql = Update.Table("EMPLOYEE")
                               .Columns(employee)
                               .Where("EMPLOYEE_NO = @no")
                               .ToString();
        }

        [TestMethod]
        public void Update_Demo_3()
        {
            string sql = Update.Table("EMPLOYEE", "EPY")
                               .ColumnsFrom("USER", "USR", "USR.EMPLOYEE_NO = EPY.EMPLOYEE_NO",
                                            "NAME", "ADDRESS", "PHONE_NUMBER")
                               .Where("EPY.EMPLOYEE_NO = @no")
                               .ToString();
        }

        [TestMethod]
        public void Update_Demo_4()
        {
            string sql = Update.Table("EMPLOYEE", "EPY")
                               .ColumnsFrom("USER", "USR", "USR.EMPLOYEE_NO = EPY.EMPLOYEE_NO",
                                            "NAME")
                               .ColumnsFrom("USER_DETAIL", "USR_DTL", "USR.EMPLOYEE_NO = EPY.EMPLOYEE_NO",
                                            "ADDRESS", "PHONE_NUMBER")
                               .Where("EPY.EMPLOYEE_NO = @no")
                               .ToString();

            
        }

        [EntityMapper]
        public class UserInfo
        {
            [MultiColumnMapper("CRT_USER_ID", "MDF_USER_ID", "MAG_USER_ID")]
            public string UserID { get; set; }

            [MultiColumnMapper("CRT_USER_NAME", "MDF_USER_NAME", "MAG_USER_NAME")]
            public string UserName { get; set; }

            [MultiColumnMapper("CRT_DEPT_ID", "MDF_DEPT_ID", "MAG_DEPT_ID")]
            public string DeptID { get; set; }

            [MultiColumnMapper("CRT_DEPT_NAME", "MDF_DEPT_NAME", "MAG_DEPT_NAME")]
            public string DeptName { get; set; }
        }

        [EntityMapper("EMPLOYEE")]
        public class Employee
        {
            [PrimaryKey, ColumnMapper("EMPLOYEE_NO")]
            public string EmployeeNo { get; set; }

            [ColumnMapper("NAME")]
            public string Name { get; set; }

            [ColumnMapper("SEX")]
            public string Sex { get; set; }

            [ColumnMapper("DEPT")]
            public string Department { get; set; }

            [ColumnMapper("TITLE")]
            public string Title { get; set; }

            
        }

        [TestMethod]
        public void UpdateExp()
        {
            #region 範例資料
            EditorModel edModel = new EditorModel
            {
                // *** 這裡故意給PrimaryKey Property值，讓程式自己略過它不處理 ***
                Property_1 = "value1",
                // *** 這裡讓Property_2 = NULL，讓程式自己略過它不處理 ***
                Property_2 = null,
                // *** 這裡也讓Property_3 = NULL，但是因為已經設定成Nullable，所以程式還是會處理 ***
                Property_3 = null,
                Property_4 = null,
                Property_5 = "value2",
                Property_6 = "value3"
            };

            // *** 額外再加一個MultiColumnModel，測試MatcheColumns Method ***
            MultiColumnModel mcModel = new MultiColumnModel
            {
                // *** Property_1有設定DataSource，所以不管Property Value為何，
                //     程式都只會從DataSource取得資料 ***
                Property_1 = "value1",
                // *** Property_2只有'Mat1_Column_2'設定DataSource，
                //     所以當MatcheColumns Method指定'Mat1_Column_2'時，
                //     資料一定都是從DataSource來，其他欄位則是取決於Property Value ***
                Property_2 = "value2",
                Property_3 = "value3",
                Property_4 = null,
                Property_5 = "value4",
                Property_6 = null,
            };
            #endregion

            // Example 1:
            SqlSet set1 = Update.Columns("Table1", "T1", edModel)           // *** 利用Columns method指定Upate的Table，並且一物件決定要Update的欄位 ***
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .Columns("OtherColumn1 : OtherData1",       // *** 利用字串格式{Column}:{DataSource}指定特定欄位要異動的值 *** 
                                         "OtherColumn2 : OtherData2")
                                .ColumnsFrom("Tabel2", "T2", "{Condition_1}",  // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn3",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn4 : SColumn4")
                                .Where("{Condition_2}")
                                .Output(new { param = "value" });

            // Example 2:
            SqlSet set2 = Update.Table("Table1", "T1")  // *** 只是單純指定Table ***
                                .Columns(edModel)       // *** 到這裡的用方就跟Example 1的.Columns("Table1", "T1", edModel)效果相同 ***
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .Columns("OtherColumn1 : OtherData1",       // *** 利用字串格式{Column}:{DataSource}指定特定欄位要異動的值 *** 
                                         "OtherColumn2 : OtherData2")
                                .ColumnsFrom("Tabel2", "T2", "{Condition_1}",  // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn3",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn4 : SColumn4")
                                .Where("{Condition_2}")
                                .Output(new { param = "value" });


            // Example 3:
            SqlSet set3 = Update.Table("Table_1", "T1")  // *** 只是單純指定Table ***
                                .Columns(edModel)       // *** 到這裡的用方就跟Example 1的.Columns("Table1", "T1", edModel)效果相同 ***
                                .MatcheColumns(mcModel, "Mat1", "Mat2")     // *** 從物件中指定欄位名符合指定值的欄位值 ***
                                .Columns("OtherColumn1 : OtherData1",       // *** 利用字串格式{Column}:{DataSource}指定特定欄位要異動的值 *** 
                                         "OtherColumn2 : OtherData2")
                                .ColumnsFrom("Table_2", "T2", "{Condition_1}",  // *** 指定Column的資料來自於其他Table ***
                                             "OtherColumn3",                // *** 如果沒有指定特定來源Column，則會從來源Table取得相同名字的Column
                                             "OtherColumn4 : SColumn4")
                                .Where(new ConditionModel
                                {
                                    Property_1 = "value_1",
                                    Property_2 = new IntervalVal("pre_value", "post_value"),
                                    Property_5 = "value_2"
                                })
                                .Output(new { param = "value" });
        }

        [TestMethod]
        public void DeleteExp()
        {
            SqlSet set1 = Delete.Table("Table")
                                .Where("{Condition}")
                                .Output(new { param = "value" });

            SqlSet set2 = Delete.Table("Table")
                                .Where(new ConditionModel
                                {
                                    Property_1 = "value_1",
                                    Property_2 = new IntervalVal("pre_value", "post_value"),
                                    Property_5 = "value_2"
                                }).Output();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void MoveDataExp()
        {
            SqlSet set1 = MoveData.From("SrcTable")
                                  .Into("TargetTable")
                                  .Where("{Condition}")
                                  .Output(new { param = "value" });

            SqlSet set2 = MoveData.From("SrcTable")
                                  .Into("TargetTable")
                                  .Where(new ConditionModel
                                  {
                                      Property_1 = "value_1",
                                      Property_2 = new IntervalVal("pre_value", "post_value"),
                                      Property_5 = "value_2"
                                  }).Output();
            Assert.IsTrue(true);
        }
    }
}
