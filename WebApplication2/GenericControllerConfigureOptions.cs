using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApplication2 {
    //public class GenericControllerConfigureOptions<TEntity> : IPostConfigureOptions<MvcOptions> where TEntity : class {
    //    public void PostConfigure(string name, MvcOptions options) {
    //        name = name ?? throw new ArgumentNullException(nameof(name));
    //        options = options ?? throw new ArgumentNullException(nameof(options));

    //        var convention = new GenericControllerConvention<TEntity>();
    //        options.Conventions.Add(convention);
    //    }
    //}

    public class GenericRestControllerFeatureProvider<TEntity> : IApplicationFeatureProvider<ControllerFeature> {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature) {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(w => w.BaseType == typeof(Controller) && w.IsGenericType).ToList();

            foreach (var model_type in types) {
                var entity_type = model_type.Key;
                var entity_request_types = model_type.Value[0];
                Type[] typeArgs = { entity_type, model_type.Value[0], model_type.Value[1] };
                var controller_type = model_type.MakeGenericType(typeArgs).GetTypeInfo();
                feature.Controllers.Add(controller_type);
            }
        }
    }



    public static class IdentityExtentions {
        public static IdentityBuilder AddIdentity<TEntity>(this IServiceCollection services) where TEntity : class {

            services.AddHttpContextAccessor();
            // Identity services


            return new IdentityBuilder(typeof(TEntity), services);
        }

        public static IdentityBuilder AddDefaultUI(this IdentityBuilder builder) {
            //builder.Services.ConfigureOptions(typeof(GenericControllerConfigureOptions<>)
            //        .MakeGenericType(builder.UserType));

            

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
