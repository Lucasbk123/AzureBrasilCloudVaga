using System.Reflection;
using System.Text;

namespace AzureBrasilCloudVaga.ApiService.Extensions
{
    public static class StringExtension
    {
        public static string ToPaginationCacheKey(this string value, int pageNumber, int pageSize)
            => string.Format(value, pageNumber, pageSize);

        public static string ToPaginationCacheKey(this string value, string endpoint, int pageNumber, int pageSize) 
            => string.Format(value, endpoint, pageNumber, pageSize);

        public static string ToCacheKey(this object obj,string key, string separator = "-")
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            var stringBuilder = new StringBuilder(key);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);

                if (HasValue(value))
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(separator);

                    stringBuilder.Append($"{property.Name}{separator}{value}");
                }
            }

            return stringBuilder.ToString();
        }

        public static string ReplacePageNumber(this string value, int oldPageNumber, int newPageNumber) 
            => value.Replace($"PageNumber-{oldPageNumber}", $"PageNumber-{newPageNumber}");

        private static bool HasValue(object value)
        {
            if (value == null)
                return false;

            if (value is string str)
                return !string.IsNullOrWhiteSpace(str);

            var defaultValue = Activator.CreateInstance(value.GetType());
            return !value.Equals(defaultValue);
        }
    }
}
