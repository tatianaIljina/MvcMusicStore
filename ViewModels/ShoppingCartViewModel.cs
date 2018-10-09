using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcMusicStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Models.Cart> CartItems { get; set; }  //список элементов в корзине
        public decimal CartTotal { get; set; } //итоговая цена
    }
}