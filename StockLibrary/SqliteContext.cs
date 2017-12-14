using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    public class SqliteContext : DbContext
    {
        public DbSet<Models.Fund> Fund { get; set; }
        public DbSet<Models.FundDay> FundDay { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=stocks.db");
        }


    }
}
