using System.IdentityModel.Tokens.Jwt;
using Budgeteer.Api.Models;
using Budgeteer.Api.Models.Auth;
using Budgeteer.Application.Domain;
using Budgeteer.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Budgeteer.Api.Controllers;

[Route("auth")]
public class AuthApiController : ApiControllerBase
{
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthApiController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseViewModel>> Login(LoginViewModel model)
        {
            var userResult = await _userService.FindByNameAsync(model.Username);
            if (!userResult)
            {
                return userResult.Error switch
                {
                    AuthServiceError.NotFound => Unauthorized(userResult.Error, "Invalid Credentials"),
                    AuthServiceError.InvalidCredentials => Unauthorized(userResult.Error, "Invalid Credentials"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }

            var passwordCheckResult = await _userService.CheckPasswordAsync(userResult.Result, model.Password);
            
            return Ok(await GenerateResponseModelAsync(userResult.Result));
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseViewModel>> Register(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                RefreshToken = _tokenService.GenerateRefreshToken()
            };
            
            var result = await _userService.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(GenerateFaultyResponse(result.Errors));

            return Ok(await GenerateResponseModelAsync(user));
        }
        
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseViewModel>> Refresh(RefreshViewModel model)
        {
            if (model is null)
            {
                return BadRequest(GenerateFaultyResponse("Provide token and refresh token"));
            }
            var principal = _tokenService.GetPrincipalFromExpiredToken(model.Token);
            var username = principal.Identity.Name;
            var user = await _userService.FindByNameAsync(username);
            if (user == null || user.RefreshToken != model.RefreshToken)
                return BadRequest(GenerateFaultyResponse("Invalid client request"));

            return Ok(await GenerateResponseModelAsync(user));
        }

        private async Task<LoginResponseViewModel> GenerateResponseModelAsync(User user)
        {
            var signingCredentials = _tokenService.GetSigningCredentials(); 
            var claims = await _tokenService.GetClaimsAsync(user, await _userService.GetRolesAsync(user)); 
            var tokenOptions = _tokenService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            await _userService.UpdateAsync(user);
            
            return new LoginResponseViewModel { 
                Token = token,
                TokenValidUntil = tokenOptions.ValidTo,
                RefreshToken = user.RefreshToken
            };
        }
        
        private static LoginResponseViewModel GenerateFaultyResponse(IEnumerable<IdentityError> resultErrors)
            => new()
            {
                Errors = resultErrors.Select(x => new ErrorModel
                {
                    Code = x.Code,
                    Message = x.Description
                }).ToList()
            };
        
        private static LoginResponseViewModel GenerateFaultyResponse(string message)
            => new() {
                ErrorMessage = message
            };
}