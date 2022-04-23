using System.IdentityModel.Tokens.Jwt;
using Budgeteer.Application.Domain;
using BudgetUs.Api.Auth;
using BudgetUs.Api.Models;
using BudgetUs.Api.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Budgeteer.Api.Controllers;

[ApiController]
public class AuthController : Controller
{
     private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<User> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseViewModel>> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new LoginResponseViewModel { GeneralErrorMessage = "Invalid Authentication" });

            return Ok(await GenerateResponseModelAsync(user));
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseViewModel>> Register(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                RefreshToken = _tokenService.GenerateRefreshToken()
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
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
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != model.RefreshToken)
                return BadRequest(GenerateFaultyResponse("Invalid client request"));

            return Ok(await GenerateResponseModelAsync(user));
        }

        private async Task<LoginResponseViewModel> GenerateResponseModelAsync(User user)
        {
            var signingCredentials = _tokenService.GetSigningCredentials(); 
            var claims = await _tokenService.GetClaimsAsync(user, await _userManager.GetRolesAsync(user)); 
            var tokenOptions = _tokenService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            
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
                GeneralErrorMessage = message
            };
}