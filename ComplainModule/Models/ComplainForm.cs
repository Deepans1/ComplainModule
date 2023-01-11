using System.ComponentModel.DataAnnotations;

namespace ComplainModule.Models
{
    public class ComplainForm
    {
        #region " Properties "

        public int Company { get; set; }
        public int ClientCode { get; set; }
        public string? CaseNo { get; set; }
        public string? StatusId { get; set; }
        public string? ComplainTypeId { get; set; }
        public string? SeverityTypeId { get; set; }
        public string? ReportTypeId { get; set; }
        public int DebtorID1 { get; set; }
        public string? UserID1 { get; set; }
        public string? SmartCodeForStatus { get; set; }
        public string? SeqForMultiples1 { get; set; }
        public string? SmartCodeForType { get; set; }
        public DateTime? InsertDate1 { get; set; }
        public DateTime? InsertTime1 { get; set; }
        public bool Delete { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "InqStartdate  is Required.")]
        public DateTime? InqStartdate { get; set; }
        [Required(ErrorMessage = "Collectorreplydate  is Required.")]
        public string? Complainttype { get; set; }
        [Required(ErrorMessage = "Collectorreplydate  is Required.")]
        public DateTime? Collectorreplydate { get; set; }
      
        [Required(ErrorMessage = "SeverityType  is Required.")]
        public string? SeverityType { get; set; }
        //[Required(ErrorMessage = "SeverityType  is Required.")]
        [Required(ErrorMessage = "Complaintfilledby  is Required.")]
        public string? Complaintfilledby { get; set; }
        [Required(ErrorMessage = "ExtraAddress  is Required.")]
        public string? ExtraAddress { get; set; }
        [Required(ErrorMessage = "StreetAddress  is Required.")]
        public string? StreetAddress { get; set; }
        [Required(ErrorMessage = "City  is Required.")]
        public string? City { get; set; }
        [Required(ErrorMessage = "State  is Required.")]
        public string? State { get; set; }
        [Required(ErrorMessage = "Zip  is Required.")]
        public int Zip { get; set; }
        [Required(ErrorMessage = "Phone  is Required.")]
        public int Phone { get; set; }
        [Required(ErrorMessage = "EventDate  is Required.")]
        public DateTime? EventDate { get; set; }
        [Required(ErrorMessage = "Reported  is Required.")]
        public string? Reported { get; set; }
        [Required(ErrorMessage = "Designation  is Required.")]
        public string? Designation { get; set; }
        [Required(ErrorMessage = "UID  is Required.")]
        public string? UID { get; set; }
        [Required(ErrorMessage = "Title  is Required.")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Description  is Required.")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Status  is Required.")]
        public string? Status { get; set; }
        [Required(ErrorMessage = "CompanyCost  is Required.")]
        public string? CompanyCost { get; set; }
        [Required(ErrorMessage = "Avoidable  is Required.")]
        public bool Avoidable { get; set; }
        [Required(ErrorMessage = "AvoidableYes  is Required.")]
        public bool AvoidableYes { get; set; }
        [Required(ErrorMessage = "AvoidableNo  is Required.")]
        public bool AvoidableNo { get; set; }
        [Required(ErrorMessage = "Howavoidable  is Required.")]
        public string? Howavoidable { get; set; }
        [Required(ErrorMessage = "CFPB  is Required.")]
        public bool CFPB { get; set; }
        [Required(ErrorMessage = "Close  is Required.")]
        public bool Close { get; set; }
        [Required(ErrorMessage = "Daterecieved  is Required.")]
        public DateTime? Daterecieved { get; set; }
        [Required(ErrorMessage = "Daterecieved  is Required.")]
        public bool Closed { get; set; }
        [Required(ErrorMessage = "MangerReplyDate  is Required.")]
        public DateTime? MangerReplyDate { get; set; }
        [Required(ErrorMessage = "DateclientId  is Required.")]
        public DateTime? DateclientId { get; set; }
        [Required(ErrorMessage = "Follow  is Required.")]
        public DateTime? Follow { get; set; }
  
        private string StatusState;
        public bool IsStatus { get; set; }
        public bool IsType { get; set; }
        public bool IsResovled { get; set; }

       
        private string ComplaintList;
        public string ComplaintList1
        {
            get { return ComplaintList; }
            set { ComplaintList = value; }
        }
        public List<ComplainForm> Name = new List<ComplainForm>();

        #endregion
    }
}