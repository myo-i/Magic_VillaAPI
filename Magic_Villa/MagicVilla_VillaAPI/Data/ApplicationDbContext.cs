using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    // DbContextはMicrosoft.EntityFrameworkCoreの機能
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { 
        }
        // VillaはDBのモデル、Villasはテーブル名
        public DbSet<Villa> Villas { get; set; }
    }
}
