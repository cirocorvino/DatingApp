using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Data;

public class UserRepository(DatingAppDBContext context, IMapper mapper) : IUserRepository
{

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
            .Include(u =>u.Photos)
            .SingleOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await context.Users
            .Include(u =>u.Photos)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(User user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    
    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await context.Users
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .ToListAsync();
    }
}
