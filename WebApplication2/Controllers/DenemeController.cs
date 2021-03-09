using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Controllers {
    [GenericController]
    public class DenemeController<TUser> : Controller where TUser : class {
        private readonly ITestManager<TUser> _testManager;
        public DenemeController(ITestManager<TUser> testManager) {
            _testManager = testManager;
        }
        public IActionResult Index() {
            var user =  _testManager.TestMethod();
            return View();
        }
    }
}
