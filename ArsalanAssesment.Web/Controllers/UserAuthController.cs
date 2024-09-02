using ArsalanAssesment.Web.DTOs.UserDTOs;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArsalanAssesment.Web.Controllers
{
    [ApiController]
	[Route("/api/users/")]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthRepository _userAuthRepository;

        public UserAuthController(IUserAuthRepository userAuthRepository)
        {
            _userAuthRepository = userAuthRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            var response = await _userAuthRepository.RegisterUser(registerUserDTO);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(LoginUserDTO loginUserDTO)
        {
            var response = await _userAuthRepository.Login(loginUserDTO);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var response = await _userAuthRepository.GetUser(id);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDTO updateUserDTO)
        {
            var response = await _userAuthRepository.UpdateUser(id, updateUserDTO);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _userAuthRepository.DeleteUser(id);

            response.StatusCode = (response.Result is null) ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;

            return Ok(response);
        }



    }
}
