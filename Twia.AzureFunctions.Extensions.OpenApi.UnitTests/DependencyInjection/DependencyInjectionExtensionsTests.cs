using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Twia.AzureFunctions.Extensions.OpenApi.DependencyInjection;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests.DependencyInjection
{
    [TestClass]
    public class DependencyInjectionExtensionsTests
    {
        private static readonly Assembly _assembly = typeof(DependencyInjectionExtensionsTests).Assembly;

        [TestMethod]
        public void AddSwaggerService_WithNullForServices_ThrowsException()
        {
            var assembly = typeof(DependencyInjectionExtensionsTests).Assembly;
            IServiceCollection services = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Action action = () => services.AddSwaggerService(assembly);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("services");
        }

        [TestMethod]
        public void AddSwaggerService_WithNullForFunctionAssembly_ThrowsException()
        {
            var services = new ServiceCollection();

            Action action = () => services.AddSwaggerService(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("functionAssembly");
        }

        [TestMethod]
        public void AddSwaggerService_ReturnsConstructedServices()
        {
            var serviceProvider = BuildServiceProvider();

            var swaggerService = serviceProvider.GetService(typeof(ISwaggerService));

            swaggerService.Should().NotBeNull();
        }

        [TestMethod]
        public void AddSwaggerService_StoresAssemblyInCorrectInstance()
        {
            var serviceProvider = BuildServiceProvider();

            var configurationStorage = serviceProvider.GetService(typeof(ISwaggerServiceConfigurationStorage)) as ISwaggerServiceConfigurationStorage;

            configurationStorage.Should().NotBeNull();
            configurationStorage.FunctionAssembly.Should().BeSameAs(_assembly);
        }

        [TestMethod]
        public void AddSwaggerService_UsesSetupSwaggerGen()
        {
            var isExecuted = false;

            var serviceProvider = BuildServiceProvider(options => { isExecuted = true;});

            var configurationStorage = serviceProvider.GetService(typeof(ISwaggerService));

            configurationStorage.Should().NotBeNull();
            isExecuted.Should().BeTrue();
        }

        private static IServiceProvider BuildServiceProvider(Action<SwaggerGenOptions> setupSwaggerGen = null)
        {
            if (setupSwaggerGen == null)
            {
                setupSwaggerGen = options => options.SwaggerDoc("v1", new OpenApiInfo());
            }

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddMvc();

            services.AddSwaggerService(_assembly, setupSwaggerGen);

            return services.BuildServiceProvider();
        }

    }
}