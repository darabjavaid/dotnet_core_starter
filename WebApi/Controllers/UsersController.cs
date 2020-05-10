using DT.Domain.DTO;
using DT.Domain.DTO.Users;
using DT.Domain.Exceptions;
using DT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;       
        ServiceResponse _response = null;

        public UsersController(
            IUserService userService)
        {
            _userService = userService;            
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            try
            {
                var user = _userService.Authenticate(model.Username, model.Password);

                if (user == null)
                    throw new AppException("Username or password is incorrect");
               
                // return basic user info and authentication token
                return Ok(user);
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            try
            {
                // create user
                await _userService.Create(model, model.Password);
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

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var model = _userService.GetAll();
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

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserId = currentUser.FindFirst(ClaimTypes.Sid)?.Value;

                // only allow admins to access other user records
                if (id != new Guid(currentUserId) && !User.IsInRole(Role.Admin))
                    return Forbid();

                var model = _userService.GetById(id);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]UpdateModel model)
        {
            try
            {
                // update user 
                await _userService.Update(id, model, model.Password);
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
        public IActionResult Delete(Guid id)
        {
            try
            {
                _userService.Delete(id);
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
