using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;
using System.Text.Json;

namespace Payment.Web.Pages;

public partial class CustomerGrid
{
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;
    private string _searchString = "";
    private string _filterBy = "name";

    public List<Customers> Datasource { get; set; }

    [Inject] public IDialogService DialogService { get; set; }
    [Inject] public CustomerService CustomerService { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Datasource = await CustomerService.ReadAsync();
    }

    private bool QuickFilter(Customers customer) => FilterFunc(customer, _searchString);

    private bool FilterFunc(Customers customer, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        searchString = searchString.Trim().ToLower();
        switch (_filterBy)
        {
            case "name":
                return customer.name.ToLower().Contains(searchString);
            case "surname":
                return customer.surname.ToLower().Contains(searchString);
            case "age":
                return customer.age.ToString().Contains(searchString);
            default:
                return false;
        }
    }

    async Task DeleteItem(Customers item)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Delete Confirmation",
            "Are you sure you want to delete this customer?",
            yesText: "Delete", cancelText: "Cancel");

        if (result == true)
        {
            Datasource.Remove(item);
            _events.Insert(0, $"Event = DeleteItem, Data = {JsonSerializer.Serialize(item)}");
            await CustomerService.DeleteAsync(item); // Changed from item.id to item
            Snackbar.Add("Customer deleted successfully", Severity.Success);
        }
    }

    void StartedEditingItem(Customers item)
    {
        _events.Insert(0, $"Event = StartedEditingItem, Data = {JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(Customers item)
    {
        _events.Insert(0, $"Event = CanceledEditingItem, Data = {JsonSerializer.Serialize(item)}");
    }

    async Task CommittedItemChanges(Customers item)
    {
        var index = Datasource.IndexOf(item);
        var response = await CustomerService.UpdateAsync(item);
        Datasource[index] = response;
        _events.Insert(0, $"Event = CommittedItemChanges, Data = {JsonSerializer.Serialize(item)}");
        Snackbar.Add("Customer updated successfully", Severity.Success);
    }

    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialogParameters = new DialogParameters();
        var dialog = DialogService.Show<CustomerDialog>("Add New Customer", dialogParameters, options);
        var result = await dialog.Result;

        if (!result.Canceled) // Changed from Cancelled to Canceled
        {
            var newCustomer = (Customers)result.Data;
            var response = await CustomerService.CreateAsync(newCustomer);
            Datasource.Add(response);
            Snackbar.Add("Customer added successfully", Severity.Success);
        }
    }
}