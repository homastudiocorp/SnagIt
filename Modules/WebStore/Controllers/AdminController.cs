using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using WebStore.Models.Admin;
using WebStore.Models;
using WebStore.Services;
using Magelia.WebStore.Web;
using Magelia.WebStore.Services.Clients;
using System.ServiceModel;
using Magelia.WebStore.Services.Contract;
using System.Threading;
using Magelia.WebStore.Services.Contract.Data;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Routable.Models;
using System.Text.RegularExpressions;
using Orchard.UI.Notify;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.ContentManagement.Aspects;

namespace WebStore.Controllers
{
    public class AdminController : Controller
    {
        private IOrchardServices _orchardServices;
        private IWebStoreConfigurationService _configurationService;
        private Localizer T;
        private INotifier _notifier;
        private readonly IContentManager _contentManager;

        public AdminController(IOrchardServices orchardServices, INotifier notifier, IContentManager contentManager, IWebStoreConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this._contentManager = contentManager;
            this._orchardServices = orchardServices;
            this.T = NullLocalizer.Instance;
            this._notifier = notifier;
        }

        [HttpGet]
        public ActionResult Synchronization()
        {
            SynchronizationViewModel viewModel = new SynchronizationViewModel();
            IEnumerable<ProductPart> products = this._contentManager.Query<ProductPart, ProductRecord>().OrderBy(pr => pr.SynchronizationDate).Slice(1);
            IEnumerable<CategoryPart> categories = this._contentManager.Query<CategoryPart, CategoryRecord>().OrderBy(cr => cr.SynchronizationDate).Slice(1);
            if (products.Any() || categories.Any())
            {
                ProductPart product = products.FirstOrDefault();
                CategoryPart category = categories.FirstOrDefault();
                if (product != default(ProductPart))
                {
                    viewModel.LastSynchronizationDate = product.SynchronizationDate;
                }
                if (category != default(CategoryPart) && (category.SynchronizationDate > viewModel.LastSynchronizationDate || !viewModel.LastSynchronizationDate.HasValue))
                {
                    viewModel.LastSynchronizationDate = category.SynchronizationDate;
                }
            }
            return this.View(viewModel);
        }

        private IEnumerable<Merchant> GetMerchantSettings(String servicesPath)
        {
            WebStoreServices webstoreServices = new WebStoreServices(servicesPath);
            IEnumerable<Merchant> merchants = webstoreServices.CreateWebStoreChannel<IProfileService>().GetMerchants().OrderBy(m => m.Name);
            webstoreServices.Dispose();
            return merchants;
        }

        private IEnumerable<Catalog> GetCatalogsSettings(String servicesPath, Guid merchantId)
        {
            WebStoreServices webstoreServices = new WebStoreServices(servicesPath);
            IEnumerable<Catalog> catalogs = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetCatalogs(new GetCatalogsParameters { CultureName = "en-US", MerchantId = merchantId }).Where(c => c.IsActive).OrderBy(c => c.Name);
            webstoreServices.Dispose();
            return catalogs;
        }

        [HttpPost]
        public JsonResult GetMerchants(String servicesPath)
        {
            var responseModel = new GetMerchantsResponseModel();
            try
            {
                responseModel.MerchantSettings = this.GetMerchantSettings(servicesPath);
                responseModel.Success = true;
            }
            catch (Exception)
            {
            }
            return this.Json(responseModel);
        }

        [HttpPost]
        public JsonResult GetCatalogs(String servicesPath, Guid merchantId)
        {
            var responseModel = new GetCatalogsResponseModel();
            try
            {
                responseModel.CatalogSettings = this.GetCatalogsSettings(servicesPath, merchantId);
                responseModel.Success = true;
            }
            catch (Exception)
            {
            }
            return this.Json(responseModel);
        }

