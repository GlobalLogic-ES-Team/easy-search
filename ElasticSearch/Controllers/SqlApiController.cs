using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using ElasticSearch.Models;

namespace ElasticSearch.Controllers
{
  public class SqlApiController : ApiController
  {

    [HttpPost]
    public void Search(JObject jsonData)
    {
      dynamic dyn = jsonData;
      var searchString = dyn.SearchString.Value as string;


      //Models.ConnectionString context = new Models.ConnectionString();
      //var result = context.People.Where(e => e.Description.Contains(searchString)).ToList<Models.Person>();

    }
    LocationSummary<Person> result = new LocationSummary<Person>();

    [HttpPost]
    public HttpResponseMessage GetLocationDetail(JObject jsonData)
    {
      dynamic dyn = jsonData;
      var lat = dyn.Lat.Value as string;
      var lng = dyn.Lng.Value as string;


      Helper helper = new Helper();
      LocationDetail location = helper.GetLocation(lat, lng);

      if (location != null && location.Results != null && location.Results.Count > 0)
      {
        var loc = location.Results[0].address_components.Where(e => e.types[0] == "postal_code");
        result.Formatted_Address = location.Results[0].formatted_address;
        if (loc != null && loc.Count() > 0)
          result.PostalCode = loc.First().long_name;
      }
      else
        return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Machli machli jal ki rani!");

      result.People = GetByZip(result.PostalCode);


      return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    private List<Person> GetByZip(String zip)
    {
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      Models.ConnectionString context = new Models.ConnectionString();
      sw.Stop();
      result.Performance.ElapsedTime = sw.Elapsed;
      return context.People.Where(e => e.zip == zip).ToList<Models.Person>();
    }

    [HttpPost]
    public HttpResponseMessage GetbyZipcode(JObject jsonData)
    {
      dynamic dyn = jsonData;
      var searchString = dyn.SearchString.Value as string;

      result.PostalCode = searchString;
      result.People = GetByZip(result.PostalCode);
      result.Formatted_Address = String.Format("Searching for zip code{0}", result.PostalCode);

      return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    //[HttpPost]
    //public void GetbyLastName(JObject jsonData)
    //{
    //  dynamic dyn = jsonData;
    //  var searchString = dyn.SearchString.Value as string;

    //  Models.ConnectionString context = new Models.ConnectionString();
    //  var result = context.People.Where(e => e.lastname.Contains(searchString)).ToList<Models.Person>();
    //}

    //[HttpPost]
    //public void GetbyGender(JObject jsonData)
    //{
    //  dynamic dyn = jsonData;
    //  var searchString = dyn.SearchString.Value as string;

    //  Models.ConnectionString context = new Models.ConnectionString();
    //  var result = context.People.Where(e => e.gender == searchString).ToList<Models.Person>();
    //}

  }
}
