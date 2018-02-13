using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NullPointer.AspNetCore.Entity.Extensions
{
    public static class TypeBuilderExtensions
    {
        /// <summary>
        /// Defines pass through constructor based on provided one
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="constructorInfo">Constructor to pass to</param>
        /// <returns></returns>
        public static ConstructorBuilder DefinePassThroughConstructor(this TypeBuilder typeBuilder, ConstructorInfo constructorInfo)
        {
            Type[] constructorParameterTypes = constructorInfo.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                constructorParameterTypes
            );
            ILGenerator constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Nop);
            constructorIL.Emit(OpCodes.Ldarg_0);

            for (int i = 1; i <= constructorParameterTypes.Count(); i++)
                constructorIL.Emit(OpCodes.Ldarg, i);

            constructorIL.Emit(OpCodes.Call, constructorInfo);
            constructorIL.Emit(OpCodes.Ret);
            return constructorBuilder;
        }

        /// <summary>
        /// Defines all available pass through constructors
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <returns></returns>
        public static ConstructorBuilder[] DefinePassThroughConstructors(this TypeBuilder typeBuilder)
        {
            Type baseType = typeBuilder.BaseType;
            ConstructorInfo[] baseTypeConstructors = baseType
                .GetConstructors()
                .Where(c => c.IsPublic)
                .ToArray();
            ConstructorBuilder[] createdConstructors = new ConstructorBuilder[baseTypeConstructors.Length];
            
            for (int i = 0; i < baseTypeConstructors.Length; i++)
                createdConstructors[i] = typeBuilder.DefinePassThroughConstructor(baseTypeConstructors[i]);

            return createdConstructors;
        }    

        /// <summary>
        /// Defines basic property getter for provided property builder
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyBuilder">Property builder for which getter will be created</param>
        /// <param name="propertyFieldBuilder">Field builder that provides values for getter</param>
        /// <returns></returns>
        public static MethodBuilder DefinePropertyGetter(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, FieldBuilder propertyFieldBuilder)
        {
            MethodBuilder propertyGetterBuilder = typeBuilder.DefineMethod(
                $"get_{propertyBuilder.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                propertyBuilder.PropertyType,
                null
            );
            ILGenerator propertyGetterIL = propertyGetterBuilder.GetILGenerator();
            propertyGetterIL.EmitGetter(propertyFieldBuilder);
            propertyBuilder.SetGetMethod(propertyGetterBuilder);
            return propertyGetterBuilder;
        }

        /// <summary>
        /// Defines basic property setter for provided property builder
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyBuilder">Property builder for which setter will be created</param>
        /// <param name="propertyFieldBuilder">Field builder that will be used in setter</param>
        /// <returns></returns>
        public static MethodBuilder DefinePropertySetter(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, FieldBuilder propertyFieldBuilder)
        {
            MethodBuilder propertySetterBuilder = typeBuilder.DefineMethod(
                $"set_{propertyBuilder.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                null,
                new Type[] { propertyBuilder.PropertyType }
            );
            ILGenerator propertySetterIL = propertySetterBuilder.GetILGenerator();
            propertySetterIL.EmitSetter(propertyFieldBuilder);
            propertyBuilder.SetSetMethod(propertySetterBuilder);
            return propertySetterBuilder;
        }

        /// <summary>
        /// Defines basic property 
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="name">Name of the property</param>
        /// <param name="propertyType">Type of the property</param>
        /// <returns></returns>
        public static PropertyBuilder DefineBasicProperty(this TypeBuilder typeBuilder, string name, Type propertyType)
        {
            FieldBuilder propertyFieldBuilder = typeBuilder.DefineField(
                $"_{name}",
                propertyType,
                FieldAttributes.Private
            );
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                CallingConventions.Standard,
                propertyType,
                null
            );
            typeBuilder.DefinePropertyGetter(propertyBuilder, propertyFieldBuilder);
            typeBuilder.DefinePropertySetter(propertyBuilder, propertyFieldBuilder);
            return propertyBuilder;
        }
    }
}