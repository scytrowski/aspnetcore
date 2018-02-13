using System.Reflection.Emit;

namespace NullPointer.AspNetCore.Entity.Extensions
{
    public static class ILGeneratorExtensions
    {
        public static void EmitGetter(this ILGenerator il, FieldBuilder propertyFieldBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, propertyFieldBuilder);
            il.Emit(OpCodes.Ret);
        }

        public static void EmitSetter(this ILGenerator il, FieldBuilder propertyFieldBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, propertyFieldBuilder);
            il.Emit(OpCodes.Ret);
        }
    }
}