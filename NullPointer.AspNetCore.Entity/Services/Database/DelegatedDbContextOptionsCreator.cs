using System;
using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    public class DelegatedDbContextOptionsCreator : IDbContextOptionsCreator
    {
        public DelegatedDbContextOptionsCreator(Action<DbContextOptionsBuilder> buildAction)
        {
            BuildAction = buildAction;
        }

        public void Build(DbContextOptionsBuilder optionsBuilder)
        {
            BuildAction?.Invoke(optionsBuilder);
        }

        public Action<DbContextOptionsBuilder> BuildAction { get; }
    }
}