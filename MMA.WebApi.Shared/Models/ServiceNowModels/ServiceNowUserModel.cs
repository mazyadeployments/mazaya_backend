namespace MMA.WebApi.Shared.Models.ServiceNowModels
{
    public class ServiceNowUserModel
    {
        public string u_data_type { get; set; }
        public string u_company { get; set; }
        public string u_request_number_fcr { get; set; }
        public string u_problem_support_group { get; set; }
        public string u_requested_by { get; set; }
        public string u_username { get; set; }
        public string u_email { get; set; }
        public string u_mobile { get; set; }
        public string u_employee_id { get; set; }
        public string u_site { get; set; }
        public string u_employment_type { get; set; }
        public string u_relationship { get; set; }
        public string u_card_number { get; set; }
        public string u_type_of_service { get; set; }
        public string u_status { get; set; }
        public string u_plan { get; set; }
        public string u_membership { get; set; }
        public string u_valid_to { get; set; }
        public string u_avail_farah_experience { get; set; }
        public string u_remarks { get; set; }
        public string u_applicant_first_name { get; set; }
        public string u_dependent_last_name { get; set; }
        public string u_dependent_middle_name { get; set; }
        public string u_member_date_of_birth { get; set; }
        public string u_photo { get; set; }
        public string sys_updated_on { get; set; }
        public string u_member_email_id { get; set; }
        public string u_req_email { get; set; }
        public string u_req_name { get; set; }
        public string u_req_num { get; set; }
        public string u_req_id { get; set; }
    }

    public class MemberModel
    {
        public string Id { get; set; }
        public string mail { get; set; }
    }
}
