using Qlib;
using System.Data;
using System.Data.Odbc;
using ComplainModule.Log;

namespace ComplainModule.Models
{
    public class DB2Operations
    {
        #region " Varibales "

        public string? Client_ID { get; set; }
        SmartCodeCaller.RMExSmartCode smar = new SmartCodeCaller.RMExSmartCode();
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        QtXLogger qtXLogger;

        #endregion

        #region " Initilizae Folder Path "

        public DB2Operations(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region " DB2Operations "

        public bool ApplySmartCode(string Casenumber, string SmarCode)
        {
            this.Initconnection();
            configurationBuilder.AddJsonFile($"appsettings.json", true, true);
            var config = configurationBuilder.Build();
            var connectionString = config["As400Connection"];
            var ApiCaller = config["ConnectionStrings:ApiCaller"];
            var As400ConnectionID = config["As400Connection:As400"];
            smar.CallerString = ApiCaller;
            smar.ClientID = Client_ID;
            smar.ConnectionStr = As400ConnectionID;//"Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);
            bool res = smar.ApplySmartCode(Casenumber, SmarCode);
            return res;
        }

        private void Initconnection()
        {
            //Retrive from Appsetting.json
            configurationBuilder.AddJsonFile($"appsettings.json", true, true);
            var config = configurationBuilder.Build();
            var connectionString = config["ConnectionString"];
            var Ipaddress = config["ConnectionStrings:Server"];
            var Database = config["ConnectionStrings:Database"];
            var UserId = config["ConnectionStrings:UserId"];
            var password = config["ConnectionStrings:password"];

            ConfigurationController.ConfigIP = Ipaddress;
            ConfigurationController.ConfigDb = Database;
            ConfigurationController.ConfigUser = UserId;
            ConfigurationController.ConfigPw = password;
            //ConfigurationController.ReadSettings();
        }
               
        public void SelectComp(string company)
        {
            OdbcConnection connection = new OdbcConnection();
            DataSet ds = new DataSet();
            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);
                connection.Open();

                OdbcCommand odbcCom = new OdbcCommand("SELECT CMCOMP FROM SCCOMPLNT Where CMCOMP='" + company + "'");

                odbcCom.Connection = connection;

                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                OA.Fill(ds);
            }
            catch (Exception selectComp)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("odbcCom is:" + selectComp.Message);
                throw new Exception(selectComp.Message);
            }
        }

        public DataSet BindStatus(string company)
        {
            OdbcConnection connection = new OdbcConnection();
            DataSet ds = new DataSet();

            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                // Get Connection Information
                connection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);;//ConfigurationController.GetConnectionString(Client_ID);

                // Open the odbc connection
                connection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT * FROM SCCM005 where C5COMP='" + company + "'AND C5DEL!='D'");
                // QtxLogger.Log("Status='" + odbcCom + "'");

                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = connection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill Dataset
                OA.Fill(ds);
            }

            catch (Exception Status)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("odbcCom is:" + Status.Message);
                //QtxLogger.Log(Status.Message);
                throw new Exception(Status.Message);
            }

            finally
            {
                if (connection != null)
                {
                    //Close Connection
                    connection.Close();
                }
            }
            return ds;
        }

        //Set pool to 32 bit if arithmetic overflow error occures
        public bool AddNewComplaint(ComplainForm newcomp)
        {
            //TODO: Add your sql insert for the iseries
            OdbcConnection connection = new OdbcConnection();
            bool IsStatus = false;
            try
            {
                Initconnection();
                // Get Connection Information
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID; //"Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);;//ConfigurationController.GetConnectionString(Client_ID);
                                                                                                                                                                                                    //Open connection
                connection.Open();

                string flagCMAVOID = "";
                if (newcomp.Avoidable == true)
                    flagCMAVOID = "Y";
                else
                    flagCMAVOID = "N";

                string flagCMCFPB = "";
                if (newcomp.CFPB == true)
                    flagCMCFPB = "Y";
                else
                    flagCMCFPB = "";

                string flagCMCLOSE = "";
                if (newcomp.Closed == true)
                    flagCMCLOSE = "Y";
                else
                    flagCMCLOSE = "";

                string flagCMDEL = "";
                if (newcomp.Delete == true)
                    flagCMDEL = "D";
                else
                    flagCMDEL = "";

                int V = 0;
                int count = -1;

                OdbcCommand odbcVcom = new OdbcCommand("select count(CMSEQ)AS reccount, MAX(CMSEQ)AS sequence from SCCOMPLNT", connection);
                // QtxLogger.Log("Selected sequence");
                //qtXLogger = new QtXLogger(_webHostEnvironment);
               // qtXLogger.Log("selected sequence:" + "elected sequence!");
                OdbcDataReader rd = odbcVcom.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        object obj = rd["sequence"];
                        object obj1 = rd["reccount"];

                        if (obj1 != null)
                        {
                            if (int.TryParse(obj1.ToString(), out count))
                            {
                                if (count == 0)
                                {
                                    V++;
                                }
                                else if (count > 0)
                                {
                                    if (int.TryParse(obj.ToString(), out V))
                                    {
                                        V++;
                                    }
                                    else
                                    {
                                       
                                        //QtxLogger.Log("Invalid Sequence No");
                                        throw new Exception("Invalid sequence!");
                                    }
                                }
                            }
                        }
                        else
                        {
                          
                            //QtxLogger.Log("Invalid Count for sequence");
                            throw new Exception("Invalid count!");
                        }
                    }
                }

                object[] parmarr = new object[]{
                newcomp.Company,
                newcomp.CaseNo,
                newcomp.ClientCode,
                this.GetFormattedDate((DateTime)newcomp.InqStartdate).TrimEnd(),
                this.GetFormattedDate((DateTime)newcomp.Collectorreplydate).TrimEnd(),
                nullCheck(newcomp.Complainttype).TrimEnd(),
                nullCheck(newcomp.SeverityType).TrimEnd(),
                nullCheck(newcomp.Complaintfilledby).TrimEnd(),
                nullCheck(newcomp.ExtraAddress).TrimEnd(),
                nullCheck(newcomp.StreetAddress).TrimEnd(),
                nullCheck(newcomp.City).TrimEnd(),
                newcomp.State,
                newcomp.Zip,
                newcomp.Phone,
                this.GetFormattedDate((DateTime)newcomp.EventDate).TrimEnd(),
                nullCheck(newcomp.Reported).TrimEnd(),
                newcomp.Designation,
                newcomp.UID,
                nullCheck(newcomp.Title).TrimEnd(),
                nullCheck(newcomp.Description).TrimEnd(),
                nullCheck(newcomp.Description).TrimEnd(),
                nullCheck(newcomp.Description).TrimEnd(),
                nullCheck(newcomp.Description).TrimEnd(),
                this.GetFormattedDate((DateTime)newcomp.MangerReplyDate).TrimEnd(),
                this.GetFormattedDate((DateTime)newcomp.DateclientId).TrimEnd(),
                nullCheck(newcomp.Status).TrimEnd(),
                newcomp.CompanyCost,
                flagCMAVOID,

                 nullCheck(newcomp.Howavoidable).TrimEnd(),
                newcomp.UID.TrimEnd(),
              this.GetFormattedDate((DateTime)newcomp.InsertDate1).TrimEnd(),
                this.GetFormattedTime((DateTime)newcomp.InsertTime1).TrimEnd(),
                flagCMCFPB,
               this.GetFormattedDate((DateTime)newcomp.Follow).TrimEnd(),
               this.GetFormattedDate((DateTime)newcomp.Daterecieved).TrimEnd(),
                nullCheck(flagCMCLOSE).ToUpper(),
                flagCMDEL,
                V.ToString().TrimEnd()};

                //////newcomp.Company,
                //////flagCMAVOID,
                //////nullCheck(newcomp.Howavoidable).TrimEnd(),
                //////newcomp.UserID1,
                //////this.GetFormattedDate((DateTime)newcomp.InsertDate1).TrimEnd(),
                //////this.GetFormattedTime((DateTime)newcomp.InsertTime1).TrimEnd(),
                //////flagCMCFPB,
                //////this.GetFormattedDate((DateTime)newcomp.Follow).TrimEnd(),
                //////this.GetFormattedDate((DateTime)newcomp.Daterecieved).TrimEnd(),
                //////nullCheck(flagCMCLOSE).ToUpper(),
                //////flagCMDEL,
                //////V.ToString().TrimEnd()};

                int len = parmarr.Length;

                string cmdIns = string.Format(@"INSERT into SCCOMPLNT (CMCOMP,CMDEBT#,CMCLNT,CMCRDT,CMCPDT,CMTYPE,CMSERV,CMWHOFILED,CMWHOADDR1,CMWHOADDR2,CMWHOCITY,CMWHOSTATE,CMWHOZIP,CMWHOPHN,CMEVTDT,CMREPORTED,CMAGAINST,CMAGAINSP,CMDESC1,CMDESC2,CMDESC3,CMDESC4,CMDESC5,CMMPDT,CMDTCLNT,CMSTATUS,CMCOST,CMAVOID,CMAVOIDHW,CMENTU,CMENTD,CMENTT,CMCFPB,CMFOLLW,CMRCVDT,CMCLOSE,CMDEL,CMSEQ) " +
               "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')", parmarr);
                //QtxLogger.Log("Inserting Values='" + cmdIns + "'");
                // QtxLogger.Log("Inserting Values='" + cmdIns + "'");

                // Odbc Command object initialized with odbc query to Insert the categories
                OdbcCommand odbcCom = new OdbcCommand(cmdIns);
                odbcCom.Connection = connection;

                
                // QtxLogger.Log("About to call samrt code pgm");
                if (newcomp.IsResovled || newcomp.Closed)
                {
                    string casenum = newcomp.Company  + newcomp.CaseNo.ToString();
                    bool res = this.ApplySmartCode(casenum, newcomp.SmartCodeForStatus);
                  
                }

                else
                {
                   
                    bool InsertNewSmartCdStatus = this.ApplySmartCode(newcomp.Company + newcomp.CaseNo, newcomp.SmartCodeForStatus);
                    
                }

                
                //QtxLogger.Log("About to insert complaint");
                if (odbcCom.ExecuteNonQuery() > 0)
                {
                    IsStatus = true;
                }
                else
                {
                    // return false;
                 
                }

                string InsertNewSmartCd = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + newcomp.Company + "','" + newcomp.DebtorID1 + "' ,(Select OPF066 from SCSYSOP2 Where OPSCOM='" + newcomp.Company + "'))");
                //odbc command object initialized with odbc query to Insert smart code for new complaints.Smart code - 132.
                // QtxLogger.Log("New complaint Inserting Smartcode='" + InsertNewSmartCd + "'");
                OdbcCommand odbcComUpdt = new OdbcCommand(InsertNewSmartCd);
                odbcComUpdt.Connection = connection;
                // QtxLogger.Log("About to insert sctrac 1");
                if (odbcComUpdt.ExecuteNonQuery() > 0)
                {
                    IsStatus = true;
                }
                else
                {
                    
                    // return false;
                }

                string FetchSmartCdComplaintType = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + newcomp.Company + "','" + newcomp.DebtorID1 + "',(select C2SCOD from SCCM002 where C2COMP='" + newcomp.Company + "' and C2TYPE='" + newcomp.Complainttype + "'))");
                //odbc command object initialized with odbc query to Insert smart codes for the type of complaint.
                // QtxLogger.Log("Fetching smartcode for Complaint Type='" + FetchSmartCdComplaintType + "'");

                OdbcCommand odbcComInsrtCmplntType = new OdbcCommand(FetchSmartCdComplaintType);
                odbcComInsrtCmplntType.Connection = connection;

                

                //QtxLogger.Log("About to insert sctrac 2");
                if (odbcComInsrtCmplntType.ExecuteNonQuery() > 0)
                {
                    IsStatus = true;
                }
                else
                {
                    
                    // return false;
                }

                string FetchSmartCdStatus = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + newcomp.Company + "','" + newcomp.DebtorID1 + "',(select C5SCOD from SCCM005 where C5COMP='" + newcomp.Company + "' AND C5STATUS='" + newcomp.Status + "'))");
                //odbc command object initilaized with odbc query to Insert smart codes for the status of complaint.
                //QtxLogger.Log("Fetching smartcode for Status='" + FetchSmartCdStatus + "'");
                OdbcCommand odbcComInsrtSatus = new OdbcCommand(FetchSmartCdStatus);
                odbcComInsrtSatus.Connection = connection;
                // QtxLogger.Log("About to insert sctrac 3");
                if (odbcComInsrtSatus.ExecuteNonQuery() > 0)
                {
                    IsStatus = true;
                }
                else
                {
                    
                    // return false;
                }

                // QtxLogger.Log("IsResovled " + newcomp.IsResovled);
                if (newcomp.IsResovled)
                {
                    string QueryUpdtClosed = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + newcomp.Company + "','" + newcomp.DebtorID1 + "',(Select OPF067 from SCSYSOP2 Where OPSCOM='" + newcomp.Company + "'))");
                    //odbc command object initialized with odbc query to Insert smartcodes if status type changes to "RESOLVED" when adding a new complaint.Smart code - 133.
                    //QtxLogger.Log("Query update for Resolved Complaints into sctrac='" + QueryUpdtClosed + "'");
                    OdbcCommand odbcComClosed = new OdbcCommand(QueryUpdtClosed, connection);
                    // QtxLogger.Log("About to insert sctrac 4");
                    if (odbcComClosed.ExecuteNonQuery() > 0)
                    {
                        IsStatus = true;
                       
                    }
                    else
                    {
                       
                        // return false;
                    }
                }
            }

            catch (Exception Insert)
            {
                //QtxLogger.Log(Insert.Message);
                //QtxLogger.Log(Insert.StackTrace);
                //QtxLogger.Log(Insert.Source);
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Insert.Message) is:" + Insert.Message);
                //return Json(new { success = false, message = "invalid" });
               
            }
            finally
            {
                if (connection != null)
                {
                    //Close Connection
                    connection.Close();
                }

            }
            return IsStatus;
        }

        private string nullCheck(string value)
        {
            if (value == null)
                return "";

            return value;
        }

        private string GetFormattedDate(DateTime date)
        {
            try
            {
                if (date != DateTime.MinValue)
                {
                    string dt = date.Year.ToString().Substring(0, 4).PadLeft(2, '0') + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');
                    return dt;
                }
                else
                {
                    return "00000000";
                }
            }
            catch (Exception ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("ex.Message) is:" + ex.Message);
                //  QtxLogger.Log("Invalid date format");
                throw new Exception("invalid date format!");
            }
            //yymmdd
        }

        private string GetFormattedTime(DateTime time)
        {
            try
            {
                string Time = time.Hour.ToString() + time.Minute.ToString();
                return Time;
            }
            catch (Exception ex)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("ex.Message is:" + ex.Message);
                //  QtxLogger.Log("Invalid time format - " + ex.Message);
                throw new Exception("invalid time format!");
            }
        }

        public bool ValidateDescription(ComplainForm ValidateTitle, ComplainForm complain)
        {
            OdbcConnection connection = new OdbcConnection();
            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID;//"Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);
                //connection.ConnectionString = //ConfigurationController.GetConnectionString(Client_ID);
                connection.Open();
                string FetchDescTitle = string.Format(@"Select COUNT(CMDESC1) from SCCOMPLNT WHERE CMDESC1='" + ValidateTitle.Title.Trim() + "' AND CMDEBT#='" + complain.CaseNo + "'");
                //string FetchDescTitle = string.Format(@"Select COUNT(CMDESC1) from SCCOMPLNT WHERE CMDESC1='" + ValidateTitle.Title.Trim() + "' AND CMDEBT#='" + CaseNo.UserID1) + "'";
                OdbcCommand odbcDescTitle = new OdbcCommand(FetchDescTitle, connection);
                int count = 0;
                object obj = odbcDescTitle.ExecuteScalar();
                if (obj != null && int.TryParse(obj.ToString(), out count))
                {
                    if (count > 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ValidationTitleDesc)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("ValidationTitleDesc.Message is:" + ValidationTitleDesc.Message);
                //QtxLogger.Log(ValidationTitleDesc.Message);
                throw new Exception(ValidationTitleDesc.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
       
        public bool ValidateUSER(ComplainForm ValidateUID)
        {
            OdbcConnection connection = new OdbcConnection();
            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID;// "Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);
                                                                                                                                                                                                                           // connection.ConnectionString = ConfigurationController.GetConnectionString(Client_ID);
                connection.Open();

                string FetchUID = string.Format(@"Select COUNT(SUSER) from SC0001 WHERE SUSER='" + ValidateUID.UID.Trim() + "'");
                //QtxLogger.Log("Validating user select query");
                OdbcCommand odbcUID = new OdbcCommand(FetchUID, connection);
                int count = 0;
                object obj = odbcUID.ExecuteScalar();
                if (obj != null && int.TryParse(obj.ToString(), out count))
                {
                    if (count > 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ValidationUID)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("ValidationUID.Message is:" + ValidationUID.Message);
                //QtxLogger.Log(ValidationUID.Message);
                throw new Exception(ValidationUID.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        public bool UpdateComplaint(ComplainForm updCom)
        {

            OdbcConnection connection = new OdbcConnection();
            bool isStatus = false;
            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID; //"Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);
                                                                                                                                                                                                                           // connection.ConnectionString = ConfigurationController.GetConnectionString(Client_ID);
                connection.Open();

                //string flagUpdtCMAVOID = "";
                //if (updCom.Avoidable == true)
                //    flagUpdtCMAVOID = "Y";
                //else
                //    flagUpdtCMAVOID = "N";

                //string flagUpdtCMCFPB = "";

                //if (updCom.CFPB == true)
                //    flagUpdtCMCFPB = "Y";
                //else
                //    flagUpdtCMCFPB = "";

                //string flagUpdtCMCLOSE = "";
                //if (updCom.Closed == true)
                //    flagUpdtCMCLOSE = "Y";
                //else
                //    flagUpdtCMCLOSE = "";

                //string flagUpdtCMDEL = "";
                //if (updCom.Delete == true)
                //    flagUpdtCMDEL = "D";
                //else

                //    flagUpdtCMDEL = "";

                string QueryUpdateComplaint = string.Format(@"UPDATE SCCOMPLNT SET CMWHOFILED='{0}',CMSTATUS='{1}' WHERE CMSEQ='{2}'",
                //  string QueryUpdateComplaint = string.Format(@"UPDATE SCCOMPLNT SET CMCRDT='{0}',CMCPDT='{1}',CMTYPE='{2}',CMSERV='{3}',CMWHOFILED='{4}',CMWHOADDR1='{5}',CMWHOADDR2='{6}',CMWHOCITY='{7}',CMWHOSTATE='{8}',CMWHOZIP='{9}',CMWHOPHN='{10}',CMEVTDT='{11}',CMREPORTED='{12}',CMAGAINST='{13}',CMAGAINSP='{14}',CMDESC1='{15}',CMDESC2='{16}',CMDESC3='{17}',CMDESC4='{18}',CMDESC5='{19}',CMMPDT='{20}',CMDTCLNT='{21}',CMSTATUS='{22}',CMCOST='{23}',CMAVOID='{24}',CMAVOIDHW='{25}',CMCFPB='{26}',CMFOLLW='{27}',CMRCVDT='{28}',CMENTU='{29}',CMENTD='{30}',CMENTT='{31}',CMCLOSE='{32}',CMDEL='{33}' WHERE CMSEQ='{34}'",
                //this.GetFormattedDate((DateTime)updCom.InqStartdate),
                //this.GetFormattedDate((DateTime)updCom.Collectorreplydate),
                //  updCom.Complainttype.TrimEnd(),
                //updCom.SeverityType.TrimEnd(),
                updCom.Complaintfilledby.TrimEnd(),
                //updCom.ExtraAddress.TrimEnd(),
                //updCom.StreetAddress.TrimEnd(),
                // updCom.City,
                // updCom.State,
                // updCom.Zip,
                // updCom.Phone,
                // this.GetFormattedDate((DateTime)updCom.EventDate),
                // updCom.Reported.TrimEnd(),
                // updCom.Designation.TrimEnd(),
                // updCom.UID.TrimEnd(),
                // updCom.Title.TrimEnd(),
                //updCom.Description.TrimEnd(),
                // updCom.Description21.TrimEnd(),
                // updCom.Description31.TrimEnd(),
                // updCom.Description41.TrimEnd(),
                // t//his.GetFormattedDate((DateTime)updCom.MangerReplyDate),
                // this.GetFormattedDate((DateTime)updCom.DateclientId),
                updCom.Status.TrimEnd(),
              //  updCom.CompanyCost.TrimEnd(),
               // flagUpdtCMAVOID.TrimEnd(),
               // updCom.Howavoidable.TrimEnd(),
               // flagUpdtCMCFPB.TrimEnd(),
               // this.GetFormattedDate((DateTime)updCom.Follow),
                //this.GetFormattedDate((DateTime)updCom.Daterecieved),
               // updCom.UserID1,
               // this.GetFormattedDate((DateTime)updCom.InsertDate1),
               // this.GetFormattedTime((DateTime)updCom.InsertTime1),
               // flagUpdtCMCLOSE.TrimEnd(),
                //flagUpdtCMDEL.TrimEnd());
                updCom.SeqForMultiples1.TrimEnd());

                // QtxLogger.Log("Update query='" + QueryUpdateComplaint + "'");
                OdbcCommand odbcCom = new OdbcCommand(QueryUpdateComplaint, connection);

                if (updCom.IsResovled || updCom.Closed)
                {
                    string casenum = (updCom.Company.ToString() +"00"+ updCom.CaseNo);
                    bool res = this.ApplySmartCode(casenum, updCom.SmartCodeForStatus);

                }
                else
                {
                    
                    bool UpdateSmartCd = this.ApplySmartCode(updCom.Company.ToString() + "00" + updCom.CaseNo, updCom.SmartCodeForStatus);
                }

                if (odbcCom.ExecuteNonQuery() > 0)
                {
                    isStatus = true;
                }
                else
                {
                    return false;
                }

                string FetchSmartCdType = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + updCom.Company + "','" + updCom.DebtorID1 + "',(select C2SCOD from SCCM002 where C2COMP='" + updCom.Company + "' and C2TYPE='" + updCom.Complainttype + "'))");
                //QtxLogger.Log("Updating smartcode query for complaint type='" + FetchSmartCdType + "'");
                //odbc command object initilaized with odbc query to Update smart codes for the type of complaint.
                OdbcCommand odbcComUpdtCmplntType = new OdbcCommand(FetchSmartCdType, connection);

                if (updCom.IsType)
                {

                    if (odbcComUpdtCmplntType.ExecuteNonQuery() > 0)
                    {
                        isStatus = true;
                    }
                    else
                    {

                        return false;

                    }
                }

                if (updCom.IsResovled || updCom.Closed)
                {
                    string QueryUpdtClosed = string.Format(@"Insert into SCTRAC(RCOMP,RDEBT#,RACCOD) VALUES('" + updCom.Company + "','" + updCom.DebtorID1 + "',(Select OPF067 from SCSYSOP2 Where OPSCOM='" + updCom.Company + "'))");
                    //odbc command object initilaized with odbc query to Update smart codes when status of complaint is changed to "RESOLVED" in update.Smart code - 133.
                    // QtxLogger.Log("Updating Resolved='" + QueryUpdtClosed + "'");
                    OdbcCommand odbcComClosed = new OdbcCommand(QueryUpdtClosed, connection);

                    if (odbcComClosed.ExecuteNonQuery() > 0)
                    {
                        isStatus = true;
                    }
                    else
                    {
                        isStatus = false;
                        return false;
                    }

                }

            }
            catch (Exception UpdateComplnt)
            {
                //QtxLogger.Log("Error on updating");
                //QtxLogger.Log(UpdateComplnt.Message);
                //QtxLogger.Log(UpdateComplnt.StackTrace);
                //QtxLogger.Log(UpdateComplnt.Source);
                //MessageBox.Show(UpdateComplnt.Message);
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("UpdateComplnt.Message is:" + UpdateComplnt.Message);
                throw new Exception(UpdateComplnt.Message);

            }
            finally
            {
                if (connection != null)
                {

                    connection.Close();
                }
            }

            //sql update
            return isStatus;

        }

        public DataSet GetComplaintByCompanyDebtor(string company, string debtorid, string Sequence)
        {
            OdbcConnection connection = new OdbcConnection();
            DataSet ds = new DataSet();

            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                connection.ConnectionString = As400ConnectionID; //"Driver={Client Access ODBC Driver (32-bit)}; Connect Timeout=120; System=10.23.144.10; DBQ=SCDATARM61 SCFIXRMXT SCFIXRMX61 SCRMX61 SCRMXKEY61; UID=KANDEE;Password=abc123;";//ConfigurationController.GetConnectionString(Client_ID);;//ConfigurationController.GetConnectionString(Client_ID);
                //connection.ConnectionString = ConfigurationController.GetConnectionString(Client_ID);

                connection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT *  FROM SCCM005,SCCOMPLNT where SCCOMPLNT.CMCOMP ='" + company + "'AND SCCOMPLNT.CMDEBT# ='" + debtorid + "' AND SCCM005.C5COMP='" + company + "' AND CMSEQ='" + Sequence + "' AND CMDEL !='D' AND SCCM005.C5STATUS = SCCOMPLNT.CMSTATUS");


                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = connection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill Dataset
                OA.Fill(ds);


            }
            catch (Exception GetDebtorInfo)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("GetDebtorInfo.Message is:" + GetDebtorInfo.Message);
                //  QtxLogger.Log(GetDebtorInfo.Message);
                throw new Exception(GetDebtorInfo.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            //Select all whre case ='';
            return ds;
        }

        #endregion

        #region " Data Set "

        public DataSet BindComplaintList(string company, string caseno)
        {
            OdbcConnection connection = new OdbcConnection();
            DataSet ds = new DataSet();
            try
            {
                Initconnection();

                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                // Get Connection Information
                connection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);

                // Open the odbc connection
                connection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT * FROM SCCOMPLNT where CMCOMP='" + company + "' AND CMDEBT#='" + caseno + "' AND CMDEL!='D'");
                //QtxLogger.Log("complaint list='" + odbcCom + "'");
                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = connection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill Dataset
                OA.Fill(ds);

            }
            catch (Exception List)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("List.Message is:" + List.Message);
                //QtxLogger.Log(List.Message);
                //throw new Exception(List.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return ds;
            //TODO: Select *
        }
        public DataSet BindComplaintType(string Company)
        {
            // odbc connection object initialized with connection string used to 
            // connect it with the complaints odbc database
            OdbcConnection conection = new OdbcConnection();

            //Dataset object to store the retrieved odbc data items
            DataSet ds = new DataSet();
            try
            {
                Initconnection();
                //GetConnectionInfo();

                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                conection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);

                // Open the odbc connection 
                conection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT *  FROM SCCM002 where C2COMP='" + Company + "'AND C2DEL!='D'");

                ///QtxLogger.Log("Complaint type='" + odbcCom + "'");
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Complaint type='" + odbcCom + "'");

                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = conection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill Dataset
                OA.Fill(ds);

            }
            catch (Exception CmplntType)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("CmplntType is:" + CmplntType.Message);
                throw new Exception(CmplntType.Message);
            }
            finally
            {
                if (conection != null)
                {
                    //Close Connection
                    conection.Close();
                }
            }

            return ds;
        }

        public DataSet BindSeverityType(string Company)
        {
            // odbc connection object initialized with connection string used to 

            // connect it with the complaints odbc database
            OdbcConnection connection = new OdbcConnection();

            //Dataset object to store the retrieved odbc data items
            DataSet ds = new DataSet();
            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                // Get Connection Information
                connection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);

                // Open the odbc connection 
                connection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT * FROM SCCM001 where C1COMP='" + Company + "' AND C1DEL!='D'");
                //QtxLogger.Log("Severity Type='" + odbcCom + "'");
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Severity Type='" + odbcCom + "'");

                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = connection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill dataset
                OA.Fill(ds);
            }
            catch (Exception Severity)
            {
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Severity is:" + Severity.Message);
                throw new Exception(Severity.Message);
            }

            finally
            {
                if (connection != null)
                {
                    //Close connection
                    connection.Close();
                }
            }

            return ds;
        }

        public DataSet BindReportedThrough(string company)
        {
            OdbcConnection connection = new OdbcConnection();
            DataSet ds = new DataSet();

            try
            {
                Initconnection();
                var config = configurationBuilder.Build();
                var As400ConnectionID = config["As400Connection:As400"];
                // Get Connection Information
                connection.ConnectionString = As400ConnectionID;//ConfigurationController.GetConnectionString(Client_ID);

                // Open the odbc connection 
                connection.Open();

                // Odbc Command object initialized with odbc query to retrieve the categories
                OdbcCommand odbcCom = new OdbcCommand("SELECT * FROM SCCM003 where C3COMP='" + company + "' AND C3DEL!='D'");

                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("Reported Through='" + odbcCom + "'");
                // QtxLogger.Log("Reported Through='" + odbcCom + "'");
                //ODBC Command initialized with odbc connection to establish the connection with the database
                odbcCom.Connection = connection;

                // Odbc Data Adapter object initialized by passing the odbc Command object
                OdbcDataAdapter OA = new OdbcDataAdapter(odbcCom);

                //Fill Dataset
                OA.Fill(ds);

            }
            catch (Exception ReprtdThrgh)
            {
                //QtxLogger.Log(ReprtdThrgh.Message);
                //MessageBox.Show(ReprtdThrgh.Message);
                qtXLogger = new QtXLogger(_webHostEnvironment);
                qtXLogger.Log("ReprtdThrgh is:" + ReprtdThrgh.Message);
                throw new Exception(ReprtdThrgh.Message);
            }

            finally
            {
                if (connection != null)
                {
                    //Close Connection
                    connection.Close();
                }
            }

            return ds;
        }

        #endregion
    }
}