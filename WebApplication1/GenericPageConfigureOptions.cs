using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace WebApplication1 {
    public class GenericPageConfigureOptions<TEntity, TKey> : IPostConfigureOptions<RazorPagesOptions> where TEntity : class where TKey : IEquatable<TKey> {
        public void PostConfigure(string name, RazorPagesOptions options) {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            var convention = new GenericPageModelConvention<TEntity, TKey>();
            options.Conventions.AddFolderApplicationModelConvention("/", pam => convention.Apply(pam));
        }
    }

    public static class IdentityExtentions {
        public static IdentityBuilder AddIdentity<TEntity>(this IServiceCollection services) where TEntity : class {

            services.AddHttpContextAccessor();
            // Identity services


            return new IdentityBuilder(typeof(TEntity), services);
        }

        public static IdentityBuilder AddDefaultUI(this IdentityBuilder builder) {
            PropertyInfo p = builder.UserType.GetProperty("Id");
            Type t = p.PropertyType;

            builder.Services.ConfigureOptions(typeof(GenericPageConfigureOptions<,>).MakeGenericType(builder.UserType,t));

            return builder;
        }
    }


    public class IdentityBuilder {
        public IdentityBuilder(Type user, IServiceCollection services) {
            UserType = user;
            Services = services;
        }

        public Type UserType { get; private set; }


        public IServiceCollection Services { get; private set; }
    }

}
