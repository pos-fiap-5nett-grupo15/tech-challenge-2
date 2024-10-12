using ContactsManagement.Infrastructure.Crypto;
using ContactsManagement.Infrastructure.Repositories.Contact;
using ContactsManagement.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;

namespace TestesIntegracao
{
    public class TestesIntegracao
    {
        private readonly Tech1Database _database;
        private readonly ContactRepository _repository;


        public TestesIntegracao()
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(System.AppContext.BaseDirectory)
             .AddJsonFile("appsettings.test.json")
             .Build();

            Environment.SetEnvironmentVariable("TECH1_API_SEC_KEY", "IGVRaqQssHbDh3adxV22rw73SmOusqrKHomyJe33TrM=");
            Environment.SetEnvironmentVariable("TECH1_API_SEC_IV", "iqs1k4XU6wADBBOP3cpg5A==");

            _database = new Tech1Database(configuration, new CryptoService());

            _repository = new ContactRepository(_database);
        }

        [Fact]
        public async Task InsertContact()
        {

            var contact = new ContactsManagement.Domain.Entities.ContactEntity
            {
                Nome = "Marta teste",
                Email = "mart.test@example.com",
                Ddd = 11,
                Telefone = 912345678
            };

            await _repository.CreateAsync(contact);

            var result = await _repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Marta teste", result.Nome);
            Assert.Equal("mar.test@example.com", result.Email);
        }

        [Fact]
        public async Task GetById()
        {

            var contact = new ContactsManagement.Domain.Entities.ContactEntity
            {
                Nome = "Jane Oliveira",
                Email = "jane@example.com",
                Ddd = 21,
                Telefone = 987654321
            };
            await _repository.CreateAsync(contact);

            var result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Jane Oliveira", result.Nome);
        }

        [Fact]
        public async Task UpdateById()
        {
            var contact = new ContactsManagement.Domain.Entities.ContactEntity
            {
                Nome = "João Miguel",
                Email = "joao@example.com",
                Ddd = 15,
                Telefone = 999999999
            };
            await _repository.CreateAsync(contact);

            await _repository.UpdateByIdAsync(1, "João Updated", null, null, null);
            var updatedContact = await _repository.GetByIdAsync(1);

            Assert.NotNull(updatedContact);
            Assert.Equal("João Updated", updatedContact.Nome);
        }

        [Fact]
        public async Task DeleteById()
        {
            var contact = new ContactsManagement.Domain.Entities.ContactEntity
            {
                Nome = "Miguel Angelo",
                Email = "miguel@example.com",
                Ddd = 31,
                Telefone = 55667788
            };
            await _repository.CreateAsync(contact);

            await _repository.DeleteByIdAsync(1);

            var result = await _repository.GetByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetListPaginatedByFilters()
        {

            await _repository.CreateAsync(new ContactsManagement.Domain.Entities.ContactEntity { Nome = "Aline", Email = "aline@example.com", Ddd = 11, Telefone = 123456789 });
            await _repository.CreateAsync(new ContactsManagement.Domain.Entities.ContactEntity { Nome = "Gabriel", Email = "gabriel@example.com", Ddd = 12, Telefone = 987654321 });

            var contacts = await _repository.GetListPaginatedByFiltersAsync(12, 0, 10);

            Assert.Equal(2, contacts.Count());
        }
    }
}