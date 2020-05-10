using DT.Domain.DTO;
using DT.Domain.DTO.Questionnaire;
using DT.Domain.Exceptions;
using DT.Interfaces.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Areas.CMS
{
    [Authorize]
    [Route("api/CMS/[controller]")]
    [ApiController]
    public class QuestionnaireController : ControllerBase
    {
        private IQuestionnaireService _questionnaireService;
        ServiceResponse _response = null;

        public QuestionnaireController(IQuestionnaireService questionnaireService)
        {
            _questionnaireService = questionnaireService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _questionnaireService.GetAll());
            }
            catch (AppException ex)
            {
                _response = new ServiceResponse();
                _response.message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, _response);
            }
            catch (Exception ex)
            {
                _response = new ServiceResponse();
                _response.message = "Something went wrong, " + ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var model = await _questionnaireService.GetById(id);
                
                if (model == null)
                    return NotFound();

                return Ok(model);
            }
            catch (AppException ex)
            {
                _response = new ServiceResponse();
                _response.message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, _response);
            }
            catch (Exception ex)
            {
                _response = new ServiceResponse();
                _response.message = "Something went wrong, " + ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]QuestionnaireCreateDTO model)
        {
            try
            {
                // create user
                await _questionnaireService.Create(model, User.Identity.Name);
                return Ok();
            }
            catch (AppException ex)
            {
                _response = new ServiceResponse();
                _response.message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, _response);
            }
            catch (Exception ex)
            {
                _response = new ServiceResponse();
                _response.message = "Something went wrong, " + ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]QuestionnaireDTO model)
        {
            try
            {
                // update user 
                await _questionnaireService.Update(id, model, User.Identity.Name);
                return Ok();
            }
            catch (AppException ex)
            {
                _response = new ServiceResponse();
                _response.message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, _response);
            }
            catch (Exception ex)
            {
                _response = new ServiceResponse();
                _response.message = "Something went wrong, " + ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _questionnaireService.Delete(id);
                return Ok();
            }
            catch (AppException ex)
            {
                _response = new ServiceResponse();
                _response.message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, _response);
            }
            catch (Exception ex)
            {
                _response = new ServiceResponse();
                _response.message = "Something went wrong, " + ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}