using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Check if cart is empty
            if (cart.Count==0||Session["cart"]==null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }
            //Cal total save to viewbag
            decimal total = 0m;

            foreach (var item  in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;
            //Return the view with the list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Init CartVM
            CartVM model = new CartVM();

            //Init Quantity
            int qty = 0;

            //Init price

            decimal price = 0m;
            //Check for cart session

            if (Session["cart"] != null)
            {
                //Get total qty and price
                var list =(List<CartVM>) Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }



                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                //Or set qty and price to 0 
                model.Quantity = 0;
                model.Price = 0m; 

            }






            //Return partial view with the model

            //
            return PartialView(model);
        }


       public ActionResult AddToCartPartial(int id)
        {
            //init the cart vm list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //init cartvm model
            CartVM model = new CartVM();
            using (Db db = new Db())
            {
                //get the product
                ProductDTO product = db.Products.Find(id);

                //check if the product is allredy in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);
                //if not add new    
                if (productInCart==null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity=1,
                        Price=product.Price,
                        Image=product.ImageName 
                        

                    });
                        
                }
                else
                {
                    //It it IS, inc
                    productInCart.Quantity++;
                } 

            }


            //Get total qty and price and add it to the model
            int qty = 0;
            decimal price = 0m;
            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }
            model.Quantity = qty;
            model.Price = price;
            //Save the cart  back to session
            Session["cart"] = cart;
            //Return the partial view with the model


            return PartialView(model);
        }

        //GET:/Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //Init the cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                model.Quantity++;
                //Increment qty

                //Store needed data
                var result = new {qty=model.Quantity,price=model.Price };

                //Return json with data

                return Json(result,JsonRequestBehavior.AllowGet);
            }

            


        }

        //GET:/Cart/DecrementProduct

        public ActionResult DecrementProduct(int productId)
        {
            //Init the cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                //Get the model from the list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Decrement qty
                if (model.Quantity>1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                //Store needed data
                var result = new { qty = model.Quantity, price = model.Price };


                //return the json
                return Json(result, JsonRequestBehavior.AllowGet);

            }


        }

        //GET:/Cart/RemoveProduct

        public void RemoveProduct(int productId)
        {
            //Init the cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Remove model from list
                cart.Remove(model);
            }


        }
    }
}