using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _user ;
        private readonly ILikesRepository _like ;

        public LikesController(IUserRepository user , ILikesRepository like)
        {
            _user = user;
            _like = like;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var SourceUserId = User.GetUserId();
            var LikedUser = await _user.GetUserByUsernameAsync(username);
            var SourceUser = await _like.GetUserWithLikes(SourceUserId);
            if(LikedUser==null)return NotFound();

            if(SourceUser.UserName == username) return BadRequest("You liked your self");
            var userlike = await _like.GetUserLike(SourceUserId,LikedUser.Id);
            if(userlike != null) return BadRequest("you already liked this user");
            userlike = new Entities.UserLike 
            {
                SourceUserId  = SourceUserId,
                LikedUserId = LikedUser.Id
            } ;
            SourceUser.LikedUsers.Add(userlike);
            if(await _user.SaveAllAsync())return Ok();

            return BadRequest("failed to like user");
        }

        [HttpGet]
public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams parms)
{
    parms.UserId = User.GetUserId();
    var users = await _like.GetUserLikes(parms);
    Response.PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
    return Ok(users);
}
    }
}