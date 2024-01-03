using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Enums
{
    public enum UserPermission
    {
        [Display(Name = "إضافة")]
        Create = 1,
        [Display(Name = "تعديل")]
        Edit = 2,
    }
}
