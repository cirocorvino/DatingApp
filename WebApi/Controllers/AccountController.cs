using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Controllers;

public class AccountController(DatingAppDBContext dBContext, ITokenService tokenService) : ApiControllerBase
{

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto) 
    {
        if(await UserExists(dto.Username)) return BadRequest("user already exists");

        return Ok();

        // using var hmac = new HMACSHA512();

        // var user = new User{
        //     UserName = dto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
        //     PasswordSalt = hmac.Key 
        // };

        // dBContext.Users.Add(user);
        // await dBContext.SaveChangesAsync();

        // return new UserDto{
        //     Username = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {

        var user = await dBContext.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.Username.ToLower());
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
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username) 
    {
        return await dBContext.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }

}
