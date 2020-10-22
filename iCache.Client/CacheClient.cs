using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iCache.Common.Models;
using Newtonsoft.Json;

namespace iCache.Client
{
    public class CacheClient : IDisposable
    {
        private bool disposedValue;
        private string _serviceUri;
        private string _username;
        private string _password;
        private string _authHeaderValue;
        private HttpClient _httpClient;

        private readonly string ServerErrorMessage = "An unknown server error occured.";

        /// <summary>
        /// Base constructor for iCache client
        /// </summary>
        /// <param name="serviceUri">The base URI of the iCache service (ie https://icache.mydomain.com)</param>
        /// <param name="userid">Valid userid, string literally of valid user <see cref="Guid"/></param>
        /// <param name="password">Valid password</param>
        public CacheClient(string serviceUri, string userid, string password)
        {
            if (string.IsNullOrEmpty(serviceUri))
                serviceUri = Environment.GetEnvironmentVariable("ICACHE_CLIENT_SERVICEURI");

            if (string.IsNullOrEmpty(userid))
                userid = Environment.GetEnvironmentVariable("ICACHE_CLIENT_USERID");

            if (string.IsNullOrEmpty(password))
                password = Environment.GetEnvironmentVariable("ICACHE_CLIENT_PASSWORD");

            if (string.IsNullOrEmpty(serviceUri))
                throw new ArgumentNullException("username", "You must supply the service URI parameter!");

            if (string.IsNullOrEmpty(userid))
                throw new ArgumentNullException("username", "You must supply the username parameter!");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "You must supply the password parameter!");

