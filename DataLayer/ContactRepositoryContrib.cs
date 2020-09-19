using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace DataLayer
{
    public class ContactRepositoryContrib : IContactRepository
    {

        private IDbConnection db;

        public ContactRepositoryContrib(string connString)
        {
            this.db = new SqlConnection(connString);
        }

        Contact IContactRepository.Add(Contact contact)
        {
            var id = this.db.Insert(contact);
            contact.Id = (int)id;
            return contact;
        }

        Contact IContactRepository.Find(int id)
        {
            return this.db.Get<Contact>(id);
        }

        List<Contact> IContactRepository.GetAll()
        {
            return this.db.GetAll<Contact>().ToList();
        }

        Contact IContactRepository.GetFullContact(int id)
        {
            throw new NotImplementedException();
        }

        void IContactRepository.Remove(int id)
        {
            this.db.Delete(new Contact { Id = id });
        }

        void IContactRepository.Save(Contact contact)
        {
            throw new NotImplementedException();
        }

        Contact IContactRepository.Update(Contact contact)
        {
            this.db.Update(contact);
            return contact;
        }
    }
}
