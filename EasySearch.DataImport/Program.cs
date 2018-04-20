using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using Nest;
using Elasticsearch.Net;

namespace EasySearch.DataImport
{
    class Program
    {
        private const string URL = "https://randomuser.me/api/";

        const int RecordCountInsert = 5000;

        static string apiParam = ConfigurationManager.AppSettings["RandomUserApiParams"];

        static string urlParameters = string.Format(apiParam, RecordCountInsert);

        public static ElasticClient EsClient()
        {
            ESClusterConfiguration esClusterConfig = new ESClusterConfiguration();

            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;

            string index = esClusterConfig.ESConfigSection.IndexName;

            //Multiple node for fail over (cluster addresses)
            var nodes = new List<Uri>();

            foreach (NodeElement node in esClusterConfig.ESConfigSection.Nodes)
            {
                nodes.Add(new Uri(node.Uri));
            }

            connectionPool = new StaticConnectionPool(nodes.ToArray());

            //Connection string for Elasticsearch
            connectionSettings = new ConnectionSettings(connectionPool)
                            .DefaultIndex(index)
                            .MaximumRetries(2)
                            .MaxRetryTimeout(TimeSpan.FromSeconds(150));

            elasticClient = new ElasticClient(connectionSettings);

            //connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);

            // create the index if it doesn't exist
            if (!elasticClient.IndexExists(index).Exists)
            {
                elasticClient.CreateIndex(index);
            }

            return elasticClient;
        }

        static void Main(string[] args)
        {
            ESClusterConfiguration esClusterConfig = new ESClusterConfiguration();

            try
            {
                #region This is for reading Salaries & Interests from a Sample Json 
                IEnumerable<SampleData> sampleModel;
                List<SampleData> listSampleModel;

                string path = ConfigurationManager.AppSettings["SampleJsonPath"];
                using (StreamReader r = new StreamReader(path))
                {
                    JObject obj = JObject.Parse(r.ReadToEnd());

                    var list = (JArray)obj.SelectToken("results");

                    sampleModel =
                       from item in (JArray)obj["results"]
                       select new SampleData
                       {
                           Salary = item["Salary"].Value<string>(),
                           Interests = item["Interests"].Value<string>()
                       };

                }
                listSampleModel = sampleModel.ToList();
                #endregion

                //Call randomuser.me API for fetching random users
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                DataTable tbl = new DataTable("PersonTable");
                tbl.Columns.Add("ssn", typeof(String));
                tbl.Columns.Add("gender", typeof(String));
                tbl.Columns.Add("firstname", typeof(String));
                tbl.Columns.Add("lastname", typeof(String));
                tbl.Columns.Add("email", typeof(String));
                tbl.Columns.Add("dob", typeof(String));
                tbl.Columns.Add("cell", typeof(String));
                tbl.Columns.Add("salary", typeof(decimal));
                tbl.Columns.Add("interests", typeof(String));
                tbl.Columns.Add("json_data", typeof(String));
                tbl.Columns.Add("street", typeof(String));
                tbl.Columns.Add("state", typeof(String));
                tbl.Columns.Add("city", typeof(String));
                tbl.Columns.Add("zip", typeof(String));
                //tbl.Columns.Add("description", typeof(String));

                DataRow row = null;

                if (!int.TryParse(ConfigurationManager.AppSettings["DataImportLoopCounter"], out int loopCounter))
                    loopCounter = 1;

                for (int i = 0; i < loopCounter; i++)
                {
                    try
                    {
                        if (i > 0)
                        {
                            //Taking some pause as API can't handle too many requests
                            Thread.Sleep(10000);
                        }

                        Console.WriteLine("Counter : " + i);

                        HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
                        if (response.IsSuccessStatusCode)
                        {
                            string json = response.Content.ReadAsStringAsync().Result;

                            PersonResponse personResp = JsonConvert.DeserializeObject<PersonResponse>(json);

                            //Inject Salary & Interest
                            for (int j = 0; j < RecordCountInsert; j++)
                            {
                                personResp.results[j].Salary = Convert.ToDecimal(listSampleModel[j].Salary);
                                personResp.results[j].Interests = listSampleModel[j].Interests;
                            }

                            //Serialize to json again
                            json = JsonConvert.SerializeObject(personResp);

                            JObject obj = JObject.Parse(json);

                            var list = (JArray)obj.SelectToken("results");

                            var listPerson = (JArray)obj.SelectToken("results");

                            for (int k = 0; k < RecordCountInsert; k++)
                            {
                                row = tbl.NewRow();
                                row["ssn"] = personResp.results[k].id.value;
                                row["gender"] = personResp.results[k].gender;
                                row["firstname"] = personResp.results[k].name.first;
                                row["lastname"] = personResp.results[k].name.last;
                                row["email"] = personResp.results[k].email;
                                row["dob"] = personResp.results[k].dob;
                                row["cell"] = personResp.results[k].cell;
                                row["salary"] = personResp.results[k].Salary;
                                row["interests"] = personResp.results[k].Interests;
                                
                                row["street"] = personResp.results[k].location.street;
                                row["state"] = personResp.results[k].location.state;
                                row["city"] = personResp.results[k].location.city;                                
                                row["zip"] = Convert.ToString(personResp.results[k].location.postcode);
                                //row["description"] = "Dummy text"; // Praveer can use this field

                                //Creating PersonRecord object as we want to store data
                                PersonRecord ps = new PersonRecord
                                {
                                    IdType = personResp.results[k].id.name,
                                    Id = personResp.results[k].id.value,
                                    Gender = personResp.results[k].gender,
                                    Title = personResp.results[k].name.title,
                                    FirstName = personResp.results[k].name.first,
                                    LastName = personResp.results[k].name.last,
                                    Email = personResp.results[k].email,
                                    Dob = personResp.results[k].dob,
                                    Cell = personResp.results[k].cell,
                                    Salary = personResp.results[k].Salary,
                                    Interests = personResp.results[k].Interests,
                                    Street = personResp.results[k].location.street,
                                    City = personResp.results[k].location.city,
                                    State = personResp.results[k].location.state,
                                    PostCode = personResp.results[k].location.postcode
                                };

                                row["json_data"] = JsonConvert.SerializeObject(ps);

                                tbl.Rows.Add(row);
                            }

                            Console.WriteLine("Counter : " + i + " DataTable Rows Count : " + tbl.Rows.Count);
                        }
                        else
                        {
                            Console.WriteLine("Bad response received");
                        }

                        Console.WriteLine("DataTable Rows Count Before Insert : " + tbl.Rows.Count);

                        CopyDataToSqlServer(tbl, "Person");

                        InsertJsonToElastic(tbl);

                        tbl.Clear();
                    }
                    catch (Exception ex) //Catch exception in for loop and then continue
                    {
                        Console.WriteLine("Contine after Exception : " + ex.Message);
                        tbl.Clear();
                        continue;                       
                    }
                }
            }
            catch (Exception e) //Catch unhandled exception and stops
            {
                Console.WriteLine("Exception : " + e.Message);
                Console.WriteLine("Exception StackTrace: " + e.StackTrace);
            }

            Console.WriteLine("Insert Done");

            Console.ReadLine();            
        }

