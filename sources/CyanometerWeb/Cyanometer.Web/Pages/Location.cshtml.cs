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
        public string PollutionIcon => "circle";
        public string PollutionColor => "#F17E19";
        public string PollutionText => "HIGH";
        public string LevelsText => "NITROGEN DIOXIDE";
        public Pollution Pollution { get; private set; }
        public void OnGet(string city, string country)
        {
            City = city.Titleize();
            Country = country.Titleize();
            AirQualitySource = "ARSO Ljubljana Bežigrad";
            AirQualityLink = "https://www.arso.gov.si/";
            Pollution = new Pollution(92, 13, 4, 6);
        }
    }

    public class Pollution
    {
        public int Ozone { get; }
        public int PM10 { get; }
        public int SO2 { get; }
        public int NO2 { get; }
        public Pollution(int ozone, int pM10, int sO2, int nO2)
        {
            Ozone = ozone;
            PM10 = pM10;
            SO2 = sO2;
            NO2 = nO2;
        }
    }
}