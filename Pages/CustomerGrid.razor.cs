using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages
{
    public partial class CustomerGrid
    {
        // Flags to control grid behavior and editing state
        private bool _readOnly;
        private bool _isCellEditMode;
        private List<string> _events = new();
        private bool _editTriggerRowClick;
        private string _searchString = "";
        private string _filterBy = "Name of the customer";

        // List to hold customer data for the grid
        public List<Customers> Datasource { get; set; }

        // Injected services for managing dialogs, customer operations, and notifications
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public CustomerService CustomerService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        // Method to initialize component and load data from the service
        protected override async Task OnInitializedAsync()
        {
            // Load customers data asynchronously
            Datasource = await CustomerService.ReadAsync();
        }

        // Quick filter method to filter customers based on search string
        private bool QuickFilter(Customers customer) => FilterFunc(customer, _searchString);

        // Method to apply filter logic based on the selected filter criteria
        private bool FilterFunc(Customers customer, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            searchString = searchString.Trim().ToLower();
            switch (_filterBy)
            {
                case "name":
                    return customer.Name.ToLower().Contains(searchString);
                case "surname":
                    return customer.Surname.ToLower().Contains(searchString);
                case "age":
                    return customer.Age.ToString().Contains(searchString);
                default:
                    return false;
            }
        }

        // Method to handle deletion of a customer item
        async Task DeleteItem(Customers item)
        {
            // Show confirmation dialog before deletion
            bool? result = await DialogService.ShowMessageBox(
                "Delete Confirmation",
                "Are you sure you want to delete this customer?",
                yesText: "Delete", cancelText: "Cancel");

            if (result == true)
            {
                // Remove item from the datasource
                Datasource.Remove(item);
                _events.Insert(0, $"Event = DeleteItem, Data = {JsonSerializer.Serialize(item)}");
                
                // Call the service to delete the customer
                await CustomerService.DeleteAsync(item);
                
                // Show success notification
                Snackbar.Add("Customer deleted successfully", Severity.Success);
            }
        }

        // Method to handle the start of editing a customer item
        void StartedEditingItem(Customers item)
        {
            _events.Insert(0, $"Event = StartedEditingItem, Data = {JsonSerializer.Serialize(item)}");
        }

        // Method to handle cancellation of editing a customer item
        void CanceledEditingItem(Customers item)
        {
            _events.Insert(0, $"Event = CanceledEditingItem, Data = {JsonSerializer.Serialize(item)}");
        }

        // Method to commit changes made to a customer item
        async Task CommittedItemChanges(Customers item)
        {
            var index = Datasource.IndexOf(item);
            
            // Update the customer item via the service
            var response = await CustomerService.UpdateAsync(item);
            
            // Update the datasource with the response
            Datasource[index] = response;
            
            _events.Insert(0, $"Event = CommittedItemChanges, Data = {JsonSerializer.Serialize(item)}");
            
            // Show success notification
            Snackbar.Add("Customer updated successfully", Severity.Success);
        }

        // Method to open a dialog for creating a new customer
        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialogParameters = new DialogParameters();
            
            // Show the dialog and wait for it to return a result
            var dialog = DialogService.Show<CustomerDialog>("Add New Customer", dialogParameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var newCustomer = (Customers)result.Data;
                
                // Add the new customer to the datasource
                var response = await CustomerService.CreateAsync(newCustomer);
                Datasource.Add(response);
                
                // Show success notification
                Snackbar.Add("Customer added successfully", Severity.Success);
            }
        }
    }
}