using Microsoft.VisualStudio.TestTools.UnitTesting;
using POCQL.DAO.ConnectionObject;
using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test
{
    [TestClass]
    public class DaoTest
    {
        [TestMethod]
        public void CreatODBC()
        {
            Factory factory = new OdbcFactory
            {
                DataSource = "{Data Source}",
                InitialCatalog = "{Initial Catalog}",
                Password = "{Password}",
                UserID = "{UserID}",
                PersistSecurityInfo = true
            };

            IDbConnection connectionObj = factory.CreateConnection();

            Assert.IsTrue(connectionObj.ConnectionString.Equals(@"Data Source=""{Data Source}"";Initial Catalog=""{Initial Catalog}"";Persist Security Info=True;User ID={UserID};Password={Password}"));
        }

        [TestMethod]
        public void CreatOLEDB()
        {
            Factory factory = new OledbFactory
            {
                DataSource = "{Data Source}",
                Provider = "{Provider}",
                ExtendedProperties = @"Excel 8.0;HDR=Yes;"
            };

            IDbConnection connectionObj = factory.CreateConnection();

            Assert.IsTrue(connectionObj.ConnectionString.Equals(@"Provider={Provider};Data Source=""{Data Source}"";Extended Properties=""Excel 8.0;HDR=Yes;"""));

        }

        [TestMethod]
        public void ReadExcel()
        {
            Factory factory = new OledbFactory
            {
                DataSource = @"TestData\DemoDB.xlsx",
                Provider = "Microsoft.ACE.OLEDB.12.0",
                ExtendedProperties = @"Excel 12.0;HDR=Yes;IMEX=1"
            };

            var dataaccess = new DAO.DataAccess(factory);

            UserInfo data = Select.Columns<UserInfo>()
                                  .From("UserInfo$", true)
                                  .Where("UserId = @id")
                                  .Query<UserInfo>(new { id = "C0009" },new DAO.DataAccess(factory))
                                  .FirstOrDefault();

            Assert.IsTrue(data.UserId.Equals("C0009"));

        }

        [EntityMapper("UserInfo$")]
        public class UserInfo
        {
            [ColumnMapper]
            public string UserId { get; set; }

            [ColumnMapper]
            public string UserName { get; set; }

            [ColumnMapper]
            public string Phone { get; set; }

            [ColumnMapper]
            public string Address { get; set; }
        }
    }
}
