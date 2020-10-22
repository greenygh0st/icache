# iCache.Client

[![nugetname](https://img.shields.io/badge/nuget%20package-iCache.Redis.Client-brightgreen)](https://www.nuget.org/packages/iCache.Redis.Client/) [![nuget](https://img.shields.io/nuget/v/iCache.Redis.Client)](https://www.nuget.org/packages/iCache.Redis.Client/)

Please note:
- User methods are currently not provided via the Client. These will be added in a later version.
- All non-queue keys exist within the context of a user/application context.

```
/// <summary>
/// Base constructor for iCache client
/// </summary>
/// <param name="serviceUri">The base URI of the iCache service (ie https://icache.mydomain.com)</param>
/// <param name="userid">Valid userid, string literally of valid user <see cref="Guid"/></param>
/// <param name="password">Valid password</param>
public CacheClient(string serviceUri, string userid, string password)

/// <summary>
/// Fetch a key
/// </summary>
/// <param name="keyName">The name of the key that you want to fetch.</param>
/// <returns>The key value, null if not set</returns>
public async Task<string> FetchKey(string keyName)

/// <summary>
/// Fetch a key
/// </summary>
/// <typeparam name="T">The type you want the value object to be parsed as</typeparam>
/// <param name="keyName"></param>
/// <returns></returns>
public async Task<T> FetchKey<T>(string keyName)

/// <summary>
/// Remove a key from iCache.
/// </summary>
/// <param name="keyName">The key you want to remove.</param>
/// <returns></returns>
public async Task<(bool Success, string FailReason)> RemoveKey(string keyName)

/// <summary>
/// Set a key with a given key name and value
/// </summary>
/// <param name="keyName">The name of the key you want to set</param>
/// <param name="value">The object you want to set as the key value</param>
/// <returns></returns>
public async Task<(bool Success, string FailReason)> SetKey(string keyName, object value)

/// <summary>
/// Pop an item from a queue. Note this does not delete the item from the queue.
/// </summary>
/// <param name="queueName">The name of the queue you want to pop from</param>
/// <returns>String value of message or null if queue empty or missing</returns>
public async Task<string> PopFromQueue(string queueName)

/// <summary>
/// Pop an item from a queue. Note this does not delete the item from the queue.
/// </summary>
/// <typeparam name="T">Type you want to attempt to parse the message as</typeparam>
/// <param name="queueName">The name of the queue you want to pop from</param>
/// <returns>Parsed value of the message or null if queue empty or missing</returns>
public async Task<T> PopFromQueue<T>(string queueName)

/// <summary>
/// Pop and delete an item from a queue. Note this does not delete the item from the queue.
/// </summary>
/// <param name="queueName">The name of the queue you want to pop from</param>
/// <returns>String value of message or null if queue empty or missing</returns>
public async Task<string> DeleteFromQueue(string queueName)

/// <summary>
/// Pop and delete an item from a queue.
/// </summary>
/// <typeparam name="T">Type you want to attempt to parse the message as</typeparam>
/// <param name="queueName">The name of the queue you want to pop from</param>
/// <returns>Parsed value of the message or null if queue empty or missing</returns>
public async Task<T> DeleteFromQueue<T>(string queueName)

/// <summary>
/// 
/// </summary>
/// <param name="queueName">The name of the queue.</param>
/// <param name="messages"><see cref="List{object}"/>, collection of messages to add to the queue</param>
/// <returns>Tupal, (bool Success, string FailReason)</returns>
public async Task<(bool Success, string FailReason)> PushToQueue(string queueName, List<object> messages)


```