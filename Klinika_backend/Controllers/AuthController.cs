using Klinika_backend.Models.DTO;
using Klinika_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Linq;
using System.Threading.Tasks;
using Klinika_backend.Models;

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
            var existingUser = await userManager.FindByEmailAsync(registerRequestDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = $"Email '{registerRequestDto.Email}' je već zauzet." });
            }

            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName
            };

            // Kreiraj korisnika sa lozinkom
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest(new { Message = "Registracija korisnika nije uspela.", Errors = identityResult.Errors });
            }

            // Proveri i dodeli uloge
            if (registerRequestDto.Roles == null || !registerRequestDto.Roles.Any())
            {
                // Ako uloge nisu prosleđene, dodeli podrazumevanu ulogu "User"
                registerRequestDto.Roles = ["User"] ;
            }

            var roleResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
            if (!roleResult.Succeeded)
            {
                // Ako dodela uloga ne uspe, obriši korisnika kako bi se sprečila nekonzistencija
                await userManager.DeleteAsync(identityUser);
                return BadRequest(new { Message = "Greška prilikom dodele uloga.", Errors = roleResult.Errors });
            }

            return Ok(new { Message = "Korisnik je uspešno registrovan! Molimo vas da se prijavite." });
        }


        // POST api/auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
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

        //[Authorize(Roles = "Admin")] // Samo admin može pristupiti
        [HttpGet("GetUsers")] // Endpoint za prikaz korisnika
        public async Task<IActionResult> GetUsers()
        {
            var users = userManager.Users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email
            }).ToList();

            return Ok(users);
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Obezbeđuje da samo admin može pristupiti ovoj metodi
        public async Task<IActionResult> DeleteUser([FromRoute]Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(new { Message = "Korisnik sa datim ID-om ne postoji." });
            }

            // Brisanje korisnika
            var identityResult = await userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Greška prilikom brisanja korisnika.",
                    Errors = identityResult.Errors
                });
            }

            return Ok(new { Message = "Korisnik je uspešno obrisan." });
        }
    }

}
