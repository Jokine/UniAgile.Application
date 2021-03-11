using System;
using System.Collections.Generic;

namespace UniAgile.Game
{
    public struct Option
    {
        public static Option<T> Some<T>(T value)
        {
            return Option<T>.Some(value);
        }
    }

    public struct Option<T>
    {
        public static Option<T> None => default;

        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }

        public bool IsSome => !EqualityComparer<T>.Default.Equals(Value, default);

        private readonly T Value;

        private Option(T value)
        {
            Value = value;
        }

        public bool TryGetValue(out T value)
        {
            value = Value;

            return IsSome;
        }
    }

    public static class OptionExtensions
    {
        public static U Match<T, U>(this Option<T> option,
                                    Func<T, U> onIsSome,
                                    Func<U> onIsNone = default)
        {
            return option.TryGetValue(out var value) ? onIsSome(value) : onIsNone();
        }

        public static Option<U> And<T, U>(this Option<T> option,
                                          Func<T, Option<U>> binder)
        {
            return option.Bind(binder);
        }


        public static Option<U> Bind<T, U>(this Option<T> option,
                                           Func<T, Option<U>> binder)
        {
            return option.Match(binder, () => Option<U>.None);
        }

        public static Option<U> Map<T, U>(this Option<T> option,
                                          Func<T, U> mapper)
        {
            return option.Bind(value => Option<U>.Some(mapper(value)));
        }

        public static Option<T> Filter<T>(this Option<T> option,
                                          Predicate<T> predicate)
        {
            return option.Bind(value => predicate(value) ? option : Option<T>.None);
        }

        public static T DefaultValue<T>(this Option<T> option,
                                        T defaultValue)
        {
            return option.Match(value => value, () => defaultValue);
        }
    }
}