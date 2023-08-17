using CoffeeBarr.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoffeeBarr.Controllers
{
    public class MenuController : Controller
    {
        private readonly ILogger<MenuController> _logger;
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static readonly string ApplicationName = "Menu";
        static readonly string SpreadsheetID = "1B0AYOxFFn1NLa0NyzFLtn5zDLDEu9icBjdyuIV4df3g";
        static readonly string sheet = "MenuItems";
        static SheetsService service;

        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Fetch data from Google Sheets
            var data =  GetMenuDataFromSheets();

            // Pass the fetched data to the view
            return View(data);
        }

        private List<IList<object>> GetMenuDataFromSheets()
        {
            var range = $"{sheet}!A1:H";
            var request = service.Spreadsheets.Values.Get(SpreadsheetID, range);
            
            var response = request.Execute();
            var values = response.Values.ToList();

            return values;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
