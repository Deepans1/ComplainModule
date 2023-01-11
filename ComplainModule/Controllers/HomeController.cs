using System.Data;
using Newtonsoft.Json;
using ComplainModule.Log;
using ComplainModule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplainModule.Controllers
{
    public class HomeController : Controller
    {
        #region " Variables "

        string Seqid = String.Empty;
        string company = String.Empty;
        bool updgo = false;
        int client_codeqq1;
        string user_ID11;
        int CompanyNum111;
        string CaseNum111;
        string ClientID11;
        QtXLogger qtXLogger;
        ComplainForm complainFor = new ComplainForm();
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region " Initlize the Folder Path  "

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region " Actions "

        [HttpGet]
        public IActionResult Index()
        {

            qtXLogger = new QtXLogger(_webHostEnvironment);
            qtXLogger.Log("Complain Form Load");

            client_codeqq1 = (int)HttpContext.Session.GetInt32("clientcode");
            user_ID11 = (string)HttpContext.Session.GetString("userID");
            CompanyNum111 = (int)HttpContext.Session.GetInt32("CompanyNum");
            CaseNum111 = (string)HttpContext.Session.GetString("CaseNum");
            ClientID11 = (string)HttpContext.Session.GetString("ClientID");

            DB2Operations db = new DB2Operations(_webHostEnvironment);

            db.Client_ID = (string)HttpContext.Session.GetString("ClientID");
            if (db.Client_ID == null)
                db.Client_ID = (string)HttpContext.Session.GetString("ClientID");
            bool Status = false;
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["f"]))
            {
                string QueryName = HttpContext.Request.Query["f"];
                Status = true;
            }
            else if (!String.IsNullOrEmpty(HttpContext.Request.Query["isupd"]))
            {
                if (!String.IsNullOrEmpty(HttpContext.Request.Query["Seqid"]))
                {
                    Seqid = HttpContext.Request.Query["Seqid"];
                    HttpContext.Session.SetString("Seqid", Seqid);
                    complainFor.SeqForMultiples1 = Seqid;
                    updgo = true;
                }
            }
            else
            {
                updgo = false;
            }

            InitData(Status);

            return View();
        }

        private void InitData(bool status)
        {
            client_codeqq1 = (int)HttpContext.Session.GetInt32("clientcode");
            user_ID11 = (string)HttpContext.Session.GetString("userID");
            CompanyNum111 = (int)HttpContext.Session.GetInt32("CompanyNum");
            CaseNum111 = (string)HttpContext.Session.GetString("CaseNum");
            ClientID11 = (string)HttpContext.Session.GetString("ClientID");
            var CaseNum = CaseNum111;
            var CompanyNum = CompanyNum111;
            if (CompanyNum != null && CaseNum != null)
            {
                string Sequence = "-1";
                string company = CompanyNum.ToString();
                string debtorid = CaseNum.ToString();
         
                this.BindComplaintype(company);
                this.BindSeverityType(company);
                this.BindReportedThrough(company);
                this.BindStatus(company);
            }
            else
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Invalid Client ID");
            }
        }

        [HttpPost]
        public IActionResult Index(ComplainForm complainForm)
        {
            try
            {
                ComplainForm complain = new ComplainForm();

                if (!String.IsNullOrEmpty(complainForm.InqStartdate.ToString()))
                {
                    DateTime stDate = DateTime.Parse(complainForm.InqStartdate.ToString());
                    complain.InqStartdate = complainForm.InqStartdate;
                }

                else
                {
                    complain.InqStartdate = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.Collectorreplydate.ToString()))
                {
                    DateTime clRplyDt = DateTime.Parse(complainForm.Collectorreplydate.ToString());
                    complain.Collectorreplydate = complainForm.Collectorreplydate;
                }

                else
                {
                    complain.Collectorreplydate = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.Daterecieved.ToString()))
                {
                    DateTime DtRcvd = DateTime.Parse(complainForm.Daterecieved.ToString());
                    complain.Daterecieved = complainForm.Daterecieved;
                }

                else
                {
                    complain.Daterecieved = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.MangerReplyDate.ToString()))
                {
                    DateTime MngrRplyDt = DateTime.Parse(complainForm.MangerReplyDate.ToString());  // Input fields - Manager Reply Date
                    complain.MangerReplyDate = complainForm.MangerReplyDate;       //assigning the manager reply date property to the datetime object 
                }

                else
                {
                    complain.MangerReplyDate = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.DateclientId.ToString()))
                {
                    DateTime ClientCntctDt = DateTime.Parse(complainForm.DateclientId.ToString());    // Input fields - Client Contact Date
                    complain.DateclientId = complainForm.DateclientId;     //assigning the collector reply property to the datetime object
                }
                else
                {
                    complain.DateclientId = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.EventDate.ToString()))
                {
                    DateTime DOE = DateTime.Parse(complainForm.EventDate.ToString());
                    complain.EventDate = complainForm.EventDate;  // assigning the Date of event property to the date time object
                }

                else
                {
                    complain.EventDate = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.Follow.ToString()))
                {
                    DateTime FllwUpDt = DateTime.Parse(complainForm.Follow.ToString());
                    complain.Follow = FllwUpDt; //assigning the follow up date property to the datetime object
                }
                else
                {
                    complain.Follow = DateTime.MinValue;
                }

                if (!String.IsNullOrEmpty(complainForm.Complaintfilledby))
                {
                    complain.Complaintfilledby = complainForm.Complaintfilledby;  // assinging textbox complaint filled by with the complaint filled by property defined in Complaint class
                }

                else
                {
                    complain.Complaintfilledby = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.ExtraAddress))
                {
                    complain.ExtraAddress = complainForm.ExtraAddress; // assigning textbox extra address with the extra address property defined in the Complaint class
                }

                else
                {
                    complain.ExtraAddress = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.CaseNo.ToString()))
                {
                    complain.CaseNo = complainForm.CaseNo; // assigning textbox extra address with the extra address property defined in the Complaint class
                }

                else
                {
                    complain.CaseNo = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.StreetAddress))
                {
                    complain.StreetAddress = complainForm.StreetAddress; // assigning textbox street address with the street address property defined in the Complaint class
                }

                else
                {
                    complain.StreetAddress = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.City))
                {
                    complain.City = complainForm.City; // assigning textbox city with the city property defined in the Complaint class
                }

                else
                {
                    complain.City = String.Empty;
                }
                if (!String.IsNullOrEmpty(complainForm.State))
                {
                    complain.State = complainForm.State; // assigning textbox state with the state property defined in the Complaint class
                }

                else
                {
                    complain.State = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.Zip.ToString()))
                {
                    complain.Zip = complainForm.Zip; // assinging textbox zip with the zip property defined in the Complaint class

                }

                else
                {
                    complain.Zip = 0;
                }

                if (!String.IsNullOrEmpty(complainForm.Phone.ToString()))
                {
                    complain.Phone = complainForm.Phone;    // assigning textbox phone with the phone propery definied in the Complaint class
                }

                else
                {
                    complain.Phone = Convert.ToInt32(complainForm.Phone);
                }

                complain.Reported = complainForm.Reported;//complainForm.Reported.TrimEnd(); // assigning dropdown reported through items with the reported by property defined in the Complaint class

                if (!String.IsNullOrEmpty(complainForm.Designation))
                {
                    complain.Designation = complainForm.Designation; // assigning complaint against designation textbox with the relevant property
                }

                else
                {
                    complain.Designation = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.UID))
                {
                    complain.UID = complainForm.UID;// assigning Complaint against UID textbox with the relevant property
                }

                else
                {
                    complain.UID = String.Empty;
                }

                complain.Title = complainForm.Title; //assigning description textbox with the description property defined in the Complaint class

                if (!String.IsNullOrEmpty(complainForm.Description))
                {
                    complain.Description = complainForm.Description.TrimEnd();
                }

                else
                {
                    complain.Description = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.Company.ToString()))
                {
                    complain.Company = complainForm.Company;
                }

                else
                {
                    complain.Company = 0;
                }

                if (!String.IsNullOrEmpty(complainForm.ClientCode.ToString()))
                {
                    complain.ClientCode = complainForm.ClientCode;
                }

                else
                {
                    complain.ClientCode = 0;
                }

                complain.Status = complainForm.Status; //assigning drop down status selected item to the status property defined in the Complaint class
                if (complain.Status != null)
                {
                    var CompanyNum = (int)HttpContext.Session.GetInt32("CompanyNum");
                    string company = CompanyNum.ToString();
                    DB2Operations db2OPS = new DB2Operations(_webHostEnvironment);
                    DataSet ds = db2OPS.BindStatus(company);
                    DataRow[] rows = ds.Tables[0].Select("C5RESO='Y'");
                    DataRow[] smrtrows1 = ds.Tables[0].Select("C5STATUS ='" + complain.Status + "'");
                    complainForm.IsResovled = false;
                    complain.SmartCodeForStatus = smrtrows1[0]["C5SCOD"].ToString();
                    foreach (DataRow srow in rows)
                    {
                        if (complain.Status == srow["C5STATUS"].ToString())
                        {
                            complain.IsResovled = true;
                            complain.Closed = true;
                            DataRow[] smrtrows = ds.Tables[0].Select("C5STATUS ='" + complain.Status + "'");
                            if (smrtrows != null && smrtrows.Length > 0 && !string.IsNullOrEmpty(smrtrows[0]["C5SCOD"].ToString()))
                            {
                                complainForm.SmartCodeForStatus = smrtrows[0]["C5SCOD"].ToString();
                                complain.SmartCodeForStatus = complainForm.SmartCodeForStatus;
                            }
                            else
                            {
                                complainForm.SmartCodeForStatus = String.Empty;
                            }
                        }
                        else
                        {
                            complainForm.Closed = false;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(complainForm.CompanyCost))
                {
                    complain.CompanyCost = complainForm.CompanyCost;
                    // assigning cost to company textbox with the cost to company property defined in the complaint class
                }

                else
                {
                   complain.CompanyCost = String.Empty;
                }

                if (!String.IsNullOrEmpty(complainForm.Howavoidable))
                {
                    complain.Howavoidable = complainForm.Howavoidable;
                }

                else
                {
                    complain.Howavoidable = String.Empty;
                }
                // assigning how avoidable textbox with the how avoidable property defined in the Complaint class

                string CFPB = complainForm.CFPB.ToString();
                // assigning checkbox CFPB with the CFPB property defined in the Complaint class
                if (CFPB == "True")
                {
                    complainForm.CFPB = true;
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("complainForm.CFPB  is:-" + complain.CFPB);
                }

                else
                {
                    complain.CFPB = false;
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("complainForm.CFPB  is:-" + complain.CFPB);
                    String.Empty.ToString();
                }

                string Close = complainForm.Close.ToString();
                if (Close == "True")
                {
                    complainForm.Close = true;
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("complainForm.Close  is:-" + complainForm.Close);
                }

                else
                {
                    complainForm.Close = false;
                    String.Empty.ToString();
                }

                complain.Complainttype = complainForm.Complainttype;
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Complainttype is:-" + complain.Complainttype);

                complain.SeverityType = complainForm.SeverityType;
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("SeverityType is:-" + complain.SeverityType);

                complain.Status = complainForm.Status;
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Status is:-" + complain.Status);

                //Comp.UserID1 = Session[].ToString();
                var userID = (string)HttpContext.Session.GetString("userID");
                complain.UserID1 = (string?)userID;
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("UserID1 is:-" + complain.UserID1);

                DateTime InsertDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                complain.InsertDate1 = InsertDate;

                DateTime InsertTime = DateTime.Parse(DateTime.Now.ToShortTimeString());
                complain.InsertTime1 = InsertTime;

                var Seqid1 = Seqid;// TempData["Seqid"];
                if (Seqid1 != null)

                    complain.SeqForMultiples1 = complainFor.SeqForMultiples1;//(string?)Seqid;
                DB2Operations Obj = new DB2Operations(_webHostEnvironment);
                var Client_ID = (string)HttpContext.Session.GetString("ClientID");

                var ClientID = Client_ID;
                Obj.Client_ID = ClientID;
                bool result = false;
                var userIDs1 = (string)HttpContext.Session.GetString("userID");
                string userID1 = (string)userIDs1;

                if (!Obj.ValidateUSER((complain)))
                {
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("user Id is:-" + "Invalid user id");
                    return Json(new { success = false, message = "invalid" });
                }

                else
                {
                    if (Obj.ValidateDescription(complain, complain))
                    {
                        qtXLogger = new QtXLogger(_webHostEnvironment);
                        qtXLogger.Log("user Description is:-" + "already exist");
                        return Json(new { success = false, message = "alreadyexit" });
                    }
                    result = Obj.AddNewComplaint(complain);
                }

                if (result == true)
                {
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("Complain Result is:-" + "Complain Successfully Saved");
                    return Json(new { success = true, message = "Savesuccess" });
                }

                else
                {
                    qtXLogger = new QtXLogger(_webHostEnvironment);
                    qtXLogger.Log("Complain Result is:-" + "Complain Save Failed");
                    return Json(new { success = false, message = "failed" });
                }

            }
            catch (Exception ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Exception is:-" + ex.Message);
                return Json(new { result = false, error = "Exmessage" });
            }
        }

        [HttpGet]
        public IActionResult LoadList()
        {
            client_codeqq1 = (int)HttpContext.Session.GetInt32("clientcode");
            user_ID11 = (string)HttpContext.Session.GetString("userID");
            CompanyNum111 = (int)HttpContext.Session.GetInt32("CompanyNum");
            CaseNum111 = (string)HttpContext.Session.GetString("CaseNum");
            ClientID11 = (string)HttpContext.Session.GetString("ClientID");
            List<ConfigureList> configureLists = new List<ConfigureList>();
            ConfigureList configureList = new ConfigureList();
            configureList.client_code = client_codeqq1;
            configureList.user_ID = user_ID11;
            configureList.CompanyNum = CompanyNum111;
            configureList.CaseNum = CaseNum111;
            configureList.ClientID = ClientID11;
            configureLists.Add(configureList);

            return Json(configureLists);
        }

        #endregion

        #region " Method "

        private void BindStatus(string company)
        {
            try
            {
                DB2Operations DbBindaStaus = new DB2Operations(_webHostEnvironment);
                var ClientID = (string)HttpContext.Session.GetString("ClientID");//ViewData["ClientID"];
                DbBindaStaus.Client_ID = (string)ClientID;
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                DataSet ds = DbBindaStaus.BindStatus(company);
                DataTable dT = ds.Tables[0];
                List<ComplainForm> StatusLst = new List<ComplainForm>();
                foreach (DataRow dr in dT.Rows)
                {
                    ComplainForm complainForm = new ComplainForm();
                    complainForm.StatusId = dr[2].ToString();
                    complainForm.Status = dr[3].ToString();
                    StatusLst.Add(complainForm);
                }

                var StatusListss = JsonConvert.SerializeObject(StatusLst);
                var StatusListssada = JsonConvert.DeserializeObject<List<ComplainForm>>(StatusListss);
                ViewBag.Status = StatusListssada;
            }
            catch (Exception Ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("BindStatus is:" + Ex.Message);
            }
        }

        private void BindReportedThrough(string company)
        {
            try
            {
                DB2Operations DbReportedThrough = new DB2Operations(_webHostEnvironment);
                var ClientID = (string)HttpContext.Session.GetString("ClientID");//ViewData["ClientID"];
                DbReportedThrough.Client_ID = (string)ClientID;
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                DataSet ds = DbReportedThrough.BindReportedThrough(company);
                DataTable dT = ds.Tables[0];
                List<ComplainForm> Reportedlist = new List<ComplainForm>();
                foreach (DataRow dr in dT.Rows)
                {
                    ComplainForm complainForm = new ComplainForm();
                    complainForm.ReportTypeId = dr[2].ToString();
                    complainForm.Reported = dr[3].ToString();
                    Reportedlist.Add(complainForm);
                }

                var Reported = JsonConvert.SerializeObject(Reportedlist);
                var Report = JsonConvert.DeserializeObject<List<ComplainForm>>(Reported);
                ViewBag.ReportedThrough = Report;
            }
            catch (Exception Ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("BindReportedThrough is:" + Ex.Message);
            }
        }

        private void BindSeverityType(string company)
        {
            try
            {
                DB2Operations DbSeverityType = new DB2Operations(_webHostEnvironment);
                var ClientID = (string)HttpContext.Session.GetString("ClientID");//TempData["ClientID"];
                DbSeverityType.Client_ID = (string)ClientID;
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                DataSet ds = DbSeverityType.BindSeverityType(company);
                DataTable dT = ds.Tables[0];
                List<ComplainForm> DbSeverityTypeLst = new List<ComplainForm>();
                foreach (DataRow dr in dT.Rows)
                {
                    ComplainForm complainForm = new ComplainForm();
                    complainForm.SeverityTypeId = dr[2].ToString();
                    complainForm.SeverityType = dr[3].ToString();
                    DbSeverityTypeLst.Add(complainForm);
                }

                var DbSeverity = JsonConvert.SerializeObject(DbSeverityTypeLst);
                var DbSeverityss = JsonConvert.DeserializeObject<List<ComplainForm>>(DbSeverity);
                ViewBag.SeverityType = DbSeverityss;
            }
            catch (Exception Ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("BindSeverityType is:" + Ex.Message);
            }
        }

        private void BindComplaintype(string company)
        {
            try
            {
                DB2Operations Db2OPS = new DB2Operations(_webHostEnvironment);
                var ClientID = (string)HttpContext.Session.GetString("ClientID");//TempData["ClientID"];
                Db2OPS.Client_ID = (string)ClientID;
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                DataSet ds = Db2OPS.BindComplaintType(company);
                DataTable dT = ds.Tables[0];
                List<ComplainForm> complainLst = new List<ComplainForm>();
                foreach (DataRow dr in dT.Rows)
                {
                    ComplainForm complainForm = new ComplainForm();
                    complainForm.ComplainTypeId = dr[2].ToString();
                    complainForm.Complainttype = dr[3].ToString();
                    complainLst.Add(complainForm);
                }

                var JsonComplain = JsonConvert.SerializeObject(complainLst);
                var objResponse1 = JsonConvert.DeserializeObject<List<ComplainForm>>(JsonComplain);

                ViewBag.Complaintype = objResponse1;
            }
            catch (Exception Ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("BindComplaintype is:" + Ex.Message);
            }
            
        }

        #endregion
    }
}