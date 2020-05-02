using System.IO;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperFileHelper
    {
        Stream GenerateStreamFromString(string content);

        string SerializeToXml(object input);

        string SerializeToJson(object input);

        string SerializeToTxt(object input);

        string SerializeToCsv(object input);
    }
}
