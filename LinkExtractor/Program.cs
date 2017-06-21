using System;
using System.ComponentModel;
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
      spiderMother.Send(excludeFilter, includeFilter);

      // Catalog
      var catalog = new Catalog(websiteRoot);
      catalog.ProcessSpider(spiderMother);

      // Process all website woth 5 spiders on different threads
      while (catalog.HasPendingUrls())
      {
        var spiders = SpiderNest.CreateSpiders(catalog, 5);
        Task[] tasks = new Task[5];

        tasks[0] = Task.Factory.StartNew(() => { if (spiders.Count >= 1) spiders[0].Send(excludeFilter, includeFilter); });
        tasks[1] = Task.Factory.StartNew(() => { if (spiders.Count >= 2) spiders[1].Send(excludeFilter, includeFilter); });
        tasks[2] = Task.Factory.StartNew(() => { if (spiders.Count >= 3) spiders[2].Send(excludeFilter, includeFilter); });
        tasks[3] = Task.Factory.StartNew(() => { if (spiders.Count >= 4) spiders[3].Send(excludeFilter, includeFilter); });
        tasks[4] = Task.Factory.StartNew(() => { if (spiders.Count == 5) spiders[4].Send(excludeFilter, includeFilter); });

        Task.WaitAll(tasks);

        if (spiders.Count >= 1 && spiders[0].IsAlive) catalog.ProcessSpider(spiders[0]);
        if (spiders.Count >= 2 && spiders[1].IsAlive) catalog.ProcessSpider(spiders[1]);
        if (spiders.Count >= 3 && spiders[2].IsAlive) catalog.ProcessSpider(spiders[2]);
        if (spiders.Count >= 4 && spiders[3].IsAlive) catalog.ProcessSpider(spiders[3]);
        if (spiders.Count == 5 && spiders[4].IsAlive) catalog.ProcessSpider(spiders[4]);

        Console.Write("T:{0} P:{1} --> %{2:0.0}\n", catalog.TotalLinks(), catalog.ProcessedLinks(), catalog.ProcessStatus());
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
