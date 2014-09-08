namespace SimpleHttpMock
{
    public static class AnonymousTypeExtensions
    {
        public static ActualRequest<T> ToRequest<T>(this T o)
        {
            return default(ActualRequest<T>);
        }

        public static T[] AsArray<T>(this T o)
        {
            return default(T[]);
        }
    }
}