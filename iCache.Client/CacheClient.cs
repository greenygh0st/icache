using System;
using System.Text;

namespace iCache.Client
{
    public class CacheClient : IDisposable
    {
        private bool disposedValue;
        private string _username;
        private string _password;
        private string _authHeaderValue;

        public CacheClient(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username", "You must supply the username parameter!");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "You must supply the password parameter!");

            _username = username;
            _password = password;
            _authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
        }

        #region Keys

        #endregion

        #region Queues

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
