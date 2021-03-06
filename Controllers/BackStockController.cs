﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gemma;
using Gemma.Repository;
using Gemma.ViewModel;

namespace Gemma.Controllers
{
    [Authorize (Users ="Admin@gmail.com")]
    public class BackStockController : Controller
    {
        private readonly StockRepository rep = new StockRepository();
        private readonly OnlineStoreRepository repo = new OnlineStoreRepository();

        // GET: Stock
        public ActionResult Index(string productName, string colorName, string size,int page = 1,string search = "false")
        {
            ViewBag.searchProductName = productName;
            ViewBag.searchColorName = colorName;
            ViewBag.searchSize = size;
            page = search == "true" ? 1 : page;
            return View(rep.GetSearchStock(productName, colorName, size , page));
        }
        // GET: Stock/Details/5
        public ActionResult Details(int? productID, int? colorID, int? sizeID)
        {
            if (productID == null || colorID == null || sizeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stock = rep.GetStockDetail(productID, colorID, sizeID);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            ViewBag.ColorID = new SelectList(rep.db.Colors, "ColorID", "ColorName");
            ViewBag.ProductID = new SelectList(rep.db.Products, "ProductID", "ProductName");
            ViewBag.SizeID = new SelectList(rep.db.Sizes, "SizeID", "SizeID");
            return View();
        }

        // POST: Stock/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ColorID,SizeID,Quantity")] StockViewModel stock, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                rep.CreateStock(stock, files);
                Session["ListProducts"] = repo.GetProducts();
                return RedirectToAction("Index");
            }

            ViewBag.ColorID = new SelectList(rep.db.Colors, "ColorID", "ColorName", stock.ColorID);
            ViewBag.ProductID = new SelectList(rep.db.Products, "ProductID", "ProductName", stock.ProductID);
            ViewBag.SizeID = new SelectList(rep.db.Sizes, "SizeID", "SizeID", stock.SizeID);
            return View(stock);
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? productID, int? colorID, int? sizeID)
        {
            if (productID == null || colorID == null || sizeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stock = rep.GetStockDetail(productID, colorID, sizeID);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stock/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ColorID,ColorName,SizeID,Quantity")] StockViewModel stock)
        {
            if (ModelState.IsValid)
            {
                rep.EditStock(stock);
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? productID, int? colorID, int? sizeID)
        {
            if (productID == null || colorID == null || sizeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stock = rep.GetStockDetail(productID, colorID, sizeID);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int productID, int colorID, int sizeID)
        {
            rep.DeleteStock(productID, colorID, sizeID);
            Session["ListProducts"] = repo.GetProducts();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                rep.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
