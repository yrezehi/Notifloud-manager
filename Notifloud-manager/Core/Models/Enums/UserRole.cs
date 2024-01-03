using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Enums
{
    public enum UserRole
    {
        [Display(Name = "مالك النظام")]
        SuperAdmin = 1,
        [Display(Name = "مدير النظام")]
        Admin = 2,
        [Display(Name = "مستخدم")]
        User = 3,
    }
}
