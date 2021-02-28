using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniAgile.Game
{
    public static class Utility
    {
        public static Notifiable GetOrCreateNotifiable(this IDictionary<string, Notifiable> notifiables,
                                                       string                               key)
        {
            if (!notifiables.TryGetValue(key, out var notifiable))
            {
                notifiable = new Notifiable();
                notifiables.Add(key, notifiable);
            }

            return notifiable;
        }

        public static T OptimisticGet<T>(this IDictionary<Type, object> values,
                                         T                              defaultValue = default)
        {
            try
            {
                return (T) values[typeof(T)];
            }
            catch (Exception)
            {
                if (defaultValue.IsDefault()) throw;

                return defaultValue;
            }
        }

        public static Option<TValue> GetOption<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                             TKey                           key)
        {
            return Option<TValue>.Some(dictionary.GetOrDefault(key));
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                        TKey                           key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : default;
        }

        public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                 TKey                           key)
            where TValue : new()
        {
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, new TValue());
        }


        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                    TKey                           key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value           = new TValue();
                dictionary[key] = value;
            }

            return value;
        }

        public static T OptimisticGet<T>(this IDictionary<Type, T> values,
                                         T                         defaultValue = default)
        {
            try
            {
                return values[typeof(T)];
            }
            catch (Exception)
            {
                if (defaultValue.IsDefault()) throw;

                return defaultValue;
            }
        }


        public static T OptimisticGet<TKey, T>(this IDictionary<TKey, T> values,
                                               TKey                      key,
                                               T                         defaultValue = default)
        {
            try
            {
                return values[key];
            }
            catch (Exception)
            {
                if (defaultValue.IsDefault()) throw;

                return defaultValue;
            }
        }

        public static T Get<T>(this IDictionary<Type, object> values,
                               T                              defaultValue = default)
        {
            var found = values.TryGetValue(typeof(T), out var value);

            return found ?
                       (T) value :
                       defaultValue.IsDefault() ?
                           throw new Exception($"Unable to find key {typeof(T)} from dictionary") :
                           defaultValue;
        }

        public static T Get<T>(this IDictionary<Type, T> values,
                               T                         defaultValue = default)
        {
            var found = values.TryGetValue(typeof(T), out var value);

            return found ?
                       value :
                       defaultValue.IsDefault() ?
                           throw new Exception($"Unable to find key {typeof(T)} from dictionary") :
                           defaultValue;
        }


        public static T Get<TKey, T>(this IDictionary<TKey, T> values,
                                     TKey                      key,
                                     T                         defaultValue = default)
        {
            var found = values.TryGetValue(key, out var value);

            return found ?
                       value :
                       defaultValue.IsDefault() ?
                           throw new Exception($"Unable to find key {key} from dictionary") :
                           defaultValue;
        }


        public static bool IsDefault<T>(this T compared)
        {
            return EqualityComparer<T>.Default.Equals(compared, default);
        }

        public static IEnumerable<Type> GetAllTypesOf<T>()
        {
            return GetAllTypesOf(typeof(T));
        }

        public static IEnumerable<Type> GetAllTypesOf<T>(Func<Type, bool> predicate)
        {
            return GetAllTypesOf(typeof(T)).Where(predicate);
        }

        public static IEnumerable<Type> GetAllNewableTypesOf<T>()
        {
            return GetAllTypesOf(typeof(T)).Where(t => !t.IsInterface);
        }

        public static IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        }

        public static IEnumerable<Type> GetAllTypes(Func<Type, bool> predicate)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(predicate);
        }

        public static IEnumerable<Type> GetAllTypesOf(Type wantedType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(wantedType.IsAssignableFrom);
        }

        public static Type GetMemberUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:    return ((FieldInfo) member).FieldType;
                case MemberTypes.Property: return ((PropertyInfo) member).PropertyType;
                case MemberTypes.Event:    return ((EventInfo) member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo",
                                                "member");
            }
        }
    }
}