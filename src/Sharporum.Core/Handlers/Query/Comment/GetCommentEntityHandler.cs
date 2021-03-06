﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Comment;

namespace Sharporum.Core.Handlers.Query.Comment
{
    public class GetCommentEntityHandler : IRequestHandler<GetCommentEntityQuery, Domain.Entities.Comment>
    {
        private readonly ICommentService _commentService;

        public GetCommentEntityHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Domain.Entities.Comment> Handle(GetCommentEntityQuery request,
            CancellationToken cancellationToken)
        {
            return await _commentService.GetCommentEntityAsync(request.CommentId);
        }
    }
}