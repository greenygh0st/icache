using System;
namespace iCache.API.Models
{
    public class User
    {
        public Guid _Id { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
    }
}
