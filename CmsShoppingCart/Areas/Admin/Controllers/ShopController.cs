using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //Declare a list of models
            List<CategoryVM> categoryVMList;
            using (Db db = new Db())
            {

                //Init lits
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //Return the view with the list 
            return View(categoryVMList);
        }
        //POST:Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {

            //Declare the DI
            string id;

            using (Db db = new Db())
            {
                //Chech that categhory name is unique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //init the dto
                CategoryDTO dto = new CategoryDTO();
                //add to dto
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Save dto
                db.Categories.Add(dto);
                db.SaveChanges();

                //Get the id
                id = dto.Id.ToString();
            }

            //Return the id
            return id;

        }

        //POST:Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {

                //Set initial count
                int count = 1;
                //Declare  page dto
                CategoryDTO dto;
                //Set sorting for each cateogry
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }

            }

        }
        //GET:Admin/Shop/DelteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {

                //GEt the category
                CategoryDTO dto = db.Categories.Find(id);
                //Remove the Category
                db.Categories.Remove(dto);
                //Save 

                db.SaveChanges();
            }

            //Redirect

            return RedirectToAction("Categories");
        }


        //POST:Admin/Shop/RenameCategory
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {

                //Check cat name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";





                //Get the dto
                CategoryDTO dto = db.Categories.Find(id);
                //Edit dto
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save
                db.SaveChanges();
            }

            //Return

            return "ok";
        }

        //GET:Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Init the model
            ProductsVM model = new ProductsVM();

            //Add select list of categories to the model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            //Return the view with the model

            return View(model);
        }

        //POST:Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductsVM model, HttpPostedFileBase file)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            //Make sure the product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }

            }

            //Declare product ID
            int id;
            //Init and save product dto
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CateogryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CateogryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);

                db.SaveChanges();

                //Get the id

                id = product.Id;
            }

            //Set temp data message
            TempData["SM"] = "You have added a prouct!";
            #region Upload Image
            //Create necessary directions
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");


            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);


            //Check if file was uploaded
            if (file != null && file.ContentLength > 0)
            {

                //Get file extension
                string ext = file.ContentType.ToLower();

                //Verify extension
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {


                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded-wrong image extension!");
                        return View(model);


                    }
                }

                //Init image name
                string imageName = file.FileName;
                //Save image name to dto
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                //Set original and thumb  image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //Save the original image
                file.SaveAs(path);
                //Create and save thumb
                WebImage img = new WebImage(file.InputStream);

                img.Resize(200, 200);
                img.Save(path2);

            }

            #endregion


            //Redirect
            return RedirectToAction("AddProduct");
        }
        //GET:Admin/Shop/Products
        public ActionResult Products(int? page, int? catId)
        {
            //Delcare a lsit of ProductVm
            List<ProductsVM> listOfProductVM;
            //Set page  number
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                //Init the list
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductsVM(x))
                    .ToList();

                //Populate category select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Set selected category
                ViewBag.SelectedCat = catId.ToString();
            }
            //Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;
            //Return the view with the list

            return View(listOfProductVM);
        }
        //GET:Admin/Shop/EditProduct\id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //Declare  productVM
            ProductsVM model;
            using (Db db = new Db())
            {
                //Get the product
                ProductDTO dto = db.Products.Find(id);
                //Make sure it exists
                if (dto == null)
                {
                    return Content("That product does not exsist.");
                }
                //init the model
                model = new ProductsVM(dto);
                //Make a select list
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Get all gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            //Return the view with the model



            return View(model);
        }
        //POST:Admin/Shop/EditProduct\id
        [HttpPost]
        public ActionResult EditProduct(ProductsVM model, HttpPostedFileBase file)
        {
            //Get the product id
            int id = model.Id;
            //Populate cat select list and gallery images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Make sure is product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }
            //Update the product
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CateogryId;
                dto.ImageName = model.ImageName;
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CateogryId);
                dto.CategoryName = catDTO.Name;
                db.SaveChanges();
            }

            //Set temp data message
            TempData["SM"] = "You have edited the product";
            #region Image Upload
            //Check for file upload 
            if (file != null && file.ContentLength > 0)
            {

                //Get the extension
                string ext = file.ContentType.ToLower();
                //Verify the extension
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {


                        ModelState.AddModelError("", "The image was not uploaded-wrong image extension!");
                        return View(model);


                    }
                }
                //Set upload directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
                //Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();


                //Save image name
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //Save original and thumb images
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);
                WebImage img = new WebImage(file.InputStream);

                img.Resize(200, 200);
                img.Save(path2);
            }



            #endregion


            //Redirect
            return RedirectToAction("EditProduct");
        }
        //GET:Admin/Shop/DeleteProduct\id
        public ActionResult DeleteProduct(int id)
        {
            //Delete product from db
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }
            //Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            //Redirect
            return RedirectToAction("Products");
        }

        //POST:Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            //Loop through files
            foreach (string fileName in Request.Files)
            {

                //Init the file
                HttpPostedFileBase file = Request.Files[fileName];
                //Check if its null
                if (file != null && file.ContentLength > 0)
                {

                    //Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //Set image path
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    //Save original image and the thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }


            }

        }
        //POST:Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {

            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);



        }





    }
}