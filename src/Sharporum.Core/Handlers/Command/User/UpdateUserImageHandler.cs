using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.User;
using Sharporum.Core.Helpers;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Responses;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Command.User
{
    public class UpdateUserImageHandler : IRequestHandler<UpdateUserImageCommand, UserResponse>
    {
        private readonly IBlobService _blobService;
        private readonly IUserService _userService;

        public UpdateUserImageHandler(IUserService userService, IBlobService blobService)
        {
            _userService = userService;
            _blobService = blobService;
        }

        public async Task<UserResponse> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
        {
            FileData data =
                BaseHelpers.GetContentFileData<Domain.Entities.User>(request.UpdateUserImageDto.Image, request.UserId);
            await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
            request.UpdateUserImageDto.Image = data.FileName;

            UserViewModel user = await _userService.UpdateUserImageAsync(request.UserId, request.UpdateUserImageDto);

            return new UserResponse {User = user};
        }
    }
}