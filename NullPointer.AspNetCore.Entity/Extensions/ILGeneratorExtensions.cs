using System.Reflection.Emit;

namespace NullPointer.AspNetCore.Entity.Extensions
{
    public static class ILGeneratorExtensions
    {
        /// <summary>
        /// Emits IL code for basic getter
        /// </summary>
        /// <param name="il"></param>
        /// <param name="propertyFieldBuilder">Field to get value from</param>
        public static void EmitGetter(this ILGenerator il, FieldBuilder propertyFieldBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, propertyFieldBuilder);
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Emits IL code for basic setter
        /// </summary>
        /// <param name="il"></param>
        /// <param name="propertyFieldBuilder">Field to set value to</param>
        public static void EmitSetter(this ILGenerator il, FieldBuilder propertyFieldBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, propertyFieldBuilder);
            il.Emit(OpCodes.Ret);
        }
    }
}