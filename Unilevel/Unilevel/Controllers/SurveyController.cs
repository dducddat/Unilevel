using Microsoft.AspNetCore.Mvc;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        public async Task<IActionResult> RequestSurvey(AddOrEditSurvey survey)
        {
            if (survey.Title == string.Empty) return BadRequest(new { Error = "Title cannot be blank" });
            var result = await _surveyRepository.CreateSurveyAsync(survey);
            return Ok(result);
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

        // GET: Survey/AllSurveyOfUser
        [HttpGet("AllSurveyOfUser")]
        public async Task<IActionResult> GetAllSurveyOfUserId(string userId)
        {
            try
            {
                var surveys = await _surveyRepository.GetAllSurveyOfUserIdAsync(userId);
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: Survey/QuestionOfSurveyId/{surveyId}
        [HttpGet("QuestionOfSurveyId/{surveyId}")]
        public async Task<IActionResult> GetAllQuestionBySurveyId(string surveyId)
        {
            var listQuestion = await _surveyRepository.GetAllQuestionBySurveyIdAsync(surveyId);

            return Ok(listQuestion);
        }

        [HttpPost("ResultSurvey/{userId}")]
        public async Task<IActionResult> ResultSurveyOfUser(ResultSurveyModel resultSurvey, string userId)
        {
            try
            {
                await _surveyRepository.ResultSurveyOfUserAsync(resultSurvey, userId);
                return Ok(new { Message = "Successful survey" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
