using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.ViewModels
{
    public class RoleCheckbox
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public string Name { get; set; }
    }

    //convention for naming VM: ControllerAction
    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }
    }

    public class UsersNew
    {
        [Required, MaxLength(128)]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public IList<RoleCheckbox> Roles { get; set; }
    }

    public class UsersEdit
    {
        [Required, MaxLength(128)]
        public string Username { get; set; }
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public IList<RoleCheckbox> Roles { get; set; }
    }

    public class UsersResetPassword
    {
        //we don't need data annotations for username. It's only used for being presented to the view, 
        //not to communicate from the view to the controller.
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }    
}