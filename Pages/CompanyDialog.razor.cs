using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CompanyDialog
    {

        [Inject]
        public CompanyService CompanyService { get; set; }

        
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }

        
        [Inject]
        public IDialogService DialogService { get; set; }

        
        public Companies Model { get; set; } = new Companies();

        
        private async Task Submit()
        {
             var response = await CompanyService.CreateAsync(Model);
            
             MudDialog.Close(response);
        }
        
        private void Cancel() => MudDialog.Cancel();
    }
}
