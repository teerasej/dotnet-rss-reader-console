using System.Xml.Linq;

string rssUrl = "https://www.google.com/alerts/feeds/14191009605607154603/12762626758933925224";

HttpClient httpClient = new HttpClient();
HttpResponseMessage response = await httpClient.GetAsync(rssUrl);

if (response.IsSuccessStatusCode)
{
    string rssData = await response.Content.ReadAsStringAsync();
    XDocument rssDoc = XDocument.Parse(rssData);

    var entries = rssDoc.Descendants("entry").Select(item => new RssEntry
    {
        Title = (string)item.Element("title"),
        Link = (string)item.Element("link"),
        PubDate = DateTime.Parse((string)item.Element("pubDate")),
        Description = (string)item.Element("description")
    });

    // var entries = from item in rssDoc.Descendants("item")
                //   select new RssEntry
                //   {
                //       Title = (string)item.Element("title"),
                //       Link = (string)item.Element("link"),
                //       PubDate = DateTime.Parse((string)item.Element("pubDate")),
                //       Description = (string)item.Element("description")
                //   };
    Console.WriteLine("Entries: " + entries.Count());
    foreach (var entry in entries)
    {
        //write to console
        // Console.WriteLine(entry.Title);
    }
}
else
{
    // write error to console
    Console.WriteLine("Error: " + response.StatusCode);
}

public class RssEntry
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PubDate { get; set; }
        public string Description { get; set; }
    }