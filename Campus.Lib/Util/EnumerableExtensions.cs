using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Campus.Lib.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<T>(
           this IEnumerable<T> collection,
           Func<T, string> textSelector,
           Func<T, string> valueSelector)
        {
            // null checking omitted for brevity
            foreach (var item in collection)
            {
                yield return new SelectListItem()
                {
                    Text = textSelector(item),
                    Value = valueSelector(item)
                };
            }
        }
    }
}
