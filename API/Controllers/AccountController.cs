using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username))
            { 
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
        
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDTO
            {
                UserName = user.UserName,
                token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include( p => p.Photos)
                .SingleOrDefaultAsync( o => o.UserName == loginDto.UserName.ToLower());

            if(user == null)
            {
                return Unauthorized("Invalid Username");
            }
            var result = await _userManager.CheckPasswordAsync(user,loginDto.Password);

            if(!result) return Unauthorized("Invalid Password");

            return new UserDTO
            {
                UserName = user.UserName,
                token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault( o => o.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private Task<bool> UserExists(string userName)
        {
            return _userManager.Users.AnyAsync(o => o.UserName == userName.ToLower());
        }
    }
}