using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CompanyDialog
    {
        // Injected service to handle company operations
        [Inject]
        public CompanyService CompanyService { get; set; }

        // Cascading parameter to manage the dialog instance
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }

        // Injected service to manage dialogs
        [Inject]
        public IDialogService DialogService { get; set; }

        // Model to bind company data in the dialog
        public Companies Model { get; set; } = new Companies();

        // Method to handle the submit action and create a new company
        private async Task Submit()
        {
            // Call the service to create a new company with the data from the model
            var response = await CompanyService.CreateAsync(Model);
            
            // Close the dialog and pass the response
            MudDialog.Close(response);
        }
        
        // Method to cancel the dialog
        private void Cancel() => MudDialog.Cancel();
    }
}