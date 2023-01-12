using Microsoft.AspNetCore.Mvc;
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

        // GET: Question/List
        [HttpGet("List")]
        public async Task<IActionResult> GetAllQues()
        {
            var questions = await _questionRepository.GetAllQuestionAsync();
            return Ok(questions);
        }

        // GET: Question/List/NotAddSurveyOrRemove
        [HttpGet("List/NotAddSurveyOrRemove")]
        public async Task<IActionResult> GetAllQuesNotAddSurveyOrRemove()
        {
            var questions = await _questionRepository.GetQuesNotAddSurveyOrRemoveAsync();
            return Ok(questions);
        }

        // GET: Question/QuestionDetails
        [HttpGet("Details")]
        public async Task<IActionResult> QuestionDetails(string quesId)
        {
            try
            {
                var questions = await _questionRepository.QuestionDetailAsync(quesId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DELETE: Question/Remove/{quesId}
        [HttpDelete("Remove/{quesId}")]
        public async Task<IActionResult> RemoveQues(string quesId)
        {
            try
            {
                await _questionRepository.RemoveQuestionAsync(quesId);
                return Ok(new {Message = "Successful delete" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // PUT: Question/Edit/{quesId}
        [HttpPut("Edit/{quesId}")]
        public async Task<IActionResult> EditQuestion(AddOrEditQuestion question ,string quesId)
        {
            try
            {
                if (question.Title == string.Empty || question.AnswerA == string.Empty
                                                   || question.AnswerB == string.Empty
                                                   || question.AnswerC == string.Empty
                                                   || question.AnswerD == string.Empty)
                    { return BadRequest(new { Error = "Invalid input" }); }

                await _questionRepository.EditQuestionAsync(question, quesId);
                return Ok(new {Message = "Edit successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(new {Error = ex.Message});
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
                    { return BadRequest(new {Error = "Invalid input"}); }

                await _questionRepository.AddQuestionAsync(question);
                return Ok(new {Message = "Successfully added new" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
