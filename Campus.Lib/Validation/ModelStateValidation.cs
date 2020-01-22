using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Campus.Lib.Validation
{
    public static class ModelStateValidation
    {
        public static void CleanAllBut(
            this ModelStateDictionary modelState,
            params string[] includes)
        {
            modelState
                .Where(x => includes.All(i => String.Compare(i, x.Key, StringComparison.OrdinalIgnoreCase) != 0))
                .ToList()
                .ForEach(k => modelState.Remove(k));
        }
    }
}
