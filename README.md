# LinkExtractor
Windows console app to extract all the page URLs from a website.

There are 4 mandatory parameters:
1. Root website URL. (e.g.: http://www.google.com)
2. Regular expresion to exclude links. (e.g.: \\?)
2. Regular expresion to include links. (e.g.: www\\.google\\.com)
3. Output file (e.g.: urls.txt)

##Root website URL
This is the URL you want to use as start page to crawl the links. It doesn't mean the spiders are not going to move up from this site level.
E.g.: http://www.google.com/photos

##Regular expresion to exclude links
This is a regular expression of the texts you want to spiders use to ignore links.
E.g: \?
Using this text, the spiders are going to ignore all the URL with the "?" character. This means all the links with parameters are going to be ignored.

##Regular expresion to include links
This is a more generic way to ignore links. On this case, the text is a regular expression you want the spiders user to include links. This means the links without this text are going to be ignored. This is very useful if you want to get links from one specific level of the site.
E.g: google\.com\/photos
On this example, the spiders are going to extract only links with the text "google.com/photos" within.

##Output file
Full path to the local file where you want to app write the link list. It should be a plain text file.


##Examples

LinkExtractor.exe https://msdn.microsoft.com/library \? microsoft\.com\/library urls.txt


