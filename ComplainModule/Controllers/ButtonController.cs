using ComplainModule.Log;
using Microsoft.AspNetCore.Mvc;

namespace ComplainModule.Controllers
{
    public class ButtonController : Controller
    {
        #region " Variables "

        private readonly IWebHostEnvironment _webHostEnvironment;
        QtXLogger qtXLogger;

        #endregion

        #region " Initlize the IWebHostEnvironment  "

        public ButtonController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region " Action "

        
        [HttpPost]
        public IActionResult Index(string userID, int clientcode, int CompanyNum, string CaseNum, string ClientID,string ThemeColor)
        {
            //Local server
            HttpContext.Session.SetInt32("clientcode", clientcode);
            HttpContext.Session.SetString("userID", userID);
            HttpContext.Session.SetInt32("CompanyNum", CompanyNum);
            HttpContext.Session.SetString("CaseNum", CaseNum);
            HttpContext.Session.SetString("ClientID", ClientID);
            HttpContext.Session.SetString("ThemeColor", ThemeColor);
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #endregion
    }
}