            _username = userid;
            _password = password;
            _serviceUri = serviceUri;
            _authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));

            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _authHeaderValue);
        }

        #region Keys

        /// <summary>
        /// Fetch a key
        /// </summary>
        /// <param name="keyName">The name of the key that you want to fetch.</param>
        /// <returns>The key value, null if not set</returns>
        public async Task<string> FetchKey(string keyName)
        {
            var response = await _httpClient.GetAsync($"{_serviceUri}/api/key/{HttpUtility.UrlEncode(keyName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<ValueItem>(jsonResponse.Response);
                return value.Value;
            }

            return null;
        }

        /// <summary>
        /// Fetch a key
        /// </summary>
        /// <typeparam name="T">The type you want the value object to be parsed as</typeparam>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<T> FetchKey<T>(string keyName)
        {
            var response = await _httpClient.GetAsync($"{_serviceUri}/api/key/{HttpUtility.UrlEncode(keyName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<ValueItem>(jsonResponse.Response);
                return JsonConvert.DeserializeObject<T>(value.Value);
            }

            return default;
        }

        /// <summary>
        /// Remove a key from iCache.
        /// </summary>
        /// <param name="keyName">The key you want to remove.</param>
        /// <returns></returns>
        public async Task<(bool Success, string FailReason)> RemoveKey(string keyName)
        {
            var response = await _httpClient.DeleteAsync($"{_serviceUri}/api/key/{HttpUtility.UrlEncode(keyName)}");

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonStatus jsonResponse = JsonConvert.DeserializeObject<JsonStatus>(json);
                return (false, jsonResponse.Message);
            }
        }

        /// <summary>
        /// Set a key with a given key name and value
        /// </summary>
        /// <param name="keyName">The name of the key you want to set</param>
        /// <param name="value">The object you want to set as the key value</param>
        /// <returns></returns>
        public async Task<(bool Success, string FailReason)> SetKey(string keyName, object value)
        {
            string valueToStore = (value is string) ? value.ToString() : JsonConvert.SerializeObject(value);

            StringContent stringContent =
                new StringContent(
                    JsonConvert.SerializeObject(new CreateValueItem {
                        Key = keyName,
                        Value = valueToStore
                    }),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync($"{_serviceUri}/api/key", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            } else
            {
                //JsonError
                if (response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JsonError error = JsonConvert.DeserializeObject<JsonError>(json);
                    return (false, string.Join(". ", error.Errors));
                } else
                {
                    return (false, ServerErrorMessage);
                }
            }
        }

        /// <summary>
        /// Save a C# object within iCache
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public async Task<(bool Success, string FailReason)> SetKey(string keyName, object value, TimeSpan expiration)
        {
            string valueToStore = (value is string) ? value.ToString() : JsonConvert.SerializeObject(value);

            StringContent stringContent =
                new StringContent(
                    JsonConvert.SerializeObject(new CreateValueItem
                    {
                        Key = keyName,
                        Value = valueToStore,
                        Expiration = (int)Math.Ceiling(expiration.TotalSeconds)
                    }),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync($"{_serviceUri}/api/key", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                //JsonError
                if (response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JsonError error = JsonConvert.DeserializeObject<JsonError>(json);
                    return (false, string.Join(". ", error.Errors));
                }
                else
                {
                    return (false, ServerErrorMessage);
                }
            }
        }

        #endregion

        #region Queues

        /// <summary>
        /// Pop an item from a queue. Note this does not delete the item from the queue.
        /// </summary>
        /// <param name="queueName">The name of the queue you want to pop from</param>
        /// <returns>String value of message or null if queue empty or missing</returns>
        public async Task<string> PopFromQueue(string queueName)
        {
            var response = await _httpClient.GetAsync($"{_serviceUri}/api/queue/{HttpUtility.UrlEncode(queueName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<QueueMessage>(jsonResponse.Response);
                return value.Message;
            }

            return null;
        }

        /// <summary>
        /// Pop an item from a queue. Note this does not delete the item from the queue.
        /// </summary>
        /// <typeparam name="T">Type you want to attempt to parse the message as</typeparam>
        /// <param name="queueName">The name of the queue you want to pop from</param>
        /// <returns>Parsed value of the message or null if queue empty or missing</returns>
        public async Task<T> PopFromQueue<T>(string queueName)
        {
            var response = await _httpClient.GetAsync($"{_serviceUri}/api/queue/{HttpUtility.UrlEncode(queueName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<QueueMessage>(jsonResponse.Response);
                return JsonConvert.DeserializeObject<T>(value.Message);
            }

            return default;
        }

        /// <summary>
        /// Pop and delete an item from a queue. Note this does not delete the item from the queue.
        /// </summary>
        /// <param name="queueName">The name of the queue you want to pop from</param>
        /// <returns>String value of message or null if queue empty or missing</returns>
        public async Task<string> DeleteFromQueue(string queueName)
        {
            var response = await _httpClient.DeleteAsync($"{_serviceUri}/api/queue/{HttpUtility.UrlEncode(queueName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<QueueMessage>(jsonResponse.Response);
                return value.Message;
            }

            return null;
        }

        /// <summary>
        /// Pop and delete an item from a queue.
        /// </summary>
        /// <typeparam name="T">Type you want to attempt to parse the message as</typeparam>
        /// <param name="queueName">The name of the queue you want to pop from</param>
        /// <returns>Parsed value of the message or null if queue empty or missing</returns>
        public async Task<T> DeleteFromQueue<T>(string queueName)
        {
            var response = await _httpClient.DeleteAsync($"{_serviceUri}/api/queue/{HttpUtility.UrlEncode(queueName)}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonWithStringResponse jsonResponse = JsonConvert.DeserializeObject<JsonWithStringResponse>(json);
                var value = JsonConvert.DeserializeObject<QueueMessage>(jsonResponse.Response);
                return JsonConvert.DeserializeObject<T>(value.Message);
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="messages"><see cref="List{object}"/>, collection of messages to add to the queue</param>
        /// <returns>Tupal, (bool Success, string FailReason)</returns>
        public async Task<(bool Success, string FailReason)> PushToQueue(string queueName, List<object> messages)
        {
            List<string> finalMessages = new List<string>();

            foreach (var message in messages)
            {
                finalMessages.Add((message is string) ? message.ToString() : JsonConvert.SerializeObject(message));
            }

            StringContent stringContent =
                new StringContent(
                    JsonConvert.SerializeObject(new QueueMessages {
                        QueueName = queueName,
                        Messages = finalMessages
                    }),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync($"{_serviceUri}/api/queue", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                //JsonError
                if (response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JsonError error = JsonConvert.DeserializeObject<JsonError>(json);
                    return (false, string.Join(". ", error.Errors));
                }
                else
                {
                    return (false, ServerErrorMessage);
                }
            }
        }

        #endregion

        #region Dispose Method
        protected virtual void Dispose(bool disposing)
        {
            _username = null;
            _password = null;
            _authHeaderValue = null;

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CacheClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
