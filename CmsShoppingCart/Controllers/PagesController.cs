using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            //Get/set page slug
            if (page=="")
                page = "home";

            //Declare the model in the dto
            PageVM model;

            PageDTO dto;

            //Check if page exists
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x=>x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            //Get the page DTO
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            //Set page title
            ViewBag.PageTitle = dto.Title;
            //Check sidebar
            if (dto.HasSidebar==true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";


            }
            //init model
            model = new PageVM(dto);
            //Return view with the model
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            //Declare a lsit of pageVM
            List<PageVM> pageVMList;
            //Get all  pages except home 
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            //Return the partial view with the list

            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            //Declare the model
            SidebarVM model;
            //Init the model
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }

            //Return partial view with the model
            return PartialView(model);
        }
    }
}