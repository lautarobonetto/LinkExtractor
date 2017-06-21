using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkExtractor
{
  static class SpiderNest
  {
    /// <summary>
    /// return the next set of spiders to continue crawling the website.
    /// </summary>
    /// <param name="catalog">Catalog object where the spiders can store de links</param>
    /// <param name="NumberSpiders">Number of spider you want to create</param>
    /// <returns></returns>
    static public List<Spider> CreateSpiders(Catalog catalog, int NumberSpiders)
    {
      var result = new List<Spider>();
      var urls = catalog.SiteLinks.Where(i => i.Value == false).Take(NumberSpiders).ToList();
      foreach (var url in urls)
      {
        result.Add(new Spider(url.Key));
        catalog.SiteLinks[url.Key] = true;
      }
      return result;
    }
  }
}
