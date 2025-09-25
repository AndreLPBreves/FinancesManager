using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController(
        RegisterService registerService,
        AuthService authService,
        MailingService mailingService,
        EmailConfirmationService emailConfirmationService
    ) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500, Type = typeof(MessageResponseDTO))]
        public async Task<ActionResult> Register([FromBody] UserRegistrationDTO userData)
        {
            var result = await registerService.NewUser(userData);
            switch (result.StatusCode)
            {
                case Common.StatusCode.OK:
                case Common.StatusCode.EmailAlredyRegistered:
                    //Send confirmation e-mail if it's a new account otherwise it sends an account alredy exists to the owner email and an option to change password
                    await mailingService.SendConfirmationEmail(result.Value!);
                    return StatusCode(200);
                default:
                    return StatusCode(500, new { Message = "An unknown error has occurred." });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        [ProducesResponseType(400, Type = typeof(MessageResponseDTO))]
        [ProducesResponseType(500, Type = typeof(MessageResponseDTO))]
        public async Task<ActionResult> Login([FromBody] UserLoginDTO userData)
        {
            var result = await authService.LoginUserAsync(userData.Email, userData.Password);

            return result.StatusCode switch
            {
                Common.StatusCode.OK => StatusCode(200, UserDTO.FromUser(result.Value!)),
                Common.StatusCode.InvalidCredentials => StatusCode(
                    400,
                    new { Message = "InvalidCredentials." }
                ),
                _ => StatusCode(500, new { Message = "An unknown error has occurred." }),
            };
        }

        [HttpGet("confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500, Type = typeof(string))]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string key)
        {
            var result = await emailConfirmationService.ConfirmEmail(key);
            switch (result.StatusCode)
            {
                case Common.StatusCode.OK:
                    return StatusCode(200);
                case Common.StatusCode.InvalidToken:
                    return StatusCode(400, new { Message = "Invalid Token." });
                case Common.StatusCode.ExpiredToken:
                    return StatusCode(
                        400,
                        new { Message = "Expired token. Request another validation email." }
                    );
                default:
                    return StatusCode(500, new { Message = "An unknown error has occurred." });
            }
        }
    }
}
