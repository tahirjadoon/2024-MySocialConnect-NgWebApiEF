using System;
using System.ComponentModel.DataAnnotations;

namespace MSC.Core.Dtos;

public class UserRegisterDto
{
    [Required(ErrorMessage = "UserName is empty")]
    [MinLength(5, ErrorMessage = "UserName length must be atleast 5 chars")]
    [RegularExpression("^[a-zA-Z][A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*$", ErrorMessage = "User name must be alpha numeric with _ - only. Number cannot be a the start. _- should only be in the middle")]
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
    //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)(?=.*?[@#$&()<>]).+$", ErrorMessage = "Password must have an upper case, a lower case, a number and one special character from the set @#$&()<>")]
    //not working[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*W)(?!.* ).{10,16}$", ErrorMessage = "Password must have an upper case, a lower case, a number and one special character")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[-+_!@#$%^&*.,?]).{10,16}$", ErrorMessage = "Password must have an upper case, a lower case, a number and one special character -+_!@#$%^&*.,?")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Gender is empty")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "DisplayName is empty")]
    [MinLength(5, ErrorMessage = "DisplayName length must be atleast 5 chars")]
    [RegularExpression(@"^[a-zA-z0-9]*$", ErrorMessage = "Display name can only be alpha numeric")]
    public string DisplayName { get; set; }

    [Required(ErrorMessage = "DateOfBirth is empty")]
    public DateOnly? DateOfBirth { get; set; } //optional to make required work

    [Required(ErrorMessage = "City is empty")]
    //[RegularExpression("^[a-zA-z]*$", ErrorMessage = "Only characters are allowed for the city")]
    [RegularExpression("^[a-zA-z ]*$", ErrorMessage = "Only characters and space are allowed for the city")]
    public string City { get; set; }

    [Required(ErrorMessage = "Country is empty")]
    //[RegularExpression("^[a-zA-z]*$", ErrorMessage = "Only characters are allowed for the country")]
    [RegularExpression("^[a-zA-z ]*$", ErrorMessage = "Only characters and space are allowed for the country")]
    public string Country { get; set; }
}
