    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuatTran.Application.DTOs;
    using QuatTran.Infrastructure;

    namespace QuatTran.Application.Interfaces
    {
        public interface IUserService
        {
            Task<IEnumerable<UserDto>> GetAllUserAsync();
            Task<UserDto> GetByIdAsync(int userId);
            Task AddUserAsync(UserDto userDto);
            Task UpdateUser(UserDto userDto);
            Task DeleteUserAsync(int userId);
            Task<UserDto?> AuthenticateAsync(string email, string password);
    }
    }
