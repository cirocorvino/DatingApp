using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MemberDto>>> Get()
    {
        var users = (await userRepository.GetMembersAsync()).ToList();

        return users;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> Get(string username)
    {
        var user = await userRepository.GetMemberAsync(username);
        if(user == null){
            return NotFound();
        }

        return  user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(username == null) {
            return BadRequest("No username found in token");
        }

        var user = await userRepository.GetUserByUsernameAsync(username);
        if(user == null) {
            return BadRequest("User not found");
        }

        mapper.Map(memberUpdateDto, user);

        if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }
}
