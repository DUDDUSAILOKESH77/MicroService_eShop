using Basket.API.Entities;
using Basket.API.Repositories.Contract;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        readonly IDistributedCache _distributedCache;

        public BasketRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task DeleteBasket(string userName)
        {
            await this._distributedCache.RemoveAsync(userName);
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var data=await this._distributedCache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(data);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await this._distributedCache
                .SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return await this.GetBasket(basket.UserName);

        }
    }
}
