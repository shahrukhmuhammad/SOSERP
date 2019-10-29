using Ecommerce.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace Ecommerce.Logic
{
    public interface IShippingManagement
    {
        [Sql("EcommerceShippingCompanies_GetAll")]
        List<ShippingCompanyModel> GetAllCompanies();

        [Sql("EcommerceShippingCompanies_GetShipCompanyById")]
        ShippingCompanyModel GetShipCompanyById(long id);

        [Sql("EcommerceShippingCompanies_Create")]
        long Create(ShippingCompanyModel model);

        [Sql("EcommerceShippingCompanies_Update")]
        void Update(ShippingCompanyModel model);

        [Sql("EcommerceShippingCompanies_IsDeleteAble")]
        int IsDeleteAble(long Id);

        [Sql("EcommerceShippingCompanies_Delete")]
        void Delete(long Id);

        #region Shipping Packages
        [Sql("EcommerceShippingPackages_GetAll")]
        List<ShippingPackageModel> GetAllPackages();

        [Sql("EcommerceShippingPackages_GetAllShipPackages")]
        List<ShippingPackageModel> GetAllShipPackages();

        [Sql("EcommerceShippingPackages_GetCompanyPackages")]
        List<ShippingPackageModel> GetCompanyPackages(long Id);

        [Sql("EcommerceShippingPackages_GetById")]
        ShippingPackageModel GetShipPackageById(long id);

        [Sql("EcommerceShippingPackages_Create")]
        ShippingPackageModel CreateShipPackage(ShippingPackageModel model);

        [Sql("EcommerceShippingPackages_Update")]
        ShippingPackageModel UpdateShipPackage(ShippingPackageModel model);

        [Sql("EcommerceShippingCompanies_IsDeleteAble")]
        int IsPackageDeleteAble(long Id);

        [Sql("EcommerceShippingCompanies_Delete")]
        void DeletePackage(long Id);

        [Sql("EcommerceShippingPackages_GetPriceById")]
        ShippingPackageModel GetPriceById(int Id);

        [Sql("EcommercePostCodes_GetAll")]
        List<PostcodeModel> GetAllPostcodes();

        [Sql("EcommercePostCodes_GetById")]
        PostcodeModel GetPostcodeById(long Id);

        [Sql("EcommercePostCodes_Create")]
        int CreatePostcode(PostcodeModel model);

        [Sql("EcommercePostCodes_Update")]
        void UpdatePostcode(PostcodeModel model);

        [Sql("EcommercePostCodes_Delete")]
        void DeletePostCode(long Id);
        #endregion

    }
}
