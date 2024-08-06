

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class BillDialog
    {
        // Injected service to handle bill operations
        [Inject]
        public BillService BillService { get; set; }

        // Cascading parameter to control the dialog instance
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        
        // Injected service to handle dialog operations
        [Inject]
        public IDialogService DialogService { get; set; }

        // Model to bind bill data in the dialog
        public Bills Model { get; set; } = new Bills();

        // Method to handle the submit action and create a new bill
        private async Task Submit()
        {
            // Call the service to create a new bill with the data from the model
            var response = await BillService.CreateAsync(Model);

            // Close the dialog and pass the response
            MudDialog.Close(response);
        }

        // Method to cancel the dialog
        private void Cancel() => MudDialog.Cancel();
    }
}