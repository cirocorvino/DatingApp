using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Controllers;

public class AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) : ApiControllerBase
{

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto) 
    {
        if(await UserExists(dto.Username)) return BadRequest("user already exists");

        using var hmac = new HMACSHA512();

        var user = mapper.Map<User>(dto);
        user.UserName = dto.Username.ToLower();

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await userManager.Users
            .Include("Photos")
                .FirstOrDefaultAsync(u => u.NormalizedUserName == dto.Username.ToUpper());
               
        if(user == null || user.UserName == null){
            return Unauthorized("username or password not valid");
        }

        var result = await userManager.CheckPasswordAsync(user, dto.Password);

        if (!result) return Unauthorized();

        return new UserDto{
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username) 
    {
        return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());
    }

}
