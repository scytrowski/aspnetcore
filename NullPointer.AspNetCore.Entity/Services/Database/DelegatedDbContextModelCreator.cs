using System;
using Microsoft.EntityFrameworkCore;

namespace NullPointer.AspNetCore.Entity.Services.Database
{
    public class DelegatedDbContextModelCreator : IDbContextModelCreator
    {
        public DelegatedDbContextModelCreator(Action<ModelBuilder> buildAction)
        {
            BuildAction = buildAction;
        }

        public void Build(ModelBuilder modelBuilder)
        {
            BuildAction?.Invoke(modelBuilder);
        }

        public Action<ModelBuilder> BuildAction { get; }
    }
}