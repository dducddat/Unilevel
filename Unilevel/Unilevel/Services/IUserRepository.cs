using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IUserRepository
    {
        public Task CreateUserAsync(AddUser user);
        public Task<TokenModel> LoginAsync(UserLogin user);
        public Task<List<UserInfo>> GetAllUserAsync();
        public Task DeleteUserAsync(string id);
        public Task EditInfoUserAsync(EditInfoUser user, string id);
        public Task EditRoleUserAsync(string id, string roleId);
        public Task ImportUserFromFileExcelAsync(List<FileExcelUser> excelUsers);
        public Task<TokenModel> RefreshTokenAsync(TokenModel token);
        public Task ChangePasswordAsync(string userId, string password, string newPassword);
        public Task Logout(string userId);
        public Task UserSendResultSurveyAsync(ResultSurveyModel resultSurvey, string userId);
        public Task<List<SurveyList>> GetAllSurveyOfUserIdAsync(string userId);
        public Task<List<QuestionDetail>> GetAllQuestionBySurveyIdAsync(string surveyId);
    }
}
