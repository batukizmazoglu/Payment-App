using Microsoft.AspNetCore.Components;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages;

public partial class CompanyGrid
{
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;
    
    [Inject]
    public IDialogService DialogService { get; set; }

    
    public List<Companies> Datasource { get; set; }

    
    [Inject]
    public CompanyService CompanyService { get; set; }
    
    
    protected override async Task OnInitializedAsync()
    {
        Datasource = await CompanyService.ReadAsync();
    }
    
    void DeleteItem(Companies item)
    {
        Datasource.Remove(item);
        _events.Insert(0, $"Event = DeleteItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void StartedEditingItem(Companies item)
    {
        _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(Companies item)
    {
        _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    async Task CommittedItemChanges(Companies item)
    {
        var index = Datasource.IndexOf(item);

        var response = await CompanyService.UpdateAsync(item);

        Datasource[index] = response;

        _events.Insert(0, $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }
    public enum ViewState
    {
        Create, Update, Delete
    }
    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var dialogReference = await DialogService.ShowAsync<CompanyDialog>("Simple Dialog", options);

        var response = await dialogReference.GetReturnValueAsync<Companies>();
        Datasource.Add(response);
    }
}
