using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Cyanometer.Web.Pages
{
    public class LocationModel : PageModel
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AirQualitySource { get; private set; }
        public string AirQualityLink { get; private set; }
        public string AirQualityHost => new Uri(AirQualityLink).Host;
        public void OnGet(string city, string country)
        {
            City = city.Titleize();
            Country = country.Titleize();
            AirQualitySource = "ARSO Ljubljana Bežigrad";
            AirQualityLink = "https://www.arso.gov.si/";
        }
    }
}