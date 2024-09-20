using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Controllers;

public class AccountController(DatingAppDBContext context, ITokenService tokenService, IMapper mapper) : ApiControllerBase
{

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto) 
    {
        if(await UserExists(dto.Username)) return BadRequest("user already exists");

        using var hmac = new HMACSHA512();

        var user = mapper.Map<User>(dto);
        user.UserName = dto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await context.Users
            .Include("Photos")
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.Username.ToLower());
               
        if(user == null){
            return Unauthorized("username or password not valid");
        }

        var hmac = new HMACSHA512(user.PasswordSalt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
        for(int i = 0; i < hash.Length; i++){
            if(hash[i] != user.PasswordHash[i]) return Unauthorized("username or password not valid");
        }

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username) 
    {
        return await context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }

}
