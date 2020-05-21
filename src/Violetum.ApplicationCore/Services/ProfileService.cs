using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ProfileService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ProfileViewModel> GetProfile(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)}:{userId} not found");
            }


            return MapUserWithClaimsToProfile(user, userClaims);
        }

        public async Task<ProfileViewModel> UpdateProfile(string userId,
            UpdateProfileDto updateProfileDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)}:{userId} not found");
            }

            IList<Claim> claims = await _userManager.GetClaimsAsync(user);

            IdentityResult removeResult;
            var removeSuccess = true;
            if (claims.Count() > 0)
            {
                removeResult = await _userManager.RemoveClaimsAsync(user, claims);
                removeSuccess = removeResult.Succeeded;
            }

            if (!removeSuccess)
            {
                throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
                    "Something went wrong during removing profile claims");
            }

            List<Claim> updatedClaims = PopulateClaimsList(updateProfileDto);

            IdentityResult updateResult = await _userManager.AddClaimsAsync(user, updatedClaims);

            if (updateResult.Succeeded)
            {
                return MapUserWithClaimsToProfile(user, updatedClaims);
            }

            // Recover old claims
            await _userManager.AddClaimsAsync(user, claims);
            throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
                "Something went wrong during updating profile claims");
        }

        private static string GetClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
        }

        private static ProfileViewModel MapUserWithClaimsToProfile(User user, IList<Claim> claims)
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

        private static List<Claim> PopulateClaimsList(UpdateProfileDto updateProfileDto)
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