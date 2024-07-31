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
        
        
        [Inject]
        public IDialogService DialogService { get; set; }
        

        public Customers Model { get; set; } = new Customers();
        
        private async Task Submit()
        {
             var response = await CustomerService.CreateAsync(Model);
            MudDialog.Close(response);
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
