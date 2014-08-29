using System;

namespace Core.Logic
{
    public interface ICacheable
    {
        String CacheDependencyFile { get; }
    }
}
