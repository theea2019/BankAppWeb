﻿using System.Web;
using System.Web.Mvc;

namespace BankAppWeb.RestApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
