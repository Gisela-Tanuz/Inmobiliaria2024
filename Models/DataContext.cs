
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria2024.Models

{
public class DataContext : DbContext
{
       public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
           public DbSet<Propietario> Propietario { get; set; }
}

}