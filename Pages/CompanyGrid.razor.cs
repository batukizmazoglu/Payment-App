using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CompanyGrid
    {
        // Flags to control grid behavior and editing state
        private bool _readOnly;
        private bool _isCellEditMode;
        private List<string> _events = new();
        private bool _editTriggerRowClick;
        
        // Injected dialog service to manage dialogs
        [Inject]
        public IDialogService DialogService { get; set; }

        // List to hold company data for the grid
        public List<Companies> Datasource { get; set; }

        // Injected service to interact with the backend for company operations
        [Inject]
        public CompanyService CompanyService { get; set; }
        
        // Method to initialize component and load data from the service
        protected override async Task OnInitializedAsync()
        {
            // Load companies data asynchronously
            Datasource = await CompanyService.ReadAsync();
        }

        // Method to delete a company item from the datasource
        void DeleteItem(Companies item)
        {
            Datasource.Remove(item);
            _events.Insert(0, $"Event = DeleteItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to handle the start of editing a company item
        void StartedEditingItem(Companies item)
        {
            _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to handle cancellation of editing a company item
        void CanceledEditingItem(Companies item)
        {
            _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Method to commit changes made to a company item
        async Task CommittedItemChanges(Companies item)
        {
            var index = Datasource.IndexOf(item);

            // Update the company item via the service
            var response = await CompanyService.UpdateAsync(item);

            // Update the datasource with the response
            Datasource[index] = response;

            _events.Insert(0, $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        }

        // Enumeration for different view states
        public enum ViewState
        {
            Create, Update, Delete
        }

        // Method to open a dialog for creating a new company
        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            // Show the dialog and wait for it to return a result
            var dialogReference = await DialogService.ShowAsync<CompanyDialog>("Simple Dialog", options);

            // Add the new company to the datasource
            var response = await dialogReference.GetReturnValueAsync<Companies>();
            Datasource.Add(response);
        }
    }
}