using System.ComponentModel.DataAnnotations;

namespace MSC.Core.Dtos;

public class UserRegisterDto
{
    [Required(ErrorMessage = "UserName is empty")]
    [MinLength(5, ErrorMessage = "UserName length must be atleast 5 chars")]
    public string UserName { get; set; }

    /*
    ^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?[@#$&()<>]).{8,}$
    (?=.*?[A-Z]): Requires at least one uppercase letter.
    (?=.*?[a-z]): Requires at least one lowercase letter.
    (?=.*?\d): Requires at least one digit (number)
    (?=.*?[@#$&()<>]): Requires at least one special character from the set @#$&()<>
    .{8,}: Ensures a minimum length of 8 characters (note used)
    */
    [Required(ErrorMessage = "Password is empty")]
    [StringLength(16, MinimumLength = 10)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)(?=.*?[@#$&()<>]).+$", ErrorMessage = "Password must have an upper case, a lower case, a number and one special character from the set @#$&()<>")]
    public string Password { get; set; }
}
