using Dapper;
using Discount.GRPC.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.GRPC.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        readonly IConfiguration configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("Database:connectionstring"));
            var add =await connection.ExecuteAsync(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", 
                new { ProductName=coupon.ProductName, Description=coupon.Description, Amount=coupon.Amount });
            return add == 0 ? false : true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("Database:connectionstring"));
            var add = await connection.ExecuteAsync(
                "delete from coupon where ProductName=@ProductName",
                new { ProductName = productName });
            return add == 0 ? false : true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("Database:connectionstring"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                ("Select * from Coupon where ProductName=@ProductName", new { ProductName = productName });
            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("Database:connectionstring"));
            var add = await connection.ExecuteAsync(
                "UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount,Id=coupon.Id });
            return add == 0 ? false : true;
        }
    }
}
