using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetAllQues()
        {
            var questions = await _questionRepository.GetAllQuestion();
            return Ok(questions);
        }

        [HttpGet("List/NotAddSurveyOrRemove")]
        public async Task<IActionResult> GetAllQuesNotAddSurveyOrRemove()
        {
            var questions = await _questionRepository.GetQuesNotAddSurveyOrRemove();
            return Ok(questions);
        }

        [HttpDelete("Remove/{quesId}")]
        public async Task<IActionResult> RemoveQues(string quesId)
        {
            try
            {
                await _questionRepository.RemoveQuestion(quesId);
                return Ok(new APIRespone(true, "success"));
            }
            catch(Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // POST: Question/Create
        [HttpPost("Create")]
        public async Task<IActionResult> AddQuestion(AddOrEditQuestion question)
        {
            try
            {
                if (question.Title == string.Empty || question.AnswerA == string.Empty
                                               || question.AnswerB == string.Empty
                                               || question.AnswerC == string.Empty
                                               || question.AnswerD == string.Empty)
                { return BadRequest(new APIRespone(false, "Invalid input")); }

                await _questionRepository.AddQuestionAsync(question);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
