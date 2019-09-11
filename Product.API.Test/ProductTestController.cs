using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using api = Product.API;

namespace Tests
{
    [TestFixture]
    public class ProductTestController
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly api.Infrastructure.Services.IUserService _userService;
        private readonly api.Infrastructure.Services.IProductService _productService;        

        public ProductTestController()
        {
            // configure services
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTestDbContext()
                              .AddTestDependencies();

            _productService = _serviceCollection.BuildServiceProvider().GetRequiredService<api.Infrastructure.Services.IProductService>();
            _userService = _serviceCollection.BuildServiceProvider().GetRequiredService<api.Infrastructure.Services.IUserService>();

        }

        /// <summary>
        /// Authenticates user with userName and password and returs JWT Bearer token
        /// </summary>
        [Test]
        public void UserControllerAuthenticate()
        {
            // ARRANGE            
            var userController = new api.Controllers.UserController(_userService);
            var userObjectWithCredentials = new api.Model.User { FirstName = "Test", LastName = "User", Username = "test", Password = "test" };

            // ACT
            var actionResult = userController.Authenticate(userObjectWithCredentials);
            var okResult = actionResult as ObjectResult;

            // ASSERT
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");            
            Assert.NotNull(okResult.Value);
            Assert.IsInstanceOf(typeof(api.Model.User), okResult.Value);

            var userWithToken = okResult.Value as api.Model.User;
            Assert.NotNull(userWithToken, "user is empty");
            Assert.IsTrue(!string.IsNullOrEmpty(userWithToken.Token), "Authentication failed");
        }

        /// <summary>
        /// Test product controller GET API
        /// </summary>
        [Test]
        public void ProductControllerGet()
        {
            // ARRANGE
            var productController = new api.Controllers.ProductController(_productService);

            // ACT
            var actionResult = productController.Get();
            var okResult = actionResult as ObjectResult;

            // ASSERT   
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");
            Assert.NotNull(okResult.Value);
            Assert.IsInstanceOf(typeof(IEnumerable<api.Model.Product>), okResult.Value);

            var products = okResult.Value as IEnumerable<api.Model.Product>;
            Assert.NotNull(products);
            Assert.IsNotEmpty(products, "product list is empty.");
            Console.WriteLine($"Total products : {products.Count()}");
        }

        /// <summary>
        /// Test product controller GET(productIdentifier) API
        /// </summary>
        [TestCase(1)]
        public void ProductControllerGetWithIdentfier(int productIdentifier)
        {
            // ARRANGE
            var productController = new api.Controllers.ProductController(_productService);

            // ACT
            var actionResult = productController.Get(productIdentifier);
            var okResult = actionResult as ObjectResult;

            // ASSERT   
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");
            Assert.NotNull(okResult.Value, "Product is null");
            Assert.IsInstanceOf(typeof(api.Model.Product), okResult.Value);

            var product = okResult.Value as api.Model.Product;
            Assert.NotNull(product);
            Assert.IsTrue(product.ProductID > 0, "Product returned is empty.");
        }

        /// <summary>
        /// Test product controller POST() API
        /// </summary>
        [Test]
        public void ProductControllerPost()
        {
            // ARRANGE
            var product = new api.Model.Product { ProductID = 0, ProductName = "Samsung Galaxy 8", ProductDescription = "Mobile phones.", Price = 14200 };
            var productController = new api.Controllers.ProductController(_productService);

            // ACT
            var actionResult = productController.Add(product);
            var okResult = actionResult as ObjectResult;

            // ASSERT   
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");
            Assert.NotNull(okResult.Value);
            Assert.AreEqual(true, okResult.Value, "Product insertion failed");
        }

        /// <summary>
        /// Test product controller PUT() API
        /// </summary>
        [Test]
        public void ProductControllerPut()
        {
            // ARRANGE
            var product = new api.Model.Product { ProductID = 6, ProductName = "iPhone X", ProductDescription = "Mobile phones.", Price = 78000 };
            var productController = new api.Controllers.ProductController(_productService);

            // ACT
            var actionResult = productController.Modify(product);
            var okResult = actionResult as ObjectResult;

            // ASSERT   
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");
            Assert.NotNull(okResult.Value);
            Assert.AreEqual(true, okResult.Value, "Product modification failed");
        }

        /// <summary>
        /// Test product controller DELETE() API
        /// </summary>
        [TestCase(2)]
        public void ProductControllerDelete(int productIdentifier)
        {
            // ARRANGE
            var productController = new api.Controllers.ProductController(_productService);

            // ACT
            var actionResult = productController.Delete(productIdentifier);
            var okResult = actionResult as ObjectResult;

            // ASSERT   
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode, $"Status code is not 200 but {okResult.StatusCode?.ToString()}");
            Assert.NotNull(okResult.Value);
            Assert.AreEqual(true, okResult.Value, "Product removal failed");
        }
    }

    public static class TestServiceExtension
    {
        /// <summary>
        /// Configure entity framework, database and unit of work
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> services</param>
        /// <returns></returns>
        public static IServiceCollection AddTestDbContext(this IServiceCollection services)
        {
            services.Configure<api.Infrastructure.Data.ConnectionSettings>(Options => {
                Options.DatabaseType = api.Infrastructure.Data.DatabaseType.Postgres;
                Options.DefaultConnection = "Server=productsdbpostgres.postgres.database.azure.com;Database=postgres;Port=5432;User Id=admin1234@productsdbpostgres;Password=Admin@1234;Ssl Mode=Require;Pooling=true";
            });
            var connectionOptions = services.BuildServiceProvider().GetRequiredService<IOptions<api.Infrastructure.Data.ConnectionSettings>>();
            var databaseInterfaceType = typeof(Product.API.Infrastructure.Data.IDatabaseType);            
            var instance = databaseInterfaceType.Assembly.GetTypes().FirstOrDefault(
                x => databaseInterfaceType.IsAssignableFrom(x) && string.Equals(connectionOptions.Value.DatabaseType.ToString(), x.Name, System.StringComparison.OrdinalIgnoreCase));
            services.AddSingleton((api.Infrastructure.Data.IDatabaseType)System.Activator.CreateInstance(instance));

            var databaseTypeInstance = services.BuildServiceProvider().GetRequiredService<api.Infrastructure.Data.IDatabaseType>();
            databaseTypeInstance.EnableDatabase(services);

            services.AddDbContext<api.Infrastructure.ProductContext>(options => databaseTypeInstance.GetContextBuilder(options, connectionOptions.Value.DefaultConnection))
                    .AddTestUnitOfWork<api.Infrastructure.ProductContext>();

            return services;
        }

        /// <summary>
        /// Configure services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> services</param>
        /// <returns></returns>
        public static IServiceCollection AddTestDependencies(this IServiceCollection services)
        {
            services.Configure<api.Infrastructure.Data.AppSettings>(options => { options.Secret = "abcxyz1234567890"; });
            services.AddTransient<api.Infrastructure.Services.IProductService, api.Infrastructure.Services.ProductService>();
            services.AddTransient<api.Infrastructure.Services.IUserService, api.Infrastructure.Services.UserService>();
            return services;
        }

        /// <summary>
        /// configure unit of work
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTestUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddScoped<api.Infrastructure.Data.IUnitOfWork, api.Infrastructure.Data.UnitOfWork<TContext>>();
            services.AddScoped<api.Infrastructure.Data.IUnitOfWork<TContext>, api.Infrastructure.Data.UnitOfWork<TContext>>();
            return services;
        }

    }
}