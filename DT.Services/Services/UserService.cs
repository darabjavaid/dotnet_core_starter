using AutoMapper;
using DT.Domain.DTO.Users;
using DT.Domain.Entities;
using DT.Domain.Exceptions;
using DT.Infrastructure.Data;
using DT.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace DT.Services.Services
{
    public class UserService : IUserService
    {
        AppDBContext _context;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(AppDBContext appDBContext, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = appDBContext;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public UserModel Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // _context.Users.SingleOrDefault(x => x.Username == username);
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // authentication successful
            var result = _mapper.Map<UserModel>(user);
            result.TK = tokenString;
            return result;
        }

        public IEnumerable<UserModel> GetAll()
        {
            return _mapper.Map<IList<UserModel>>(_context.Users);
        }

        public UserModel GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            return _mapper.Map<UserModel>(user);
        }

        public UserModel GetByUserName(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> GetByUserNameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> Create(RegisterModel model, string password)
        {            
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
            
            if (string.IsNullOrWhiteSpace(model.Username))
                throw new AppException("Username is required");

            if (_context.Users.Any(x => x.Username == model.Username.Trim()))
                throw new AppException("Username \"" + model.Username + "\" is already taken");

            // map model to entity
            var user = _mapper.Map<User>(model);            

            //create password hash
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.Id = Guid.NewGuid();
            user.Username = user.Username.Trim();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;            

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserModel>(user);
        }

        public async Task Update(string id, UpdateModel model, string password = null)
        {
            // map model to entity and set id
            var userParam = _mapper.Map<User>(model);
            userParam.Id = new Guid(id);

            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            if (!string.IsNullOrWhiteSpace(userParam.Email))
                user.Email = userParam.Email;

            if (!string.IsNullOrWhiteSpace(userParam.Role))
                user.Role = userParam.Role;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public void Delete(Guid id)
        {
            //_context.Users.Find(id);
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}