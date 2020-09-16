using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DataLayer
{
    public class ContactRepositorySP : IContactRepository
    {
        private IDbConnection db;

        public ContactRepositorySP(string connString)
        {
            this.db = new SqlConnection(connString);
        }

        Contact IContactRepository.Add(Contact contact)
        {
            throw new NotImplementedException();
        }

        Contact IContactRepository.Find(int id)
        {
            return this.db.Query<Contact>("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        List<Contact> IContactRepository.GetAll()
        {
            throw new NotImplementedException();
        }

        Contact IContactRepository.GetFullContact(int id)
        {
            using (var multipleResults = this.db.QueryMultiple("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure))
            {
                var contact = multipleResults.Read<Contact>().SingleOrDefault();

                var addresses = multipleResults.Read<Address>().ToList();
                if (contact != null && addresses != null)
                {
                    contact.Addresses.AddRange(addresses);
                }

                return contact;
            }
        }

        void IContactRepository.Remove(int id)
        {
            this.db.Execute("DeleteContact", new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        void IContactRepository.Save(Contact contact)
        {
            using var txScope = new TransactionScope();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", value: contact.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameters.Add("@FirstName", contact.FirstName);
            parameters.Add("@LastName", contact.LastName);
            parameters.Add("@Company", contact.Company);
            parameters.Add("@Title", contact.Title);
            parameters.Add("@Email", contact.Email);
            this.db.Execute("SaveContact", parameters, commandType: CommandType.StoredProcedure);
            contact.Id = parameters.Get<int>("@Id");

            foreach (var addr in contact.Addresses.Where(a => !a.IsDeleted))
            {
                addr.ContactId = contact.Id;

                var addrParams = new DynamicParameters(new
                {
                    ContactId = addr.ContactId,
                    AddressType = addr.AddressType,
                    StreetAddress = addr.StreetAddress,
                    City = addr.City,
                    StateId = addr.StateId,
                    PostalCode = addr.PostalCode
                });
                addrParams.Add("@Id", addr.Id, DbType.Int32, ParameterDirection.InputOutput);
                this.db.Execute("SaveAddress", addrParams, commandType: CommandType.StoredProcedure);
                addr.Id = addrParams.Get<int>("@Id");
            }

            foreach (var addr in contact.Addresses.Where(a => a.IsDeleted))
            {
                this.db.Execute("DeleteAddress", new { Id = addr.Id }, commandType: CommandType.StoredProcedure);
            }

            txScope.Complete();
        }

        Contact IContactRepository.Update(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
