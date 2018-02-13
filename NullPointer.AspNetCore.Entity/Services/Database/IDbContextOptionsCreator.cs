using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    public interface IDbContextOptionsCreator
    {
        void Build(DbContextOptionsBuilder optionsBuilder);
    }
}