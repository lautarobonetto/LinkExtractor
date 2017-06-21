using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkExtractor
{
  static class Cleaner
  {
    /// <summary>
    /// Generic task to clean up the link list fo the spider
    /// </summary>
    /// <param name="spider">Spider to clean up</param>
    /// <param name="excludeFilter">Regular expresion to exclude links</param>
    /// <param name="includeFilter">Regular expresion to include links</param>
    static public List<String> SpiderClean(Spider spider, Regex excludeFilter, Regex includeFilter)
    {
      spider.PageLinks = RemoveDuplicateUrls(spider.PageLinks);
      spider.PageLinks = FixUrls(spider.PageLinks, spider.PageUrl);
      spider.PageLinks = RemoveWrongUrls(spider.PageLinks);
      spider.PageLinks = FilterUrls(spider.PageLinks, excludeFilter, includeFilter);
      return spider.PageLinks;
    }

    /// <summary>
    /// The spider removes all the duplicated stored links
    /// </summary>
    /// <param name="pageLinks">Raw link list</param>
    /// <returns>Link list without duplicates</returns>
    static private List<String> RemoveDuplicateUrls(List<String> pageLinks)
    {
      return pageLinks.GroupBy(url => url)
                           .Select(group => group.First())
                           .ToList();
    }

    /// <summary>
    /// Makes all absolute paths and relative paths as full URI
    /// </summary>
    /// <param name="pageLinks">Raw link list</param>
    /// <param name="rootPagelink">Uri of the page used to start crawling</param>
    /// <returns></returns>
    static private List<String> FixUrls(List<String> pageLinks, Uri rootPagelink)
    {
      return pageLinks.Select(url => {
        if (!url.ToLower().Contains("http"))
        {
          return new Uri(rootPagelink, url).ToString();
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
    /// <param name="pageLinks">Raw link list</param>
    static private List<String> RemoveWrongUrls(List<String> pageLinks)
    {
      foreach (var url in pageLinks.ToList())
      {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
          pageLinks.Remove(url);
        }
      }
      return pageLinks;
    }

    /// <summary>
    /// The spider remove all the stored links using the match of the regular expression
    /// </summary>
    /// <param name="pageLinks">Raw link list</param>
    /// <param name="excludeFilter">Regular expresion to exclude links</param>
    /// <param name="includeFilter">Regular expresion to include links</param>
    static private List<String> FilterUrls(List<String> pageLinks, Regex excludeFilter, Regex includeFilter)
    {
      pageLinks = pageLinks.Where(url => !excludeFilter.IsMatch(url)).ToList();
      pageLinks = pageLinks.Where(url => includeFilter.IsMatch(url)).ToList();
      return pageLinks;
    }
  }
}
