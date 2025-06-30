using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.EF.DbCreating
{
    public class SummerDbContextFactory : IDesignTimeDbContextFactory<SummerDbContext>
    {
        public SummerDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<SummerDbContext> optionsBuilder = new DbContextOptionsBuilder<SummerDbContext>();
            var connectionString = "Host=localhost;Port=5432;Database=dataSummer;Username=postgres;Password=zxc123";
            optionsBuilder.UseNpgsql(connectionString);
            return new SummerDbContext(optionsBuilder.Options);
        }
    }
}
