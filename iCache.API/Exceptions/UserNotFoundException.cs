using System;
namespace iCache.API.Exceptions
{
    /// <summary>
    /// Thrown when a user is not found under certain circumstances
    /// </summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>
        /// UserNotFoundException const
        /// </summary>
        public UserNotFoundException() : base("User not found!")
        {
        }
    }
}
