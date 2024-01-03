using Core.Models.Abstracts.Interfaces;
using Core.Models.Enums;
using Core.Utils.Extenstions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("users")]
    public class User : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("role")]
        public int Role { get; set; } = UserRole.User.GetId();

        public User() { }

        public User(string email, UserRole role) =>
            (Username, Email, Id) = (StripUsername(email), email, role.GetId());

        public User WithId(int id)
        {
            Id = id;
            return this;
        }

        public static User Create(string email, UserRole role) =>
            new User(email, role);

        private static string StripUsername(string email) =>
            email.Contains('@') ? email.Split('@')[0] : email;
    }
}
