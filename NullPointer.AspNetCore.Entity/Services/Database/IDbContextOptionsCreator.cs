using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    /// <summary>
    /// Inteface for implementing delegated DbContext model building
    /// </summary>
    public interface IDbContextOptionsCreator
    {
        void Build(DbContextOptionsBuilder optionsBuilder);
    }
}