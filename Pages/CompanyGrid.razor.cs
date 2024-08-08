using System.Text.Json;
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
        private string _searchString = "";
        private string _filterBy = "Name of the company";

        // List to hold company data for the grid
        public List<Companies> Datasource { get; set; }

        // Injected services for managing dialogs, companies operations, and notifications
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public CompanyService CompanyService { get; set; }

        [Inject] public ISnackbar Snackbar { get; set; }

        // Method to initialize component and load data from the service
        protected override async Task OnInitializedAsync()
        {
            // Load companies data asynchronously
            Datasource = await CompanyService.ReadAsync();
        }

        // Quick filter method to filter companies based on search string
        private bool QuickFilter(Companies company) => FilterFunc(company, _searchString);

        // Method to apply filter logic based on the selected filter criteria
        private bool FilterFunc(Companies company, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            searchString = searchString.Trim().ToLower();
            switch (_filterBy)
            {
                case "Name":
                    return company.Name.ToLower().Contains(searchString);
                case "Address":
                    return company.Address.ToLower().Contains(searchString);
                case "City":
                    return company.City.ToLower().Contains(searchString);
                default:
                    return false;
            }
        }

        // Method to delete a company item from the datasource
        async Task DeleteItem(Companies item)
        {
            // Show confirmation dialog before deletion
            bool? result = await DialogService.ShowMessageBox(
                "Delete Confirmation",
                "Are you sure you want to delete this company?",
                yesText: "Delete", cancelText: "Cancel");

            if (result == true)
            {
                // Remove item from the datasource
                Datasource.Remove(item);
                _events.Insert(0, $"Event = DeleteItem, Data = {JsonSerializer.Serialize(item)}");

                // Call the service to delete the company
                await CompanyService.DeleteAsync(item);

                // Show success notification
                Snackbar.Add("Company deleted successfully", Severity.Success);
            }
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

            _events.Insert(0,
                $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");

            // Show success notification
            Snackbar.Add("Company updated successfully", Severity.Success);
        }

        // Enumeration for different view states
        public enum ViewState
        {
            Create,
            Update,
            Delete
        }

        // Method to open a dialog for creating a new company
        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialogParameters = new DialogParameters();

            // Show the dialog and wait for it to return a result
            var dialog = DialogService.Show<CompanyDialog>("Add New Company", dialogParameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var newCompanies = (Companies)result.Data;

                // Add the new customer to the datasource
                var response = await CompanyService.CreateAsync(newCompanies);
                Datasource.Add(response);
                
                // Show success notification
                Snackbar.Add("Customer added successfully", Severity.Success);
                
            }
        }
    }
}