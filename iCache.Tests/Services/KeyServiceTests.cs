using System;
using Xunit;
using iCache.API.Services;
using System.Threading.Tasks;
using System.Linq;

namespace iCache.Tests.Services
{
    public class KeyServiceTests
    {
        #region Basic Tests

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

        #endregion

        #region Search Tests

        [Fact]
        public async Task SearchKeyPermutation_One()
        {
            using (KeyService _keyService = new KeyService())
            {
                string key = "toy1";
                await _keyService.SetKey(key, "toy test 1");

                var results = await _keyService.SearchKeys("toy1");

                Assert.Single(results);

                await _keyService.RemoveKey(key);
            }
        }

        [Fact]
        public async Task SearchKeyPermutation_Two()
        {
            using (KeyService _keyService = new KeyService())
            {
                string key = "bear1";
                await _keyService.SetKey(key, "toy test 2");

                var results = await _keyService.SearchKeys("bea*");

                Assert.Single(results);

                await _keyService.RemoveKey(key);
            }
        }

        [Fact]
        public async Task SearchKeyPermutation_Three()
        {
            using (KeyService _keyService = new KeyService())
            {
                string key = "fiddle2";
                await _keyService.SetKey(key, "toy test 3");

                var results = await _keyService.SearchKeys("*ddle2");

                Assert.Single(results);

                await _keyService.RemoveKey(key);
            }
        }

        [Fact]
        public async Task SearchKeyPermutation_Four()
        {
            using (KeyService _keyService = new KeyService())
            {
                string key = "lol3";
                await _keyService.SetKey(key, "toy test 4");

                var results = await _keyService.SearchKeys("*o*l*");

                Assert.Single(results);

                await _keyService.RemoveKey(key);
            }
        }

        [Fact]
        public async Task SearchKeyPermutation_Five()
        {
            using (KeyService _keyService = new KeyService())
            {
                string key = "rock4";
                await _keyService.SetKey(key, "toy test 4");

                var results = await _keyService.SearchKeys("*oc*");

                Assert.Single(results);

                await _keyService.RemoveKey(key);
            }
        }

        [Fact]
        public async Task SearchKeyPermutation_Six()
        {
            using (KeyService _keyService = new KeyService())
            {
                string keyOne = "bob1";
                await _keyService.SetKey(keyOne, "toy test 4");

                string keyTwo = "bob2";
                await _keyService.SetKey(keyTwo, "toy test 4");

                var results = await _keyService.SearchKeys("*ob*");

                Assert.Equal(2, results.Count);

                await _keyService.RemoveKey(keyOne);
                await _keyService.RemoveKey(keyTwo);
            }
        }

        [Fact]
        public async Task SearchKeyWithValues()
        {
            using (KeyService _keyService = new KeyService())
            {
                string keyOne = "toy-phil1";
                await _keyService.SetKey(keyOne, "phil test 1");

                string keyTwo = "toy-phil2";
                await _keyService.SetKey(keyTwo, "phil test 2");

                var results = await _keyService.SearchKeysGetValues("*hil*");

                Assert.Equal(2, results.Count);

                // right value 1
                Assert.Equal("phil test 1", results.SingleOrDefault(x => x.Key == "toy-phil1").Value);

                // right value 2
                Assert.Equal("phil test 2", results.SingleOrDefault(x => x.Key == "toy-phil2").Value);

                await _keyService.RemoveKey(keyOne);
                await _keyService.RemoveKey(keyTwo);
            }
        }

        #endregion
    }
}
