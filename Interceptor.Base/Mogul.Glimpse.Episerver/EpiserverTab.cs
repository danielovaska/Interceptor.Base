using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;

namespace Mogul.Glimpse.Episerver
{
    public class EpiserverTab : AspNetTab
    {
        public override object GetData(ITabContext context)
        {
            var result = new List<object>();
            var routeHelper = ServiceLocator.Current.GetInstance<IPageRouteHelper>();
            var currentPage = routeHelper.Page;
            if (currentPage != null)
            {
                var contentTypeRepo = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                var contentType = contentTypeRepo.Load(currentPage.ContentTypeID);
                result.Add(new
                {
                    Key = "Content type",
                    Value = contentType.Name
                });
                result.Add(new
                {
                    Key = "Properties",
                    Value = currentPage.Property.OrderBy(p => p.OwnerTab).ThenBy(p => p.FieldOrder).Select(p => new { Key = p.Name, Value = p.Value })
                });
            }
            return result;
        }

        public override string Name
        {
            get { return "Episerver"; }
        }
    }
}
