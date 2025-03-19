using System.Security.Cryptography;
using System.Text;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(WarmasterContext db) : BaseAPIController {

    [HttpPost("user/register")]
    public async Task<ActionResult<AppUser>> RegisterUser(DoRegisterUserDTO dto) {
        //check for existing user with same username
        if(await UserExists(dto.UserName)) return BadRequest("This username is taken");
        
        //set up password encryption
        using var hmac = new HMACSHA512();

        //create user
        var user = new AppUser {
            UserName = dto.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key
        };

        //add to db
        db.Users.Add(user);
        
        //attempt to save changes
        try {
            var result = await db.SaveChangesAsync();

            //if save failed
            if(result == 0) {
                return BadRequest("Failed to save user to the database.");
            }

            //else return OK with user
            return Ok(user);
        } catch(Exception ex) {
            //if exception return error message
            return BadRequest(ex.Message);
        }
    } 

    private async Task<bool> UserExists(string userName) {
        //check if username is already in the database
        return await db.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
    }
}