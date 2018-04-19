using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ElasticSearch
{
    public class Helper
  {
    private String ApiKey = "AIzaSyBoB7nh-_GdCKkiqMlz41AJ9XuLxQcwlmA";
    public Models.LocationDetail GetLocation(string lat, string lng)
    {
      String requestApi = String.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}", lat, lng, ApiKey);
      var request = (HttpWebRequest)WebRequest.Create(requestApi);

      request.Method = "GET";
      request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
      request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

      var response = (HttpWebResponse)request.GetResponse();

      string content = string.Empty;
      using (var stream = response.GetResponseStream())
      {
        using (var sr = new StreamReader(stream))
        {
          content = sr.ReadToEnd();
        }
      }
      //Models.LocationDetail loc = (Models.LocationDetail)content);
    return  JObject.Parse(content).ToObject<Models.LocationDetail>();
    }

    public static string ToReadbileTime(long ticks)
    {
      int minutes = TimeSpan.FromTicks(ticks).Minutes;
      int seconds = TimeSpan.FromTicks(ticks).Seconds;
      int milliSeconds = TimeSpan.FromTicks(ticks).Milliseconds;

      List<string> timeParts = new List<string>();

      if (minutes > 0) timeParts.Add(String.Format("{0} minutes", minutes));
      if (seconds > 0) timeParts.Add(String.Format("{0} seconds", seconds));
      if (milliSeconds > 0) timeParts.Add(String.Format("{0} milliSeconds", milliSeconds));

      return string.Join(" ", timeParts);
    }
  }
}