using Discount.GRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.GRPCService
{
    public class DiscountGRPCService
    {
        readonly Greeter.GreeterClient greeterClient;

        public DiscountGRPCService(Greeter.GreeterClient greeterClient)
        {
            this.greeterClient = greeterClient;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };

            return await greeterClient.GetDiscountAsync(discountRequest);
        }
    }
}
