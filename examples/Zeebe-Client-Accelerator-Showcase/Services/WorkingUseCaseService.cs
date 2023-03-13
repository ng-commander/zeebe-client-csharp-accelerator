using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Zeebe_Client_Accelerator_Showcase.Services
{
    public class WorkingUseCaseService : IUseCaseService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkingUseCaseService(IServiceScopeFactory serviceScopeFactory)
        {
            Debug.WriteLine("WorkingUseCaseService::Create");
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ExecuteAsync()
        {
            Debug.WriteLine("WorkingUseCaseService::ExecuteAsync");
            using var scope = _serviceScopeFactory.CreateScope();
            var disposableService = scope.ServiceProvider.GetRequiredService<DisposableService>();
            await disposableService.GenerateContentAsync();
            var content = await disposableService.GetContentAsync();
            Debug.WriteLine(content);
        }
    }
}