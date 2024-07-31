using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class BillDialog
    {
        
        [Inject]
        public BillService BillService { get; set; }

        
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        

        [Inject]
        public IDialogService DialogService { get; set; }

        
        public Bills Model { get; set; } = new Bills();

        
        private async Task Submit()
        {
            var response = await BillService.CreateAsync(Model);

            MudDialog.Close(response);

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
