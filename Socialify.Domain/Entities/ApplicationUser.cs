using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(200)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(200)]
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Gender { get; set; }
        public string? Bio { get; set; }
        public string ProfilePicUrl { get; set; } = "images/profilePics/default-profile-pic.jpg";
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string FullName { get; private set; } = string.Empty;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
    }
}
