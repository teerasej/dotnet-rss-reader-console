using System.Xml.Linq;

string rssUrl = "https://www.google.com/alerts/feeds/14191009605607154603/12762626758933925224";

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
    
}
else
{
    // write error to console
    Console.WriteLine("Error: " + response.StatusCode);
}

public class RssEntry
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public DateTime? PubDate { get; set; }
    public string? Description { get; set; }
}