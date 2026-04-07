using System;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(SignInManager<User> signInManager) : BaseApiController
{

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User{UserName = registerDto.Email, Email = registerDto.Email};

        await signInManager.UserManager.CreateAsync(user, registerDto.Password);
        await signInManager.UserManager.AddToRoleAsync(user, "Member");

        return Ok();
    }

    
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();

        var user = await signInManager.UserManager.GetUserAsync(User);

        if (user == null) return Unauthorized();

        var roles = await signInManager.UserManager.GetRolesAsync(user);

        return Ok(new 
        {
            user.Email,
            user.UserName,
            Roles = roles
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }
}
