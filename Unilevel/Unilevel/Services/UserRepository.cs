using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using System.Security.Cryptography;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UnilevelContext _context;
        private readonly IEmailServices _emailService;

        public UserRepository(UnilevelContext context, IEmailServices emailServices) 
        {
            _context = context;
            _emailService = emailServices;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VeryfiPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }    
        }

        public async Task CreateUserAsync(AddUserDTO user)
        {
            var userEmail = _context.Users.SingleOrDefault(u => u.Email == user.Email);
            if (userEmail != null) { throw new Exception("Email already exist"); }
            Random rand = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int stringlen = rand.Next(8, 16);
            string password = "";
            for (int i = 0; i < stringlen; i++)
            {
                int x = rand.Next(chars.Length);

                password += chars[x];
            }

            EmailDTO emailDTO = new EmailDTO();
            emailDTO.To = user.Email;
            emailDTO.Subject = "Login Infor";
            emailDTO.Body = "<h1>Login Infor</h1> <p>Username:" + user.Email + "</p> </br> <p>Password:" + password + "</p>"; 
            _emailService.SendEmail(emailDTO);
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            string id = DateTime.Now.ToString("ddMMyyhhmmssfffffff");

            var userID = _context.Users.FirstOrDefault(u => u.Id == id);
            if(userID != null) { throw new Exception("user id already exist"); }
            User userData = new User()
            {
                Id = id,
                FullName = user.FullName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = user.Email,
                RoleId = user.RoleId,
                AreaCode = user.AreaCode,
                ReportTo = user.ReportTo,
                Status = user.Status,
                CreatedDate = DateTime.Now
            };

            _context.Add(userData);
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> LoginAsync(UserLoginDTO user)
        {
            var us = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (us == null) { throw new Exception("user not found"); }
            if (!VeryfiPasswordHash(user.Password, us.PasswordHash, us.PasswordSalt))
            {
                throw new Exception("wrong password");
            }
            byte [] pass = us.PasswordHash;
            return pass;
        }
    }
}
