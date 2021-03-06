﻿using Gemma.Models;
using Gemma.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using System.IO;
using Gemma.Repository;

namespace Gemma.Controllers
{
    public class OnlineStoreController : Controller
    {
        private readonly OnlineStoreRepository repo = new OnlineStoreRepository();
        public ActionResult FindBrand(string CategoryName = "ALL", string ColorName = "ALL", string OrderBy = "None")
        {
            Session["ListProducts"] ??= repo.GetProducts();
            Session["CategoryName"] = CategoryName;
            Session["ColorName"] = ColorName;
            Session["PriceOrderBy"] = OrderBy;
            var ProductVM = repo.GetProductsSearch(CategoryName, ColorName, OrderBy, ((List<OnlineStoreProductVM>)Session["ListProducts"]).AsQueryable());
            Session["ProductModel"] = ProductVM;
            return View(ProductVM);
        }
    }
}