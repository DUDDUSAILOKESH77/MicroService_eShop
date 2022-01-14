using Basket.API.Entities;
using Basket.API.GRPCService;
using Basket.API.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGRPCService _discountGRPCService;

        public BasketController(IBasketRepository repository,
            DiscountGRPCService discountGRPCService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGRPCService = discountGRPCService ?? throw new ArgumentNullException(nameof(discountGRPCService));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            var updatedData = await _repository.UpdateBasket(basket);
            foreach(var data in updatedData.ShoppingCartItem)
            {
                var d2 = await this._discountGRPCService.GetDiscount(data.ProductName);
                data.Price = data.Price - d2.Amount;
            }
            return Ok(updatedData);
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }
}
