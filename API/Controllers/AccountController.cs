using System.Security.Cryptography;
using System.Text;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using API.Interfaces;
using API.Services;

namespace API.Controllers;

public class AccountController(WarmasterContext db, IJWTService tokenService) : BaseAPIController {

    [HttpPost("user/register")]
    public async Task<ActionResult<UserDTO>> RegisterUser(DoRegisterUserDTO dto) {
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
            return Ok(new UserDTO {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            });
        } catch(Exception ex) {
            //if exception return error message
            return BadRequest(ex.Message);
        }
    } 

    [HttpPost("user/login")]
    public async Task<ActionResult<UserDTO>> DoLogin(DoLoginDTO dto) {
        //grab user - we don't need to track as no changes will be made
        var user = await db.Users.Where(x => x.UserName.ToLower() == dto.UserName.ToLower()).AsNoTracking().FirstOrDefaultAsync();

        //null check 
        if(user is null) return Unauthorized("Sorry but we are unable to log you in.");

        //create hmac for hashing pw
        using var hmac = new HMACSHA512(user.PasswordSalt);

        //hash entered password
        var enteredPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

        //loop through hash and check to ensure is matches
        for (int i = 0; i < user.PasswordHash.Length; i++)
        {
            if(user.PasswordHash[i] != enteredPasswordHash[i]) return Unauthorized("Sorry but we are unable to log you in.");
        } 

        //create token
        var token = tokenService.CreateToken(user);

        //return user 
        return Ok(new UserDTO() { Token = token, UserName = user.UserName });
    }

    private async Task<bool> UserExists(string userName) {
        //check if username is already in the database
        return await db.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
    }
}