using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApplication.Controllers {
    //public abstract class TestController : Controller {


    //    public virtual IActionResult Index() {
    //        return View();
    //    }
    //}
    //[GenericControllerName]
    [IdentityDefaultUI(typeof(TestController<>),typeof(long))]
    public class TestController<T> : Controller {
        private readonly ITestManager<T> _testManager;


        public TestController(ITestManager<T> testManager) {
            _testManager = testManager;
        }


        [HttpGet]
        public IActionResult IndexAsync() {
            var test = new Test() { Key = 5 };

            _testManager.GetId(test);
            return Content($"GET from a {typeof(T).Name} controller.");
        }
        [HttpPost]
        public IActionResult Create([FromBody] IEnumerable<T> items) {
            return Content($"POST to a {typeof(T).Name} controller.");
        }
    }


    public static class IncludedEntities {
        public static IReadOnlyList<TypeInfo> Types = new List<TypeInfo> {
            typeof(Test).GetTypeInfo() };
    }

    //[ApiEntityAttribute] public class Animals { }
    //[ApiEntityAttribute] public class Insects { }

    //public static class IncludedEntities {
    //    public static IReadOnlyList<TypeInfo> Types;
    //    static IncludedEntities() {
    //        var assembly = typeof(IncludedEntities).GetTypeInfo().Assembly;
    //        var typeList = new List<TypeInfo>();
    //        foreach (Type type in assembly.GetTypes()) {
    //            if (type.GetCustomAttributes(typeof(ApiEntityAttribute), true).Length > 0) {
    //                typeList.Add(type.GetTypeInfo());
    //            }
    //        }
    //        Types = typeList;
    //    }
    //}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerNameAttribute : Attribute, IControllerModelConvention {
        public void Apply(ControllerModel controller) {
            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(TestController<>)) {
                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class IdentityDefaultUIAttribute : Attribute, IControllerModelConvention {
        public IdentityDefaultUIAttribute(Type implementationTemplate,Type entityType) {
            Template = implementationTemplate;
            EntityType = entityType;
        }

        public Type Template { get; }
        public Type EntityType { get; }

        public void Apply(ControllerModel model) {
            ValidateTemplate(Template);
            var templateInstance = Template.MakeGenericType(EntityType);
            model.ControllerName = templateInstance.Name;
        }

        private void ValidateTemplate(Type template) {
            if (template.IsAbstract || !template.IsGenericTypeDefinition) {
                throw new InvalidOperationException("Implementation type can't be abstract or non generic.");
            }
            var genericArguments = template.GetGenericArguments();
            if (genericArguments.Length != 1) {
                throw new InvalidOperationException("Implementation type contains wrong generic arity.");
            }
        }
    }

    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature> {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature) { // Get the list of entities that we want to support for the generic controller   
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(s => s.GetTypes()).Where(w => w.GetCustomAttributes(typeof(IdentityDefaultUIAttribute), true).Length > 0).ToList();

            foreach (var entityType in types) {
                if (!feature.Controllers.Any(t => t.Name == entityType.Name)) {
                    // Create a generic controller for this type                
                    //var controllerType = typeof(TestController<>).MakeGenericType(entityType).GetTypeInfo();
                    feature.Controllers.Add(entityType.GetTypeInfo());
                }
            }

            //foreach (var entityType in IncludedEntities.Types) {
            //    var typeName = entityType.Name + "Controller";
            //    // Check to see if there is a "real" controller for this class            
            //    if (!feature.Controllers.Any(t => t.Name == typeName)) {
            //        // Create a generic controller for this type                
            //        var controllerType = typeof(TestController<>).MakeGenericType(entityType.AsType()).GetTypeInfo();
            //        feature.Controllers.Add(controllerType);
            //    }
            //}
        }
    }

    //internal class IdentityPageModelConvention<T> : IControllerModelConvention where T : class {
    //    public void Apply(ControllerModel model) {
    //        var defaultUIAttribute = model.ControllerType.GetCustomAttribute<IdentityDefaultUIAttribute>();
    //        if (defaultUIAttribute == null) {
    //            return;
    //        }

    //        ValidateTemplate(defaultUIAttribute.Template);
    //        var templateInstance = defaultUIAttribute.Template.MakeGenericType(typeof(T));
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
}