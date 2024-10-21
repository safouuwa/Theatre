using Microsoft.AspNetCore.Mvc.Filters;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Services;


namespace StarterKit.Filters;
public class GuestOnly : Attribute, IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
  {
    var context = actionContext.HttpContext;
    if (LoginController.LoggedIn == LoginStatus.AdminSuccess || LoginController.LoggedIn == LoginStatus.UserSuccess)
    {
        context.Response.StatusCode = 403;
        context.Response.WriteAsJsonAsync("You are currently logged in and attempting to use a functionality for guests. Log out to proceed.");
        return;
    }
    await next();
  }
}