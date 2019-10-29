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
    [AppAuthorize(AppPermission.All, AppPermission.ViewEcommerce, AppPermission.Ecommerce)]
    public class OrderController : AppController
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

        public OrderController()
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

        public ActionResult Sale()
        {
            //ViewBag.Contacts = new HtmlString(users.Get("select id,FirstName,LastName from appusers where role=@0 OR role=@1", AppUserRole.Customer, AppUserRole.Anonymous).Select(x => new { value = x.Id, text = x.FullName }).ToJson());
            //ViewBag.ShippingAdmins = new HtmlString(users.Get("SELECT id, FirstName, LastName FROM AppUsers WHERE Role=@0", AppUserRole.ShippingAdmin).Select(x => new { value = x.Id, text = x.FullName }).ToJson());
            ViewBag.OrderStatus = new HtmlString(Enum.GetValues(typeof(OrderStatus))
                                                    .Cast<OrderStatus>().Where(x => x >= OrderStatus.Pending).Select(e => new
                                                    {
                                                        value = e,
                                                        text = e.ToString().ToSpacedTitleCase()
                                                    }).ToJson());
            ViewBag.OrderSource = new HtmlString(Enum.GetValues(typeof(OrderSource))
                                        .Cast<OrderSource>().Select(e => new
                                        {
                                            value = e,
                                            text = e.ToString().ToSpacedTitleCase()
                                        }).ToJson());
            ViewBag.PaymentMethod = new HtmlString(Enum.GetValues(typeof(PaymentMethod))
                                                    .Cast<PaymentMethod>().Select(e => new
                                                    {
                                                        value = e,
                                                        text = e.ToString().ToSpacedTitleCase()
                                                    }).ToJson());
            ViewBag.SalesGrid = orders.GetSaleOrders((int)OrderType.Sale, (int)OrderType.SalesReturn, (int)OrderType.SalesExchange);
            ViewBag.OrderDetails = orders.GetOrderDetails();
            return View();
        }

        public ActionResult SaleOrderDetail(long id)
        {
            if (id > 0)
            {
                var model = orders.GetSaleOrderById(id);
                model.Details = orders.GetByIdOrderDetails(id);
                model.OrderAddressDetails = orders.GetByIdOrderAddressDetails(id);
                return View(model);
            }

            return RedirectToAction("Sale");
        }

        public ActionResult InventorySale(long? id)
        {
            var model = new Order();
            
            model.Tax = Convert.ToDecimal(AppSettings.GetVal("Tax"));
            model.StoreId = "Inventory";
            //ViewBag.Clients = new SelectList(users.Get("select id,FirstName,LastName from appusers where role=@0 or role=@1", AppUserRole.Customer, AppUserRole.WholeSeller), "Id", "FullName");
            //ViewBag.Shippers = new SelectList(users.Get("select id,FirstName,LastName from appusers where role=@0", AppUserRole.ShippingAdmin), "Id", "FullName");
            //ViewBag.Shippings = new SelectList(shippers.GetAllCompanies(), "ShippingCompanyId", "Title");

            ViewBag.OrderSource = new SelectList(Enum.GetValues(typeof(OrderSource))
                                                      .Cast<OrderSource>().Select(e => new
                                                      {
                                                          value = e,
                                                          text = e.ToString().ToSpacedTitleCase()
                                                      }).ToList(), "value", "text");
            ViewBag.PaymentMethod = new SelectList(Enum.GetValues(typeof(PaymentMethod))
                                                      .Cast<PaymentMethod>().Select(e => new
                                                      {
                                                          value = e,
                                                          text = e.ToString().ToSpacedTitleCase()
                                                      }).ToList(), "value", "text");
            return View(model);
        }

        [HttpPost]
        public ActionResult InventorySale(Order model, string sr)
        {
            try
            {
                var obj = new Order();
                obj = model;
                obj.Details = new List<OrderDetail>();
                if (model.OrderId == 0)
                {
                    obj.OrderType = OrderType.Sale;
                    obj.OrderSource = OrderSource.Inventory;
                    obj.OrderStatus = OrderStatus.New;
                    obj.OrderCode = Uuid.Id(16);
                    obj.StoreId = "Inventory";
                    obj.ContactId = model.ContactId;
                    obj.IsLocalPickup = LocalPickup.No;
                    obj.DateTime = DateTime.Now;
                    obj.PaymentMethod = (model.PaymentMethod == 0) ? PaymentMethod.CashOnDelivery : model.PaymentMethod;
                }
                //obj.ContactTitle = users.GetFullName(model.ContactId);

                #region OrderDetails
                foreach (var m in model.Details)
                {
                    var o = new OrderDetail();
                    var productModel = products.GetById(m.ProductId);
                    o.OrderId = obj.OrderId;
                    o.Sku = productModel.Sku;
                    o.ApplyVAT = productModel.ApplyVAT;
                    o.CategoryId = productModel.CategoryId;
                    o.ProductDetails = productModel.Description;
                    o.EAN = productModel.EAN;
                    o.EANCodeType = productModel.EANCodeType;
                    o.Height = productModel.Height;
                    o.Length = productModel.Length;
                    o.ManufacturerId = productModel.ManufacturerId;
                    o.MPN = productModel.MPN;
                    o.PackageSize = productModel.PackageSize;
                    o.ProductStatus = productModel.ProductStatus;
                    o.PurchasePrice = productModel.PurchasePrice;
                    o.SalePrice = productModel.SalePrice;
                    o.ShortDescription = productModel.ShortDescription;
                    o.TradeSalePrice = productModel.TradeSalePrice;
                    o.Type = productModel.Type;
                    o.UPC = productModel.UPC;
                    o.Weight = productModel.Weight;
                    o.Width = productModel.Width;
                    o.PriorityLevel = productModel.PriorityLevel;

                    obj.Details.Add(o);

                    #region Notification for low inventory Level
                    //Get Product Qty
                    var productDetails = products.GetBySku(o.Sku);
                    if (productDetails.Quantity <= 0)
                    {
                        //Out of Stock Notification
                    }
                    #endregion

                }
                #endregion
                var res = orders.Create(obj);
                foreach (var d in obj.Details)
                {
                    d.OrderId = res;
                    orders.Create(d);
                }

                #region Addresses
                OrderAddressDetails addressModel = new OrderAddressDetails();
                addressModel.FirstName = model.OrderBillingDetails.FirstName;
                addressModel.LastName = model.OrderBillingDetails.LastName;
                addressModel.Company = model.OrderBillingDetails.Company;
                addressModel.PostalAddress = model.OrderBillingDetails.PostalAddress;
                addressModel.PostalAddress2 = model.OrderBillingDetails.PostalAddress2;
                addressModel.City = model.OrderBillingDetails.City;
                addressModel.County = model.OrderBillingDetails.County;
                addressModel.PostalCode = model.OrderBillingDetails.PostalCode;
                addressModel.Country = model.OrderBillingDetails.Country;
                addressModel.Phone = model.OrderBillingDetails.Phone;
                addressModel.AddressType = OrderAddressType.Billing;
                addressModel.OrderId = res;
                orders.Create(addressModel);
                if (model.OrderShippingDetails != null)
                {
                    addressModel.FirstName = model.OrderShippingDetails.FirstName;
                    addressModel.LastName = model.OrderShippingDetails.LastName;
                    addressModel.Company = model.OrderShippingDetails.Company;
                    addressModel.PostalAddress = model.OrderShippingDetails.PostalAddress;
                    addressModel.PostalAddress2 = model.OrderShippingDetails.PostalAddress2;
                    addressModel.City = model.OrderShippingDetails.City;
                    addressModel.County = model.OrderShippingDetails.County;
                    addressModel.PostalCode = model.OrderShippingDetails.PostalCode;
                    addressModel.Country = model.OrderShippingDetails.Country;
                    addressModel.Phone = model.OrderShippingDetails.Phone;
                    addressModel.AddressType = OrderAddressType.Shipping;
                    addressModel.OrderId = res;
                    orders.Create(addressModel);
                }
                #endregion

                //Create Notification
               // realtime.UpdateEmployees("New Order created.");

                TempData["SuccessMsg"] = "New Order has been created successfully. An email has also been sent to the application user's email address.";
            }
            catch (Exception ex)
            {

                #region Error Log
                // appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Employee Management", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            
            return (sr == "save") ? RedirectToAction("Sale") : RedirectToAction("InventorySale");

        }
        [HttpPost]
        public JsonResult GetShippingPackages(PriorityLevel name)
        {
            var packages = shippers.GetAllShipPackages().Where(m => m.PriorityLevel == name).ToList().Select(c => new
            {
                Value = c.Id,
                Text = c.CompanyTitle
            });
            return Json(packages);
        }

        //Get Shipping PackagePrice
        [HttpPost]
        public JsonResult GetShippingPriceById(string id)
        {
            var res = shippers.GetPriceById(Convert.ToInt32(id));
            return Json(res.Price);
        }

        #region Purchase
        public ActionResult Purchase()
        {
            var statusCodes = new OrderStatus[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Completed };
            ViewBag.OrderStatus = new HtmlString(Enum.GetValues(typeof(OrderStatus))
                                        .Cast<OrderStatus>().Where(x => statusCodes.Contains(x)).Select(e => new
                                        {
                                            value = e,
                                            text = e.ToString().ToSpacedTitleCase()
                                        }).ToJson());
            //ViewBag.Contacts = new HtmlString(users.Get("select id,FirstName,LastName from appusers where role=@0", AppUserRole.Supplier).Select(x => new { value = x.Id, text = x.FullName }).ToJson());
            ViewBag.POList = orders.GetPOList((int)OrderType.Purchase);
            ViewBag.OrderDetails = orders.GetOrderDetails();
            return View();
        }

        public ActionResult InventoryPurchase(long? id, string sku)
        {
            var model = new Order();
            if (id.GetValueOrDefault() > 0)
            {
                //var obj = orders.GetwithDetails(id).FirstOrDefault();
                //obj.CopyProperties(model);

                //foreach (var o in obj.Details)
                //{
                //    var m = new OrderDetailRecord();
                //    o.CopyProperties(m);
                //    model.Details.Add(m);
                //}
            }
            else
            {
                model.DateTime = DateTime.Now;
                model.OrderType = OrderType.Purchase;
                model.OrderSource = OrderSource.Inventory;
                model.Tax = Convert.ToDecimal(AppSettings.GetVal("Tax"));
                model.StoreId = "Inventory";
            }
            //ViewBag.Suppliers = new SelectList(users.Get("select id,FirstName,LastName from appusers where role=@0", AppUserRole.Supplier), "Id", "FullName");
            //if(pid.HasValue)
            //    return View("InventoryMiniPurchase", model);
            //else
            return View(model);
        }

        #endregion
    }
}