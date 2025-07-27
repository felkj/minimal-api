using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Services;
using minimal_api.Infra.Db;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdminServiceTest
    {
        private DbContexto CriarContextoTeste()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContexto(configuration);
        }
        [TestMethod]
        public void TestandoSalvarAdmin()
        {
            var context = CriarContextoTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE admins;");
            //Arrange
            var admin = new Admin();
            admin.Email = "teste@teste.com";
            admin.Password = "teste";
            admin.Role = "admin";

            
            var adminService = new AdminService(context);

            //Act
            adminService.Create(admin);

            //Assert
            Assert.AreEqual(1, adminService.FindAll(1).Count());

        }
    }
}