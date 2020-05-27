using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public ProfileService(UserManager<User> userManager, IMapper mapper, IUserValidators userValidators)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userValidators = userValidators;
        }

        public async Task<ProfileViewModel> GetProfile(string userId)
        {
            User user = await _userValidators.GetReturnedUserOrThrow(userId);
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);

            return ProfileHelpers.MapUserWithClaimsToProfile(user, userClaims);
        }

        public async Task<ProfileViewModel> UpdateProfile(string userId,
            UpdateProfileDto updateProfileDto)
        {
            if (userId != updateProfileDto.Id)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                    $"{nameof(Comment)}:(uid[{userId}]|dtoid[{updateProfileDto.Id}] update");
            }

            User user = await _userValidators.GetReturnedUserOrThrow(userId);
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

            List<Claim> updatedClaims = ProfileHelpers.PopulateClaimsList(updateProfileDto);

            IdentityResult updateResult = await _userManager.AddClaimsAsync(user, updatedClaims);

            if (updateResult.Succeeded)
            {
                return ProfileHelpers.MapUserWithClaimsToProfile(user, updatedClaims);
            }

            // Recover old claims
            await _userManager.AddClaimsAsync(user, claims);
            throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
                "Something went wrong during updating profile claims");
        }
    }
}