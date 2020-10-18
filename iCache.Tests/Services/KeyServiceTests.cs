using System;
using Xunit;
using iCache.API.Services;
using System.Threading.Tasks;

namespace iCache.Tests
{
    public class KeyServiceTests
    {
        [Fact]
        public async Task CreateAndRemoveKey()
        {
            using (KeyService _keyService = new KeyService())
            {
                await _keyService.SetKey("testkey", "test123");

                string result = await _keyService.FetchKey("testkey");

                // check to make sure that the value exists
                Assert.NotNull(result);
                Assert.Equal("test123", result);

                await _keyService.RemoveKey("testkey");
                string resultCleared = await _keyService.FetchKey("testkey");
                Assert.True(string.IsNullOrEmpty(resultCleared));
            }
        }

        [Fact]
        public async Task CreateKeyWithExpiration()
        {
            using (KeyService _keyService = new KeyService())
            {
                await _keyService.SetKey("testkey1", "test123", 7);

                string result = await _keyService.FetchKey("testkey1");

                // check to make sure that the value exists
                Assert.NotNull(result);
                Assert.Equal("test123", result);

                await Task.Delay(TimeSpan.FromSeconds(8));

                string resultCleared = await _keyService.FetchKey("testkey1");
                Assert.True(string.IsNullOrEmpty(resultCleared));
            }
        }

        [Fact]
        public async Task KeyExists()
        {
            using (KeyService _keyService = new KeyService())
            {
                string keyName = "testkey4";

                await _keyService.SetKey(keyName, "test123");

                Assert.True(await _keyService.KeyExists(keyName));

                await _keyService.RemoveKey(keyName);

                Assert.False(await _keyService.KeyExists(keyName));
            }
        }
    }
}
