using System.ComponentModel.DataAnnotations;

namespace CarsWeb.Models
{
    public class SearchModel
    {
        public List<CarModel> Cars { get; set; }
        public bool Authenticated { get; set; }
        public string? SearchWith { get; set; }
        public string ErrorMessage { get; set; }
    }
}
