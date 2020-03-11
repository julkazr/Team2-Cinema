using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUsersRepository> _mockUserRepository;

        [TestMethod]
        public void UserService_GetAllAsync_ReturnNull()
        {
            //Arrange

            IEnumerable<User> users = null;
            Task<IEnumerable<User>> responseTask = Task.FromResult(users);

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetAll()).Returns(responseTask);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act

            var resultAction = usersService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert

            Assert.IsNull(resultAction);
        }


        [TestMethod]
        public void UserService_GetAllAsync_ReturnListOfUsers()
        {
            //Arrange
            int expectedResultCount = 1;
            List<User> userModelList = new List<User>();
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };
            userModelList.Add(user);

            IEnumerable<User> list = userModelList;
            Task<IEnumerable<User>> responseTask = Task.FromResult(list);

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetAll()).Returns(responseTask);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act
            var resultAction = usersService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<UserDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(user.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(UserDomainModel));
        }


        [TestMethod]
        public void UserService_GetUserByIdAsync_ReturnNull()
        {
            //Arrange

            Guid guID = Guid.NewGuid();
            User user = null;

            Task<User> responseTask = Task.FromResult(user);

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act

            var resultAction = usersService.GetUserByIdAsync(guID).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert

            Assert.IsNull(domainModel);

        }


        [TestMethod]
        public void UserService_GetUserByIdAsync_ReturnUser()
        {
            //Arrange

            Guid guId = Guid.NewGuid();
            User user = new User
            {
                Id = guId,
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };

            Task<User> responseTask = Task.FromResult(user);

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act
            var resultAction = usersService.GetUserByIdAsync(guId).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(guId, domainModel.Id);
            Assert.IsInstanceOfType(domainModel, typeof(UserDomainModel));
        }


        [TestMethod]
        public void UserService_GetUserByUserName_ReturnNull()
        {
            //Arrange

            string username = "someUsername";
            User user = null;

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act

            var resultAction = usersService.GetUserByUserName(username).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert

            Assert.IsNull(domainModel);

        }


        [TestMethod]
        public void UserService_GetUserByUserName_ReturnUser()
        {
            //Arrange

            string username = "someUsername";
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true
            };

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act
            var resultAction = usersService.GetUserByUserName(username).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(user.Id, domainModel.Id);
            Assert.IsInstanceOfType(domainModel, typeof(UserDomainModel));
        }


        [TestMethod]
        public void UserService_IncreaseBonus_ReturnUser()
        {
            //Arrange

            Guid guId = Guid.NewGuid();
            User user = new User
            {
                Id = guId,
                FirstName = "Pera",
                LastName = "Peric",
                UserName = "Perr",
                IsAdmin = true,
                IsSuperUser = false,
                bonus = 0
            };

            Task<User> responseTask = Task.FromResult(user);
            user.bonus++;

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(user);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act
            var resultAction = usersService.IncreaseBonus(guId).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(guId, domainModel.Id);
            Assert.AreEqual(user.bonus, domainModel.bonus);
            Assert.IsInstanceOfType(domainModel, typeof(UserDomainModel));
        }

        [TestMethod]
        public void UserService_IncreaseBonus_ReturnNull()
        {
            //Arrange
            Guid guID = Guid.NewGuid();
            User user = null;

            Task<User> responseTask = Task.FromResult(user);

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            UserService usersService = new UserService(_mockUserRepository.Object);

            //Act
            var resultAction = usersService.IncreaseBonus(guID).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (UserDomainModel)resultAction;

            //Assert
            Assert.IsNull(domainModel);

        }
    }
}
