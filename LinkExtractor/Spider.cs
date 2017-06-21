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
    public List<String> PageLinks { get; private set; }
    public bool IsAlive { get; private set; }

    public Spider(string url)
    {
      this.PageUrl = new Uri(url);
      this.IsAlive = true;
      this.PageLinks = new List<string>();
    }

    public Spider(Uri url)
    {
      this.PageUrl = url;
      this.IsAlive = true;
      this.PageLinks = new List<string>();
    }

    /// <summary>
    /// Starts the spider process to extract all the links
    /// </summary>
    public void Send()
    {
      this.FindUrls();
      this.RemoveDuplicateUrls();
      this.FixUrls();
      this.RemoveWrongUrls();
    }

    /// <summary>
    /// Starts the spider process to extract all the links
    /// </summary>
    /// <param name="excludeFilter">Regular expresion to exclude links</param>
    /// <param name="includeFilter">Regular expresion to include links</param>
    public void Send(Regex excludeFilter, Regex includeFilter)
    {
      this.Send();
      this.FilterUrls(excludeFilter, includeFilter);
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

    /// <summary>
    /// The spider removes all the duplicated stored links
    /// </summary>
    private void RemoveDuplicateUrls()
    {
      PageLinks = PageLinks.GroupBy(url => url)
                           .Select(group => group.First())
                           .ToList();
    }

    /// <summary>
    /// Makes all absolute paths and relative paths as full URI
    /// </summary>
    private void FixUrls()
    {
      PageLinks = PageLinks.Select(url => {
        if (!url.ToLower().Contains("http"))
        {
          return new Uri(PageUrl, url).ToString();
        }
        else
        {
          return url;
        }
        
      }).ToList();
    }

    /// <summary>
    /// Remove wrong formted links
    /// </summary>
    private void RemoveWrongUrls()
    {
      foreach (var url in PageLinks.ToList())
      {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
          PageLinks.Remove(url);
        }
      }
    }

    /// <summary>
    /// The spider remove all the stored links using the match of the regular expression
    /// </summary>
    /// <param name="excludeFilter">Regular expresion to exclude links</param>
    /// <param name="includeFilter">Regular expresion to include links</param>
    private void FilterUrls(Regex excludeFilter, Regex includeFilter)
    {
      PageLinks = PageLinks.Where(url => !excludeFilter.IsMatch(url)).ToList();
      PageLinks = PageLinks.Where(url => includeFilter.IsMatch(url)).ToList();
    }

  }
}
