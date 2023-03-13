using Zeebe.Client.Accelerator.Abstractions;
using Zeebe.Client.Accelerator.Attributes;
using Zeebe_Client_Accelerator_Showcase.Services;

namespace Zeebe_Client_Accelerator_Showcase.Worker
{
    [JobType("accountService")]
    [FetchVariables("ApplicantName")] // fetches only the variable 'applicantName' - not the 'businessKey'
    public class AccountServiceWorker : IAsyncZeebeWorker
    {
        private readonly IUseCaseService _useCaseService;
        private readonly ILogger<AccountServiceWorker> _logger;

        public AccountServiceWorker(IUseCaseService useCaseService, ILogger<AccountServiceWorker> logger)
        {
            _useCaseService = useCaseService;
            _logger = logger;
        }

        public Task HandleJob(ZeebeJob job, CancellationToken cancellationToken)
        {
            // get process variables
            ProcessVariables variables = job.getVariables<ProcessVariables>();
            // get custom headers
            AccountServiceHeaders headers = job.getCustomHeaders<AccountServiceHeaders>();

            // call the account service adapter
            _logger.LogInformation("Do {Action} Account for {ApplicantName}", headers.Action, variables.ApplicantName);

            return _useCaseService.ExecuteAsync();
        }
    }

    class AccountServiceHeaders
    {
        public string Action { get; set; }
    }
}