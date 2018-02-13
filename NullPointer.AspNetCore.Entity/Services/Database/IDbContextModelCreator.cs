using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    public interface IDbContextModelCreator
    {
        void Build(ModelBuilder modelBuilder);
    }
}