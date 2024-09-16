using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Controllers;

[Authorize]
public class UsersController(DatingAppDBContext dBContext) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        return await dBContext.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await dBContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if(user == null){
            return NotFound();
        }
        return  user;
    }
}
