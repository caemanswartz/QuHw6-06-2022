using Microsoft.EntityFrameworkCore;
using QuHWBlazorWasmJwtUse.Server.Models;
using QuHWBlazorWasmJwtUse.Shared.Models;
public class ApiDbContext:DbContext
{
#pragma warning disable CS8618
    public ApiDbContext(DbContextOptions options)
        :base(options){}
#pragma warning restore CS8618
    public DbSet<User> Users{get;set;}
}