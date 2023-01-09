using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IQuestionRepository
    {
        public Task AddQuestionAsync(AddOrEditQuestion question);
        public Task EditQuestionAsync(AddOrEditQuestion question, string quesId);
        public Task<QuestionDetail> GetQuestionById(string quesId);
        public Task<List<ViewQuestion>> GetAllQuestion();
        public Task RemoveQuestion(string quesId);
        public Task<List<ViewQuestion>> GetQuesNotAddSurveyOrRemove();
        public Task AddQuesToSurvey(string quesId, string surveyId);
    }
}
