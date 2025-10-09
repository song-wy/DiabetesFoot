using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    public class MedicationController : Controller
    {
        // GET: Medication
        public ActionResult Index()
        {
            return View();
        }
    }
}