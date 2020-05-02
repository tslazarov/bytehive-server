using Bytehive.Scraper.Contracts;
using CsvHelper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Bytehive.Scraper
{
    public class ScraperFileHelper : IScraperFileHelper
    {
        public Stream GenerateStreamFromString(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.Unicode);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public string SerializeToTxt(object input)
        {
            var entries = input as List<Dictionary<string, string>>;

            StringBuilder sb = new StringBuilder();

            if (entries.Count > 0)
            {
                sb.AppendLine(string.Join(";", entries[0].Keys));
            }

            foreach (var entry in entries)
            {
                sb.AppendLine(string.Join(";", entry.Values));
            }

            return sb.ToString();
        }

        public string SerializeToXml(object input)
        {
            var entries = input as List<Dictionary<string, string>>;

            XElement root = new XElement("entries");

            foreach (var entry in entries)
            {
                XElement el = new XElement("entry", entry.Select(kv => new XElement(kv.Key.ToLower(), kv.Value)));
                root.Add(el);
            }

            return root.ToString();
        }

        public string SerializeToJson(object input)
        {
            return JsonConvert.SerializeObject(input, Newtonsoft.Json.Formatting.Indented);
        }

        public string SerializeToCsv(object input)
        {
            var entries = input as List<Dictionary<string, string>>;

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(entries);
                }

                return Encoding.Unicode.GetString(memoryStream.ToArray());
            }
        }
    }
}
