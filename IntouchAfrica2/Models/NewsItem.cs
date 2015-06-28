using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace IntouchAfrica2.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public Dictionary<string, object> Properties
        {
            get;
            set;
        }

        public static NewsItem FromContent(IContent content, string author)
        {
            return new NewsItem()
            {
                Id = content.Id,
                Author = author,
                AuthorId = (int) content.Properties["Creator"].Value,
                Title = (string) content.Properties["Title"].Value,
                Content = (string) content.Properties["Content"].Value,
                Date = (DateTime) content.Properties["Date"].Value,
                Properties = content.Properties.Where(p => p.Alias != "Title" && p.Alias != "Content" && p.Alias != "Date").ToDictionary(p => p.Alias, p => p.Value)
            };
        }
    }
}