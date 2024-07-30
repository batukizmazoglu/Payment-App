using MudBlazor;
using Microsoft.AspNetCore.Components;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages;

public partial class BillGrid
{
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;

    [Inject]
    public IDialogService DialogService { get; set; }

    public List<Bills> Datasource { get; set; }

    [Inject]
    public BillService BillService { get; set; }

    // override 
    //protected override async Task OnInitializedAsync() => 
    // protected async Task OnInitializedAsync()
    // {
    //     Datasource = await BillService.ReadAsync();
    //     await base.OnInitializedAsync();
    // }

    void DeleteItem(Bills item)
    {
        Datasource.Remove(item);
        _events.Insert(0, $"Event = DeleteItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void StartedEditingItem(Bills item)
    {
        _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(Bills item)
    {
        _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    async Task CommittedItemChanges(Bills item)
    {
        var index = Datasource.IndexOf(item);

        var response = await BillService.UpdateAsync(item);

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

        var dialogReference = await DialogService.ShowAsync<BillDialog>("Simple Dialog", options);

        var response = await dialogReference.GetReturnValueAsync<Bills>();
        Datasource.Add(response);
        Console.WriteLine();
    }
}
