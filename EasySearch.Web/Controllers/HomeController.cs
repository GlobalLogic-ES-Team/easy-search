using EasySearch.Domain;
using EasySearch.Web.Models;
using System.Web.Mvc;

namespace EasySearch.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var client = EasySearchConfiguration.GetClient();

            var searchResults = client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                     .Term(p => p.Zip, "80301")
                )
            );

            var model = new SearchViewModel();
            model.Persons = searchResults.Documents;
            model.Total = searchResults.Total;

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}