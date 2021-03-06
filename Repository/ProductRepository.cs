﻿using Gemma.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.IO;

namespace Gemma.Repository
{
    public class ProductRepository
    {
        public GemmaDBContext db = new GemmaDBContext();
        public bool IsSuccess { get; set; }
        public IPagedList<ProductViewModel> GetSearchProduct(string productName, string categoryName, int page)
        {
            var products = from p in db.Products.Include(s => s.Category)
                           select new ProductViewModel
                           {
                               ProductID = p.ProductID,
                               ProductName = p.ProductName,
                               UnitPrice = p.UnitPrice,
                               Discount = p.Discount,
                               CategoryID = p.CategoryID,
                               Explain = p.Explain,
                               CategoryName = p.Category.CategoryName,
                               PictureUrl = p.Category.CategoryName + "/" + p.ProductName + "/index1.jpg"
                           };
            products = !string.IsNullOrEmpty(productName) ? products.Where(x => x.ProductName.ToUpper().Contains(productName.ToUpper())) : products;
            products = !string.IsNullOrEmpty(categoryName) ? products.Where(x => x.CategoryName.ToUpper().Contains(categoryName.ToUpper())) : products;
            var results = products.OrderBy(x => x.ProductID).ToPagedList(page, 10);
            return results;
        }
        public ProductViewModel GetProductDetail(int? id)
        {
            var products = db.Products.Include(s => s.Category)
                        .Where(x => x.ProductID == id)
                        .ToList()[0];
            if (products == null)
            {
                return null;
            }
            var result = new ProductViewModel
            {
                ProductID = products.ProductID,
                ProductName = products.ProductName,
                UnitPrice = products.UnitPrice,
                Discount = products.Discount,
                CategoryID = products.CategoryID,
                Explain = products.Explain,
                CategoryName = products.Category.CategoryName,
                PictureUrl = products.Category.CategoryName + "/" + products.ProductName + "/index1.jpg"
            };
            return result;
        }
        public void CreateProduct(ProductViewModel product, HttpPostedFileBase file)
        {
            var result = db.Products.Find(product.ProductID);
            if (result != null)
            {
                IsSuccess = false;
                return;
            }
            IsSuccess = true;
            if (file != null)
            {
                var catagory = db.Categories.Find(product.CategoryID).CategoryName;
                var path = $"{AppDomain.CurrentDomain.BaseDirectory}Assets/images/Product/{catagory}/{product.ProductName}";
                var di = new DirectoryInfo(@path);
                if (!di.Exists)
                {
                    di.Create();
                }
                file.SaveAs($"{path}/index1.jpg");
            }
            var data = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Discount = product.Discount,
                CategoryID = product.CategoryID,
                Explain = product.Explain
            };
            db.Products.Add(data);
            db.SaveChanges();
        }
        public void EditProduct(ProductViewModel product)
        {
            var result = db.Products.Find(product.ProductID);
            result.CategoryID = product.CategoryID;
            result.ProductName = product.ProductName;
            result.UnitPrice = product.UnitPrice;
            result.Discount = product.Discount;
            result.Explain = product.Explain;
            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void DeleteProduct(int? id)
        {
            var catagory = db.Categories.Find(db.Products.Find(id).CategoryID).CategoryName;
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Assets/images/Product/{catagory}/{db.Products.Find(id).ProductName}";
            var di = new DirectoryInfo(@path);
            di.Delete(true);
            var result = db.Products.Find(id);
            db.Products.Remove(result);
            db.SaveChanges();
        }
    }
}