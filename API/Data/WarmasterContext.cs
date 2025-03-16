using Microsoft.EntityFrameworkCore;
using Warmaster.Models;

namespace API.Data;

public class WarmasterContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
