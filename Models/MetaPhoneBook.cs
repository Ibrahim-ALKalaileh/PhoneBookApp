using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhoneBookApp.Models
{
    [MetadataType(typeof(MetaPhoneBook))]
    public partial class PhoneBook
    {


    }
    public class MetaPhoneBook
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "DOB")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:Y}", ApplyFormatInEditMode = false)]
        public System.DateTime Date_of_birth { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Photo { get; set; }
        [Required]
        public string Address { get; set; }
    }
}