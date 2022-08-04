using CarsWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarsWeb.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _configuration;
        private readonly string _webApiUrl = "WebApi";
        private readonly string _loginEndpoint = "LoginUrl";
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index(LoginModel loginModel)
        {
            if (loginModel == null || loginModel.Empty() == true)
            {
                //suppress the validation messages
                ModelState.Clear();
                return View(new LoginModel());
            }
           
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnLogin(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var request = new HttpClient();
                var uri = new Uri(_configuration.GetSection(_webApiUrl).GetValue<string>(_loginEndpoint));
                var response = await request.PostAsJsonAsync(uri.ToString(), loginModel);
                if (response.IsSuccessStatusCode)
                {
                    var isLoggedIn = await response.Content.ReadAsStringAsync();
                    if (isLoggedIn.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return RedirectToAction("Index", "Cars", new SearchModel { Authenticated = true });
                    }
                    else
                    {
                        loginModel.LoginResult = "Unable to login";
                        return RedirectToAction("Index", "Login", loginModel);
                    }
                }
                else
                {
                    loginModel.LoginResult = "Unable to login";
                    return RedirectToAction("Index", "Login", loginModel);
                }
            }
            else
            {
                return View("Index", loginModel);
            }           
        }
    }
}
