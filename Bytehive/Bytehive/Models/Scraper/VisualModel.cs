namespace Bytehive.Models.Scraper
{
    public class VisualModel
    {
        public string Url { get; set; }
        
        public string Text { get; set; }

        public string Element { get; set; }

        public string ElementName { get; set; }

        public bool ScrapeLink { get; set; }

        public bool IsUnique { get; set; }
    }
}
