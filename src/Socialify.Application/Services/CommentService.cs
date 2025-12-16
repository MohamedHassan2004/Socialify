using Humanizer;
﻿using Microsoft.Extensions.Logging;
﻿using Socialify.Application.DTOs.Comment;
﻿using Socialify.Application.Mappers;
﻿using Socialify.Application.Repos_Interfaces;
﻿using Socialify.Application.Services_Interfaces;
﻿using Socialify.Domain.Common;
﻿using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Text;
﻿using System.Threading.Tasks;
﻿
﻿namespace Socialify.Application.Services
﻿{
﻿    public class CommentService : ICommentService
﻿    {
﻿        private readonly ILogger<CommentService> _logger;
﻿        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CommentService(ILogger<CommentService> logger, IUnitOfWork unitOfWork, INotificationService notificationService)
﻿        {
﻿            _logger = logger;
﻿            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }﻿
﻿
﻿        public async Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto, string currentUserId)
﻿        {
﻿            try
﻿            {
﻿                var comment = new Comment
﻿                {
﻿                    Content = addCommentDto.Content,
﻿                    PostId = addCommentDto.PostId,
﻿                    UserId = currentUserId,
﻿                };
﻿                await _unitOfWork.Comments.AddAsync(comment);
﻿
﻿                var post = await _unitOfWork.Posts.GetByIdAsync(addCommentDto.PostId);
﻿                if (post == null)
﻿                    return Result<CommentDto>.Failure("Post not found.");
﻿                post.IncrementCommentsCount();
﻿
﻿                await _unitOfWork.SaveAsync();
﻿
﻿                var commentWithUser = await _unitOfWork.Comments.GetCommentWithUserAsync(comment.Id);
﻿                var commentDto = commentWithUser.ToCommentDto(currentUserId);

                await _notificationService.SendNotificationAsync(currentUserId, NotificationType.Comment, post.UserId, post.Id);
﻿
﻿                _logger.LogInformation("Comment added to post {PostId} by user {UserId}", addCommentDto.PostId, currentUserId);
﻿                return Result<CommentDto>.Success(commentDto);
﻿            }
﻿            catch(Exception ex)
﻿            {
﻿                _logger.LogError(ex, "Error adding comment to post {PostId} by user {UserId}", addCommentDto.PostId, currentUserId);
﻿                return Result<CommentDto>.Failure("An unexpected error occurred while adding the comment. Please try again.");
﻿            }
﻿        }﻿
﻿        public async Task<Result> EditCommentAsync(EditCommentDto editCommentDto, string currentUserId)
﻿        {
﻿            try
﻿            {
﻿                var comment = await _unitOfWork.Comments.GetByIdAsync(editCommentDto.Id);
﻿                if (comment == null)
﻿                {
﻿                    return Result.Failure("Comment not found.");
﻿                }
﻿                if (string.IsNullOrWhiteSpace(editCommentDto.Content))
﻿                {
﻿                    return Result.Failure("Comment content cannot be empty.");
﻿                }
﻿                if (currentUserId != comment.UserId)
﻿                {
﻿                    return Result.Failure("You are not authorized to edit this comment.");
﻿                }
﻿
﻿                comment.Content = editCommentDto.Content;
﻿                comment.IsEdited = true;
﻿                _unitOfWork.Comments.Update(comment);
﻿                await _unitOfWork.SaveAsync();
﻿
﻿                _logger.LogInformation("Comment {CommentId} edited by user {UserId}", editCommentDto.Id, currentUserId);
﻿                return Result.Success();
﻿            }
﻿            catch(Exception ex)
﻿            {
﻿                _logger.LogError(ex, "Error editing comment {CommentId} by user {UserId}", editCommentDto.Id, currentUserId);
﻿                return Result.Failure("An unexpected error occurred while editing the comment. Please try again.");
﻿            }
﻿        }
﻿        public async Task<Result> DeleteCommentAsync(int commentId, string currentUserId)
﻿        {
﻿            try
﻿            {
﻿                var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
﻿                if (comment == null)
﻿                    return Result.Failure("Comment not found.");
﻿
﻿                if (comment.UserId != currentUserId)
﻿                    return Result.Failure("You are not authorized to delete this comment.");
﻿
﻿                var post = await _unitOfWork.Posts.GetByIdAsync(comment.PostId);
﻿                if (post == null)
﻿                    return Result.Failure("Post not found.");
﻿
﻿                _unitOfWork.Comments.Remove(comment);
﻿                post.DecrementCommentsCount();
                await _notificationService.DeleteNotificationAsync(currentUserId, NotificationType.Comment, post!.UserId, post.Id);

                await _unitOfWork.SaveAsync();
﻿
﻿                _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, currentUserId);
﻿                return Result.Success();
﻿            }
﻿            catch (Exception ex)
﻿            {
﻿                _logger.LogError(ex, "Error deleting comment {CommentId} by user {UserId}", commentId, currentUserId);
﻿                return Result.Failure("An unexpected error occurred while deleting the comment. Please try again.");
﻿            }
﻿        }
﻿    }
﻿}
﻿
