using Microsoft.AspNetCore.Mvc.Filters;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Services;


namespace StarterKit.Filters;
public class UserOnly : Attribute, IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
  {
    var context = actionContext.HttpContext;
    if (LoginController.LoggedIn != LoginStatus.UserSuccess)
    {
        context.Response.StatusCode = 403;
        context.Response.WriteAsJsonAsync("User needs to be logged in to be able to access this functionality");
        return;
    }
    await next();
  }
}