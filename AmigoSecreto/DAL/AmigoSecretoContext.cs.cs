using AmigoSecreto.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AmigoSecreto.DAL
{
    public class AmigoSecretoContext : DbContext
    {

        public AmigoSecretoContext() : base("DefaultConnection")
        {
        }

        public DbSet<Amigo> Amigos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<AmigoSecreto.Models.Impedimento> Impedimentoes { get; set; }
    }
}