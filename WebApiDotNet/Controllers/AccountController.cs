using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiDotNet.Data.Entities;
using WebApiDotNet.Interfaces;
using WebApiDotNet.Models.Account;

namespace WebApiDotNet.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController(
    UserManager<UserEntity> userManager,
    IJwtTokenService jwtTokenService,
    IWebHostEnvironment env) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = await jwtTokenService.CreateTokenAsync(user);
            return Ok(new { Token = token });
        }
        return Unauthorized("Не вірно вказані дані");
    }
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Користувач з таким Email вже існує" });

        // 1. Збереження фотографії
        string imageName = string.Empty;
        if (model.Image != null)
        {
            string extension = Path.GetExtension(model.Image.FileName);
            imageName = $"{Guid.NewGuid()}{extension}";

            string uploadFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string filePath = Path.Combine(uploadFolder, imageName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }
        }

        // 2. Створення сутності
        var user = new UserEntity
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            Image = imageName
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // 3. Генерація токена
        var token = await jwtTokenService.CreateTokenAsync(user);
        return Ok(new { Token = token });
    }
}