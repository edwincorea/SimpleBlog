using System.Collections.Generic;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.ViewModels
{
    //convention for naming VM: ControllerAction
    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }
    }
}