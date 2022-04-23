
using Microsoft.EntityFrameworkCore;

namespace Sc3S.Data;

public class Sc3SContext : DbContext
{
    public Sc3SContext(DbContextOptions<Sc3SContext> options)
        : base(options)
    {
    }
    
    
}
