using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Entity.Extensions;
using NullPointer.AspNetCore.Entity.Services.Database;

namespace NullPointer.AspNetCore.Entity.Builders
{
    public class DbContextBuilder
    {
        /// <summary>
        /// Includes provided entity type to builder
        /// </summary>
        /// <param name="entityType">Type of entity to be included. Must be not null class-like type object.</param>
        /// <returns></returns>
        public DbContextBuilder WithEntity(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("Entity type cannot be null");
            else if (!entityType.IsClass)
                throw new ArgumentException("Entity type must be a class");
            
            _registeredEntities.Add(entityType);
            return this;
        }

        /// <summary>
        /// Generic version of <see cref="WithEntity"/>
        /// </summary>
        /// <returns></returns>
        public DbContextBuilder WithEntity<TEntity>() where TEntity : class
        {
            Type entityType = typeof(TEntity);
            return WithEntity(entityType);
        }

        /// <summary>
        /// Includes provided options creator that takes responsibility of configuring built type
        /// </summary>
        /// <param name="optionsCreator">Options creator concrete object that cannot be null</param>
        /// <returns></returns>
        public DbContextBuilder WithOnConfiguring(IDbContextOptionsCreator optionsCreator)
        {
            if (optionsCreator == null)
                throw new ArgumentNullException("Options creator cannot be null");

            OptionsCreator = optionsCreator;
            return this;
        }

        /// <summary>
        /// Includes delegated options creator based on provided build action
        /// </summary>
        /// <param name="buildAction">Build action to be delegated</param>
        /// <returns></returns>
        public DbContextBuilder WithOnConfiguring(Action<DbContextOptionsBuilder> buildAction)
        {
            IDbContextOptionsCreator optionsCreator = new DelegatedDbContextOptionsCreator(buildAction);
            return WithOnConfiguring(optionsCreator);
        }

        /// <summary>
        /// Includes provided model creator that takes responsibility of creating model for built type
        /// </summary>
        /// <param name="modelCreator">Model creator concrete object that cannot be null</param>
        /// <returns></returns>
        public DbContextBuilder WithOnModelCreating(IDbContextModelCreator modelCreator)
        {
            if (modelCreator == null)
                throw new ArgumentNullException("Model creator cannot be null");

            ModelCreator = modelCreator;
            return this;
        }

        /// <summary>
        /// Includes delegated model creator based on provided build action
        /// </summary>
        /// <param name="buildAction">Build action to be delegated</param>
        /// <returns></returns>
        public DbContextBuilder WithOnModelCreating(Action<ModelBuilder> buildAction)
        {
            IDbContextModelCreator modelCreator = new DelegatedDbContextModelCreator(buildAction);
            return WithOnModelCreating(modelCreator);
        }

        /// <summary>
        /// Builds ConfigurableDbContext subtype based on included entities
        /// </summary>
        /// <returns></returns>
        public Type Build()
        {
            TypeBuilder dbContextTypeBuilder = CreateDbContextTypeBuilder();
            return dbContextTypeBuilder.CreateType();
        }

        private TypeBuilder CreateDbContextTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run
            );
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
                "Entity"
            );
            Type configurableDbContextType = typeof(ConfigurableDbContext);
            TypeBuilder dbContextTypeBuilder = moduleBuilder.DefineType(
                "GeneratedDbContext",
                TypeAttributes.Public | TypeAttributes.Class,
                configurableDbContextType
            );
            dbContextTypeBuilder.DefinePassThroughConstructors();
            Type dbSetType = typeof(DbSet<>);

            foreach (Type entityType in _registeredEntities)
            {
                dbContextTypeBuilder.DefineBasicProperty(
                    entityType.Name,
                    dbSetType.MakeGenericType(entityType)
                );
            }

            return dbContextTypeBuilder;
        }

        /// <summary>
        /// Enumerable of included entity types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> RegisteredEntities => _registeredEntities.ToList().AsReadOnly();
        public IDbContextOptionsCreator OptionsCreator { get; private set; }
        public IDbContextModelCreator ModelCreator { get; private set; }

        private HashSet<Type> _registeredEntities = new HashSet<Type>(); 
    }
}