using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebMVCTest.Models
{
    public class GuestResponse
    {
        [Required(ErrorMessage = "Please enter you name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="please enter you email address")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage ="Please enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter you phone number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please specify whether you'll attend")]
        public bool? WillAttend { get; set; }

    }
}