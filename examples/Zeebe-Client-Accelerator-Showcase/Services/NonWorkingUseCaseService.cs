using System;
using System.Diagnostics;
using System.Threading;

namespace Zeebe_Client_Accelerator_Showcase.Services
{
    public class NonWorkingUseCaseService : IUseCaseService
    {
        private readonly DisposableService _disposableService;

        public NonWorkingUseCaseService(DisposableService disposableService)
        {
            Debug.WriteLine("NonWorkingUseCaseService::Create");
            _disposableService = disposableService;
        }

        public async Task ExecuteAsync()
        {
            Debug.WriteLine("NonWorkingUseCaseService::ExecuteAsync");
            await _disposableService.GenerateContentAsync();
            var content = await _disposableService.GetContentAsync();
            Debug.WriteLine(content);
        }
    }
}