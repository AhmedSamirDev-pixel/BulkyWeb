using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVm shoppingCartVm { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVm shoppingCartVm = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(user => user.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVm);
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVm shoppingCartVm = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(user => user.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            shoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                .Get(user => user.Id == userId);

            shoppingCartVm.OrderHeader.Name = shoppingCartVm.OrderHeader.ApplicationUser.Name;
            shoppingCartVm.OrderHeader.PhoneNumber = shoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVm.OrderHeader.StreetAddress = shoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVm.OrderHeader.City = shoppingCartVm.OrderHeader.ApplicationUser.City;
            shoppingCartVm.OrderHeader.State = shoppingCartVm.OrderHeader.ApplicationUser.State;
            shoppingCartVm.OrderHeader.PostalCode = shoppingCartVm.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View("Summary", shoppingCartVm);
        }


        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVm.ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(user => user.ApplicationUserId == userId, includeProperties: "Product");
            shoppingCartVm.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVm.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser
                .Get(user => user.Id == userId);

            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // it's a regular customer 
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVm.OrderHeader.OrderStatus = SD.StatusPending;
            }

            else
            {
                // it's a company account and we can delay the payment
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVm.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(shoppingCartVm.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVm.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();

            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // it's a regular customer account and we need to capture payment
                // strip logic would go here later

                var domain = "https://localhost:7004";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + "/Customer/Cart/OrderConfirmation?id=" + shoppingCartVm.OrderHeader.Id,
                    CancelUrl = domain + "/Customer/Cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVm.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // Convert to cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(shoppingCartVm.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);


            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVm.OrderHeader.Id });
        }


        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {
            // Get the order details including the user
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader == null)
            {
                return NotFound();
            }

            // If it's NOT a company account (so payment was through Stripe)
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                // check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // Update stripe payment info & order status
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();
            }

            // After confirming payment, clear the user's shopping cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(cart => cart.ApplicationUserId == orderHeader.ApplicationUserId)
                .ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            //  Return a view with order details
            return View(id);
        }



        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(cart => cart.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(cart => cart.Id == cartId, tracked: true);
            if (cartFromDb.Count <= 1)
            {
                // Remove item from the cart
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(cart => cart.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(cart => cart.Id == cartId, tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(cart => cart.ApplicationUserId == cartFromDb.ApplicationUserId).Count()-1);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                    return shoppingCart.Product.Price50;
                else
                    return shoppingCart.Product.Price100;
            }
        }

    }
}
