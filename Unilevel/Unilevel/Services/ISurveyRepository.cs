using Unilevel.Models;

namespace Unilevel.Services
{
    public interface ISurveyRepository
    {
        public Task<SurveyInfo> CreateSurveyAsync(AddOrEditSurvey survey);

        public Task<SurveyDetail> SurveyDetailAsync(string surveyId);

        public Task<List<string>> AddQuestionAsync(string surveyId, AddListQuestionId ListQuestionId);

        public Task RemoveSurveyAsync(string surveyId);

        public Task<List<SurveyList>> GetAllSurveyAsync();

        public Task<UserDidOrDontDoSurvey> DidAndDidNotDoTheSurveyAsync(string surveyId, bool finish);

        public Task<List<string>> RequestSurveyAsync(string surveyId, SendRequestSurvey requestSurvey, bool add);

        public Task<List<SurveyList>> GetAllSurveyOfUserIdAsync(string userId);

        public Task<List<QuestionDetail>> GetAllQuestionBySurveyIdAsync(string surveyId);

        public Task ResultSurveyOfUserAsync(ResultSurveyModel resultSurvey, string userId);
    }
}
