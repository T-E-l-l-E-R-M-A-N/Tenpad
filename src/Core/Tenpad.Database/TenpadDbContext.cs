using System;
using Microsoft.EntityFrameworkCore;

namespace Tenpad.Database
{
    public class TenpadDbContext : DbContext
    {
        public DbSet<DataObjectModel> Data { get; set; }
        public TenpadDbContext()
        {

        }

        public TenpadDbContext(DbContextOptions<TenpadDbContext> options) : base(options)
        {

        }
    }
}
