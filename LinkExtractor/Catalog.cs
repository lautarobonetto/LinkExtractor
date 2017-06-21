using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkExtractor
{
  class Catalog
  {
    public Dictionary<String, Boolean> SiteLinks { get; set; }
    public Uri RootUri { get; set; }

    public Catalog(Uri root)
    {
      this.SiteLinks = new Dictionary<string, bool>();
      this.RootUri = root;
    }

    /// <summary>
    /// Returns the number of the total of links extracted
    /// </summary>
    /// <returns></returns>
    public int TotalLinks()
    {
      return this.SiteLinks.Count;
    }

    /// <summary>
    /// Returns the number of links already processed
    /// </summary>
    /// <returns></returns>
    public int ProcessedLinks()
    {
      return this.SiteLinks.Where(url => url.Value == true).Count();
    }

    /// <summary>
    /// Returns the percentage of progress of the link extraction
    /// </summary>
    /// <returns></returns>
    public decimal ProcessStatus()
    {
      return ((decimal)ProcessedLinks()/TotalLinks())*100;
    }

    /// <summary>
    /// Remove duplocated links
    /// </summary>
    public void RemoveDuplicateUrls()
    {
      SiteLinks = SiteLinks.GroupBy(url => url.Key)
                           .Select(group => group.First())
                           .ToDictionary(url => url.Key, url => url.Value);
    }

    /// <summary>
    /// Returns true is there is any pending URL to process yet
    /// </summary>
    /// <returns></returns>
    public bool HasPendingUrls()
    {
      return SiteLinks.Any(url => url.Value == false);
    }

    /// <summary>
    /// Processes all the spide's links. It ignores external links.
    /// </summary>
    /// <param name="spider">Spider to process</param>
    public void ProcessSpider(Spider spider)
    {
      foreach (var url in spider.PageLinks)
      {
        if (!SiteLinks.ContainsKey(url))
        {
          if (new Uri(url).Host == RootUri.Host)
          {
            SiteLinks.Add(url, false);
          }
          else
          {
            SiteLinks.Add(url, true);
          }
        }
      }
      this.RemoveDuplicateUrls();
    }

  }
}
