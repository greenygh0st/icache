using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCache.API.Exceptions;
using Xunit;

namespace iCache.Tests.Exceptions
{
    public class UserNotFoundTests
    {
        [Fact]
        public void UserNotFound_Instantiate()
        {
            UserNotFoundException ex = new UserNotFoundException();
            Assert.NotNull(ex);
        }
    }
}
