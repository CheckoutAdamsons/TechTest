using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Clients;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Checkout.PaymentGateway.Api.Validation;
using Checkout.PaymentGateway.Domain.Commands;
using Checkout.PaymentGateway.Domain.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.PaymentGateway.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            ConfigureSwagger(services);
            ConfigureMediator(services);
            ConfigureVersioning(services);
            ConfigureValidation(services);

            services.AddDomainServices();

            // Add dummy authentication
            services.AddAuthentication("SimulatedAuthentication").AddScheme<AuthenticationSchemeOptions, Authentication.AuthenticationHandler>("SimulatedAuthentication", null);

            // Add refit client for the acquiring bank.
            services.AddHttpClient<IAcquiringBankClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AcquiringBankUrl"]);
            }).AddTypedClient(c => RestService.For<IAcquiringBankClient>(c));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureValidation(IServiceCollection services)
        {
            services.AddScoped<IExpiryValidator, ExpiryValidator>();
        }

        private void ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private void ConfigureMediator(IServiceCollection services)
        {
            // Add command / event handlers
            services.AddMediatR(typeof(CreatePaymentCommand))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Checkout.com Payment Gateway",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Simulated", new OpenApiSecurityScheme
                {
                    Description = "Simulated authentication, provide any value and that will be used as your username",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Simulated"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Simulated"
                            },
                            Scheme = "oauth2",
                            Name = "Simulated",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }
    }
}
