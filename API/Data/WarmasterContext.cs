using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class WarmasterContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
