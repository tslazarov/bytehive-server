using Bytehive.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bytehive.Data.Models
{
    public class File : IIdentifier
    {
        public File()
        {
        }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public DateTime LastModified { get; set; }

        public Guid ScrapeRequestId { get; set; }

        public ScrapeRequest ScrapeRequest { get; set; }
    }
}
