using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace Pustok.Controllers
{
    public class OrderController : Controller
    {
		private readonly PustokDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public OrderController(PustokDbContext context,UserManager<AppUser> userManager)
        {
			_context = context;
			_userManager = userManager;
		}
        public IActionResult Checkout()
        {
            CheckoutViewModel vm = new CheckoutViewModel();
            vm.Order = new OrderCreateViewModel();
           

            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            vm.Items = _generateCheckoutItem(userId);
            vm.TotalAmount = vm.Items.Sum(x => x.Price);

            if (userId != null)
            {
                AppUser user = _userManager.FindByIdAsync(userId).Result;

                vm.Order.FullName = user.FullName;
                vm.Order.Phone = user.PhoneNumber;
                vm.Order.Email= user.Email;
            }



            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel orderVM)
        {
            string userId = (User.Identity.IsAuthenticated && User.IsInRole("Member") )? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            AppUser user = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? await _userManager.FindByIdAsync(userId) : null;

            if (!ModelState.IsValid)
            {
                CheckoutViewModel vm = new CheckoutViewModel();
                vm.Order = orderVM;
                vm.Items = _generateCheckoutItem(userId);
                vm.TotalAmount = vm.Items.Sum(x => x.Price);
                return View("Checkout",vm);
            }

            Order order = new Order
            {
                FullName = user==null?orderVM.FullName:user.FullName,
                Address =orderVM.Address,
                Email = user==null?orderVM.Email:user.Email,
                Phone = user==null?orderVM.Phone:user.PhoneNumber,
				Note = orderVM.Note,
				CreatedAt = DateTime.UtcNow.AddHours(4),
                Status = Enums.OrderStatus.Pending,
                AppUserId = userId,
                OrderItems = _generateOrderItems(userId),
            };
            order.TotalAmount = order.OrderItems.Sum(x => x.Count * (x.DiscountPercent > 0 ? (x.UnitSalePrice * (100 - x.DiscountPercent) / 100) : x.UnitSalePrice));
            _context.Orders.Add(order);
            _context.SaveChanges();

            _clearBasket(userId);

            if (userId != null)
                return RedirectToAction("profile", "account", new {tab="Orders"});

            return RedirectToAction("index", "home");
        }

        private List<OrderItem> _generateOrderItems(string userId = null)
        {
			List < OrderItem > items= new List<OrderItem>();
			if (userId != null)
			{
				var basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == userId).ToList();

				items = basketItems.Select(x =>
				new OrderItem
				{
					BookId = x.BookId,
					Count = x.Count,
					UnitCostPrice = x.Book.CostPrice,
					UnitSalePrice = x.Book.SalePrice,
					DiscountPercent = x.Book.DiscountPercent,
				}).ToList();
			}
            else
            {
                var basketStr = Request.Cookies["basket"];

                if (basketStr != null)
                {
                    var basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basketStr);

                    foreach (var bi in basketItems)
                    {
                        var book = _context.Books.Find(bi.BookId);
                        OrderItem orderItem = new OrderItem
                        {
                            BookId = bi.BookId,
                            Count = bi.Count,
                            UnitSalePrice = book.SalePrice,
                            UnitCostPrice = book.CostPrice,
                            DiscountPercent = book.DiscountPercent,
                        };
                        items.Add(orderItem);
                    }
                }
            }

            return items;
		}

        private List<CheckoutItemViewModel> _generateCheckoutItem(string userId = null)
        {
			List < CheckoutItemViewModel > items = new List<CheckoutItemViewModel>();
			if (userId != null)
			{
				var basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == userId).ToList();
				items = basketItems.Select(x => new CheckoutItemViewModel
				{
					Count = x.Count,
					BookName = x.Book.Name,
					Price = x.Count * (x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice)
				}).ToList();
			}
			else
			{
				string basketStr = Request.Cookies["basket"];

				if (basketStr != null)
				{
					List<BasketCookieItemViewModel> basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basketStr);

					foreach (var item in basketItems)
					{
						var book = _context.Books.Find(item.BookId);
						var checkoutItem = new CheckoutItemViewModel
						{
							Count = item.Count,
							BookName = book.Name,
							Price = item.Count * (book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice)
						};

						items.Add(checkoutItem);
					}
				}
			}

            return items;
		}

        private void _clearBasket(string userId = null)
        {
            if (userId == null)
                Response.Cookies.Delete("basket");
            else
            {
                _context.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == userId).ToList());
                _context.SaveChanges();
            }
        }

    }
}
