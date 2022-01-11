using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extension
{
    public static class SeedingPostgre
    {
        public static IHost MigrationData<TContext>(this IHost host,int? retry = 0)
        {
            int retryonAvailiable = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Migration started");
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("Database:connectionstring"));
                    connection.Open();
                    using var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Sam', 'DIscount', 100);";
                    command.ExecuteNonQuery();
                    connection.Close();
                    logger.LogInformation("Migration completed");
                }
                catch(NpgsqlException sqp)
                {
                    logger.LogError(sqp.InnerException.ToString());
                    if (retryonAvailiable < 50)
                    {
                        retryonAvailiable++;
                        System.Threading.Thread.Sleep(2000);
                        MigrationData<TContext>(host, retryonAvailiable);
                    }
                }
                
            }
            return host;
        }
    }
}