        private static void InsertJsonToElastic(DataTable table)
        {
            try
            {
                var descriptor = new BulkDescriptor();

                foreach (DataRow row in table.Rows)
                {
                    PersonRecord personData = new PersonRecord
                    {
                        Id = Convert.ToString(row["ssn"]),
                        Gender = Convert.ToString(row["gender"]),
                        FirstName = Convert.ToString(row["firstname"]),
                        LastName = Convert.ToString(row["lastname"]),
                        Email = Convert.ToString(row["email"]),
                        Dob = Convert.ToString(row["dob"]),
                        Cell = Convert.ToString(row["cell"]),
                        Salary = Convert.ToDecimal(row["salary"]),
                        Interests = Convert.ToString(row["interests"]),
                        Street = Convert.ToString(row["street"]),
                        City = Convert.ToString(row["city"]),
                        State = Convert.ToString(row["state"]),
                        PostCode = Convert.ToInt32(row["zip"]),
                        PersonData = Convert.ToString(row["json_data"])
                    };


                    //ESPersonData personData = new ESPersonData
                    //{
                    //    PersonData = Convert.ToString(row["json_data"]),
                    //    Id = Convert.ToString(row["ssn"])
                    //};

                    descriptor.Index<PersonRecord>(op => op.Document(personData));
                                       
                }

                var result = EsClient().Bulk(descriptor);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void CopyDataToSqlServer(DataTable table, string tableName)
        {
            string conString = ConfigurationManager.AppSettings["conString"];
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("Insert_Persons"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@tblPersons", table);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    con.Close();
                }
                
                con.Close();
            }
        }
    }
}
