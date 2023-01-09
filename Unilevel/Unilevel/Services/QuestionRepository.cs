using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly UnilevelContext _context;

        public QuestionRepository(UnilevelContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAsync(AddOrEditQuestion question)
        {
            string id = DateTime.Now.ToString("ddMMyyhhmmss");
            if (await _context.Questions.AnyAsync(q => q.Id == id))
                throw new Exception("Please wait a second and try again");
            _context.Add(new Question
            {
                Id = id,
                Title = question.Title,
                AnswerA = question.AnswerA,
                AnswerB = question.AnswerB,
                AnswerC = question.AnswerC,
                AnswerD = question.AnswerD,
                Status = true
            });
            await _context.SaveChangesAsync();
        }

        public async Task AddQuesToSurvey(string quesId, string surveyId)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);
            var ques = await _context.Questions.SingleOrDefaultAsync(q => q.Id == quesId);
            if (survey is null || ques is null) throw new Exception("Survey or question not found");
            ques.SurveyId = surveyId;
            ques.Status = false;
            _context.Update(ques);
            _context.SaveChanges();
        }

        public async Task EditQuestionAsync(AddOrEditQuestion question, string quesId)
        {
            var ques = await _context.Questions.SingleOrDefaultAsync(q => q.Id == quesId);
            if (ques == null) throw new Exception("Not found");
            ques.Title = question.Title;
            ques.AnswerA = question.AnswerA;
            ques.AnswerB = question.AnswerB;
            ques.AnswerC = question.AnswerC;
            ques.AnswerD = question.AnswerD;
            _context.Update(ques);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ViewQuestion>> GetAllQuestion()
        {
            var questions = await _context.Questions.ToListAsync();
            List<ViewQuestion> quesList = new List<ViewQuestion>();
            foreach (var ques in questions)
            {
                string status = ques.Status == true ? "Availble" : "Disable";
                quesList.Add(new ViewQuestion
                {
                    Id = ques.Id,
                    Status = status,
                    Title = ques.Title,
                });
            }
            return quesList;
        }

        public async Task<List<ViewQuestion>> GetQuesNotAddSurveyOrRemove()
        {
            var questions = await _context.Questions.Where(q => q.Status == true).ToListAsync();
            List<ViewQuestion> quesList = new List<ViewQuestion>();
            foreach (var ques in questions)
            {
                quesList.Add(new ViewQuestion
                {
                    Id = ques.Id,
                    Status = "Availble",
                    Title = ques.Title,
                });
            }
            return quesList;
        }

        public async Task<QuestionDetail> GetQuestionById(string quesId)
        {
            var ques = await _context.Questions.SingleOrDefaultAsync(q => q.Id == quesId);
            if (ques is null) throw new Exception("Not found");
            return new QuestionDetail
            {
                Id = ques.Id, 
                Title = ques.Title,
                AnswerA = ques.AnswerA,
                AnswerB = ques.AnswerB,
                AnswerC = ques.AnswerC,
                AnswerD = ques.AnswerD            };
        }

        public async Task RemoveQuestion(string quesId)
        {
            var ques = await _context.Questions.SingleOrDefaultAsync(q => q.Id == quesId);
            if (ques is null) throw new Exception("Question not found");
            ques.Status = false;
            _context.Update(ques);
            await _context.SaveChangesAsync();
        }
    }
}
