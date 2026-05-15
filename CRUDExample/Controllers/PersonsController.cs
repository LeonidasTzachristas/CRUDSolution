using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers;

public class PersonsController : Controller
{
    [Route("/")]
    [Route("/persons/index")]
    public IActionResult Index()
    {
        return View();
    }
}