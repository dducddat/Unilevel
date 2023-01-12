using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IQuestionRepository
    {
        public Task AddQuestionAsync(AddOrEditQuestion question);
        public Task EditQuestionAsync(AddOrEditQuestion question, string quesId);
        public Task<QuestionDetail> QuestionDetailAsync(string quesId);
        public Task<List<ViewQuestion>> GetAllQuestionAsync();
        public Task RemoveQuestionAsync(string quesId);
        public Task<List<ViewQuestion>> GetQuesNotAddSurveyOrRemoveAsync();
    }
}
