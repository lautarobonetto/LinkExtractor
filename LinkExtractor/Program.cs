using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkExtractor
{
  class Program
  {
    static void Main(string[] args)
    {
      // Check parameters
      if (args == null || args.Length < 4)
      {
        Console.Write("There are 4 mandatory parameters:\n");
        Console.Write("1) Root website URL. (e.g.: http://www.google.com)\n");
        Console.Write("2) Regular expresion to exclude links. (e.g.: \\?)\n");
        Console.Write("2) Regular expresion to include links. (e.g.: www\\.google\\.com)\n");
        Console.Write("3) Output file (e.g.: urls.txt)\n");
        return;
      }

      // Parameters convertion
      var websiteRoot = new Uri(args[0]);
      var excludeFilter = new Regex(args[1]);
      var includeFilter = new Regex(args[2]);
      var outputFile = args[3];

      // Mather spider
      var spiderMother = new Spider(websiteRoot);
      spiderMother.Send();

      // Catalog
      var catalog = new Catalog(websiteRoot);
      Cleaner.SpiderClean(spiderMother, excludeFilter, includeFilter);
      catalog.ProcessSpider(spiderMother);

      // Process all website woth 5 spiders on different threads
      while (catalog.HasPendingUrls())
      {
        var spiders = SpiderNest.CreateSpiders(catalog, 5);
        Task[] tasks = new Task[5];

        tasks[0] = Task.Factory.StartNew(() => { if (spiders.Count >= 1) spiders[0].Send(); });
        tasks[1] = Task.Factory.StartNew(() => { if (spiders.Count >= 2) spiders[1].Send(); });
        tasks[2] = Task.Factory.StartNew(() => { if (spiders.Count >= 3) spiders[2].Send(); });
        tasks[3] = Task.Factory.StartNew(() => { if (spiders.Count >= 4) spiders[3].Send(); });
        tasks[4] = Task.Factory.StartNew(() => { if (spiders.Count == 5) spiders[4].Send(); });

        Task.WaitAll(tasks);

        if (spiders.Count >= 1 && spiders[0].IsAlive)
        {
          Cleaner.SpiderClean(spiders[0], excludeFilter, includeFilter);
          catalog.ProcessSpider(spiders[0]);
        }
        if (spiders.Count >= 2 && spiders[1].IsAlive)
        {
          Cleaner.SpiderClean(spiders[1], excludeFilter, includeFilter);
          catalog.ProcessSpider(spiders[1]);
        }
        if (spiders.Count >= 3 && spiders[2].IsAlive)
        {
          Cleaner.SpiderClean(spiders[2], excludeFilter, includeFilter);
          catalog.ProcessSpider(spiders[2]);
        }
        if (spiders.Count >= 4 && spiders[3].IsAlive)
        {
          Cleaner.SpiderClean(spiders[3], excludeFilter, includeFilter);
          catalog.ProcessSpider(spiders[3]);
        }
        if (spiders.Count == 5 && spiders[4].IsAlive)
        {
          Cleaner.SpiderClean(spiders[4], excludeFilter, includeFilter);
          catalog.ProcessSpider(spiders[4]);
        }

        Console.Write("Tl:{0}\tPl:{1}\t--> %{2:0.0}\n", catalog.TotalLinks(), catalog.ProcessedLinks(), catalog.ProcessStatus());
      }

      // Output the results
      foreach (var url in catalog.SiteLinks.Keys)
      {
        System.IO.File.WriteAllLines(outputFile, catalog.SiteLinks.Keys.OrderBy(x => x).ToArray());
      }

      Console.Write("... Finished\n\n");
    }
  }
}
