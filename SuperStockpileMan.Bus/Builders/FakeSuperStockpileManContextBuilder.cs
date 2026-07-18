using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SuperStockpileMan.Bus.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Builders
{
    public class FakeSuperStockpileManContextBuilder : IDesignTimeDbContextFactory<SuperStockpileManContext>
    {
        public SuperStockpileManContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<SuperStockpileManContext> optionsBuilder = new();
            optionsBuilder.UseSqlite("Data Source=Database.db");

            return new(optionsBuilder.Options);
        }
    }
}
