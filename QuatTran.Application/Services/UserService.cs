using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuatTran.Domain.Interfaces;
using QuatTran.Application.Interfaces;
using QuatTran.Infrastructure;
using QuatTran.Application.DTOs;
using System.Security.Cryptography;


namespace QuatTran.Application.Services
{
    public class UserService : IUserService
    {
        public readonly IRepository<User> _repository;
        public UserService(IRepository<User> repository)
        {
            _repository = repository;
        }
        public async Task AddUserAsync(UserDto userDto)
        {
            var user = new User()
            {
                UserId = userDto.UserId,
                FullName = userDto.FullName,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                Phone = userDto.Phone,
                Address = userDto.Address
            };
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<UserDto>> GetAllUserAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(u => new UserDto()
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                Phone = u.Phone,
                Address = u.Address,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }
        public async Task<UserDto> GetByIdAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            return new UserDto()
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
        public async Task UpdateUser(UserDto userDto)
        {
            var user = await _repository.GetByIdAsync(userDto.UserId);
            if (user != null)
            {
                user.FullName = userDto.FullName;
                user.Email = userDto.Email;
                user.PasswordHash = userDto.PasswordHash;
                user.Phone = userDto.Phone;
                user.Address = userDto.Address;
                user.Role = userDto.Role;
                await _repository.SaveChangesAsync();
            }
        }
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user != null)
            {
                await _repository.DeleteAsync(user);
                await _repository.SaveChangesAsync();
            }
        }
        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            var users = await _repository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            var hash = ComputeSha256Hash(password);
            if (user.PasswordHash != hash) return null;

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var t in bytes)
                builder.Append(t.ToString("x2"));
            return builder.ToString();
        }

    }

}
