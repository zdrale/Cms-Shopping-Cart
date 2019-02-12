using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare list of CategoryVM
            List<CategoryVM> categoryVMList;
            //Init the list
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();

            }
            //Return partial with list 


            return PartialView(categoryVMList);
        }
        //GET:/Shop/category/Name
        public ActionResult Category(string name)
        {
            //Declare a list of ProductVM
            List<ProductsVM> productsVMList;
            using (Db db = new Db())
            {

                //Get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;


                //Init the list  
                productsVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductsVM(x)).ToList();


                //Get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }


            //Return view with the list

            return View(productsVMList);
        }

        //GET:/Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare the vm and dto
            ProductsVM model;
            ProductDTO dto;

            //Init the product id
            int id = 0;

            using (Db db = new Db())
            {
                //Check if the product exists
                if (!db.Products.Any(x=>x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index","Shop");
                }

                //Init productDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //get id
                id = dto.Id;
                //Init model
                model = new ProductsVM(dto);
            }

            //get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            //Return the view with the model
            return View("ProductDetails",model);

        }
    }
}