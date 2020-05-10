using DT.Domain.DTO.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DT.Interfaces
{
   public interface IUserService
    {
        UserModel Authenticate(string username, string password);
        IEnumerable<UserModel> GetAll();
        UserModel GetById(Guid id);
        Task<UserModel> Create(RegisterModel user, string password);
        Task Update(string id, UpdateModel user, string password = null);
        void Delete(Guid id);
        Task<UserModel> GetByUserNameAsync(string username);
        UserModel GetByUserName(string username);
    }
}
