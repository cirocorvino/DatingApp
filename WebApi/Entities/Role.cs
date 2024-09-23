using System;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
