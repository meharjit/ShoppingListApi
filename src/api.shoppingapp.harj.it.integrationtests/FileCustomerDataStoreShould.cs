using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.shoppingapp.harj.it.Domain.DataStores;
using NUnit.Framework;

namespace api.shoppingapp.harj.it.integrationtests
{
    [TestFixture]
    public class FileCustomerDataStoreShould
    {
        private FileCustomerDataStore _fileCustomerDataStore;
        private const string _filePAth = "e:\\db\\";

        [SetUp]
        public void TestSetUp()
        {
            _fileCustomerDataStore = new FileCustomerDataStore(_filePAth);

        }

        [Test]
        public void Write_Customer_to_disk()
        {

            string custGuid = _fileCustomerDataStore.Write("peter", "test@example.com", "p@ssword");
            var concatedFilePath = $"{_filePAth}{custGuid}";

            Assert.AreEqual("peter,test@example.com,p@ssword", File.ReadAllText(concatedFilePath));
            File.Delete(concatedFilePath);
        }

        [Test]
        public void Find_A_Customer_when_written_to_disk()
        {
            var cleanUpList = new List<string>();

            var myCustomerGuid = "my_customer_guid";
            cleanUpList.Add(myCustomerGuid);
            File.WriteAllText(_filePAth + myCustomerGuid, $"some_name,{myCustomerGuid}@example.com");

            for (int i = 0; i < 5; i++)
            {
                string guid = Guid.NewGuid().ToString();
                File.WriteAllText(_filePAth + guid, $"{guid},{guid}@example.com");
                cleanUpList.Add(guid);
            }

            var results = _fileCustomerDataStore.FindCustomerByEmail($"{myCustomerGuid}@example.com");

            Assert.AreEqual(results.Count, 1);
            results[0] = myCustomerGuid;
            cleanUpList.ForEach(e=> File.Delete(_filePAth + e));
        }


        [Test]
        public void Get_A_Customer_when_written_to_disk()
        {
            var cleanUpList = new List<string>();

            var myCustomerGuid = "my_customer_guid";
            cleanUpList.Add(myCustomerGuid);
            File.WriteAllText(_filePAth + myCustomerGuid, $"some_name,{myCustomerGuid}@example.com");

            for (int i = 0; i < 5; i++)
            {
                string guid = Guid.NewGuid().ToString();
                File.WriteAllText(_filePAth + guid, $"{guid},{guid}@example.com");
                cleanUpList.Add(guid);
            }

            var results = _fileCustomerDataStore.GetCustomerByGuid(myCustomerGuid);

            Assert.AreEqual(results.Email, myCustomerGuid + "@example.com");
            Assert.AreEqual(results.Guid, myCustomerGuid);
            Assert.AreEqual(results.Name, "some_name");

            cleanUpList.ForEach(e => File.Delete(_filePAth + e));
        }

        [Test]
        public void Get_A_Customer_when_not_written_to_disk()
        {
            var results = _fileCustomerDataStore.GetCustomerByGuid("any_guid");
            Assert.IsNull(results);
        }

        [Test]
        public void Update_existing_Customer_on_disk()
        {

            string custGuid = _fileCustomerDataStore.Write("peter", "test@example.com", "p@ssword");
            var concatedFilePath = $"{_filePAth}{custGuid}";

            Assert.AreEqual("peter,test@example.com,p@ssword", File.ReadAllText(concatedFilePath));

            var custGuidReturned = _fileCustomerDataStore.Update(custGuid, "john", "john@example.com", "p@ssword");

            Assert.AreEqual("john,john@example.com,p@ssword", File.ReadAllText(concatedFilePath));
            Assert.AreEqual(custGuid, custGuidReturned);
            File.Delete(concatedFilePath);
        }


        [Test]
        public void Get_A_Customer_By_email_and_password_when_not_written_to_disk()
        {
            var results = _fileCustomerDataStore.GetCustomerByEmailAndPassword("test@example.com", "p@ssword1");
            Assert.IsNull(results);
        }
        [Test]
        public void Get_A_Customer_By_email_and_password_when_written_to_disk()
        {
            var cleanUpList = new List<string>();

            var myCustomerGuid = "my_customer_guid";
            cleanUpList.Add(myCustomerGuid);
            File.WriteAllText(_filePAth + myCustomerGuid, $"some_name,{myCustomerGuid}@example.com,p@ssword1");

            for (int i = 0; i < 5; i++)
            {
                string guid = Guid.NewGuid().ToString();
                File.WriteAllText(_filePAth + guid, $"{guid},{guid}@example.com,p@ssword1");
                cleanUpList.Add(guid);
            }

            var results = _fileCustomerDataStore.GetCustomerByEmailAndPassword(myCustomerGuid+ "@example.com", "p@ssword1");

            Assert.AreEqual(results.Email, myCustomerGuid + "@example.com");
            Assert.AreEqual(results.Guid, myCustomerGuid);
            Assert.AreEqual(results.Name, "some_name");

            cleanUpList.ForEach(e => File.Delete(_filePAth + e));
        }
    }
}
