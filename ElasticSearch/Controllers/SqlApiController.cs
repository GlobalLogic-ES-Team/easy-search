using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ElasticSearch.Models;
using System.Web.Mvc;

namespace ElasticSearch.Controllers
{
  public class SqlApiController : Controller
  {
    LocationSummary<Person> result = new LocationSummary<Person>();

    [HttpPost]
    public JsonResult GetLocationDetail(String Lat, String Lng)
    {
      //dynamic dyn = jsonData;
      //var lat = dyn.Lat.Value as string;
      //var lng = dyn.Lng.Value as string;


      Helper helper = new Helper();
      LocationDetail location = helper.GetLocation(Lat, Lng);

      if (location != null && location.Results != null && location.Results.Count > 0)
      {
        var loc = location.Results[0].address_components.Where(e => e.types[0] == "postal_code");
        result.Formatted_Address = location.Results[0].formatted_address;
        if (loc != null && loc.Count() > 0)
          result.PostalCode = loc.First().long_name;
      }
      else
        throw new Exception("Machli machli jal ki rani!");
      //Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Machli machli jal ki rani!");

      result.People = GetByZip(result.PostalCode);
      result.People = result.People.OrderBy(e => e.firstname).ToList();
      return Json(result, JsonRequestBehavior.AllowGet);
      // Request.CreateResponse(HttpStatusCode.OK, result);
    }

    private List<Person> GetByZip(String zip)
    {
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      Models.ConnectionString context = new Models.ConnectionString();
      var people = context.People.Where(e => e.zip == zip).ToList<Models.Person>();
      sw.Stop();
      result.Performance.ElapsedTime = Helper.ToReadbileTime(sw.ElapsedTicks);

      return people;
    }

    //[HttpPost]
    //public HttpResponseMessage GetbyZipcode(JObject jsonData)
    //{
    //  dynamic dyn = jsonData;
    //  var searchString = dyn.SearchString.Value as string;

    //  result.PostalCode = searchString;
    //  result.People = GetByZip(result.PostalCode);
    //  result.Formatted_Address = String.Format("Searching for zip code{0}", result.PostalCode);

    //  return Request.CreateResponse(HttpStatusCode.OK, result);
    //}

    [HttpPost]
    public JsonResult SearchText(String SearchString)
    {
      //dynamic dyn = jsonData;
      //var searchString = dyn.SearchString.Value as string;

      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      Models.ConnectionString context = new Models.ConnectionString();

      sw.Start();
      result.People = context.People.Where(e => e.json_data.Contains(SearchString)).ToList<Models.Person>();
      sw.Stop();

      result.Performance.ElapsedTime = Helper.ToReadbileTime(sw.ElapsedTicks);

      return Json(result, JsonRequestBehavior.AllowGet);
      //Request.CreateResponse(HttpStatusCode.OK, result);
    }

  }
}
