using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.API.Infrastructure
{
    public static class Messages
    {        
        public const string RequiredProductName = "Product name is required.";
        public const string RequiredProductPrice = "Product price is required.";
        public const string ProductNameLength = "Product name must be between 2 to 100 characters.";
        public const string ProductDescriptionLength = "Product description must be less than 500 characters.";
    }
}
