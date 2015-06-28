using IntouchAfrica2.Events;
using IntouchAfrica2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace IntouchAfrica2.Controllers
{
    public class NewsController : UmbracoApiController
    {
        public IEnumerable<NewsItem> GetAll()
        {
            var roots = Services.ContentService.GetRootContent();
            var news = roots.FirstOrDefault(c => c.Name == "News");
            if (news == null) return null;

            var newsItems = Services.ContentService.GetChildren(news.Id).Where(c=>c.ContentType.Alias == "News Item");
            return newsItems.Select(i=>NewsItem.FromContent(i, Services.MemberService.GetById((int) i.Properties["creator"].Value).Name));
        }

        public NewsItem GetNewsItem(int id)
        {
            var content = Services.ContentService.GetById(id);
            if (content == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return NewsItem.FromContent(content, Services.MemberService.GetById((int) content.Properties["creator"].Value).Name);
        }

        [MemberAuthorize(AllowType = "Teacher")]
        public NewsItem PostNewsItem(NewsItem item)
        {
            /// Additional modules (eg communications module) can add fields to the news item document type
            /// They can then listen for the NewsCreatedEvent and act on the values of those new fields which they obtain from the Properties dictionary

            var roots = Services.ContentService.GetRootContent();
            var news = roots.FirstOrDefault(c => c.Name == "News");
            if (news == null)
                throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(HttpStatusCode.NotFound) { ReasonPhrase = "No News node found in content structure" });

            var content = Services.ContentService.CreateContent(item.Title, news, "News Item");
            SetContentFromNewsItem(content, item);
            Services.ContentService.Save(content);

            EventAggregator.Instance.Publish(new NewsPublished(item));
            return item;
        }

        //TODO: Make the content creation functionality for members (structure) more generic and use the same mechanism to generate forms for News items
        //NEWS UI: The normal news page should have CRUD functionality, with the add/edit buttons only added if there is a logged-in member. 

        public NewsItem PutNewsItem(NewsItem item)
        {
            var content = Services.ContentService.GetById(item.Id);
            if (content == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            SetContentFromNewsItem(content, item);
            Services.ContentService.Save(content);
            return item;
        }

        private void SetContentFromNewsItem(IContent content, NewsItem item)
        {
            content.SetValue("title", item.Title);
            content.SetValue("content", item.Content);
            content.SetValue("creator", item.AuthorId);
            content.SetValue("date", item.Date);

            foreach (var property in item.Properties)
                content.SetValue(property.Key, property.Value);
        }

        public void DeleteNewsItem(int id)
        {
            var content = Services.ContentService.GetById(id);
            if (content == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Services.ContentService.Delete(content);
        }
    }
}