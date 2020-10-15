using System;
namespace iCache.API.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found!")
        {
        }
    }
}
