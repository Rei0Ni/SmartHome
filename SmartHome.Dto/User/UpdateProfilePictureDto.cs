using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartHome.Dto.User
{
    public class UpdateProfilePictureDto
    {
        [Required(ErrorMessage = "The Profile Picture Is Required")]
        public IFormFile ProfilePicture { get; set; }
    }
}
