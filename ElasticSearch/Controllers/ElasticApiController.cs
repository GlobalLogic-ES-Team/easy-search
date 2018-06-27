using AutoMapper;
using EasySearch.Domain;
using ElasticSearch.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ElasticSearch.Controllers
{
  public class ElasticApiController : Controller
  {
    LocationSummary<ElasticSearch.Models.Person> result = new LocationSummary<ElasticSearch.Models.Person>();

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
        throw new Exception("Did you click Ocean? There is no zipcode.");

      result.People = GetByZip(result.PostalCode);
      result.People = result.People.OrderBy(e => e.firstname).ToList();

      if (result != null && result.People != null)
      {
        result.Count = result.People.Count;
      }

      return Json(result, JsonRequestBehavior.AllowGet);
    }

    private List<ElasticSearch.Models.Person> GetByZip(String zip)
    {
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      var client = EasySearchConfiguration.GetClient();
      var searchResults = client.Search<PersonData>(s => s
          .From(0)
          .Size(10000)
          .Query(q => q
               .Term(p => p.postCode, zip)
          )
      );
      var search_result = searchResults.Documents.ToList();

      sw.Stop();
      result.Performance.ElapsedTime = Helper.ToReadbileTime(sw.ElapsedTicks);
      result.PostalCode = zip;

      return Mapper.Map<List<PersonData>, List<Person>>(search_result);
    }

    


    [HttpPost]
    public JsonResult SearchText(String SearchString)
    {
      //dynamic dyn = jsonData;
      //var searchString = dyn.SearchString.Value as string;

      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

      sw.Start();
      var client = EasySearchConfiguration.GetClient();
      var searchResults = client.Search<PersonData>(s => s
      .AllTypes()
       .From(0)
       .Take(10000)
       .Query(qry => qry
       .Bool(b => b
       .Must(m => m
       .QueryString(qs => qs
       .DefaultField("_all")
       .Query(String.Format("*{0}*", SearchString)))))));

      var search_result = searchResults.Documents.ToList<PersonData>();

      sw.Stop();
      result.Performance.ElapsedTime = Helper.ToReadbileTime(sw.ElapsedTicks);

      result.People = Mapper.Map<List<PersonData>, List<Person>>(search_result);

      if (result != null && result.People != null)
      {
        result.Count = result.People.Count;
        if (result.Count > 1000)
          result.People = null;
      }

      return Json(result, JsonRequestBehavior.AllowGet);
      //Request.CreateResponse(HttpStatusCode.OK, result);
    }
  }
}
