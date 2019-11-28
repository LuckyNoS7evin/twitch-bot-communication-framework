
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Amazon.Runtime;
using Amazon;
using Amazon.DynamoDBv2;
using backend.Repositories;
using backend.Services;
using System.Collections.Generic;
using System.Text;
using System;
using System.Security.Claims;
using backend.Hubs;

namespace backend
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new BasicAWSCredentials(Configuration["AWS:Key"], Configuration["AWS:Secret"]));
            services.AddSingleton(RegionEndpoint.USEast2);
            services.AddScoped<AmazonDynamoDBClient>();
            services.AddScoped<ChannelRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<MessageRepository>();
            services.AddHttpClient<TwitchService>();

            var userService = services.BuildServiceProvider().GetService<UserRepository>();

            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://id.twitch.tv/oauth2/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
            

            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = "Twitch";
                cfg.DefaultChallengeScheme = "Twitch";
            })
            .AddJwtBearer("Twitch", cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = false,
                    LifetimeValidator = new LifetimeValidator((notBefore, expires, token, validationParams) =>
                    {
                        return true;
                    }),
                    ValidateIssuer = true,
                    ValidIssuer = "https://id.twitch.tv/oauth2",
                    ValidateAudience = true,
                    ValidAudience = Configuration["Authorization:ClientId"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    NameClaimType = "preferred_username"
                };
            })
            .AddJwtBearer("BCF", cfg => {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = "https://botcommunicationframework.com",
                    ValidateAudience = true,
                    AudienceValidator = new AudienceValidator((audiences, securityToken, validationParameters) => {
                        var valid = false;
                        foreach (var id in audiences)
                        {
                            var users = userService.GetUsersByClientIdAsync(id).GetAwaiter().GetResult();
                            if (users != null && users.Count > 0) valid = true;
                        }
                        return valid;
                    }),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = new IssuerSigningKeyResolver((token, securityToken, kid, validationParameters) => {
                        var users = userService.GetUsersByClientIdAsync(kid).GetAwaiter().GetResult();
                        var keys = new List<SecurityKey>();
                        if (users == null || users.Count == 0) return keys;

                        users.ForEach(user => {
                            var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(user.Secret));
                            keys.Add(signingKey);
                        });
                        
                        return keys;
                    }),
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                        .WithOrigins("http://localhost:8080")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddControllers();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessageHub>("/messageHub");
                endpoints.MapControllers();
            });
        }
    }
}
