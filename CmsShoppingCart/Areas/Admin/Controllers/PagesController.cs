using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare a list of page models
            List<PageVM> pagesList;


            using (Db db = new Db())
            {
                //Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            //Return with the list


            return View(pagesList);
        }
        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {


                //Declare the slug
                string slug;
                //Init page dto

                PageDTO dto = new PageDTO();
                //DTO Title

                dto.Title = model.Title;
                //Check or and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists");
                    return View(model);
                }
                //DTO The rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Save the dto
                db.Pages.Add(dto);
                db.SaveChanges();

            }
            //Set TempData message
            TempData["SM"] = "You have added a new page!";
            //Redirect

            return RedirectToAction("AddPage");
        }
      
        
        //GET:Admin/Pages/EditPage/id
        [HttpGet]
       public ActionResult EditPage(int id)
        {

            //Declare the pageVM
            PageVM model;
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //COnfirm that page exists
                if (dto == null)
                {
                    return Content("Page does not exist.");
                }
                //Init pageVm
                model = new PageVM(dto);
            }



            //Return the view with the model
            return View(model);



        }

        //POST:Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {

                //Get page id
                int id = model.Id;
                //init slug
                string slug="home";
                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //Dto the title
                dto.Title = model.Title;

                //Check for slug and set it
                if (model.Slug!="home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //Make sure the title and slug are unique
                if (db.Pages.Where(x=>x.Id!=id).Any(x=>x.Title==model.Title)|| 
                    db.Pages.Where(x=>x.Id!=id).Any(x=>x.Slug==slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }
                //Dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save the dto
                db.SaveChanges();
            }


            //Set temp data message
            TempData["SM"] = "You have edited the page!";
            //Redirect
            
            return RedirectToAction("EditPage");
        }
        //GET:Admin/Pages/EditPage/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM model;
            using (Db db = new Db())
            {


                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Confirm that page exists
                if (dto==null)
                {
                    return Content("Tha page does not exists.");
                }
                //Init page VM
                model = new PageVM(dto);
            }


            // Return the view with the model 
            return View(model);

        }

        //GET:Admin/Pages/EditPage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {

                //GEt the page
                PageDTO dto = db.Pages.Find(id);
                //Remove the page
                db.Pages.Remove(dto);
                //Save 

                db.SaveChanges();
            }

            //Redirect

            return RedirectToAction("Index");
        }

        //GET:Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id )
        {
            using (Db db = new Db())
            {

                //Set initial count
                int count = 1;
                //Declare the page dto
                PageDTO dto;
                //Set sorting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }

            }

        }


        //GET:Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {

            //Declare the model
            SidebarVM model;
            using (Db db = new Db())
            {
                //Get the dto
                SidebarDTO dto = db.Sidebar.Find(1);

                //Init the model
                model = new SidebarVM(dto);

            }

            //Return view with the model
            return View(model);
        }

        //POST:Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {


                //Get the dto
                SidebarDTO dto = db.Sidebar.Find(1);
                //Dto the body
                dto.Body = model.Body;
                //Save
                db.SaveChanges();
            }
            //Set temp data message
            TempData["SM"] = "You have edited the sidebar!";




            //Redirect
            return RedirectToAction("EditSidebar");
        }
    }
}