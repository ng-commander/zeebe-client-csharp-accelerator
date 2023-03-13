using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Zeebe_Client_Accelerator_Showcase.Services
{
    public class DisposableService : IDisposable
    {
        private bool disposed = false;

        private readonly StringWriter _disposableResource = new StringWriter();

        public DisposableService()
        {
            Debug.WriteLine("DisposableService::Create");
        }

        public async Task GenerateContentAsync()
        {
            Debug.WriteLine("DisposableService::GenerateContentAsync");
            CheckDisposed();
            await WriteLoremIpsum();
        }

        public async Task<string> GetContentAsync()
        {
            Debug.WriteLine("DisposableService::GetContentAsync");
            CheckDisposed();
            return await Task.FromResult(_disposableResource.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine($"DisposableService::Dispose({disposing})");

            if (disposing)
            {
                _disposableResource.Dispose();
                disposed = true;
            }
        }

        private void CheckDisposed()
        {
            Debug.WriteLine("DisposableService::CheckDisposed");
            if (disposed) throw new ObjectDisposedException(nameof(DisposableService));
        }

        private async Task WriteLoremIpsum()
        {
            Debug.WriteLine("DisposableService::WriteLoremIpsum");
            CheckDisposed();
            await _disposableResource.WriteLineAsync("Lorem ipsum");
            await Task.Delay(500);
        }
    }
}