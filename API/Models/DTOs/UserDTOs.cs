using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class DoRegisterUserDTO { 
    [Required]
    [MaxLength(100)]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
};

public class DoLoginDTO(){
    [Required]
    [MaxLength(100)]
    public required string UserName { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class UserDTO {
    public required string UserName { get; set; }
    public required string Token { get; set; }
}