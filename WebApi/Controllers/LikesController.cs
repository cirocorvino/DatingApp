using System;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Extensions;
using WebApi.Interfaces;

namespace WebApi.Controllers;

public class LikesController(ILikesRepository likesRepository) : ApiControllerBase
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");

        var existringLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

        if(existringLike == null){
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            likesRepository.AddLike(like);
        }
        else {
            likesRepository.DeleteLike(existringLike);
        }

        if (await likesRepository.SaveChanges()) return Ok();

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes(string predicate)
    {
        var users = await likesRepository.GetUserLikes(predicate, User.GetUserId());

        return Ok(users);
    }

}
