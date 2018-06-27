using Nest;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace EasySearch.Domain
{
  public class EasySearchConfiguration
  {
    private static readonly ConnectionSettings _connectionSettings;

    private static String ElasticServer
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("ElasticServer");
      }
    }

    public static Uri CreateUri(int port)
    {

      var isFiddlerRunning = Process.GetProcessesByName("fiddler").Any();
      var host = isFiddlerRunning ? "ipv4.fiddler" : ElasticServer;

      return new Uri("http://" + host + ":" + port);
    }

    static EasySearchConfiguration()
    {
      _connectionSettings = new ConnectionSettings(CreateUri(9200))
          .DefaultIndex("persondata")
          .InferMappingFor<PersonData>(i => i
              .TypeName("personrecord")
              .IndexName("persondata")
          );
    }

    public static ElasticClient GetClient()
    {
      return new ElasticClient(_connectionSettings);
    }
  }
}
