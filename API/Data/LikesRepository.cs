using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _data;
        public LikesRepository(DataContext context)
        {
            _data = context;
        }

        public async Task<UserLike> GetUserLike(int SourceUserId, int LikedUserId)
        {
            return await _data.Likes.FindAsync(SourceUserId,LikedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams parms)
        {
           var users = _data.Users.OrderBy(u=>u.UserName).AsQueryable();
           var likes = _data.Likes.AsQueryable();
           if(parms.Predicate == "liked")
           {
               likes = likes.Where(like => like.SourceUserId == parms.UserId);
               users = likes.Select(like => like.LikedUser);               
           }
           if(parms.Predicate == "likedBy")
           {
                  likes = likes.Where(like => like.LikedUserId == parms.UserId);
               users = likes.Select(like => like.SourceUser);  
           }

           var likedUsers = users.Select(user => new LikeDto
           {
             Username = user.UserName,
             KnownAs = user.KnownAs,
             Age  = user.DateOfBirth.CalculateAge(),
             PhotoUrl = user.Photos.FirstOrDefault(p=>p.IsMain).Url,
             City=user.City,
             UserID= user.Id
                      });

                      return await PagedList<LikeDto>.CreateAsync(likedUsers,parms.pageNumber,parms.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int UserId)
        {
            return await _data.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x => x.Id == UserId);
        }
    }
}