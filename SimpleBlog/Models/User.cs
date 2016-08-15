using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    public class User
    {
        private const int WorkFactor = 13;

        //Avoid timing attacks hashing a blank password when user/password submitted are incorrect
        public static void FakeHash()
        {
            BCrypt.Net.BCrypt.HashPassword(string.Empty, WorkFactor);
        }

        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string PasswordHash { get; set; }

        public virtual IList<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }

        //Bcrypt:
        //- Slow hashing.
        //- Inmune to GPUs.
        //- Future proof.
        //- Salt built in.
        public virtual void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }

    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("users");

            Id(x => x.Id, x => x.Generator(Generators.Identity));

            Property(x => x.Username, x => x.NotNullable(true));
            Property(x => x.Email, x => x.NotNullable(true));
            Property(x => x.PasswordHash, x => 
            {
                x.Column("password_hash"); //override name of the column
                x.NotNullable(true);
            });

            //In Hibernate, a bag is a collection. Hibernate has bags, sets and lists.
            Bag(x => x.Roles, x => 
            {
                x.Table("role_users");
                x.Key(k => k.Column("user_id"));
            }, x => x.ManyToMany(k => k.Column("role_id")));
        }
    }
}