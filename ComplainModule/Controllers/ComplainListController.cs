using System.Data;
using ComplainModule.Log;
using ComplainModule.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplainModule.Controllers
{
    public class ComplainListController : Controller
    {
        #region " Variables "
        int client_code1411;
        string user_ID13;
        int Company_Num14;
        string Case_Num14;
        string Client_ID4;
        string Client_ID15;
        int Seq;
        QtXLogger qtXLogger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region " Initlize the IWebHostEnvironment  "

        public ComplainListController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region " Actions "

        [HttpGet]
        public IActionResult Index()
        {

            client_code1411 = (int)HttpContext.Session.GetInt32("clientcode");
            user_ID13 = (string)HttpContext.Session.GetString("userID");
            Company_Num14 = (int)HttpContext.Session.GetInt32("CompanyNum");
            Case_Num14 = (string)HttpContext.Session.GetString("CaseNum");
            Client_ID15 = (string)HttpContext.Session.GetString("ClientID");

            DB2Operations Bind = new DB2Operations(_webHostEnvironment);
            Bind.Client_ID = Client_ID15;
            DataSet ds = Bind.BindComplaintList(Company_Num14.ToString(), Case_Num14.ToString());
            List<ComplainForm> complainLst = new List<ComplainForm>();

            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                int count = ds.Tables[0].Rows.Count;
                if (count > 0)
                {
                    DataTable dT = ds.Tables[0];
                    foreach (DataRow dr in dT.Rows)
                    {
                        ComplainForm ComplainForm = new ComplainForm();
                        ComplainForm.Company = (int)(decimal)dr[1];
                        ComplainForm.CaseNo = dr[2].ToString();
                        ComplainForm.ClientCode = (int)(decimal)dr[3];
                        ComplainForm.Status = dr[26].ToString();
                        ComplainForm.Complaintfilledby = dr[6].ToString();
                        complainLst.Add(ComplainForm);
                    }

                    ViewBag.List = complainLst;
                }
            }

            return View("Index");
        }

        [HttpPost]// Pass the query string
        public IActionResult Index(string userID, int clientcode, int CompanyNum, int CaseNum, string ClientID, string ThemeColor)
        {
            //Test Server
            HttpContext.Session.SetInt32("clientcode", clientcode);
            HttpContext.Session.SetString("userID", userID);
            HttpContext.Session.SetInt32("CompanyNum", CompanyNum);
            HttpContext.Session.SetInt32("CaseNum", CaseNum);
            HttpContext.Session.SetString("ClientID", ClientID);
            HttpContext.Session.SetString("ThemeColor", ThemeColor);
            return View();
        }

        [HttpGet]
        public JsonResult BindList(string Company, string CaseNo, string Seq)
        {
            DB2Operations db2OPS = new DB2Operations(_webHostEnvironment);
            List<ComplainForm> complainLst = new List<ComplainForm>();
            Client_ID15 = (string)HttpContext.Session.GetString("ClientID");
            db2OPS.Client_ID = Client_ID15; 
            DataSet ds = new DataSet();
            HttpContext.Session.SetInt32("Seq", Convert.ToInt32(Seq));
            ds = db2OPS.GetComplaintByCompanyDebtor(Company, CaseNo, Seq);

            if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                ComplainForm complainForm = new ComplainForm();
                complainForm.Company = (int)(decimal)row[7];
                complainForm.CaseNo = (string)row[8].ToString();
                complainForm.Status = (string)row[2];
                complainForm.Complaintfilledby = (string)row[12];
                complainLst.Add(complainForm);
            }

            return Json(complainLst);
        }

        [HttpPost]
        public JsonResult UpadateComplin(ComplainForm complainForm)
        {
            DB2Operations db2OPS = new DB2Operations(_webHostEnvironment);
            complainForm.Status = complainForm.Status; //assigning drop down status selected item to the status property defined in the Complaint class
            if (complainForm.Status != null)
            {
                var CompanyNum = (int)HttpContext.Session.GetInt32("CompanyNum");
                string company = CompanyNum.ToString();
             
                DataSet ds = db2OPS.BindStatus(company);
                DataRow[] rows = ds.Tables[0].Select("C5RESO='Y'");
                DataRow[] smrtrows1 = ds.Tables[0].Select("C5STATUS ='" + complainForm.Status + "'");
                complainForm.IsResovled = false;
                complainForm.SmartCodeForStatus = smrtrows1[0]["C5SCOD"].ToString();
                foreach (DataRow srow in rows)
                {
                    if (complainForm.Status == srow["C5STATUS"].ToString())
                    {
                        complainForm.IsResovled = true;
                        complainForm.Closed = true;

                        DataRow[] smrtrows = ds.Tables[0].Select("C5STATUS ='" + complainForm.Status + "'");
                        if (smrtrows != null && smrtrows.Length > 0 && !string.IsNullOrEmpty(smrtrows[0]["C5SCOD"].ToString()))
                        {
                            complainForm.SmartCodeForStatus = smrtrows[0]["C5SCOD"].ToString();
                            complainForm.SmartCodeForStatus = complainForm.SmartCodeForStatus;
                        }
                        else
                        {
                            complainForm.SmartCodeForStatus = String.Empty;
                        }
                    }
                }
            }

            Seq= (int)HttpContext.Session.GetInt32("Seq");
            complainForm.SeqForMultiples1 = Seq.ToString();
            db2OPS.UpdateComplaint(complainForm);
            qtXLogger = new QtXLogger(_webHostEnvironment);
            qtXLogger.Log("Update Is:" + "Updated Has been Done");
            return Json(new { success = true, message = "updatesuccess" });
        }
        #endregion
    }
}