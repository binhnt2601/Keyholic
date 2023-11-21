using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BanPhimCanhCach.Models 
{
    public class AppUser: IdentityUser 
    {
          [StringLength(100)]  
          public string Address { get; set; }
    }
}
