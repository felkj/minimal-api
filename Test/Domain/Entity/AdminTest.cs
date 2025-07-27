using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Domain.Entity;

namespace Test.Domain.Entity
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            //Arrange
            var admin = new Admin();

            //Act
            admin.Id = 1;
            admin.Email = "teste@teste.com";
            admin.Password = "teste";
            admin.Role = "admin";

            //Assert
            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual("teste@teste.com", admin.Email);
            Assert.AreEqual("teste", admin.Password);
            Assert.AreEqual("admin", admin.Role);
        }
    }
}