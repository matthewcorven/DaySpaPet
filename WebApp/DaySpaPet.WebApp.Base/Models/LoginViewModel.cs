using System.ComponentModel.DataAnnotations;

namespace DaySpaPet.Web.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    public bool LoginFailureHidden { get; set; } = true;
}