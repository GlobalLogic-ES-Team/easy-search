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
        LocationSummary<EasySearch.Domain.Person> result = new LocationSummary<EasySearch.Domain.Person>();

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

        private List<EasySearch.Domain.Person> GetByZip(String zip)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var client = EasySearchConfiguration.GetClient();

            var searchResults = client.Search<EasySearch.Domain.Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                     .Term(p => p.Zip, zip)
                )
            );

            var results = searchResults.Documents.ToList();

            sw.Stop();

            result.Performance.ElapsedTime = sw.Elapsed;

            return results;
        }
    }
}