        [HttpGet]
        public ActionResult Synchronize()
        {
            WebStoreConfigurationPart configuration = this._configurationService.GetConfiguration();
            if (configuration != default(WebStoreConfigurationPart) && configuration.IsComplete)
            {
                using (WebStoreServices webStoreServices = new WebStoreServices(configuration))
                {
                    ICatalogService catalogService = webStoreServices.CreateWebStoreChannel<ICatalogService>();
                    Category root = catalogService.GetCategory(new GetCategoryParameters { CatalogId = configuration.CatalogId.Value, LoadSubCategories = true, CultureName = Thread.CurrentThread.CurrentUICulture.Name, Depth = int.MaxValue });
                    IList<Category> categories = root == default(Category) ? new List<Category>() : root.ChildCategories.Where(cat => cat.IsActive).ToList();
                    IEnumerable<CategoryPart> categoriesLocal = this._contentManager.List<CategoryPart>("WebStore Category").ToList();
                    IEnumerable<Category> categoriesToAdd = categories.Where(cat => !categoriesLocal.Select(c => c.CategoryGuid).Contains(cat.CategoryId));
                    IEnumerable<CategoryPart> categoriesToRemove = categoriesLocal.Where(cat => !categories.Select(c => c.CategoryId).Contains(cat.CategoryGuid));                    
                    

                    // Categories from service which are not in Orchard
                    foreach (Category category in categoriesToAdd)
                    {
                        String categoryName = String.IsNullOrEmpty(category.Name) ? category.Code : category.Name;
                        CategoryPart categoryPart = this._contentManager.Create<CategoryPart>("WebStore Category");
                        categoryPart.CategoryGuid = category.CategoryId;
                        categoryPart.Code = category.Code;
                        categoryPart.SynchronizationDate = DateTime.Now;

                        RoutePart categoryRoutePart = categoryPart.ContentItem.As<RoutePart>();
                        categoryRoutePart.Slug = "WebStore/Catalog/Category/" + categoryPart.Code;
                        categoryRoutePart.Title =  categoryPart.Code;
                        categoryRoutePart.Path = "WebStore/Catalog/Category/" + categoryPart.Code;


                    }

                    //Categories in orchard which are not in the service
                    foreach (CategoryPart categoryPart in categoriesToRemove)
                    {
                        this._contentManager.Remove(categoryPart.ContentItem);
                    }

                    List<Product> products = new List<Product>();
                    IEnumerable<ProductPart> productsLocal = _contentManager.List<ProductPart>("WebStore Product").ToList();

                    foreach (Category category in categories)
                    {
                        products.AddRange(catalogService.GetProducts(new GetProductsParameters { CatalogId = configuration.CatalogId.Value, CategoryId = category.CategoryId, CultureName = Thread.CurrentThread.CurrentUICulture.Name }));
                    }

                    IEnumerable<Product> productsToAdd = products.Where(p => !productsLocal.Select(pl => pl.ProductGuid).Contains(p.ProductId));
                    IEnumerable<ProductPart> productsToRemove = productsLocal.Where(pl => !products.Select(p => p.ProductId).Contains(pl.ProductGuid));

                    //product from service which are not in orchard
                    foreach (Product product in productsToAdd)
                    {
                        ProductPart productPart = this._contentManager.Create<ProductPart>("WebStore Product");
                        productPart.ProductGuid = product.ProductId;
                        productPart.SynchronizationDate = DateTime.Now;
                        productPart.SKU = product.Sku;

                        CategoryPart category = _contentManager.List<CategoryPart>("WebStore Category").Single(c => c.CategoryGuid == product.VirtualCategoriesIds.First());


                        RoutePart categoryRoutePart = productPart.ContentItem.As<RoutePart>();
                        categoryRoutePart.Slug = "Product/" + productPart.SKU;
                        categoryRoutePart.Title = productPart.SKU;
                        categoryRoutePart.Path = "WebStore/Catalog/Category/" + categories.First(c => c.CategoryId == product.VirtualCategoriesIds.First()).Code +  "/Product/" + productPart.SKU;


                        ICommonPart productContainablePart = productPart.ContentItem.As<ICommonPart>();
                        productContainablePart.Container = category;
                        
                        
                    }
                    foreach (ProductPart productPart in productsToRemove)
                    {
                        this._contentManager.Remove(productPart.ContentItem);
                    }
                    this._notifier.Information(
                        new LocalizedString(String.Format(
                                this.T("Synchronization successfully completed : {0} categorie(s) added, {1} categorie(s) removed, {2} product(s) added and {3} product(s) removed").ToString(),
                                categoriesToAdd.Count(),
                                categoriesToRemove.Count(),
                                productsToAdd.Count(),
                                productsToRemove.Count()
                            )
                        )
                    );
                }
            }
            else
            {
                this._notifier.Warning(this.T("WebStore configuration is missing or is not complete"));
            }
            return this.RedirectToAction("Synchronization", "Admin", new { area = "WebStore" });
        }
    }
}