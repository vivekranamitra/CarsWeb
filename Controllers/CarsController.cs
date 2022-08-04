using CarsWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarsWeb.Controllers
{
    public class CarsController : Controller
    {
        private IConfiguration _configuration;
        private readonly string _webApiUrl = "WebApi";
        private readonly string _carSearchEndpoint = "CarsSearchUrl";
        private readonly string _carAddEndpoint = "CarsSearchUrl";

        public CarsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchModel searchModel)
        {
            //check for authentication
            if (searchModel.Authenticated == false)
            {
                return RedirectToAction("Index", "Login");
            }

            var uri = new Uri(_configuration.GetSection(_webApiUrl).GetValue<string>(_carSearchEndpoint));
            if (!string.IsNullOrEmpty(searchModel.SearchWith))
            {
                uri = new Uri(uri.ToString() + $"/{searchModel.SearchWith}");
            }

            var request = new HttpClient();
            var response = await request.GetAsync(uri.ToString());
            if (response.IsSuccessStatusCode)
            {
                string? carList = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(carList) && carList.Length > 0)
                {
                    return View(new SearchModel
                    {
                        Cars = JsonConvert.DeserializeObject<List<CarModel>>(carList),
                        SearchWith = searchModel.SearchWith,
                        Authenticated = searchModel.Authenticated,
                        ErrorMessage = searchModel.ErrorMessage
                    });
                }
            }
            else
            {
                return View();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string make, string model, string searchWith, bool authenticated)
        {
            if (string.IsNullOrEmpty(make) || string.IsNullOrEmpty(model))
            {
                return RedirectToAction("Index", "Cars", new SearchModel
                {
                    SearchWith = searchWith,
                    Authenticated = authenticated,
                    ErrorMessage = "Make and Model are required"
                });
            }
            var newCarModel = new CarModel
            {
                Make = make,
                Model = model
            };
            var uri = new Uri(_configuration.GetSection(_webApiUrl).GetValue<string>(_carAddEndpoint));

            var request = new HttpClient();
            var response = await request.PostAsJsonAsync(uri.ToString(), newCarModel);
            if (response.IsSuccessStatusCode)
            {
                string returnValue = await response.Content.ReadAsStringAsync();
                if (returnValue.ToLower().Equals("true"))
                {
                    return RedirectToAction("Index", "Cars", new SearchModel
                    {
                        SearchWith = searchWith,
                        Authenticated = authenticated,
                        ErrorMessage = ""
                    });
                }
                else
                {
                    return RedirectToAction("Index", "Cars", new SearchModel
                    {
                        SearchWith = searchWith,
                        Authenticated = authenticated,
                        ErrorMessage = "Could not add car"
                    });
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Cars", new SearchModel
                {
                    SearchWith = searchWith,
                    Authenticated = authenticated,
                    ErrorMessage = "Could not add car"
                });
            }
        }
    }
}
