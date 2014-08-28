namespace Core.Common
{
    public abstract class Singleton<T> where T : new()
    {
        static Singleton()
        {
        }

        private static readonly T PrivateInstance = new T();

        public static T Instance
        {
            get
            {
                return PrivateInstance;
            }
        }
    }
}
