using System;

namespace Core.Logic.ImageQuantizer.Helpers
{
    public static class Guard
    {
        /// <summary>
        ///   Checks if an argument is null
        /// </summary>
        /// <param name="argument"> argument </param>
        /// <param name="argumentName"> argument name </param>
        public static void CheckNull(Object argument, String argumentName)
        {
            if (argument == null)
            {
                var message = string.Format("Cannot use '{0}' when it is null!", argumentName);
                throw new ArgumentNullException(message);
            }
        }
    }
}