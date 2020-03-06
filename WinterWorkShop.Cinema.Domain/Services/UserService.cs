using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<IEnumerable<UserDomainModel>> GetAllAsync()
        {
            var data = await _usersRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<UserDomainModel> result = new List<UserDomainModel>();
            UserDomainModel model;
            foreach (var item in data)
            {
                model = new UserDomainModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserName = item.UserName,
                    IsAdmin = item.IsAdmin,
                    bonus = item.bonus ?? 0,
                    IsSuperUser = item.IsSuperUser ?? false
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<UserDomainModel> GetUserByIdAsync(Guid id)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                IsAdmin = data.IsAdmin,
                bonus = data.bonus ?? 0,
                IsSuperUser = data.IsSuperUser ?? false
            };

            return domainModel;
        }

        public async Task<UserDomainModel> GetUserByUserName(string username)
        {
            var data = _usersRepository.GetByUserName(username);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                IsAdmin = data.IsAdmin,
                bonus = data.bonus ?? 0,
                IsSuperUser = data.IsSuperUser ?? false
            };

            return domainModel;
        }

        public async Task<UserDomainModel> IncreaseBonus(Guid id, int bonusIncrease=1)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            if(data.bonus == null)
            {
                data.bonus = 0;
            }

            data.bonus += bonusIncrease;
            var userAfterUpdate =  _usersRepository.Update(data);

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = userAfterUpdate.Id,
                FirstName = userAfterUpdate.FirstName,
                LastName = userAfterUpdate.LastName,
                UserName = userAfterUpdate.UserName,
                IsAdmin = userAfterUpdate.IsAdmin,
                bonus = userAfterUpdate.bonus ?? 0,
                IsSuperUser = userAfterUpdate.IsSuperUser ?? false
            };

            return domainModel;
        }


    }
}
