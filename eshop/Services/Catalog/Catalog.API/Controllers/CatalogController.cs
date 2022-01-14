using Catalog.API.Entities;
using Catalog.API.GRPCService;
using Catalog.API.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;
        readonly DiscountGRPCService discountGRPCService;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger, DiscountGRPCService discountGRPCService)
        {
            _repository = repository;
            _logger = logger;
            this.discountGRPCService = discountGRPCService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Products>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Products), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Products>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            var discountPrice = await this.discountGRPCService.GetDiscount(product.Name);
            if (discountPrice.Amount > 0)
            {
                product.discountoff = product.Price - discountPrice.Amount;
            }

            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Products>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Products>>> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [Route("[action]/{name}", Name = "GetProductByName")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Products>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Products>>> GetProductByName(string name)
        {
            var items = await _repository.GetProductByName(name);
            if (items == null)
            {
                _logger.LogError($"Products with name: {name} not found.");
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Products), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Products>> CreateProduct([FromBody] Products product)
        {
            await _repository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Products), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Products product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Products), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}
