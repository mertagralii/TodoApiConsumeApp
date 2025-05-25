using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApiConsumeApp.Data.Entities;

namespace TodoApiConsumeApp.Data.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Todo> Todos { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    
}