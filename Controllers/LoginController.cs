using System.Text;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Models;

namespace StarterKit.Controllers;


[Route("api/v1/Login")]
public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    public static LoginStatus LoggedIn;
    public static Customer CurrentLoggedIn = null;
    

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginBody loginBody)
    {
        // TODO: Impelement login method
        LoginStatus status = _loginService.CheckPassword(loginBody.Username, loginBody.Password);
        if (status is LoginStatus.AdminSuccess)
        {
            LoggedIn = LoginStatus.AdminSuccess;
            return Ok("Logged in as Admin");
        }
        if (status is LoginStatus.UserSuccess)
        {
            LoggedIn = LoginStatus.UserSuccess;
            CurrentLoggedIn = _loginService.GetCustomerAccount(loginBody.Username, loginBody.Password);
            return Ok("Logged in!");
        }
        if (status is LoginStatus.IncorrectUsername) return Unauthorized("Incorrect username");
        return Unauthorized($"Incorrect password");
    }

    [HttpGet("IsAdminLoggedIn")]
    public IActionResult IsAdminLoggedIn()
    {
        // TODO: This method should return a status 200 OK when logged in, else 403, unauthorized
        if (LoggedIn == LoginStatus.AdminSuccess) return Ok("You are logged in!");
        return Unauthorized("You are not logged in");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        LoggedIn = LoginStatus.LoggedOut;
        CurrentLoggedIn = null;
        return Ok("Logged out");
    }

}

public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
