using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticSearch.Models
{
  public class LocationSummary
  {
    public LocationSummary()
    {
      Performance = new Performance();
    }
    public String Formatted_Address { get; set; }
    public String PostalCode { get; set; }
    public List<Person> People { get; set; }
    public Performance Performance { get; set; }
  }

  public class Performance
  {
    public TimeSpan ElapsedTime { get; set; }
  }
}