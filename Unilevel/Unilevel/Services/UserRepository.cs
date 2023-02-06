using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Unicode;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UnilevelContext _context;
        private readonly IEmailServices _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserRepository(UnilevelContext context, IEmailServices emailServices, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _emailService = emailServices;
            _configuration = configuration;
            _mapper = mapper;
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
            if (userEmail != null) { throw new DuplicateNameException($"email {user.Email} already exist"); }
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

            string id = DateTime.Now.ToString("ddMMyyHHmmssfffffff");

            var userID = _context.Users.FirstOrDefault(u => u.Id == id);
            if (userID != null) { throw new Exception("user id already exist, please wait a second and recreate"); }
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
            if (user.Email == string.Empty || user.Password == string.Empty) { throw new Exception("invalid username/password"); }
            var us = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (us == null) { throw new Exception("invalid username/password"); }
            if (!VeryfiPasswordHash(user.Password, us.PasswordHash, us.PasswordSalt))
            {
                throw new Exception("wrong password");
            }
            if (!us.Status) throw new Exception("Account has been disabled");

            var token = GenerateToken(us);

            return token;
        }

        public async Task<TokenModel> RefreshTokenAsync(TokenModel token)
        {
            if (token is null) throw new Exception("Invalid client request");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("SecretKey").Value)),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwtTokenHandler.ValidateToken(token.AccessToken,
                tokenValidationParameters, out var validateToken);
            if (!(validateToken is JwtSecurityToken jwtSecurityToken)) throw new Exception("access token invalid format");

            var result = jwtSecurityToken.Header.Alg.Equals
                    (SecurityAlgorithms.HmacSha512,
                    StringComparison.InvariantCultureIgnoreCase);

            if (!result) throw new SecurityTokenException("Invalid token");

            var jwtId = tokenInVerification.FindFirstValue(JwtRegisteredClaimNames.Jti);

            var storedToken = _context.RefreshTokens.SingleOrDefault(r => r.JwtId == jwtId);

            if (storedToken is null || storedToken.RefToken != token.RefreshToken)
                throw new Exception("Invalid client request");
            if (storedToken.Expires <= DateTime.Now)
            {
                _context.Remove(storedToken);
                await _context.SaveChangesAsync();
                throw new Exception("refresh token has expried please login agian");
            }

            var user = _context.Users.Include(u => u.Role).SingleOrDefault(u => u.Id == storedToken.UserId);
            var newToken = GenerateToken(user);
            _context.Remove(storedToken);
            await _context.SaveChangesAsync();

            return newToken;
        }

        private TokenModel GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            string JwtId = DateTime.Now.ToString("ddMMyyhhmmssfffffff");

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, JwtId),
                    new Claim(ClaimTypes.Role, user.RoleId)
                }),
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = signinCredentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            string refreshToken = GenerateRefreshToken();

            _context.Add(new RefreshToken {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefToken = refreshToken,
                JwtId = JwtId,
                Expires = DateTime.Now.AddDays(7)
            });
            _context.SaveChanges();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
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

        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.Status = false;
            _context.Update(user);
            _context.SaveChanges();
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
            else if (userData == null)
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

        public async Task ImportUserFromFileExcelAsync(List<FileExcelUser> excelUsers)
        {
            var fileExcelUsers = excelUsers;
            foreach (var user in fileExcelUsers) {
                var role = _context.Roles.FirstOrDefault(r => r.Name == user.Role && r.Remove == false);
                var reportToRole = _context.Roles.FirstOrDefault(r => r.Name == user.ReportTo && r.Remove == false);
                if (role != null && reportToRole != null)
                {
                    try
                    {
                        await CreateUserAsync(new AddUser()
                        {
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleId = role.Id,
                            Status = false,
                            ReportTo = reportToRole.Id
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"User:{user.FullName} {user.Role} {user.ReportTo} {ex.Message} ");
                    }
                }
                else
                {
                    throw new Exception($"User: {user.FullName} {user.Email} Role/ReportTo {user.Role}/{user.ReportTo} not exist");
                }
            }
        }

        public async Task ChangePasswordAsync(string userId, string password, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var permission = await _context.LinkRoleMenus.Include(lrm => lrm.Menu).Where(lrm => lrm.RoleId == user.RoleId).ToListAsync();

            if (!permission.Where(p => p.Menu.Permission == "User.ChangePassword").Any())
                throw new Exception("Access denied");

            if (!VeryfiPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            { throw new Exception("Old password is not correct"); }
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Logout(string userId)
        {
            var storedToken = _context.RefreshTokens.Where(r => r.UserId == userId).ToList();

            if (storedToken == null)
                throw new Exception("Not found");

            foreach (var token in storedToken) {
                _context.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SurveyList>> GetAllSurveyOfUserIdAsync(string userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new Exception("User not found");

            var requestSurveys = await _context.RequestSurveys.Where(rs => rs.UserId == userId)
                                                              .Where(rs => rs.Status == false)
                                                              .Join(_context.Surveys,
                                                                     rs => rs.SurveyId,
                                                                     s => s.Id,
                                                                     (rs, s) => new {
                                                                         SurveyId = rs.SurveyId,
                                                                         Title = s.Title,
                                                                         EndDate = s.EndDate
                                                                     })
                                                              .ToListAsync();

            List<SurveyList> surveys = new List<SurveyList>();
            foreach (var requestSurey in requestSurveys)
            {
                surveys.Add(new SurveyList()
                {
                    SurveyId = requestSurey.SurveyId,
                    TitleSurvey = requestSurey.Title,
                    EndDate = requestSurey.EndDate
                });
            }

            return surveys;
        }

        public async Task<List<QuestionDetail>> GetAllQuestionBySurveyIdAsync(string surveyId)
        {
            var questions = await _context.Questions.Where(q => q.SurveyId == surveyId).ToListAsync();

            List<QuestionDetail> ListQuestion = new List<QuestionDetail>();

            if (questions != null)
            {
                foreach (var question in questions)
                {
                    ListQuestion.Add(new QuestionDetail
                    {
                        Id = question.Id,
                        Title = question.Title,
                        AnswerA = question.AnswerA,
                        AnswerB = question.AnswerB,
                        AnswerC = question.AnswerC,
                        AnswerD = question.AnswerD,
                        SurveyId = question.SurveyId
                    });
                }
            }

            return ListQuestion;
        }

        public async Task UserSendResultSurveyAsync(ResultSurveyModel resultSurvey, string userId)
        {
            var requestSurvey = await _context.RequestSurveys
                .Where(rs => rs.SurveyId == resultSurvey.SurveyId)
                .Where(rs => rs.UserId == userId)
                .SingleOrDefaultAsync();

            if (requestSurvey is null)
                throw new Exception("Request survey not found");
            else if (requestSurvey.Status == true)
                throw new Exception("User who have taken the survey");

            requestSurvey.Status = true;

            _context.Update(requestSurvey);
            await _context.SaveChangesAsync();

            foreach (var result in resultSurvey.Results)
            {
                _context.Add(new ResultSurvey
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    QuestionId = result.QuestionId,
                    ResultA = result.ResultA,
                    ResultB = result.ResultB,
                    ResultC = result.ResultC,
                    ResultD = result.ResultD,
                    SurveyId = resultSurvey.SurveyId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<dynamic> ForgotPassword(string emailUser, string url)
        {
            var user = await _context.Users.Where(u => u.Email == emailUser)
                .SingleOrDefaultAsync();

            if (user is null) throw new Exception("Email invalid");

            var permission = await _context.LinkRoleMenus.Include(lrm => lrm.Menu).Where(lrm => lrm.RoleId == user.RoleId).ToListAsync();

            if (!permission.Where(p => p.Menu.Permission == "User.ResetPassword").Any())
            {
                return new { Message = "Access denied" };
            }

            string token = GenerateConfirmMailToken(user);

            EmailModel email = new EmailModel();
            email.To = user.Email;
            email.Subject = "Fogot Password";
            email.Body = "<p><big>You need to confirm your email by clicking the link below</big></p>" +
                "<p><big>" + url +
                "?email=" + user.Email +
                "&token=" + token +
                "</big></p>";
            _emailService.SendEmail(email);

            Console.WriteLine(token);

            return new { Status = "Successful", Message = "We have sent an email on " + user.Email + " please open the email and click the link" };
        }

        public async Task ResetPassword(ResetPassword resetPassword)
        {
            try
            {
                var user = await _context.Users.Where(u => u.Email == resetPassword.Email)
                .SingleOrDefaultAsync();

                if (user is null) throw new Exception("Email invalid");

                if (!CheckTokenConfirmMail(resetPassword.Token)) throw new Exception("Invalid token");

                if (resetPassword.NewPassword != resetPassword.ConfirmNewPassword) throw new Exception("Please enter a new password and confirm the same password");

                CreatePasswordHash(resetPassword.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;

                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<dynamic> ConfirmMail(string token, string email)
        {
            var user = await _context.Users.Where(u => u.Email == email)
                .SingleOrDefaultAsync();

            if (user is null) throw new Exception("Email invalid");

            try
            {
                if (!CheckTokenConfirmMail(token))
                    return new { Error = "Invalid token" };

                return new { Message = "Successful", Token = GenerateConfirmMailToken(user) };
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }

        private string GenerateConfirmMailToken(User user)
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value));

                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

                string JwtId = DateTime.Now.ToString("ddMMyyhhmmssfffffff");

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, JwtId)
                }),
                    Expires = DateTime.Now.AddMinutes(2),
                    SigningCredentials = signinCredentials
                };

                var token = jwtTokenHandler.CreateToken(tokenDescription);
                var accessToken = jwtTokenHandler.WriteToken(token);

                return accessToken;
            }

        private bool CheckTokenConfirmMail(string token)
            {
                if (token is null) throw new Exception("Invalid client request");

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_configuration.GetSection("SecretKey").Value)),

                    ClockSkew = TimeSpan.Zero,

                    ValidateLifetime = false
                };

                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var tokenInVerification = jwtTokenHandler.ValidateToken(token,
                    tokenValidationParameters, out var validateToken);
                if (!(validateToken is JwtSecurityToken jwtSecurityToken)) throw new Exception("access token invalid format");

                var result = jwtSecurityToken.Header.Alg.Equals
                        (SecurityAlgorithms.HmacSha512,
                        StringComparison.InvariantCultureIgnoreCase);

                if (!result) throw new SecurityTokenException("Invalid token");

                var expireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x =>
                x.Type == JwtRegisteredClaimNames.Exp).Value);

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (expireDate <= now) throw new Exception("Token has expired");

                return true;
            }

        public async Task<ProfileUser> GetProfileUserAsync(string userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).Select(u => new {Name = u.FullName, CreateDate = u.CreatedDate}).SingleOrDefaultAsync();

            if (user == null)
                throw new Exception("User not found");

            var taskDone = _context.Jobs.Include(cm => cm.User).Where(t => t.CreateByUserId == userId && t.Status != 1 && t.Status != 2 && t.Status != 3)
                .OrderByDescending(t => t.CreateDate)
                .ToList();

            var taskNotDone = _context.Jobs.Include(cm => cm.User).Where(t => t.CreateByUserId == userId && t.Status != 4 && t.Status != 5)
                .OrderByDescending(t => t.CreateDate)
                .ToList();

            var lastComment = _context.Comments.Include(cm => cm.User).Where(cm => cm.JobId == taskDone.First().Id).Take(3).ToList();

            var courses =  _context.Courses.Where(course => course.UserId == userId).ToList();

            return new ProfileUser
            {
                FullName = user.Name,
                CratedDate = user.CreateDate.ToString("dd-MM-yyyy"),
                TaskDone = _mapper.Map<List<JobSummary>>(taskDone),
                TaskNotDone = _mapper.Map<List<JobSummary>>(taskNotDone),
                LastComment = _mapper.Map<List<CommentSummary>>(lastComment),
                Courses = _mapper.Map<List<CourseModel>>(courses)
            };
        }
    }
} 
