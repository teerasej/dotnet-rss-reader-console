using System.Xml.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

var configuration =  new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            
var config = configuration.Build();


string rssUrl = "https://www.google.com/alerts/feeds/14191009605607154603/12762626758933925224";
string connectionString = config["CosmosDB:ConnectionString"];
string databaseId = config["CosmosDB:DatabaseId"];
string containerId = config["CosmosDB:ContainerId"];
string partitionKeyPath = config["CosmosDB:PartitionKeyPath"];

CosmosClient client = new CosmosClient(connectionString);


HttpClient httpClient = new HttpClient();
HttpResponseMessage response = await httpClient.GetAsync(rssUrl);

if (response.IsSuccessStatusCode)
{
    string rssData = await response.Content.ReadAsStringAsync();
    XDocument rssDoc = XDocument.Parse(rssData);
    XNamespace ns = "http://www.w3.org/2005/Atom";

    // Select only entry element from rssDoc with LINQ
    var entries = from item in rssDoc.Descendants(ns + "entry")
                  select new RssEntry
                  {
                      Title = (string)item.Element(ns + "title"),
                      Link = (string)item.Element(ns + "link").Attribute("href"),
                      PubDate = DateTime.Parse((string)item.Element(ns + "published")),
                      Description = (string)item.Element(ns + "content")
                  };


    Console.WriteLine("Entries: " + entries.Count());

    Database database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
    Container container = await database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);

    foreach (var entry in entries)
    {
        // set new guid for entry's id
        entry.Id = Guid.NewGuid().ToString();

        // replace <b> in entry.title with empty string
        entry.Title = entry.Title.Replace("<b>", "");
        entry.Title = entry.Title.Replace("</b>", "");

        // replace <b> in entry.description with empty string
        entry.Description = entry.Description.Replace("<b>", "");
        entry.Description = entry.Description.Replace("</b>", "");

        var createItemResponse = await container.CreateItemAsync<RssEntry>(entry, new PartitionKey(entry.Link));
        
        if(createItemResponse.StatusCode != System.Net.HttpStatusCode.Created) {
            Console.WriteLine("Error: " + createItemResponse.StatusCode);
        }
    }
}
else
{
    // write error to console
    Console.WriteLine("Error: " + response.StatusCode);
}

public class RssEntry
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string? Title { get; set; }

    [JsonProperty(PropertyName = "link")]
    public string? Link { get; set; }

    [JsonProperty(PropertyName = "pubDate")]
    public DateTime? PubDate { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string? Description { get; set; }
}