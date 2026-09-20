using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiDotNet.Data.Entities;
using WebApiDotNet.Models.Account;

namespace WebApiDotNet.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager;

    public AccountController(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Невірний email або пароль" });
        }

        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Невірний email або пароль" });
        }

        return Ok(new { message = "Успішний вхід", userId = user.Id, email = user.Email });
    }
}