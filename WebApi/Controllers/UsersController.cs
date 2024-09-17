using System;
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
public class UsersController(IUserRepository userRepository) : ApiControllerBase
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
}
