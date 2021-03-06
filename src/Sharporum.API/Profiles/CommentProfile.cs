﻿using System.Linq;
using AutoMapper;
using Sharporum.Core.Dtos.Comment;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Domain.Entities;

namespace Sharporum.API.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>()
                .ForMember(
                    c => c.VoteCount,
                    opt => opt.MapFrom(
                        x => x.CommentVotes.Sum(y => y.Direction)
                    )
                );
            CreateMap<CreateCommentDto, Comment>();
        }
    }
}