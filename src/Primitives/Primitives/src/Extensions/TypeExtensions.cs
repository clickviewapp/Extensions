namespace ClickView.Extensions.Primitives.Extensions
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return IsGenericType(type, typeof(Nullable<>));
        }

        public static bool IsGenericType(this Type type, Type typeDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeDefinition;
        }
    }
}
