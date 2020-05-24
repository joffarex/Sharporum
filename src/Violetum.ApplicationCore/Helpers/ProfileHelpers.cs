using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public static class ProfileHelpers
    {
        public static string GetClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
        }

        public static ProfileViewModel MapUserWithClaimsToProfile(User user, IEnumerable<Claim> claims)
        {
            return new ProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Name = GetClaimByType(claims, JwtClaimTypes.Name),
                GivenName = GetClaimByType(claims, JwtClaimTypes.GivenName),
                FamilyName = GetClaimByType(claims, JwtClaimTypes.FamilyName),
                Picture = GetClaimByType(claims, JwtClaimTypes.Picture),
                Gender = GetClaimByType(claims, JwtClaimTypes.Gender),
                Birthdate = GetClaimByType(claims, JwtClaimTypes.BirthDate),
                Website = GetClaimByType(claims, JwtClaimTypes.WebSite),
            };
        }

        public static List<Claim> PopulateClaimsList(UpdateProfileDto updateProfileDto)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(updateProfileDto.Name))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, updateProfileDto.Name));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.GivenName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, updateProfileDto.GivenName));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.FamilyName))
            {
                claims.Add(new Claim(JwtClaimTypes.FamilyName, updateProfileDto.FamilyName));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Picture))
            {
                claims.Add(new Claim(JwtClaimTypes.Picture, updateProfileDto.Picture));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Gender))
            {
                claims.Add(new Claim(JwtClaimTypes.Gender, updateProfileDto.Gender));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Birthdate))
            {
                claims.Add(new Claim(JwtClaimTypes.BirthDate, updateProfileDto.Birthdate));
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Website))
            {
                claims.Add(new Claim(JwtClaimTypes.WebSite, updateProfileDto.Website));
            }

            return claims;
        }
    }
}