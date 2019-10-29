using BaseApp;
using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using Ecommerce.Entity;
using Ecommerce.Logic;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Hubs;

namespace WebApp.Areas.Ecommerce.Controllers
{
    public class CatalogController : AppController
    {
        private IAppUser appUser;
        private IAppLog appLog;
        private IOffice ofcRepo;
        private IAppNotification notify;
        private IAppRole appRole;
        private IOffice ofc;
        private IProduct products;
        private ICategory categories;
        private ICatalog catalog;

        public CatalogController()
        {
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            ofcRepo = db.As<IOffice>();
            notify = db.As<IAppNotification>();
            appRole = db.As<IAppRole>();
            ofc = db.As<IOffice>();
            categories = db.As<ICategory>();
            catalog = db.As<ICatalog>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }
        public ActionResult AddProducts()
        {
            var model = catalog.GetProducts();
            return View(model);
        }

        [HttpPost]
        public ActionResult BulkFeatured(string ids)
        {
            CatalogProduct cp = new CatalogProduct();
            long res;
            foreach (var u in ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new CatalogProduct { ProductId = long.Parse(x) }))
            {
                var productIsInCatalog = catalog.GetByProductId(u.ProductId, "Website");
                if (productIsInCatalog == null)
                {
                    var model = products.GetById(u.ProductId);
                    cp.ProductId = u.ProductId;
                    cp.Title = model.Title;
                    cp.Price = model.SalePrice;
                    cp.ShortDescription = model.ShortDescription;
                    cp.Description = model.Description;
                    cp.Slug = model.Title.ToUrlFriendly();
                    cp.IsFeatured = true;
                    cp.Target = "Website";
                    //cp.CreatedById = CurrentUser.Id;
                    cp.CreatedOn = DateTime.UtcNow;
                    catalog.Create(cp);
                    res = cp.ProductId;
                    //var variants = products.GetVariants(model.ProductId);
                    //foreach (var pro in variants)
                    //{
                    //    cp = new CatalogProduct();
                    //    cp.ProductId = pro.ProductId;
                    //    cp.Title = pro.Title;
                    //    cp.Price = pro.SalePrice;
                    //    cp.ParentId = pro.ParentId;
                    //    cp.Slug = pro.Title.UrlFriendly();
                    //    cp.CreatedById = ((AppIdentity)User.Identity).UserId;
                    //    cp.CreatedOn = DateTime.UtcNow;
                    //    cp.Target = "Website";
                    //    catalog.Create(cp);
                    //}
                }
                else
                {
                    cp.IsFeatured = true;
                    cp.ProductId = u.ProductId;
                    catalog.UpdateIsFeatured(cp.ProductId, cp.IsFeatured);
                }
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult BulkTopSeller(string ids)
        {
            CatalogProduct cp = new CatalogProduct();
            long res;
            foreach (var u in ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new CatalogProduct { ProductId = long.Parse(x) }))
            {
                var productIsInCatalog = catalog.GetByProductId(u.ProductId, "Website");
                if (productIsInCatalog == null)
                {
                    var model = products.GetById(u.ProductId);
                    cp.ProductId = u.ProductId;
                    cp.Title = model.Title;
                    cp.Price = model.SalePrice;
                    cp.ShortDescription = model.ShortDescription;
                    cp.Description = model.Description;
                    cp.Slug = model.Title.ToUrlFriendly();
                    cp.IsBestSelling = true;
                    cp.Target = "Website";
                    //cp.CreatedById = CurrentUser.Id;
                    cp.CreatedOn = DateTime.UtcNow;
                    catalog.Create(cp);
                    res = cp.ProductId;
                    //var variants = products.GetVariants(model.ProductId);
                    //foreach (var pro in variants)
                    //{
                    //    cp = new CatalogProduct();
                    //    cp.ProductId = pro.ProductId;
                    //    cp.Title = pro.Title;
                    //    cp.Price = pro.SalePrice;
                    //    cp.ParentId = pro.ParentId;
                    //    cp.Slug = pro.Title.UrlFriendly();
                    //    cp.CreatedById = ((AppIdentity)User.Identity).UserId;
                    //    cp.CreatedOn = DateTime.UtcNow;
                    //    cp.Target = "Website";
                    //    catalog.Create(cp);
                    //}
                }
                else
                {
                    cp.IsBestSelling = true;
                    cp.ProductId = u.ProductId;
                    catalog.UpdateIsBestSelling(cp.ProductId, cp.IsBestSelling);
                }
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult BulkPublishOnWebsite(string ids)
        {
            CatalogProduct cp = new CatalogProduct();
            long res;
            foreach (var u in ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new CatalogProduct { ProductId = long.Parse(x) }))
            {
                var productIsInCatalog = catalog.GetByProductId(u.ProductId, "Website");
                if (productIsInCatalog == null)
                {
                    var model = products.GetById(u.ProductId);
                    cp.ProductId = u.ProductId;
                    cp.Title = model.Title;
                    cp.Price = model.SalePrice;
                    cp.ShortDescription = model.ShortDescription;
                    cp.Description = model.Description;
                    cp.Slug = model.Title.ToUrlFriendly();
                    cp.IsPublished = true;
                    cp.Target = "Website";
                    //cp.CreatedById = CurrentUser.Id;
                    cp.CreatedOn = DateTime.UtcNow;
                    catalog.Create(cp);
                    res = cp.ProductId;
                    //var variants = products.GetVariants(model.ProductId);
                    //foreach (var pro in variants)
                    //{
                    //    cp = new CatalogProduct();
                    //    cp.ProductId = pro.ProductId;
                    //    cp.Title = pro.Title;
                    //    cp.Price = pro.SalePrice;
                    //    cp.ParentId = pro.ParentId;
                    //    cp.Slug = pro.Title.UrlFriendly();
                    //    cp.CreatedById = ((AppIdentity)User.Identity).UserId;
                    //    cp.CreatedOn = DateTime.UtcNow;
                    //    cp.Target = "Website";
                    //    catalog.Create(cp);
                    //}
                }
                else
                {
                    cp.IsPublished = true;
                    cp.ProductId = u.ProductId;
                    catalog.UpdateIsPublished(cp.ProductId, cp.IsPublished);
                }
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult BulkPublishOnTradeOnly(string ids)
        {
            CatalogProduct cp = new CatalogProduct();
            long res;
            foreach (var u in ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new CatalogProduct { ProductId = long.Parse(x) }))
            {
                var productIsInCatalog = catalog.GetByProductId(u.ProductId, "Website");
                if (productIsInCatalog == null)
                {
                    var model = products.GetById(u.ProductId);
                    cp.ProductId = u.ProductId;
                    cp.Title = model.Title;
                    cp.Price = model.SalePrice;
                    cp.ShortDescription = model.ShortDescription;
                    cp.Description = model.Description;
                    cp.Slug = model.Title.ToUrlFriendly();
                    cp.IsPublishedTradeOnly = true;
                    cp.Target = "Website";
                    //cp.CreatedById = CurrentUser.Id;
                    cp.CreatedOn = DateTime.UtcNow;
                    catalog.Create(cp);
                    res = cp.ProductId;
                    //var variants = products.GetVariants(model.ProductId);
                    //foreach (var pro in variants)
                    //{
                    //    cp = new CatalogProduct();
                    //    cp.ProductId = pro.ProductId;
                    //    cp.Title = pro.Title;
                    //    cp.Price = pro.SalePrice;
                    //    cp.ParentId = pro.ParentId;
                    //    cp.Slug = pro.Title.UrlFriendly();
                    //    cp.CreatedById = ((AppIdentity)User.Identity).UserId;
                    //    cp.CreatedOn = DateTime.UtcNow;
                    //    cp.Target = "Website";
                    //    catalog.Create(cp);
                    //}
                }
                else
                {
                    cp.IsPublishedTradeOnly = true;
                    cp.ProductId = u.ProductId;
                    catalog.UpdateIsPublishedTradeOnly(cp.ProductId, cp.IsPublishedTradeOnly);
                }
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult BulkPublishOnFacebook(string ids)
        {
            CatalogProduct cp = new CatalogProduct();
            long res;
            foreach (var u in ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new CatalogProduct { ProductId = long.Parse(x) }))
            {
                var productIsInCatalog = catalog.GetByProductId(u.ProductId, "Website");
                if (productIsInCatalog == null)
                {
                    var model = products.GetById(u.ProductId);
                    cp.ProductId = u.ProductId;
                    cp.Title = model.Title;
                    cp.Price = model.SalePrice;
                    cp.ShortDescription = model.ShortDescription;
                    cp.Description = model.Description;
                    cp.Slug = model.Title.ToUrlFriendly();
                    cp.IsPublishedFacebook = true;
                    cp.Target = "Website";
                    //cp.CreatedById = CurrentUser.Id;
                    cp.CreatedOn = DateTime.UtcNow;
                    catalog.Create(cp);
                    res = cp.ProductId;
                    //var variants = products.GetVariants(model.ProductId);
                    //foreach (var pro in variants)
                    //{
                    //    cp = new CatalogProduct();
                    //    cp.ProductId = pro.ProductId;
                    //    cp.Title = pro.Title;
                    //    cp.Price = pro.SalePrice;
                    //    cp.ParentId = pro.ParentId;
                    //    cp.Slug = pro.Title.UrlFriendly();
                    //    cp.CreatedById = ((AppIdentity)User.Identity).UserId;
                    //    cp.CreatedOn = DateTime.UtcNow;
                    //    cp.Target = "Website";
                    //    catalog.Create(cp);
                    //}
                }
                else
                {
                    cp.IsPublishedFacebook = true;
                    cp.ProductId = u.ProductId;
                    catalog.UpdateIsPublishedFacebook(cp.ProductId, cp.IsPublishedFacebook);
                }
            }
            return Json(true);
        }



    }
}