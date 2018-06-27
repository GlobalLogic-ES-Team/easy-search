using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticSearch.Models
{
  public class LocationSummary<T>
  {
    public LocationSummary()
    {
      Performance = new Performance();
    }
    public String Formatted_Address { get; set; }
    public String PostalCode { get; set; }
    public List<T> People { get; set; }
    public Int32 Count
    {
      get;set; //intentional. We will nullify People in some cases to avoid json serialize problem 

      //get
      //{
      //  var count = 0;
      //  if (People != null)
      //    count = People.Count;
      //  return count;
      //}
    } 
    public Performance Performance { get; set; }
  }


  public class Performance
  {
    public string ElapsedTime { get; set; }
  }
}