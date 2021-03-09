using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    [GenericPage(typeof(SecondPageModel<,>))]
    public abstract class SecondPageModel : PageModel {
        public virtual Task<IActionResult> OnGetAsync() => throw new NotImplementedException();


        public virtual Task<IActionResult> OnPostAsync() => throw new NotImplementedException();
    }


    public class SecondPageModel<TUser, TKey> : SecondPageModel where TUser : class where TKey : IEquatable<TKey> {
        private readonly ITestManager<TUser> _testManager;
        public SecondPageModel(ITestManager<TUser> testManager) {
            _testManager = testManager;
        }

        public override async Task<IActionResult> OnGetAsync() {
            var user = await _testManager.TestMethod();
            return Page();
        }
    }
}
