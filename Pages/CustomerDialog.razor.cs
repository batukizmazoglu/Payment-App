
using GenFu;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CustomerDialog
    {

        [Inject]
        public CustomerService CustomerService { get; set; }

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }

        //[Parameter]
        //public Customers Customers { get; set; }

        public Customers newAddedCustomer { get; set; } = new Customers();

        private async Task Submit()
        {
             var response = await CustomerService.CreateAsync(newAddedCustomer);

            MudDialog.Close(response);
            
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
