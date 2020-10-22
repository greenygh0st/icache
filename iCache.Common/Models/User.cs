using System;
namespace iCache.Common.Models
{
    public class User
    {
        public Guid _Id { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public int LoginAttempts { get; set; }
        public bool Locked { get; set; }
    }
}
