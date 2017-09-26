using System;
using System.Collections.Generic;
using System.IO;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.DataStores
{
    public class FileCustomerDataStore : ICustomerDataStore
    {
        private readonly string _filePAth;

        public FileCustomerDataStore(string filePAth)
        {
            _filePAth = filePAth;
            Directory.CreateDirectory(_filePAth);
        }

        public string Write(string name, string email, string password)
        {
            string guid = Guid.NewGuid().ToString();
            return Update(guid, name, email, password);
        }

        public List<string> FindCustomerByEmail(string email)
        {
            List<string> returnList = new List<string>();
            var files = Directory.GetFiles(_filePAth);
            foreach (var file in files)
            {
                var fileData = File.ReadAllText(file).Split(',');
                if (fileData[1] == email)
                    returnList.Add(file);
            }
            return returnList;
        }

        public string Update(string guid, string name, string email, string password)
        {
            File.WriteAllText(_filePAth + guid, $"{name},{email},{password}");
            return guid;
        }

        public Customer GetCustomerByGuid(string customerGuid)
        {
            if (!File.Exists(_filePAth + customerGuid)) return null;
            var fileData = File.ReadAllText(_filePAth + customerGuid).Split(',');
            return new Customer {Email = fileData[1], Name = fileData[0], Guid = customerGuid};
        }

        public Customer GetCustomerByEmailAndPassword(string email, string password)
        {
            var files = Directory.GetFiles(_filePAth);
            foreach (var file in files)
            {
                var fileData = File.ReadAllText(file).Split(',');
                if (fileData[1] == email && fileData[2] == password)
                    return new Customer { Email = fileData[1], Name = fileData[0], Guid = new FileInfo(file).Name };
            }
            return null;
        }

        public string GetPasswordForGuid(string customerObjectGuid)
        {
            throw new NotImplementedException();
        }
    }
}