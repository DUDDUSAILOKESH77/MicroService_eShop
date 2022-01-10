using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            this.ShoppingCartItem = new List<ShoppingCartItem>();
        }

        public ShoppingCart(string userName)
        {
            this.UserName = userName;
        }
        public string UserName { get; set; }

        public List<ShoppingCartItem> ShoppingCartItem { get; set; }

        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;
                foreach (var item in ShoppingCartItem)
                {
                    totalprice += item.Price * item.Quantity;
                }
                return totalprice;
            }
        }
    }
}
