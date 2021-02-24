using System;
using System.Collections.Generic;
using System.Linq;

namespace UniAgile.Game
{
    public static class IdentifierExtensions
    {
        public const           string LOCAL                 = "LOCAL";
        public const           string DYNAMIC               = "DYNAMIC";
        public static readonly string LocalUniqueIdentifier = $"{LOCAL}-{NewGuid().ToString()}";

        private static readonly Dictionary<string, string[]> Cache = new Dictionary<string, string[]>();

        private static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public static string ToIdentifier(this Type type)
        {
            return type.Name;
        }

        public static string ToDynamic(this string id)
        {
            return $"{id}.DYNAMIC-{NewGuid().ToString()}";
        }

        public static bool IsLocal(this string id)
        {
            return id.Contains(LOCAL);
        }

        public static bool IsDynamic(this string id)
        {
            return id.Contains(DYNAMIC);
        }

        public static string[] GetIdentifierTypes(this string id)
        {
            if (!Cache.TryGetValue(id, out var splits))
            {
                splits = id.Split('.');
                Cache.Add(id, splits);
            }

            return splits;
        }

        public static bool HasSubIdentifier(this string thisId,
                                            string      otherId)
        {
            var split = thisId.GetIdentifierTypes();

            return split.Any(t => t == otherId);
        }

        public static bool EitherIsSubIdentifier(this string thisId,
                                                 string      identifier)
        {
            return identifier.HasSubIdentifier(thisId) || thisId.HasSubIdentifier(identifier);
        }


        public static string GetCombinedDynamic<T, TSecond>()
        {
            return $"{typeof(T).Name}.{typeof(TSecond).Name}.{DYNAMIC}-{Guid.NewGuid().ToString()}";
        }

        public static string GetCombinedStatic<T, TSecond>()
        {
            return $"{typeof(T).Name}.{typeof(TSecond).Name}";
        }

        // Static across this instance of the program, unique (as unique as guid is) across different instances of this program
        public static string GetLocalStatic<T>()
        {
            return GetNamed<T>(LocalUniqueIdentifier);
        }

        // Static across this instance of the program, unique (as unique as guid is) across different instances of this program
        public static string GetLocalStatic(Type type)
        {
            return GetNamed(type, LocalUniqueIdentifier);
        }

        public static string GetStatic<T>()
        {
            return GetStatic(typeof(T));
        }

        public static string GetStatic(Type type)
        {
            return $"{type.Name}";
        }

        public static string GetNamed<T>(string name)
        {
            return $"{typeof(T).Name}.{name}";
        }

        public static string GetNamed(Type   type,
                                      string name)
        {
            return $"{type.Name}.{name}";
        }

        public static string GetDynamic<T>()
        {
            return GetDynamic(typeof(T));
        }

        public static string GetDynamic(Type type)
        {
            var id = GetStatic(type);

            return id.ToDynamic();
        }
    }
}