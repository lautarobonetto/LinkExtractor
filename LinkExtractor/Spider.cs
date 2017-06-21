using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkExtractor
{
  class Spider
  {
    public Uri PageUrl { get; set; }
    public List<String> PageLinks { get; set; }
    public bool IsAlive { get; private set; }

    public Spider(Uri url)
    {
      this.PageUrl = url;
      this.IsAlive = true;
      this.PageLinks = new List<string>();
    }

    public Spider(string url) : this(new Uri(url))
    {
    }

    /// <summary>
    /// Starts the spider process to extract all the links
    /// </summary>
    public void Send()
    {
      this.FindUrls();
    }

    /// <summary>
    /// The spider opens the web page and it extracts all the links
    /// </summary>
    private void FindUrls()
    {
      try
      {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = web.Load(PageUrl.ToString());
        PageLinks = document.DocumentNode.SelectNodes("//a[@href]").Select(node => node.Attributes["href"].Value).ToList();
      }
      catch (Exception)
      {
        this.IsAlive = false;
      }
    }

  }
}
