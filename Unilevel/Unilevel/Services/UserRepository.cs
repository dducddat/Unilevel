using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UnilevelContext _context;
        private readonly IEmailServices _emailService;
        private readonly IConfiguration _configuration;

        public UserRepository(UnilevelContext context, IEmailServices emailServices, IConfiguration configuration) 
        {
            _context = context;
            _emailService = emailServices;
            _configuration = configuration;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VeryfiPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }    
        }

        public async Task CreateUserAsync(AddUser user)
        {
            var userEmail = _context.Users.SingleOrDefault(u => u.Email == user.Email);
            if (userEmail != null) { throw new DuplicateNameException("email already exist"); }
            Random rand = new Random();
            string chars = "0123456789";
            int stringlen = rand.Next(8, 16);
            string password = "";
            for (int i = 0; i < stringlen; i++)
            {
                int x = rand.Next(chars.Length);

                password += chars[x];
            }

            EmailModel email = new EmailModel();
            email.To = user.Email;
            email.Subject = "Login Info";
            email.Body = "<p><big>Your login account info</big></p>" +
                "<p><big>Username: " + user.Email + "</big></p>" +
                "<p><big>Password: " + password + "</big></p>"; 
            _emailService.SendEmail(email);
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            string id = DateTime.Now.ToString("ddMMyyhhmmssfffffff");

            var userID = _context.Users.FirstOrDefault(u => u.Id == id);
            if(userID != null) { throw new Exception("user id already exist, please wait a second and recreate"); }
            User userData = new User()
            {
                Id = id,
                FullName = user.FullName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = user.Email,
                RoleId = user.RoleId,
                ReportTo = user.ReportTo,
                Status = user.Status,
                CreatedDate = DateTime.Now
            };

            _context.Add(userData);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenModel> LoginAsync(UserLogin user)
        {
            if (user.Email.Length == 0 || user.Password.Length == 0) { throw new Exception("invalid username/password"); }
            var us = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == user.Email);
            if (us == null) { throw new Exception("user not found"); }
            if (!VeryfiPasswordHash(user.Password, us.PasswordHash, us.PasswordSalt))
            {
                throw new Exception("wrong password");
            }

            TokenModel token = GenerateToken(us); 

            return token;
        }

        private TokenModel GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value));

            var cred = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim("Id", user.Id)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken { 
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                Created = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            _context.SaveChanges();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return refreshToken;
        }

        public async Task<List<UserInfo>> GetAllUserAsync()
        {
            var users = await _context.Users.Include(u => u.Area)
                                            .Include(u => u.Role)
                                            .Include(u => u.ReportT)
                                            .ToListAsync();
            List<UserInfo> lstUser = new List<UserInfo>();
            foreach (var user in users)
            {
                string areaName = user.AreaCode != null ? user.Area.Name : "not in area";
                string statusName = user.Status ? "Active" : "Inactive";
                lstUser.Add(new UserInfo()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = user.Role.Name,
                    AreaName = areaName,
                    ReportTo = user.ReportT.Name,
                    StatusName = statusName,
                });
            }
            return lstUser;
        }

        public async Task<List<UserInfo>> GetAllUsersNotInAreaAsync()
        {
            var users = await _context.Users.Include(u => u.Role).Where(u => u.AreaCode == null).ToListAsync();
            List<UserInfo> listUsers = new List<UserInfo>();
            foreach (var user in users)
            {
                listUsers.Add(new UserInfo
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = user.Role.Name,
                });
            }  
            return listUsers;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.Status = false;
            _context.Update(user);
            _context.SaveChanges();
        }

        public async Task AddUserIntoAreaAsync(string areaCode, string id)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.AreaCode == areaCode);
            if (area == null) { throw new Exception("area not exist"); }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.AreaCode = areaCode;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromAreaAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.AreaCode = null; 
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task EditInfoUserAsync(EditInfoUser user, string id)
        {
            var userPhone = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            var userData = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (userPhone != null)
            {
                throw new DuplicateNameException("phone already exist");
            } 
            else if (user.FullName != string.Empty && user.Address != string.Empty && user.PhoneNumber != string.Empty)
            {
                userData.FullName = user.FullName;
                userData.PhoneNumber = user.PhoneNumber;
                userData.Address = user.Address;
                _context.Update(userData);
                await _context.SaveChangesAsync();
            }
            else if(userData == null)
            {
                throw new Exception("user not exist");
            }
            else
            {
                throw new Exception("invalid fullname/address/phonenumber");
            }
        }

        public async Task EditRoleUserAsync(string id, string roleId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role != null && user != null) 
            {
                user.RoleId = role.Id;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("role/user not found");
            }
        }
    }
}
