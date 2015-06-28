using IntouchAfrica2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.Events
{
    public class NewsPublished : DomainEvent
    {
        public NewsPublished(NewsItem item)
        {
            Item = item;
        }

        public NewsItem Item { get; set; }
    }
}