using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.API.Infrastructure.Data
{
    public class ConnectionSettings
    {
        public DatabaseType DatabaseType { get; set; }
        public string DefaultConnection { get; set; }
    }
}
