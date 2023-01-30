using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly UnilevelContext _context;

        public SurveyRepository(UnilevelContext context)
        {
            _context = context;
        }

        public async Task<List<string>> AddQuestionAsync(string surveyId, AddListQuestionId ListQuestionId)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                throw new Exception("Survey not found");

            List<string> ListQuesInvalid = new List<string>();

            int index = 0;

            foreach (var questionId in ListQuestionId.QuestionId)
            {
                index++;

                if (questionId == string.Empty)
                {
                    ListQuesInvalid.Add($"Can't add question number: {index} - input can't be empty");
                    continue;
                }

                var question = await _context.Questions.SingleOrDefaultAsync(q => q.Id == questionId);

                if (question == null)
                {
                    ListQuesInvalid.Add($"Can't add question number: {index} - wrong syntax or id: {questionId}");
                    continue;
                }

                question.SurveyId = surveyId;
                question.Status = false;
                _context.Update(question);
                await _context.SaveChangesAsync();
            }

            return ListQuesInvalid;
        }

        public async Task<SurveyInfo> CreateSurveyAsync(AddOrEditSurvey survey)
        {
            DateTime created = DateTime.Now;
            string id = created.ToString("ddMMyyHHmmss");

            _context.Add(new Survey { Id = id, Title = survey.Title, Created = created, Remove = false});
            await _context.SaveChangesAsync();

            return new SurveyInfo { SurveyId = id, TitleSurvey = survey.Title, Created = created };
        }

        public async Task<SurveyDetail> SurveyDetailAsync(string surveyId)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);
            if (survey == null)
                throw new Exception("Survey not found");

            var questions = await _context.Questions.Where(q => q.SurveyId == surveyId).ToListAsync();

            SurveyDetail surveyDetail = new SurveyDetail();
            surveyDetail.SurveyId = survey.Id;
            surveyDetail.TitleSurvey = survey.Title;
            surveyDetail.Created = survey.Created;
            surveyDetail.QuestionDetails = new List<QuestionDetail>();

            foreach (var question in questions)
            {
                surveyDetail.QuestionDetails.Add(new QuestionDetail { Id = question.Id,
                                                                              Title = question.Title,
                                                                              AnswerA = question.AnswerA,
                                                                              AnswerB = question.AnswerB,
                                                                              AnswerC = question.AnswerC,
                                                                              AnswerD = question.AnswerD
                                                                            });
            }

            return surveyDetail;
        }

        public async Task<List<SurveyList>> GetAllSurveyAsync()
        {
            var surveys = await _context.Surveys.Where(s => s.Remove == false).ToListAsync();

            List<SurveyList> surveyLists = new List<SurveyList>();

            foreach (var survey in surveys) {
                var userRequest = await _context.RequestSurveys.CountAsync(rs => rs.SurveyId == survey.Id);

                string numberOfPeopleDid = string.Empty;
                if (userRequest == 0)
                {
                    numberOfPeopleDid = "This survey has not been submitted yet";
                }   
                else
                {
                    var userDid = await _context.RequestSurveys.Where(rs => rs.SurveyId == survey.Id)
                                                               .Where(rs => rs.Status == true).CountAsync();

                    numberOfPeopleDid = $"{userDid}/{userRequest}";
                }    

                surveyLists.Add(new SurveyList {
                    SurveyId = survey.Id,
                    TitleSurvey = survey.Title,
                    StartDate = survey.StartDate,
                    EndDate = survey.EndDate,
                    NumberOfPeopleDid = numberOfPeopleDid
                });
            }
            return surveyLists;
        }

        public async Task<UserDidOrDontDoSurvey> DidAndDidNotDoTheSurveyAsync(string surveyId, bool finish)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);

            if (survey is null)
                throw new Exception("Survey not found");

            UserDidOrDontDoSurvey userDidOrDontDo = new UserDidOrDontDoSurvey();
            userDidOrDontDo.SurveyId = survey.Id;
            userDidOrDontDo.TitleSurvey = survey.Title;
            userDidOrDontDo.Created = survey.Created;
            userDidOrDontDo.Users = new List<UserInfo>();

            var requestSurvey = await _context.RequestSurveys.Where(rs => rs.SurveyId == surveyId)
                                                             .Where(rs => rs.Status == finish).ToListAsync();

            foreach (var item in requestSurvey)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == item.UserId);
                userDidOrDontDo.Users.Add(
                    new UserInfo
                    {
                        Id = item.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                    });
            }

            return userDidOrDontDo;
        }

        public async Task RemoveSurveyAsync(string surveyId)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);
            
            if (survey == null) 
                throw new Exception("Survey not found");

            survey.Remove = true;
            _context.Update(survey);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> RequestSurveyAsync(string surveyId, SendRequestSurvey requestSurvey, bool add)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                throw new Exception("Survey not found");

            string error = "";

            if (add)
            {
                error = "Can't send the request user number:";
            }
            else
            {
                error = "Could not remove user number:";
            }    

            List<string> ListUserInvalid = new List<string>();

            int index = 0;

            foreach (var userId in requestSurvey.UserId)
            {
                index++;

                if (userId == string.Empty)
                {
                    ListUserInvalid.Add($"{error} {index} - input can't be empty");
                    continue;
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    ListUserInvalid.Add($"{error} {index} - wrong syntax or id: {userId}");
                    continue;
                }

                if(add)
                {
                    var requestSurveyUser = await _context.RequestSurveys.Where(rs => rs.UserId == userId)
                                                                         .Where(rs => rs.SurveyId == surveyId)
                                                                         .AnyAsync();
                    if(requestSurveyUser)
                    {
                        ListUserInvalid.Add($"{error} {index} - This user has been posted survey request");
                        continue;
                    }
                    _context.Add(new RequestSurvey
                    {
                        Id = Guid.NewGuid().ToString(),
                        SurveyId = surveyId,
                        UserId = userId,
                        Status = false
                    });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var requestSurveyUser = await _context.RequestSurveys.Where(rs => rs.UserId == userId)
                                                                         .Where(rs => rs.SurveyId == surveyId)
                                                                         .FirstOrDefaultAsync();
                    if (requestSurveyUser == null)
                    {
                        ListUserInvalid.Add($"{error} {index} - This user is not in the survey request");
                        continue;
                    } 
                    else
                    {
                        _context.Remove(requestSurveyUser);
                        await _context.SaveChangesAsync();
                    }   
                }    
            }

            if (add)
            {
                survey.StartDate = requestSurvey.StartDate;
                survey.EndDate = requestSurvey.EndDate;

                _context.Update(survey);
                await _context.SaveChangesAsync();
            }

            return ListUserInvalid;
        }

        public async Task<List<UserSurveyResultsInfor>> UserSurveyResultsInforAsync(string userId, string surveyId)
        {
            var result = await _context.ResultSurveys
                .Include(rs => rs.User)
                .Include(rs => rs.Survey)
                .Include(rs => rs.Question)
                .Where(rs => rs.SurveyId == surveyId)
                .Where(rs => rs.UserId == userId)
                .ToListAsync();

            List<UserSurveyResultsInfor> listUserSurveyResultsInfor = new List<UserSurveyResultsInfor>();

            foreach (var r in result)
            {
                listUserSurveyResultsInfor.Add(new UserSurveyResultsInfor
                {
                    SurveyTitle = r.Survey.Title,
                    FullName = r.User.FullName,
                    Email = r.User.Email,
                    QuestionTitle = r.Question.Title,
                    AnswerA = r.Question.AnswerA,
                    AnswerB = r.Question.AnswerB,
                    AnswerC = r.Question.AnswerC,
                    AnswerD = r.Question.AnswerD,
                    ResultA = r.ResultA,
                    ResultB = r.ResultB,
                    ResultC = r.ResultC,
                    ResultD = r.ResultD
                });
            }

            return listUserSurveyResultsInfor;
        }

        public async Task EditSurveyAsync(AddOrEditSurvey survey, string surveyId)
        {
            var s = await _context.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId);

            if (s == null)
                throw new Exception("Survey not found");

            s.Title = survey.Title;

            _context.Update(s);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ViewQuestion>> GetQuesNotAddSurveyOrRemoveAsync()
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
    }
}
