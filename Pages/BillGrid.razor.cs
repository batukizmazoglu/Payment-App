using System.Text.Json;
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
        private string _searchString = "";
        private string _filterBy = "Amount of the bill";

        // List to hold bill data for the grid
        public List<Bills> Datasource { get; set; }
        
        // Injected services for managing dialogs, bill operations, and notifications
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public BillService BillService { get; set; }
        
        [Inject] public ISnackbar Snackbar { get; set; }
        
        // Method to initialize component and load data from the service
        protected override async Task OnInitializedAsync()
        {
            // Load bills data asynchronously
            Datasource = await BillService.ReadAsync();
        }
        
        // Quick filter method to filter bills based on search string
        private bool QuickFilter(Bills bill) => FilterFunc(bill, _searchString);

        // Method to apply filter logic based on the selected filter criteria
        private bool FilterFunc(Bills bill, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            searchString = searchString.Trim().ToLower();
            switch (_filterBy)
            {
                case "Company's ID":
                    return bill.CompanyNo.ToString().Contains(searchString);
                case "Customer's ID":
                    return bill.CustomerNo.ToString().Contains(searchString);
                case "Type":
                    return bill.Type.ToLower().Contains(searchString);
                case "Amount":
                    return bill.Amount.ToString().Contains(searchString);
                // case "Paid":
                //     return bill.Paid().Contains(searchString);
                default:
                    return false;
            }
        }

        // Method to delete a bill item from the datasource
        async Task DeleteItem(Bills item)
        {
            // Show confirmation dialog before deletion
            bool? result = await DialogService.ShowMessageBox(
                "Delete Confirmation",
                "Are you sure you want to delete this bill?",
                yesText: "Delete", cancelText: "Cancel");

            if (result == true)
            {
                // Remove item from the datasource
                Datasource.Remove(item);
                _events.Insert(0, $"Event = DeleteItem, Data = {JsonSerializer.Serialize(item)}");
                
                // Call the service to delete the bill
                await BillService.DeleteAsync(item);
                
                // Show success notification
                Snackbar.Add("Bill deleted successfully", Severity.Success);
            }
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
            
            // Show success notification
            Snackbar.Add("Bill updated successfully", Severity.Success);
        }
        
        // Enumeration for different view states
        public enum ViewState
        {
            Create, Update, Delete
        }
        
        // Method to open a dialog for creating a new bill
        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialogParameters = new DialogParameters();
            
            // Show the dialog and wait for it to return a result
            var dialog = DialogService.Show<BillDialog>("Add New Bill", dialogParameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var newBills = (Bills)result.Data;

                // Add the new bill to the datasource
                var response = await BillService.CreateAsync(newBills);
                Datasource.Add(response);

                // Show success notification
                Snackbar.Add("Bill added successfully", Severity.Success);
            }
        }
    }
}