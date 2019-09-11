using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Infrastructure.Services;

namespace Product.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        ///     Gets all the products.
        /// </summary>
        /// <returns>Collection of products</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_productService.GetProducts());
        }


        /// <summary>
        ///     Gets the products by specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A product</returns>
        [HttpGet("{id:int}")]
        [ApiVersion("1")]
        public IActionResult Get(int id)
        {
            var product = _productService.GetProduct(id);
            return Ok(product);
        }


        /// <summary>
        /// Deletes a specific product.
        /// </summary>
        /// <param name="id"></param>        
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var isDeleted = _productService.RemoveProduct(id);
            if (isDeleted)
            {
                return Ok(isDeleted);
            }

            return NotFound();
        }

        /// <summary>
        /// Create new product.
        /// </summary>
        /// <param name="product">An object of <see cref="Model.Product"/> type.</param>        
        [HttpPost]
        public IActionResult Add(Model.Product product)
        {
            var isAdded = _productService.AddProduct(product);
            if (isAdded)
            {
                return Ok(isAdded);
            }

            return BadRequest();
        }

        /// <summary>
        /// Update exsting product
        /// </summary>
        /// <param name="product">An object of <see cref="Model.Product"/> type.</param>
        /// <returns>Returns <see cref="StatusCodeResult"/></returns>
        [HttpPut]
        public IActionResult Modify(Model.Product product)
        {
            var isUpdated = _productService.UpdateProduct(product);
            if (isUpdated)
            {
                return Ok(isUpdated);
            }

            return BadRequest();
        }
    }
}