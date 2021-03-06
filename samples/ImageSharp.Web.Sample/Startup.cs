// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace SixLabors.ImageSharp.Web.Sample
{
    /// <summary>
    /// Contains application configuration allowing the addition of services to the container.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration properties.</param>
        public Startup(IConfiguration configuration) => this.AppConfiguration = configuration;

        /// <summary>
        /// Gets the configuration properties.
        /// </summary>
        public IConfiguration AppConfiguration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The collection of service desscriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddImageSharpCore()
                .SetRequestParser<QueryCollectionRequestParser>()
                .Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheFolder = "is-cache";
                })
                .SetCache(provider =>
                {
                    return new PhysicalFileSystemCache(
                                provider.GetRequiredService<IOptions<PhysicalFileSystemCacheOptions>>(),
                                provider.GetRequiredService<IWebHostEnvironment>(),
                                provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>(),
                                provider.GetRequiredService<FormatUtilities>());
                })
                .SetCacheHash<CacheHash>()
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>();

            // Add the default service and options.
            //
            // services.AddImageSharp();

            // Or add the default service and custom options.
            //
            // this.ConfigureDefaultServicesAndCustomOptions(services);

            // Or we can fine-grain control adding the default options and configure all other services.
            //
            // this.ConfigureCustomServicesAndDefaultOptions(services);

            // Or we can fine-grain control adding custom options and configure all other services
            // There are also factory methods for each builder that will allow building from configuration files.
            //
            // this.ConfigureCustomServicesAndCustomOptions(services);
        }

        private void ConfigureDefaultServicesAndCustomOptions(IServiceCollection services)
        {
            services.AddImageSharp(
                options =>
                    {
                        options.Configuration = Configuration.Default;
                        options.MaxBrowserCacheDays = 7;
                        options.MaxCacheDays = 365;
                        options.CachedNameLength = 8;
                        options.OnParseCommands = _ => { };
                        options.OnBeforeSave = _ => { };
                        options.OnProcessed = _ => { };
                        options.OnPrepareResponse = _ => { };
                    });
        }

        private void ConfigureCustomServicesAndDefaultOptions(IServiceCollection services)
        {
            services.AddImageSharp()
                    .RemoveProcessor<FormatWebProcessor>()
                    .RemoveProcessor<BackgroundColorWebProcessor>();
        }

        private void ConfigureCustomServicesAndCustomOptions(IServiceCollection services)
        {
            services.AddImageSharpCore(
                options =>
                    {
                        options.Configuration = Configuration.Default;
                        options.MaxBrowserCacheDays = 7;
                        options.MaxCacheDays = 365;
                        options.CachedNameLength = 8;
                        options.OnParseCommands = _ => { };
                        options.OnBeforeSave = _ => { };
                        options.OnProcessed = _ => { };
                        options.OnPrepareResponse = _ => { };
                    })
                .SetRequestParser<QueryCollectionRequestParser>()
                .SetMemoryAllocator(provider => ArrayPoolMemoryAllocator.CreateWithMinimalPooling())
                .Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheFolder = "different-cache";
                })
                .SetCache(provider =>
                {
                    return new PhysicalFileSystemCache(
                        provider.GetRequiredService<IOptions<PhysicalFileSystemCacheOptions>>(),
                        provider.GetRequiredService<IWebHostEnvironment>(),
                        provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>(),
                        provider.GetRequiredService<FormatUtilities>());
                })
                .SetCacheHash<CacheHash>()
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment the application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseImageSharp();
            app.UseStaticFiles();
        }
    }
}
