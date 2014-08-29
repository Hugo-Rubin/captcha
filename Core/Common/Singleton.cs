namespace Core.Common
{
    public abstract class Singleton<T> : ISingleton
        where T : new()
    {
        static Singleton() { }

        private static readonly T PrivateInstance = new T();

        public static T Instance
        {
            get
            {
                return PrivateInstance;
            }
        }

        public object GetInstance()
        {
            return Instance;
        }
    }

    public interface ISingleton
    {
        object GetInstance();
    }
}
