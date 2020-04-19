namespace Bytehive.Models.Scraper
{
    public class CodeModel
    {
        public string Url { get; set; }

        public int Line { get; set; }
        
        public string Text { get; set; }

        public bool ScrapeLink { get; set; }
    }
}
