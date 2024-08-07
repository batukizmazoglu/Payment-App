using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CustomerDialog
    {
        // Injected service to handle customer operations
        [Inject]
        public CustomerService CustomerService { get; set; }

        // Cascading parameter to manage the dialog instance
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }

        // Injected service to manage dialogs
        [Inject]
        public IDialogService DialogService { get; set; }

        // Model to bind customer data in the dialog
        public Customers Model { get; set; } = new Customers();

        // Method to handle the submit action and create a new customer
        private async Task Submit()
        {
            // Call the service to create a new customer with the data from the model
            var response = await CustomerService.CreateAsync(Model);
            
            // Close the dialog and pass the response
            MudDialog.Close(response);
        }

        // Method to cancel the dialog
        private void Cancel() => MudDialog.Cancel();
    }
}