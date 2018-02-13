using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    public abstract class ConfigurableDbContext : DbContext
    {
        public ConfigurableDbContext(IDbContextOptionsCreator optionsCreator, IDbContextModelCreator modelCreator)
        {
            optionsCreator = OptionsCreator;
            modelCreator = ModelCreator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            OptionsCreator?.Build(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelCreator?.Build(modelBuilder);
        }

        public IDbContextOptionsCreator OptionsCreator { get; }
        public IDbContextModelCreator ModelCreator { get; }
    }
}