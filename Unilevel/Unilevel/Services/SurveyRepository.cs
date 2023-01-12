using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
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
                var userDid = _context.RequestSurveys.Where(rs => rs.SurveyId == survey.Id)
                                                     .Where(rs => rs.Status == true)
                                                     .Count();
                var userRequest = _context.RequestSurveys.Count(rs => rs.SurveyId == survey.Id);

                surveyLists.Add(new SurveyList {
                    SurveyId = survey.Id,
                    TitleSurvey = survey.Title,
                    StartDate = survey.StartDate,
                    EndDate = survey.EndDate,
                    NumberOfPeopleDid = $"{userDid}/{userRequest}"
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

            var requestSurvey = await _context.RequestSurveys.Where(rs => rs.SurveyId == surveyId)
                                                             .Where(rs => rs.Status == finish).ToListAsync();

            if (requestSurvey != null)
            {
                userDidOrDontDo.Users = new List<UserInfo>();

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

        public async Task<List<SurveyList>> GetAllSurveyOfUserIdAsync(string userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new Exception();

            var requestSurveys = await _context.RequestSurveys.Where(rs => rs.UserId == userId)
                                                              .Join( _context.Surveys, 
                                                                     rs => rs.SurveyId,
                                                                     s => s.Id,
                                                                     (rs, s) => new {SurveyId = rs.SurveyId,
                                                                                     Title = s.Title,
                                                                                     EndDate = s.EndDate}) 
                                                              .ToListAsync();

            List<SurveyList> surveys = new List<SurveyList>();
            foreach (var requestSurey in requestSurveys)
            {
                surveys.Add(new SurveyList() {
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

            if(questions != null)
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

        public async Task ResultSurveyOfUserAsync(ResultSurveyModel resultSurvey, string userId)
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
                _context.Add(new ResultSurvey { Id = Guid.NewGuid().ToString(),
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
    }
}
