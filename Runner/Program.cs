using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace Runner
{
    class Program
    {
        private static IConfigurationRoot config;

        static void Main(string[] args)
        {

            Initialize();

            //Insert_should_assign_identity_to_new_entity();
            //Get_all_should_return_6_results();

            var id = Insert_should_assign_identity_to_new_entity();
            Find_should_retrieve_existing_entity(id);
            Modify_should_update_existing_entity(id);
        }


        static void Modify_should_update_existing_entity(int id)
        {
            // arrange
            IContactRepository repository = CreateRepository();

            // act
            var contact = repository.Find(id);
            //var contact = repository.GetFullContact(id);
            contact.FirstName = "Bob";
           // contact.Addresses[0].StreetAddress = "456 Main Street";
            repository.Update(contact);
            //repository.Save(contact);

            // create a new repository for verification purposes
            IContactRepository repository2 = CreateRepository();
            var modifiedContact = repository2.Find(id);
            //var modifiedContact = repository2.GetFullContact(id);

            // assert
            Console.WriteLine("*** Contact Modified ***");
            modifiedContact.Output();
            Debug.Assert(modifiedContact.FirstName == "Bob");
            //Debug.Assert(modifiedContact.Addresses.First().StreetAddress == "456 Main Street");
        }


        static void Find_should_retrieve_existing_entity(int id)
        {
            // arrange
            IContactRepository repository = CreateRepository();

            // act
            var contact = repository.Find(id);
            //var contact = repository.GetFullContact(id);

            // assert
            Console.WriteLine("*** Get Contact ***");
            contact.Output();
            Debug.Assert(contact.FirstName == "Joe");
            Debug.Assert(contact.LastName == "Blow");
            //Debug.Assert(contact.Addresses.Count == 1);
            //Debug.Assert(contact.Addresses.First().StreetAddress == "123 Main Street");
        }


        static int Insert_should_assign_identity_to_new_entity()
        {
            // arrange
            IContactRepository repository = CreateRepository();
            var contact = new Contact
            {
                FirstName = "Joe",
                LastName = "Blow",
                Email = "joe.blow@gmail.com",
                Company = "Microsoft",
                Title = "Developer"
            };
            //var address = new Address
            //{
            //    AddressType = "Home",
            //    StreetAddress = "123 Main Street",
            //    City = "Baltimore",
            //    StateId = 1,
            //    PostalCode = "22222"
            //};
            //contact.Addresses.Add(address);

            // act
            repository.Add(contact);
            //repository.Save(contact);

            // assert
            Debug.Assert(contact.Id != 0);
            Console.WriteLine("*** Contact Inserted ***");
            Console.WriteLine($"New ID: {contact.Id}");
            return contact.Id;
        }


        static void Get_all_should_return_6_results()
        {
            // arrange
            var repository = CreateRepository();

            // act
            var contacts = repository.GetAll();

            // assert
            Console.WriteLine($"Count: {contacts.Count}");
            Debug.Assert(contacts.Count == 6);
            contacts.Output();
        }



        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = builder.Build();
        }


        private static IContactRepository CreateRepository()
        {
            return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
        }

    }
}
