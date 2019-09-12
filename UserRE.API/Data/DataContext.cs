using Microsoft.EntityFrameworkCore;

namespace UserRE.API.Data
{
    public class DataContext :DbContext
    {
        public  DataContext(DbContextOptions<DataContext> options):base(options){}
        public DbSet<Model.Value> Values {get; set;}
        public DbSet<Model.User> Users {get; set;}
       

    }
}