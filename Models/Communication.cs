using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomBooking.Models
{
    [Table("Communication")]
    public class Communication
    {
        [Key]
        public int Comm_CommunicationId { get; set; }
        public int? Comm_OpportunityId { get; set; }
        public int? Comm_CaseId { get; set; }
        public int? Comm_ChannelId { get; set; }
        public string? Comm_Type { get; set; }
        public string? Comm_Action { get; set; }
        public string? Comm_Status { get; set; }
        public string? Comm_Priority { get; set; }
        public DateTime? Comm_DateTime { get; set; }
        public DateTime? Comm_ToDateTime { get; set; }
        public string? Comm_Private { get; set; }
        public string? Comm_OutCome { get; set; }
        public string? Comm_Note { get; set; }
        public string? Comm_Email { get; set; }
        public int? Comm_CreatedBy { get; set; }
        public DateTime? Comm_CreatedDate { get; set; }
        public int? Comm_UpdatedBy { get; set; }
        public DateTime? Comm_UpdatedDate { get; set; }
        public DateTime? Comm_TimeStamp { get; set; }
        public byte? Comm_Deleted { get; set; }
        public string? Comm_DocDir { get; set; }
        public string? Comm_DocName { get; set; }
        public int? Comm_TargetListId { get; set; }
        public DateTime? Comm_NotifyTime { get; set; }
        public int? Comm_NotifyDelta { get; set; }
        public string? Comm_Description { get; set; }
        public string? Comm_SMSMessageSent { get; set; }
        public string? Comm_SMSNotification { get; set; }
        public int? Comm_WaveItemId { get; set; }
        public int? Comm_RecurrenceId { get; set; }
        public int? Comm_LeadID { get; set; }
        public int? Comm_SecTerr { get; set; }
        public int? Comm_WorkflowId { get; set; }
        public int? Comm_MessageId { get; set; }
        public string? Comm_From { get; set; }
        public string? Comm_TO { get; set; }
        public string? Comm_CC { get; set; }
        public string? Comm_BCC { get; set; }
        public string? Comm_ReplyTo { get; set; }
        public int? Comm_IsReplyToMsgId { get; set; }
        public int? Comm_SolutionId { get; set; }
        public string? Comm_IsHtml { get; set; }
        public string? Comm_HasAttachments { get; set; }
        public string? Comm_EmailLinksCreated { get; set; }
        public DateTime? Comm_CompletedTime { get; set; }
        public int? Comm_PercentComplete { get; set; }
        public string? Comm_TaskReminder { get; set; }
        public string? Comm_CRMOnly { get; set; }
        public DateTime? Comm_OriginalDateTime { get; set; }
        public DateTime? Comm_OriginalToDateTime { get; set; }
        public string? Comm_Exception { get; set; }
        public string? Comm_Organizer { get; set; }
        public int? Comm_OrderId { get; set; }
        public int? Comm_QuoteId { get; set; }
        public string? Comm_OutlookID { get; set; }
        public string? Comm_MeetingID { get; set; }
        public string? Comm_IsAllDayEvent { get; set; }
        public string? Comm_Subject { get; set; }
        public string? Comm_Location { get; set; }
        public string? Comm_IsStub { get; set; }
        public string? Comm_MailChimpCampaignId { get; set; }
        public int? Comm_ExtraApprovalsId { get; set; }
        public string? Comm_MeetingRoom { get; set; }
        public string? Comm_RecurrenceRule { get; set; }
        public int? Comm_RecurrenceParentID { get; set; }
        public string? Comm_RequiredInvitees { get; set; }
        public string? Comm_OptionalInvitees { get; set; }
        public string? Comm_RecurrenceGroupId { get; set; }
    }
}