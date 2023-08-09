using System;

namespace LoginFinal.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string EncId { get; set; }

        public string Encemail { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Password { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Refferal_Code { get; set; }
        public int? Role { get; set; }
        public int SubCat { get; set; }
        public int IsActive { get; set; }
        public string Country { get; set; }
        public string ImageName { get; set; }

        public string ZipCode { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string City  { get; set; }
        public string Organization { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public DateTime Experience_From { get; set; }
        public DateTime Experience_To { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public int? Status { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public DateTime CreatedAt { get; set; }
        public string GoogleLink { get; set; }
        public string GitHubLink { get; set; }
        public string StackOverFlowLink { get; set; }
        public string DribbleLink { get; set; }
        public string VimeoLink { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string Acc_Deactive_Reason { get; set; }
        public int? isDeActivate { get; set; }
    }
}
