using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoginReg.Models
{
    public class LoginUser
    {
        public string LoginUserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}