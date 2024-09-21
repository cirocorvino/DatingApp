using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Interfaces;

namespace WebApi.Data;

public class LikesRepository(DatingAppDBContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(l => l.SourceUserId == currentUserId)
            .Select(l => l.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikeParams likeParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch(likeParams.Predicate) {
            case "liked":
                query = likes
                    .Where(l => l.SourceUserId == likeParams.UserId)
                    .Select(l => l.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(l => l.TargetUserId == likeParams.UserId)
                    .Select(l => l.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            default:
                //estrae gli utenti che si piacciono (quelli che hanno messo un like all'utente corrente e che ne hanno ricevuto uno da lui)
                var likeIds = await GetCurrentUserLikeIds(likeParams.UserId);
                query = likes
                    .Where(l => l.TargetUserId == likeParams.UserId && likeIds.Contains(l.SourceUserId))
                    .Select(l => l.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDto>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
