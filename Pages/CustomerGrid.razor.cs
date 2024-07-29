using GenFu;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Payment.Client;
using Payment.Shared;

namespace Payment.Web.Pages;

  
 
 
public partial class CustomerGrid
{
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;

    public List<Customers> Datasource { get; set; }

    [Inject]
    public CustomerService CustomerService { get; set; }

    public Customers newCustomer { get; set; }

    [CascadingParameter(Name = "Khalid")]

    public string Khalid { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //Datasource = await Database.Customers.ToListAsync();
       
        Datasource = await CustomerService.ReadAsync();

    }
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }
    //void AddNewCustomer()
    //{
    //    newCustomer = A.New<Customers>();
    //    newCustomer.CustomerNo = 0;
    //    Datasource.Add(newCustomer);
    //    Database.Customers.Add(newCustomer);
    //    _events.Insert(0, $"Event = AddNewCustomer, Data = {System.Text.Json.JsonSerializer.Serialize(newCustomer)}");
    //    Database.SaveChanges();
    //}

    void DeleteItem(Customers item)
    {
        Datasource.Remove(item);
        _events.Insert(0, $"Event = DeleteItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
        //Database.Customers.Remove(item);
        //Database.SaveChanges();
    }

    void StartedEditingItem(Customers item)
    {
        _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(Customers item)
    {
        _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    async Task CommittedItemChanges(Customers item)
    {
         var index = Datasource.IndexOf(item);

         var response = await CustomerService.UpdateAsync(item);

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

        var dialogReference = await DialogService.ShowAsync<CustomerDialog>("Simple Dialog", options);

        var response = await dialogReference.GetReturnValueAsync<Customers>();
        Datasource.Add(response);
        Console.WriteLine(  );
    }
}
