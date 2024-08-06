using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class BillGrid
    {
        // Flags to control grid behavior and editing state
        private bool _readOnly;
        private bool _isCellEditMode;
        private List<string> _events = new();
        private bool _editTriggerRowClick;

        // Injected dialog service to manage dialogs
        [Inject]
        public IDialogService DialogService { get; set; }
        
        // List to hold bill data for the grid
        public List<Bills> Datasource { get; set; }
        
        // Injected service to interact with the backend for bill operations
        [Inject]
        public BillService BillService { get; set; }
        
        // Method to initialize component and load data from the service
        protected override async Task OnInitializedAsync()
        {
            // Load bills data asynchronously
            Datasource = await BillService.ReadAsync();
        }

        // Method to delete a bill item from the datasource
        void DeleteItem(Bills item)
        {
            Datasource.Remove(item);
            _events.Insert(0, $"Event = DeleteItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to handle the start of editing a bill item
        void StartedEditingItem(Bills item)
        {
            _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to handle cancellation of editing a bill item
        void CanceledEditingItem(Bills item)
        {
            _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to commit changes made to a bill item
        async Task CommittedItemChanges(Bills item)
        {
            var index = Datasource.IndexOf(item);

            // Update the bill item via the service
            var response = await BillService.UpdateAsync(item);

            // Update the datasource with the response
            Datasource[index] = response;

            _events.Insert(0, $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }
        
        // Enumeration for different view states
        public enum ViewState
        {
            Create, Update, Delete
        }
        
        // Method to open a dialog for creating a new bill
        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            // Show the dialog and wait for it to return a result
            var dialogReference = await DialogService.ShowAsync<BillDialog>("Simple Dialog", options);

            // Add the new bill to the datasource
            var response = await dialogReference.GetReturnValueAsync<Bills>();
            Datasource.Add(response);
        }
    }
}