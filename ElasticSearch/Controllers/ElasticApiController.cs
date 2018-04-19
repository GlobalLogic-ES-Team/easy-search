using AutoMapper;
using EasySearch.Domain;
using ElasticSearch.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ElasticSearch.Controllers
{
  public class ElasticApiController : ApiController
  {
    LocationSummary<ElasticSearch.Models.Person> result = new LocationSummary<ElasticSearch.Models.Person>();

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
      result.Formatted_Address = String.Format("Searching {0} from ElasticSearch", result.PostalCode);

      return Mapper.Map<List<PersonData>, List<Person>>(search_result);
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
  }
}
