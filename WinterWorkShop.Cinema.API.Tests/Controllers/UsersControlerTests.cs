using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class UsersControlerTests
    {
        private Mock<IUserService> _userService;


        [TestMethod]
        public void GetAsync_Return_All_Users()
        {
            //Arrange

            List<UserDomainModel> userDomainModelList = new List<UserDomainModel>();
            UserDomainModel user = new UserDomainModel{
                Id = Guid.NewGuid(),
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };
            userDomainModelList.Add(user);
            IEnumerable<UserDomainModel> userDomainModels = userDomainModelList;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(userDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act

            var result = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResultList = (List<UserDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(userDomainModelResultList);
            Assert.AreEqual(expectedResultCount, userDomainModelResultList.Count);
            Assert.AreEqual(user.Id, userDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange

            IEnumerable<UserDomainModel> userDomainModelList = null;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(userDomainModelList);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act

            var result = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResultList = (List<UserDomainModel>)resultList;


            //Assert

            Assert.IsNotNull(userDomainModelResultList);
            Assert.AreEqual(expectedResultCount, userDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void GetbyIdAsync_Return_OK()
        {
            //Arrange

            Guid guId = Guid.NewGuid();
            UserDomainModel user = new UserDomainModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };
            Task<UserDomainModel> responseTask = Task.FromResult(user);

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);
            int expectedStatusCode = 200;

            //Act

            var result = usersController.GetbyIdAsync(guId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultValue = ((OkObjectResult)result).Value;
            var userDomainModelResult = (UserDomainModel)resultValue;
                
            //Assert

            Assert.IsNotNull(userDomainModelResult);
            Assert.AreEqual(user.Id, userDomainModelResult.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void GetbyIdAsyncGetReturnNull_Return_NotFound()
        {
            //Arrange

            Guid guId = Guid.NewGuid();

            UserDomainModel user = null;
            Task<UserDomainModel> responseTask = Task.FromResult(user);

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            int expectedStatusCode = 404;
            string expectedMessage = Messages.USER_NOT_FOUND;


            //Act

            var result = usersController.GetbyIdAsync(guId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultValue = ((ObjectResult)result).Value;

            //Assert

            Assert.IsNotNull(resultValue);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
            Assert.AreEqual(expectedMessage, resultValue);
        }

        [TestMethod]
        public void GetbyUserNameAsync_Return_Ok()
        {
            //Arrange

            UserDomainModel user = new UserDomainModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };
            Task<UserDomainModel> responseTask = Task.FromResult(user);

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            int expectedStatusCode = 200;
            string username = "SomeUsername";


            //Act

            var result = usersController.GetbyUserNameAsync(username).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultValue = ((OkObjectResult)result).Value;
            var userDomainModelResult = (UserDomainModel)resultValue;

            //Assert

            Assert.IsNotNull(userDomainModelResult);
            Assert.AreEqual(user.Id, userDomainModelResult.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetbyUserNameAsync_Return_NotFound()
        {
            //Arrange

            UserDomainModel user = null;
            
            Task<UserDomainModel> responseTask = Task.FromResult(user);

            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            int expectedStatusCode = 404;
            string username = "SomeUsername";
            string expectedMessage = Messages.USER_NOT_FOUND;

            //Act

            var result = usersController.GetbyUserNameAsync(username).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultValue = ((ObjectResult)result).Value;

            //Assert

            Assert.IsNotNull(resultValue);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
            Assert.AreEqual(expectedMessage, resultValue);
        }


    }
}
