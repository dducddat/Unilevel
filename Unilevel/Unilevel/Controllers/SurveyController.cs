using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageSurvey")]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyController(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        // GET: Survey/List
        [HttpGet("List")]
        public async Task<IActionResult> GetAllSurvey()
        {
            var surveyLists = await _surveyRepository.GetAllSurveyAsync();
            return Ok(surveyLists);
        }

        // GET: Survey/UsersHaveNotTakenSurvey/{surveyId}
        [HttpGet("UsersHaveNotTakenSurvey/{surveyId}")]
        public async Task<IActionResult> UsersHaveNotTakenSurvey(string surveyId)
        {
            try
            {
                var userDontDo = await _surveyRepository.DidAndDidNotDoTheSurveyAsync(surveyId, false);

                return Ok(userDontDo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: Survey/UsersWhoHaveTakenSurvey/{surveyId}
        [HttpGet("UsersWhoHaveTakenSurvey/{surveyId}")]
        public async Task<IActionResult> UsersWhoHaveTakenSurvey(string surveyId)
        {
            try
            {
                var userDid = await _surveyRepository.DidAndDidNotDoTheSurveyAsync(surveyId, true);

                return Ok(userDid);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: Survey/Create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateSurvey(AddOrEditSurvey survey)
        {
            if (survey.Title == string.Empty) return BadRequest(new { Error = "Title cannot be blank" });
            var result = await _surveyRepository.CreateSurveyAsync(survey);
            return Ok(result);
        }

        // PUT: Survey/Edit/{surveyId}
        [HttpPut("Edit/{surveyId}")]
        public async Task<IActionResult> EditSurvey(AddOrEditSurvey survey, string surveyId)
        {
           try
            {
                if (survey.Title == string.Empty) return BadRequest(new { Error = "Title cannot be blank" });
                await _surveyRepository.EditSurveyAsync(survey, surveyId);
                return Ok(new { Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new {Error = ex.Message});
            }
        }

        // POST: Survey/AddQuestion/{surveyId}
        [HttpPost("AddQuestion/{surveyId}")]
        public async Task<IActionResult> AddQuestion(string surveyId, AddListQuestionId ListQuestionId)
        {
            try
            {
                List<string> error =  await _surveyRepository.AddQuestionAsync(surveyId, ListQuestionId);
                if (error.Any()) return BadRequest(new { success = false, error });
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: Survey/RequestSurvey/AddUser/{surveyId}
        [HttpPost("RequestSurvey/AddUser/{surveyId}")]
        public async Task<IActionResult> RequestSurveyAddUser(string surveyId, SendRequestSurvey requestSurvey)
        {
            try
            {
                List<string> error = await _surveyRepository.RequestSurveyAsync(surveyId, requestSurvey, true);
                if (error.Any()) return BadRequest(new { success = false, error });
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: Survey/RequestSurvey/Remove/{surveyId}
        [HttpPost("RequestSurvey/Remove/{surveyId}")]
        public async Task<IActionResult> RequestSurveyRemoveUser(string surveyId, SendRequestSurvey requestSurvey)
        {
            try
            {
                List<string> error = await _surveyRepository.RequestSurveyAsync(surveyId, requestSurvey, false);
                if (error.Any()) return BadRequest(new { success = false, error });
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: Survey/SurveyDetail/{surveyId}
        [HttpGet("Detail/{surveyId}")]
        public async Task<IActionResult> SurveyDetail(string surveyId)
        {
            try
            {
                var questionDetails = await _surveyRepository.SurveyDetailAsync(surveyId);
                return Ok(questionDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DELETE: Survey/Remove/{surveyId}
        [HttpDelete("Remove/{surveyId}")]
        public async Task<IActionResult> RemoveSurvey(string surveyId)
        {
            try
            {
                await _surveyRepository.RemoveSurveyAsync(surveyId);
                return Ok(new { Message = "Successful delete" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: Survey/UserResultSurveyInfor/{userId}/{surveyId}}
        [HttpGet("UserResultSurveyInfor/{userId}/{surveyId}")]
        public async Task<IActionResult> UserResultSurveyInfor(string userId, string surveyId)
        {
            var list = await _surveyRepository.UserSurveyResultsInforAsync(userId, surveyId);

            return Ok(list);
        }

        // GET: Survey/ListQuestion
        [HttpGet("ListQuestion")]
        public async Task<IActionResult> GetAllQuesNotAddSurveyOrRemove()
        {
            var questions = await _surveyRepository.GetQuesNotAddSurveyOrRemoveAsync();
            return Ok(questions);
        }
    }
}
