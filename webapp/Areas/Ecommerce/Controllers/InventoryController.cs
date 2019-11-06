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
    [AppAuthorize(AppPermission.All, AppPermission.ViewEcommerce, AppPermission.Ecommerce)]
    public class InventoryController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUser;
        private IAppLog appLog;
        private IOffice ofcRepo;
        private IAppNotification notify;
        private IAppRole appRole;
        private IOffice ofc;
        private IProduct products;
        private IShippingManagement shippers;
        private IManufacturer manufacturers;
        private ICategory categories;
        private IStock stockRepo;
        private IOrder orders;

        public InventoryController()
        {
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            ofcRepo = db.As<IOffice>();
            notify = db.As<IAppNotification>();
            appRole = db.As<IAppRole>();
            ofc = db.As<IOffice>();
            products = db.As<IProduct>();
            shippers = db.As<IShippingManagement>();
            manufacturers = db.As<IManufacturer>();
            categories = db.As<ICategory>();
            orders = db.As<IOrder>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }
        public ActionResult Product()
        {
            return View();
        }

        public ActionResult ProductDetails(long? id)
        {
            //var model = new ProductRecordModel { Sku = products.GetProductSKU() };
            var model = new Product();
            model.Stock = new StockRecordModel();
            if (id.HasValue)
            {
                model = products.GetById(id.Value);
                model.BulkPrices = products.GetBulkPricesById(id.Value);
                //model.Quantity = obj.Quantity;
                //model.Attributes = vRep.GetSpecAttributes(obj.ProductId);
                //model.SalePrice = obj.SalePrice;
                //ViewBag.ProductTotalOrder = products.ProductTotalOrder(id.Value);
                //ViewBag.ProductSoldQty = products.ProductSoldQty(id.Value);
                //ViewBag.SalesGrid = orders.GetProductSaleGrid(id.Value);
            }
            return View(model);
        }
        public ActionResult ProductRecord(long? id)
        {
            var model = new Product();
            model.Stock = new StockRecordModel();
            ViewBag.EconomyList = new SelectList(shippers.GetAllShipPackages().Where(m => m.PriorityLevel == PriorityLevel.Economy).ToList(), "Id", "CompanyTitle");
            ViewBag.StandardList = new SelectList(shippers.GetAllShipPackages().Where(m => m.PriorityLevel == PriorityLevel.Standard).ToList(), "Id", "CompanyTitle");
            ViewBag.ExpressList = new SelectList(shippers.GetAllShipPackages().Where(m => m.PriorityLevel == PriorityLevel.Express).ToList(), "Id", "CompanyTitle");
            ViewBag.Types = Enum.GetValues(typeof(EANCodeType)).Cast<EANCodeType>().Select(x => new { value = x, text = x.ToString().ToSpacedTitleCase() });

            ViewBag.Manufacturer = new SelectList(manufacturers.GetAll().Select(x => new { Text = x.Title, Value = x.ManufacturerId.ToString() }).OrderBy(m => m.Text).ToList(), "Value", "Text");
            ViewBag.Categories = categories.GetAll().Select(x => new { Text = x.Title, Value = x.CategoryId.ToString() }).OrderBy(m => m.Text);
            //ViewBag.Suppliers = users.GetAllSuppliers().Select(x => new SelectListItem { Text = x.FullName, Value = x.Id.ToString() });

            string[] packageSizes = AppSettings.GetVal("PackageSize").Split(',');
            ViewBag.PackageSizes = packageSizes.Select(x => new SelectListItem { Text = x, Value = x });

            if (id.HasValue)
            {
                var obj = products.GetById(id.Value);
                model = obj;
                model.BulkPrices = products.GetBulkPricesById(id.Value);
                //model.Quantity = obj.Quantity;
                //model.Attributes = vRep.GetSpecAttributes(obj.ProductId);
                //model.SalePrice = obj.SalePrice;
                //model.Stock.LotCode = obj.LotCode;
                //var variantRepo = new VariantsRepository();
                //model.ProductVariants = variantRepo.ReturnVariantRecordSets(id.Value);
            }
            else
            {
                //var repo = new StockRepository();
                //model.Quantity = 0;
                //model.SalePrice = 0;
                //model.Stock.LotCode = repo.GetNextLotCode();

                //ViewBag.Types = Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Select(x => new { value = x, text = x.ToString().SplitCamelCase() });
                //model.Attributes = vRep.GetSpecAttributes(1);
                //model.ProductVariants = null;
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ProductRecord(Product model, string sr, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var obj = model.ProductId > 0 ? products.GetById(model.ProductId) : new Product();
                var stockRepo = db.As<IStock>();
                long res;
                if (model.ProductId == 0)
                {
                    #region Add Product
                    //model.Sku = products.GetProductSKU();
                    model.CreatedOn = DateTime.UtcNow;
                    //obj.CreatedById = Convert.ToInt32(CurrentUser.Identities.Name);
                    //Stock stockObj = model.Stock.StockId == 0 ? new Stock() { DateTime = DateTime.Now } : stockRepo.GetById(model.Stock.StockId);
                    model.ProductStatus = ProductStatus.Active;
                    res = products.Create(model);
                    //products.UpdateAttributes(obj.ProductId, obj.ParentId ?? obj.ProductId, model.Attributes);

                    #region For Bulk Buyers
                    if (model.BulkPrices.Count > 0 )
                    {
                        foreach (var x in model.BulkPrices)
                        {
                            var bulkprice = new BulkPrices();
                            bulkprice.ProductId = model.ProductId;
                            bulkprice.Quantity = x.Quantity;
                            bulkprice.Price = x.Price;
                            products.CreateBulkPrices(bulkprice);
                        }
                    }
                    #endregion

                    //products.SetSuppliers(res, model.SelectedSuppliers);

                    #region Purchase Entry
                    //Purchase Entry

                    Order purchaseOrderModel = new Order();

                    var purchaseObj = (purchaseOrderModel.OrderId == 0) ? new Order() : orders.GetById(purchaseOrderModel.OrderId);
                    //purchaseObj.ContactTitle = users.GetFullName(Convert.ToInt32(model.SelectedSuppliers));
                    purchaseObj.Details = new List<OrderDetail>();
                    purchaseObj.OrderType = OrderType.Purchase;
                    purchaseObj.OrderSource = OrderSource.Inventory;
                    purchaseObj.OrderStatus = OrderStatus.New;
                    purchaseObj.Total = model.Quantity * model.PurchasePrice;
                    purchaseObj.DateTime = DateTime.UtcNow;
                    purchaseObj.Rating = 1;
                    purchaseObj.ContactId = model.SelectedSuppliers;
                    //purchaseObj.DateTime = DateTime.UtcNow;
                    if (purchaseOrderModel.OrderId == 0)
                    {
                        purchaseObj.OrderCode = Uuid.Id(16);
                    }

                    //var o = new OrderDetail();
                    //o.OrderId = purchaseObj.OrderId;
                    //o.ProductId = res;
                    //o.Quantity = model.Quantity;
                    //o.Price = model.PurchasePrice;
                    //o.Sku = model.Sku;
                    //o.Description = model.Title;

                    //purchaseObj.Details.Add(o);
                    //long oId = 0;
                    //var purchaseRes = (purchaseOrderModel.OrderId == 0) ? oId = orders.Create(purchaseObj) : orders.Update(purchaseObj);

                    //purchaseOrderModel.OrderStatus = OrderStatus.Processing;
                    //purchaseOrderModel.OrderId = oId;
                    //orders.PurchaseOrderApprove(purchaseOrderModel);
                    //var accRepo = new AccoutingRepository();
                    //accRepo.JournalEntery(purchaseOrderModel);
                    ////Emails.Instance.PurchaseAwaitingShipment(purchaseOrderModel.OrderId);

                    //purchaseOrderModel.OrderStatus = OrderStatus.Completed;
                    //orders.PurchaseOrderApprove(purchaseOrderModel);
                    //orders.PurchaseOrderRating(purchaseOrderModel);
                    ////Emails.Instance.PurchaseOrderRecieve(purchaseOrderModel.OrderId);
                    #endregion

                    #region Stock Entry
                    /*var stockrepo = new StockRepository();
                    var prorepo = new ProductRepository();

                    // Stock Entry

                    if (model.Stock.StockId == 0)
                    {
                        stockObj.DateTime = DateTime.Now;
                        stockObj.ProductId = res;
                        stockObj.StockType = StockType.StockIn;
                        stockObj.Quantity = model.Quantity;
                        stockObj.PurchasePrice = model.PurchasePrice;
                        stockObj.SalePrice = model.SalePrice;
                        stockObj.LotCode = model.Stock.LotCode;
                        stockObj.SupplierId = Convert.ToInt64(model.SelectedSuppliers);
                        stockObj.OrderId = purchaseOrderModel.OrderId;
                        stockObj.Description = "Stock Received from PO#" + purchaseOrderModel.OrderId;
                        repo.Create(stockObj);
                        var accrepo = new AccoutingRepository();
                        accrepo.JournalEntery(stockObj);
                        var productsRepo = new ProductRepository();
                        var p = productsRepo.GetById(stockObj.ProductId);
                        //Emails.Instance.StockAdded(p.Title, stockObj.Quantity, stockObj.LotCode);
                    }
                    else
                    {
                        var stockId = repo.GetAll().FirstOrDefault(m => m.ProductId == res).StockId;
                        stockObj.ProductId = res;
                        stockObj.StockType = StockType.StockIn;
                        stockObj.Quantity = model.Quantity;
                        stockObj.PurchasePrice = model.PurchasePrice;
                        stockObj.SalePrice = model.SalePrice;
                        stockObj.LotCode = model.Stock.LotCode;
                        stockObj.SupplierId = Convert.ToInt64(model.SelectedSuppliers);
                        stockObj.StockId = stockId;
                        repo.Update(stockObj);
                    }
                    products.AddInventory(stockObj); */


                    //End Stock Entry
                    #endregion
                    #endregion

                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Employee Management", model.Code + " " + model.FullName + " Employee Created", "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> employee created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    
                    #endregion

                    realtime.UpdateEmployees("New Product created.");

                    TempData["SuccessMsg"] = "New employee has been created successfully. An email has also been sent to the application user's email address.";
                }
                else
                {
                    #region Update Product
                    res = products.Update(obj);
                    //products.UpdateAttributes(obj.ProductId, obj.ParentId ?? obj.ProductId, model.Attributes);
                    //products.SetSuppliers(res, model.SelectedSuppliers);
                    #region For Bulk Buyers
                    if (model.BulkPrices.Count > 0)
                    {
                        //Delete Existing Entries
                        products.DeletePrices(model.ProductId);
                        foreach (var x in model.BulkPrices)
                        {
                            var bulkprice = new BulkPrices();
                            bulkprice.ProductId = model.ProductId;
                            bulkprice.Quantity = x.Quantity;
                            bulkprice.Price = x.Price;
                            products.CreateBulkPrices(bulkprice);
                        }
                    }
                    #endregion

                    #region Stock
                    //Stock stockObj = model.Stock.StockId == 0 ? new Stock() { DateTime = DateTime.Now } : repo.GetById(model.Stock.StockId);
                    //var stockrepo = new StockRepository();

                    //var stockId = repo.GetAll().FirstOrDefault(m => m.ProductId == res).StockId;
                    //stockObj.ProductId = res;
                    //stockObj.StockType = StockType.StockIn;
                    //stockObj.Quantity = model.Quantity;
                    //stockObj.PurchasePrice = model.PurchasePrice;
                    //stockObj.SalePrice = model.SalePrice;
                    //stockObj.LotCode = model.Stock.LotCode;
                    //stockObj.SupplierId = Convert.ToInt64(model.SelectedSuppliers);
                    //stockObj.StockId = stockId;
                    //repo.Update(stockObj);

                    //var variantRepo = new VariantsRepository();
                    //if (product != null)
                    //{
                    //    foreach (var item in product)
                    //    {
                    //        variantRepo.UpdateProductVariantById(item.ProVariantId, item.VariantPrice);
                    //    }
                    //}
                    #endregion

                    #endregion

                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Employee Management", model.Code + " " + model.FullName + " Employee Updated", "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> employee updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateEmployees("Product has been updated.");

                    TempData["SuccessMsg"] = "Product has been updated successfully.";
                }

                var imgPath = Server.MapPath("~/Content/Uploads/Ecommerce/");
                if (model.Image.ContentLength > 0)
                {
                    model.Image.SaveAs(string.Format(@"{0}p_{1}.jpg", imgPath, res));
                }
                #region Upload MultiImages
                var ids = new List<string>();
                foreach (var f in files)
                {
                    if (f != null && f.ContentLength > 0)
                    {
                        if (f.ContentLength > 5242880) throw new System.NullReferenceException();

                        var guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 4);
                        var file = string.Format(@"{0}-{1}.jpg", model.ProductId, guid);
                        f.SaveAs(string.Format("{0}{1}", Server.MapPath("~/Content/Uploads/Ecommerce/"), file));
                        ids.Add(file);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                #region Error Log
               // appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Employee Management", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return (sr == "savereturn") ? RedirectToAction("Product") : RedirectToAction("ProductRecord", new { id = String.Empty }) ;
        }
        #region Json Requests
        public JsonResult GetAllProducts()
        {
            var productsList = products.GetAllProducts();
            return Json(productsList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductBySku(string id)
        {
            var res = products.GetBySku(id);
            return Json(res);
        }
        #endregion
    }
}