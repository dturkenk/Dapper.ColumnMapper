using System;
using System.Linq;
using System.Reflection;

namespace Dapper.ColumnMapper
{
    public class ColumnTypeMapper : SqlMapper.ITypeMap
    {
        private SqlMapper.ITypeMap _internalMapper;

        public static void RegisterForTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                SqlMapper.SetTypeMap(type, new ColumnTypeMapper(type));      
            }
        }
        
        public ColumnTypeMapper(Type type)
        {
            _internalMapper = new CustomPropertyTypeMap(type, _propertyResolver);
        }

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            return _internalMapper.FindConstructor(names, types);
        }
        
        public ConstructorInfo FindExplicitConstructor()
        {
            return _internalMapper.FindExplicitConstructor();
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            return _internalMapper.GetConstructorParameter(constructor, columnName);
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            return _internalMapper.GetMember(columnName);
        }

        private readonly Func<Type, string, PropertyInfo> _propertyResolver = (type, name) =>
        {
            var properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                   BindingFlags.Public);

            return properties.FirstOrDefault(property =>
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var attribute = property.GetCustomAttribute<ColumnMappingAttribute>();

                return attribute != null &&
                       attribute.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase);
            });
        };
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnMappingAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public ColumnMappingAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
