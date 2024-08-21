using Microsoft.AspNetCore.Components;
using Payment.Client;
using Payment.Shared.Models;

namespace Payment.Web.Pages;

public partial class Index
{
    public string UserInput { get; set; }
    private LoginModel _loginModel = new LoginModel();
    [Inject] public UserService UserService { get; set; }

    private async Task CheckUsername()
    {
       var isUserNameValid =  await UserService.CheckUsername(_loginModel.Username);
       
    }
}
    
