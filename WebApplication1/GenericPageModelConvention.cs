using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Reflection;

namespace WebApplication1 {
    public class GenericPageModelConvention<TEntity, TKey> : IPageApplicationModelConvention where TEntity : class where TKey : IEquatable<TKey> {
        public void Apply(PageApplicationModel model) {
            var defaultUIAttribute = model.ModelType.GetCustomAttribute<GenericPageAttribute>();
            if (defaultUIAttribute == null) {
                return;
            }

            var templateInstance = defaultUIAttribute.Template.MakeGenericType(typeof(TEntity), typeof(TKey));
            model.ModelType = templateInstance.GetTypeInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class GenericPageAttribute : Attribute {
        public GenericPageAttribute(Type implementationTemplate) {
            Template = implementationTemplate;
        }

        public Type Template { get; }
    }
}
