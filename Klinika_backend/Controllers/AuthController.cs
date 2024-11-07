using Klinika_backend.Models.DTO;
using Klinika_backend.Repositories;
using Klinika_backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Linq;
using System.Threading.Tasks;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // POST api/auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            // Proveri da li korisnik sa datim email-om već postoji
            var existingUser = await userManager.FindByEmailAsync(registerRequestDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { Message = $"Email '{registerRequestDto.Username}' je već zauzet." });
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    var roleResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if (roleResult.Succeeded)
                    {
                        return Ok(new { Message = "Korisnik je registrovan! Molimo vas da se prijavite" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Neuspešno dodeljivanje uloga", Errors = roleResult.Errors });
                    }
                }
                return Ok(new { Message = "Korisnik je registrovan bez uloga! Molimo vas da se prijavite" });
            }
            else
            {
                return BadRequest(new { Message = "Registracija korisnika nije uspela", Errors = identityResult.Errors });
            }
        }

        // POST api/auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    // Kreiranje tokena
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(response);
                    }
                }
                return BadRequest(new { Message = "Šifra nije ispravna" });
            }
            return BadRequest(new { Message = "Uneta email adresa ne postoji" });
        }

        [HttpDelete]
        [Route("DeleteReaders")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteReaders()
        {
            // Nabavi sve korisnike
            var users = userManager.Users.ToList();
            var readers = new List<IdentityUser>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Reader"))
                {
                    readers.Add(user);
                }
            }

            if (readers.Count == 0)
            {
                return NotFound(new { Message = "Nema korisnika sa rodom 'Reader'." });
            }

            foreach (var user in readers)
            {
                var identityResult = await userManager.DeleteAsync(user);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(new { Message = "Greška prilikom brisanja korisnika.", Errors = identityResult.Errors });
                }
            }

            return Ok(new { Message = "Svi korisnici sa rodom 'Reader' su uspešno obrisani." });
        }
        //[HttpDelete]
        //[Route("DeleteUser")]
        //[Authorize(Roles = "Writer")] // Obezbeđuje da samo admin može pristupiti ovoj metodi
        //public async Task<IActionResult> DeleteUser([FromBody] CheckEmailRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Email))
        //    {
        //        return BadRequest(new { Message = "Email je obavezan." });
        //    }

        //    var user = await userManager.FindByEmailAsync(request.Email);
        //    if (user == null)
        //    {
        //        return NotFound(new { Message = "Korisnik sa datim email-om ne postoji." });
        //    }

        //    var identityResult = await userManager.DeleteAsync(user);
        //    if (!identityResult.Succeeded)
        //    {
        //        return BadRequest(new { Message = "Greška prilikom brisanja korisnika.", Errors = identityResult.Errors });
        //    }

        //    return Ok(new { Message = "Korisnik je uspešno obrisan." });
        //}
    }

}
