using Microsoft.VisualStudio.TestTools.UnitTesting;
using POCQL.DAO.ConnectionObject;
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
    }
}
