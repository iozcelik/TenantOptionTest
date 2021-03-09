using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Reflection;

namespace WebApplication2 {
    //internal class GenericControllerConvention<TEntity> : IControllerModelConvention where TEntity : class {
    //    public void Apply(ControllerModel model) {
    //        var defaultUIAttribute = model.ControllerType.GetCustomAttribute<GenericControllerAttribute>();
    //        if (defaultUIAttribute == null) {
    //            return;
    //        }

    //        ValidateTemplate(defaultUIAttribute.Template);
    //        var templateInstance = defaultUIAttribute.Template.MakeGenericType(typeof(TEntity));
    //        model.ControllerName = templateInstance.Name;
    //    }

    //    private void ValidateTemplate(Type template) {
    //        if (template.IsAbstract || !template.IsGenericTypeDefinition) {
    //            throw new InvalidOperationException("Implementation type can't be abstract or non generic.");
    //        }
    //        var genericArguments = template.GetGenericArguments();
    //        if (genericArguments.Length != 1) {
    //            throw new InvalidOperationException("Implementation type contains wrong generic arity.");
    //        }
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    //internal sealed class GenericControllerAttribute : Attribute {
    //    public GenericControllerAttribute() {
    //    }

    //}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerAttribute : Attribute, IControllerModelConvention {
        public void Apply(ControllerModel controller) {
            if (!controller.ControllerType.IsGenericType) {
                return;
            }
            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
            controller.RouteValues["Controller"] = entityType.Name;
        }
    }
}
