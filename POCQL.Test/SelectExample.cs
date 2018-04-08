using Microsoft.VisualStudio.TestTools.UnitTesting;
using POCQL.DAO.Model;
using POCQL.Model;
using POCQL.Model.MapAttribute;
using POCQL.MSSQL;
using POCQL.Test.Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example
{
    [TestClass]
    public class SelectExample
    {
        /// <summary>
        /// 常用查詢
        /// </summary>
        [TestMethod]
        public void CommonSelectExp()
        {
            // *** 所有查詢: SELECT * FROM [TABEL] (WHERE [CONDITION]) ***
            var allSelect = Select.Columns()        // *** 所有查詢就不要指定欄位即可 ***
                                  .From("TABEL")    // *** 設定查詢Table ***
                                  .Where("COLUMN = @para")  // *** 設定查詢條件 ***
                                  .Output();
            //.Query(new        // *** 直接執行查詢並且回傳結果 ***
            //{                 // *** 可以使用.ToString()輸出SQL
            //    para = ""
            //});

            // *** 有條件查詢 ***
            string select = Select.Columns<QueryModel_1>()                  // *** 透過Model決定要查詢的欄位 ***
                                  .Columns("Column_1", "Column_2", "Column_3")                   // *** 指定要查詢的欄位 ***
                                  .Columns("Column_4 : Property_1", "Column_5 : Property_1")     // *** 指定要查詢的欄位，同時指定要Map的Property ***
                                  .MatcheColumns<MultiColumnModel>("Mat1")  // *** 從特定型態中指定欄位名符合指定值的欄位值 ***
                                  .From("TABEL", "T")                       // *** 指定資料來源的Table，並且給予別名(可以不給) ***
                                  .Where("T.COLUMN = @para")                // *** 設定查詢條件式 ***
                                  .OrderBy("Column or Property", "defaultOrder", "DESC")            // *** 設定排序，可以給DB的欄位或是Model的Property
                                  .ToString();

            string speSelect = Select.Columns<QueryModel_1>()
                                     .Columns("(CASE WHEN RTN_DT IS NULL THEN OUT_DT ELSE RTN_DT END):[Result]")
                                     .From("TABEL", "T", true)
                                     .Where("T.COLUMN = @para")                // *** 設定查詢條件式 ***
                                     .OrderBy("Column or Property", "defaultOrder", "DESC")            // *** 設定排序，可以給DB的欄位或是Model的Property
                                     .OrderBy("Column_2", "defaultOrder_2", "DESC")            // *** 設定排序，可以給DB的欄位或是Model的Property
                                     .OrderBy("Property_3", "defaultOrder_3", "DESC")            // *** 設定排序，可以給DB的欄位或是Model的Property
                                     .ToString();
        }

        /// <summary>
        /// 分頁查詢
        /// </summary>
        [TestMethod]
        public void PagingSelectExp()
        {
            var pagingSelect = Select.Columns<QueryModel_1>()             // *** 透過Model決定要查詢的欄位 ***
                                     .From("TABEL", "T")                // *** 指定資料來源的Table，並且給予別名(可以不給) ***
                                     .Where("T.COLUMN = @para")         // *** 設定查詢條件式
                                     .OrderBy("Column or Property", "Default Order", "DESC")     // *** 設定排序，可以給DB的欄位或是Model的Property ***
                                     .Paging(50, 1)                     // *** 分頁設定 ***
                                     .Output();
            //.PagingQuery<QueryModel_1>(new       // *** 如果有做分頁設定，建議直接使用分頁查詢 ***
            //{
            //    para = ""
            //});
        }

        /// <summary>
        /// 多型態查詢
        /// </summary>
        public static void MultiTypeSelectExp()
        {
            var select = Select.Columns<QueryModel_1>()     // *** 透過Model決定要查詢的欄位 ***
                               .SplitOn()                   // !!! 很重要: 如果要做多型態查詢時，型態之間要做切割點，這是為了配合Dapper底層的規則 !!!
                               .Columns<QueryModel_2>()     // *** 透過Model決定要查詢的欄位 ***
                               .From("TABEL", "T")          // *** 指定資料來源的Table，並且給予別名(可以不給) ***
                               .Where("T.COLUMN = @para")   // *** 設定查詢條件式 ***
                               .Output();
            //.Query<QueryModel_1, QueryModel_2, QueryModel_1>(new { para = "" },
            //                                                 (i, j) =>
            //                                                 {
            //                                                     i.SubType = j;
            //                                                     return i;
            //                                                 });
        }

        /// <summary>
        /// Join查詢
        /// </summary>
        public static void JoinSelectExp()
        {
            // *** 目前SQLTool只提供INNER JOIN和LEFT JOIN兩種 ***
            // *** 日後有需要再追加 ***

            var innerJoin = Select.Columns<JoinQuertModel>()
                                  .From("Table_1", "T1")
                                  .InnerJoin("Table_2", "T2", "T1.Column_n = T2.Column_n")      // *** 設定INNER JOIN ***
                                  .Where("T1.COLUMN = @para")
                                  .Output(new
                                  {
                                      para = ""
                                  });
            //.Query(new 
            //{                 
            //    para = ""
            //});

            var leftJoion = Select.Columns<JoinQuertModel>()
                                  .From("Table_1", "T1")
                                  .LeftJoin("Table_2", "T2", "T1.Column_n = T2.Column_n")       // *** 設定LEFT JOIN ***
                                  .Where("T1.COLUMN = @para")
                                  .Output(new
                                  {
                                      para = ""
                                  });
            //.Query(new
            //{
            //    para = ""
            //});
        }

        /// <summary>
        /// 條件物件查詢
        /// </summary>
        [TestMethod]
        public void ConditionObjectSelectExp()
        {
            // *** 使用Condition Model查詢資料 ***
            ConditionModel condition = new ConditionModel
            {
                Property_1 = "value1",
                Property_2 = new IntervalVal(null, "postNo"),
                Property_3 = new string[] { "val_1", "val_2", "val_3" },
                // *** 條件式解析時會略過Value = NULL的Property ***
                // *** 這邊故意給Property_4, Property_5 NULL值 ***
                Property_4 = DateTime.Now,
                Property_5 = "124",
                Property_6 = "測試",
                Property_7 = "XDD",
            };


        }

        /// <summary>
        /// 子查詢
        /// </summary>
        public static void SubQuerySelectExp()
        {
            // *** 使用Condition Model查詢資料 ***
            ConditionModel condition = new ConditionModel
            {
                Property_4 = null,
                Property_5 = "ORZ",
                Property_6 = "測試",
            };

            var select = Select.Columns("Column_1", "Column_2")
                               .SubQuery(Select.Columns("Column_3")
                                               .From("Table_2", "T2")
                                               .Where(condition), "subResult")
                               .From("Table_1")
                               .Where("{Condition_1}")
                               .Output();

        }

        [TestMethod]
        public void DEMO_2()
        {

            string sql = Select.Columns("EMPLOYEE_NO : No",
                                        "NAME",
                                        "SEX")
                               .From("EMPLOYEE")
                               .Where("DEPT = @dept")
                               .ToString();

        }

        [TestMethod]
        public void DEMO_1()
        {

            string sql = Select.Columns<Employee>()
                               .Columns<Employee>(new ColumnBinder<Employee>
                               {
                                   { "KIND" },
                                   { "CASE WHEN KIND = '1' THEN 1 ELSE 0 END", i=>i.IsSupervisor },
                               })
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();

        }

        [TestMethod]
        public void DEMO_3()
        {

            string sql = Select.MatcheColumns<UserInfo>("MDF")
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();

        }

        [TestMethod]
        public void DEMO_4()
        {
            string sql = Select.Columns<EmployeeForJoin>()
                               .From("EMPLOYEE", "EPY")
                               .InnerJoin("USER", "USR", "EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO")
                               .Where("DEPT = @Dept")
                               .ToString();

        }

        [TestMethod]
        public void DEMO_5()
        {
            string sql = Select.Columns<Employee>()
                               .Columns<User>()
                               .From("EMPLOYEE", "EPY")
                               .InnerJoin("USER", "USR", "EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO")
                               .Where("DEPT = @Dept")
                               .ToString();

        }

        [TestMethod]
        public void DEMO_6()
        {
            string sql = Select.Columns<Employee, Employee>()
                               .SubQuery<Employee, Employee>("SELECT ID FROM USER USR WHERE USR.EMPLOYEE_NO = EMPLOYEE_NO", i => i.Id)
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_7()
        {
            string sql = Select.Columns<Employee, Employee>()
                               .SubQuery<Employee, Employee>(Select.Columns("ID")
                                                                   .From("USER", "USR")
                                                                   .Where("USR.EMPLOYEE_NO = EMPLOYEE_NO"),
                                                             i => i.Id)
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_8()
        {
            string sql = Select.Top(5)
                               .Columns<Employee>()
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_9()
        {
            string sql = Select.Distinct()
                               .Columns<Employee>()
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_10()
        {
            string sql = Select.Columns<Employee>()
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .OrderBy("EMPLOYEE_NO")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_11()
        {
            string sql = Select.Columns<Employee>()
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .GroupBy("EMPLOYEE_NO", "NAME")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_12()
        {
            string sql = Select.Columns<Employee>()
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .OrderBy("EMPLOYEE_NO")
                               .Paging(50, 2)
                               .ToString();
        }

        [TestMethod]
        public void DEMO_13()
        {
            //IEnumerable<Employee> result = Select.Columns<Employee>()
            //                                     .From("EMPLOYEE")
            //                                     .Where("DEPT = '100'")
            //                                     .Query<Employee>();

            IEnumerable<Employee> result = Select.Columns<Employee>()
                                                 .From("EMPLOYEE")
                                                 .Where("DEPT = @Dept")
                                                 .Query<Employee>(new
                                                 {
                                                     Dept = "100"
                                                 });


        }

        [TestMethod]
        public void DEMO_14()
        {
            MyPagingResult result = Select.Columns<Employee>()
                                                 .From("EMPLOYEE")
                                                 .Where("DEPT = @Dept")
                                                 .OrderBy("EMPLOYEE_NO")
                                                 .Paging(50, 2)
                                                 .PagingQuery<Employee, MyPagingResult>(new
                                                 {
                                                     Dept = "100"
                                                 },
                                                 i => new MyPagingResult
                                                 {
                                                     QueryDatas = i.Datas,
                                                     DataCount = i.Total
                                                 });


        }

        [TestMethod]
        public void DEMO_15()
        {
            SelectObject cteSql = Select.Columns<Employee>()
                                        .From("EMPLOYEE")
                                        .Where("DEPT = @Dept");

            string sql = Select.Columns()
                               .Cte($";WITH CTE AS ({cteSql})")
                               .From("CTE")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_16()
        {
            string sql = Select.Columns<Employee>()
                               .SetTable("EPY_TABLE", "EMPLOYEE")
                               .From("EMPLOYEE")
                               .Where("DEPT = @Dept")
                               .ToString();
        }

        [TestMethod]
        public void DEMO_17()
        {
            string[] unionTables = new string[] { "EMPLOYEE", "EMPLOYEE_END", "EMPLOYEE_BACKUP" };

            IEnumerable<SelectObject> cteSqls =
                unionTables.Select(table => Select.Columns<Employee>()
                                                  .SetTable("EPY_TABLE", table)
                                                  .From(table)
                                                  .Where("DEPT = @Dept"));

            string sql = Select.Columns()
                               .Cte($";WITH CTE AS({string.Join(" UNION ", cteSqls)})")
                               .From("CTE")
                               .ToString();
        }

        public class MyPagingResult
        {
            public IEnumerable<object> QueryDatas { get; set; }
            public int DataCount { get; set; }
        }

        public class MySqlSet
        {
            public int Index { get; set; }
            public string Sql { get; private set; }
            public object Parameters { get; private set; }
        }

        [EntityMapper("CUSTOMER_ORDER")]
        public class CustomerOrder
        {
            [PrimaryKey, ColumnMapper("ID")]
            public string ID { get; set; }

            [ColumnMapper("NAME")]
            public string Name { get; set; }

            [ColumnMapper("GOODS")]
            public string Goods { get; set; }

            [ColumnMapper("PRICE"), Aggregation(AggregateFunction.SUM)]
            public string TotalPrice { get; set; }

        }

        /// <summary>
        /// 員工資訊
        /// </summary>
        [EntityMapper("{#EPY_TABLE#}")]
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

            public string Id { get; set; }

            public bool IsSupervisor { get; set; }
        }

        /// <summary>
        /// 員工資訊
        /// </summary>
        [EntityMapper("EMPLOYEE")]
        public class EmployeeForJoin
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

            [ColumnMapper("ID", "USER")]
            public string Id { get; set; }

            [ColumnMapper("BIRTH_DATE", "USER")]
            public DateTime? BirthDate { get; set; }

            [ColumnMapper("MARITAL_STATUS", "USER")]
            public string MaritalStatus { get; set; }

            [ColumnMapper("ADDRESS", "USER")]
            public string Address { get; set; }

            [ColumnMapper("PHONE_NUMBER", "USER")]
            public string PhoneNumber { get; set; }
        }

        /// <summary>
        /// 員工資訊查詢Model
        /// </summary>
        [EntityMapper("EMPLOYEE")]
        public class EmployeeCdt
        {
            [Condition("EMPLOYEE_NO")]
            public string EmployeeNo { get; set; }

            [Condition("NAME")]
            public string Name { get; set; }

            [Condition("SEX")]
            public string Sex { get; set; }

            [Condition("DEPT")]
            public string Department { get; set; }

            [Condition("TITLE")]
            public string Title { get; set; }
        }

        /// <summary>
        /// 員工詳細資料
        /// </summary>
        [EntityMapper("USER")]
        public class User
        {
            [PrimaryKey, ColumnMapper("ID")]
            public string Id { get; set; }

            [ColumnMapper("EMPLOYEE_NO")]
            public string EmployeeNo { get; set; }

            [ColumnMapper("NAME")]
            public string Name { get; set; }

            [ColumnMapper("SEX")]
            public string Sex { get; set; }

            [ColumnMapper("BIRTH_DATE")]
            public DateTime? BirthDate { get; set; }

            [ColumnMapper("MARITAL_STATUS")]
            public string MaritalStatus { get; set; }

            [ColumnMapper("ADDRESS")]
            public string Address { get; set; }

            [ColumnMapper("PHONE_NUMBER")]
            public string PhoneNumber { get; set; }
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


    }
}
