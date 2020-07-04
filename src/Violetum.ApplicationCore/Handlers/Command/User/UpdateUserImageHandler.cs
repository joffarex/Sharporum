using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Command.User
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