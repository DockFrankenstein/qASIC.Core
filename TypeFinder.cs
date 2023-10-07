using System.Reflection;

namespace qASIC
{
    public static class TypeFinder
    {
        private const BindingFlags _DEFAULT_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        #region Types
        public static IEnumerable<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t));
        }
        #endregion

        #region Attributes
        //Attributes of classes
        public static IEnumerable<Type> FindAllClassesWithAttribute<T>(BindingFlags bindingFlags = _DEFAULT_FLAGS)
            where T : Attribute =>
            FindAllClassesWithAttribute(typeof(T), bindingFlags);

        public static IEnumerable<Type> FindAllClassesWithAttribute(Type type, BindingFlags bindingFlags = _DEFAULT_FLAGS) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .Where(x => x.GetCustomAttributes(type, false).Count() > 0);

        //Attributes of methods in all classes
        public static IEnumerable<MethodInfo> FindAllAttributesInMethods<T>(BindingFlags bindingFlags = _DEFAULT_FLAGS)
            where T : Attribute =>
            FindAllAttributesInMethods(typeof(T), bindingFlags);

        public static IEnumerable<MethodInfo> FindAllAttributesInMethods(Type type, BindingFlags bindingFlags = _DEFAULT_FLAGS) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(bindingFlags))
                .Where(x => x.GetCustomAttributes(type, false).FirstOrDefault() != null);

        //Attributes of fields in specified class
        public static IEnumerable<FieldInfo> FindAllFieldAttributesInClass<TClass, TAttribute>(BindingFlags bindingFlags = _DEFAULT_FLAGS)
            where TClass : class
            where TAttribute : Attribute =>
            FindAllFieldAttributesInClass(typeof(TClass), typeof(TAttribute), bindingFlags);

        public static IEnumerable<FieldInfo> FindAllFieldAttributesInClass(Type classType, Type attributeType, BindingFlags bindingFlags = _DEFAULT_FLAGS) =>
            classType.GetFields(bindingFlags)
                .Where(x => x.GetCustomAttributes(attributeType, false).Count() > 0);

        //Attributes of properties in specified class
        public static IEnumerable<PropertyInfo> FindAllPropertyAttributesInClass<TClass, TAttribute>(BindingFlags bindingFlags = _DEFAULT_FLAGS)
            where TClass : class
            where TAttribute : Attribute =>
            typeof(TClass).GetProperties(bindingFlags)
                .Where(x => x.GetCustomAttributes<TAttribute>(false).Count() > 0);

        public static IEnumerable<PropertyInfo> FindAllPropertyAttributesInClass(Type classType, Type attributeType, BindingFlags bindingFlags = _DEFAULT_FLAGS) =>
            classType.GetProperties(bindingFlags)
                .Where(x => x.GetCustomAttributes(attributeType, false).Count() > 0);
        #endregion

        public static object? CreateConstructorFromType(Type type) =>
            CreateConstructorFromType(type, null);

        public static object? CreateConstructorFromType(Type type, params object[]? parameters)
        {
            if (type == null)
                return null;

            ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null || constructor.IsAbstract) return null;
            return constructor.Invoke(parameters);
        }

        public static IEnumerable<T?> CreateConstructorsFromTypes<T>(IEnumerable<Type> types) =>
            types.SelectMany(x =>
            {
                if (x == null)
                    return new T?[] { default };

                ConstructorInfo? constructor = x.GetConstructor(Type.EmptyTypes);
                if (constructor == null || constructor.IsAbstract) return new T[0];
                return new T[] { (T)constructor.Invoke(null) };
            });
    }
}
