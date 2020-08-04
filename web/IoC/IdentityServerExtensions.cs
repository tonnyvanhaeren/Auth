using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using web.Data;

namespace web.IoC
{
    public static class IdentityServerExtensions
    {
        private const string GroupManagementApiScopeName = "GroupManagement";

        public static IServiceCollection AddConfiguredIdentityServer(this IServiceCollection services,
            IHostEnvironment environment, IConfiguration configuration)
        {
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                // using in memory, but we could also get it, for example, from the database

                // access to data regarding the user's identity
                .AddInMemoryIdentityResources(GetIdentityResources())
                // APIs that may be accessed
                .AddInMemoryApiResources(GetApis())
                // client applications that may access users data and APIs on the user's behalf
                .AddInMemoryClients(GetClients())
                // configures IdentityServer integration with ASP.NET Core Identity
                .AddAspNetIdentity<PlayBallUser>()

                // more about EF integration:
                // - http://docs.identityserver.io/en/latest/quickstarts/7_entity_framework.html
                // - http://docs.identityserver.io/en/latest/reference/ef.html?highlight=dbcontext

                // is we wanted the configurations in the database, we would need this
                //.AddConfigurationStore()
                // use the database to store operational data, so it's persisted across servers in a cluster and/or restarts
                // otherwise, as an example, refresh tokens become invalid
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(configuration.GetConnectionString("PersistedGrantDbContext"),
                            npgOptions =>
                                npgOptions.MigrationsAssembly(typeof(IdentityServerExtensions).Assembly.GetName()
                                    .Name));
                })
                // to avoid bombarding the db with checks, make use of cache
                .AddInMemoryCaching();
            // we could implement our own cache if we wanted, for instance, using Redis
            //.AddResourceStoreCache<SomeCacheImplementation>()


            if (environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            return services;
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile { Required = true }
            };
        }

        private static IEnumerable<ApiResource> GetApis()
        {
            var apiResource = new ApiResource(GroupManagementApiScopeName, "Group Management");
            // apiResource.Scopes.First().Required = true;
            return new[]
            {
                apiResource
            };
        }

        private static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "WebFrontend",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    RedirectUris = new[] {"https://localhost:5003/signin-oidc"},
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "GroupManagement",
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RequireConsent = false
                }
            };
        }
    }
}
