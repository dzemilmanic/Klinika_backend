using Klinika_backend.Models;
using Klinika_backend.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeUserDataController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeUserDataController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Endpoint za ažuriranje imena, prezimena ili lozinke
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserData([FromBody] ChangeUserDataDto updateUserDataDto)
        {
            // Pronađi trenutno prijavljenog korisnika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Korisnik nije pronađen");
            }

            // Ažuriraj ime ako je prosleđeno
            if (!string.IsNullOrWhiteSpace(updateUserDataDto.FirstName))
            {
                if (updateUserDataDto.FirstName.Length < 2)
                {
                    return BadRequest("Ime mora biti duže od 2 karaktera.");
                }
                user.FirstName = updateUserDataDto.FirstName;
            }

            // Ažuriraj prezime ako je prosleđeno
            if (!string.IsNullOrWhiteSpace(updateUserDataDto.LastName))
            {
                if (updateUserDataDto.LastName.Length < 2)
                {
                    return BadRequest("Prezime mora biti duže od 2 karaktera.");
                }
                user.LastName = updateUserDataDto.LastName;
            }

            // Ažuriraj lozinku ako je prosleđena
            if (!string.IsNullOrEmpty(updateUserDataDto.OldPassword) && !string.IsNullOrEmpty(updateUserDataDto.NewPassword))
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, updateUserDataDto.OldPassword);
                if (!passwordCheck)
                {
                    return BadRequest("Stara lozinka nije tačna");
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, updateUserDataDto.OldPassword, updateUserDataDto.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(passwordChangeResult.Errors);
                }
            }

            // Sačuvaj izmene
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok("Podaci korisnika su uspešno ažurirani");
        }
    }
}
