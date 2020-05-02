using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Data.Contracts
{
    public interface IIdentifier
    {
        public Guid Id { get; set; }
    }
}
