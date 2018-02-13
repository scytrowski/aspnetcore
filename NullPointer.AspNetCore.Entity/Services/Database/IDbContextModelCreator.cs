using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    /// <summary>
    /// Interface for implementing delegated DbContext options building
    /// </summary>
    public interface IDbContextModelCreator
    {
        void Build(ModelBuilder modelBuilder);
    }
}