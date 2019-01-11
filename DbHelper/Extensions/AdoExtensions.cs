using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility
{
    public static class AdoExtensions
    {
        /// <summary>
        /// 将 DataRow 对象转换成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dr) where T : EntityObject<T>, new()
        {
            T eo = new T();

            eo.Mappings.Each(mapping =>
               {
                   string mappingColumnName = mapping.ColumnName;

                   if (dr.Table.Columns.Contains(mappingColumnName)
                   && !dr[mappingColumnName].IsNullOrEmpty())
                   {
                       mapping.Expression.ToMember().SetValue(eo, dr[mappingColumnName]);
                   }
               });

            eo.Reset();

            return eo;
        }

        /// <summary>
        /// 将 DataRow 对象转换成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static IList<T> ToEntity<T>(this DataTable dt) where T : EntityObject<T>, new()
        {
            IList<T> eos = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return eos;
            }

            foreach (DataRow dr in dt.Rows)
            {
                eos.Add(dr.ToEntity<T>());
            }

            return eos;
        }

        public static T SetPropertyValue<T>(DataRow dr) where T : new()
        {
            T t = new T();

            var propertyInfoList = typeof(T).GetProperties()
                .Where(p => p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType.Name == nameof(String)));

            foreach (var propertyInfo in propertyInfoList)
            {
                if (propertyInfo != null && dr.Table.Columns.Contains(propertyInfo.Name))
                {
                    object columnValue = dr[propertyInfo.Name];

                    if (columnValue is DBNull)
                    {
                        columnValue = ConvertDbNullValue(propertyInfo.PropertyType.Name);
                        propertyInfo.SetValue(t, columnValue);
                        continue;
                    }

                    bool isNullable = (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null);
                    if (isNullable)
                    {
                        columnValue = Convert.ChangeType(columnValue, Nullable.GetUnderlyingType(propertyInfo.PropertyType));
                    }
                    else
                    {
                        try
                        {
                            columnValue = propertyInfo.PropertyType.IsEnum ? Enum.Parse(propertyInfo.PropertyType, columnValue.AsStringWithDefault()) : Convert.ChangeType(columnValue, propertyInfo.PropertyType);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    propertyInfo.SetValue(t, columnValue);
                }
            }

            return t;
        }

        public static IList<T> ToModel<T>(this DataTable dt) where T : new()
        {
            IList<T> eos = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return eos;
            }

            foreach (DataRow dr in dt.Rows)
            {
                eos.Add(SetPropertyValue<T>(dr));
            }

            return eos;
        }

        private static object ConvertDbNullValue(string typeName)
        {
            if (typeName == nameof(String))
            {
                return string.Empty;
            }
            if (typeName == nameof(Int64))
            {
                return 0L;
            }
            if (typeName == nameof(Int32) || typeName == nameof(Int16))
            {
                return 0;
            }
            if (typeName == nameof(DateTime))
            {
                return DateTime.MinValue;
            }
            if (typeName == nameof(Boolean))
            {
                return false;
            }
            if (typeName == nameof(Decimal))
            {
                return 0.0M;
            }

            return null;
        }
    }
}
