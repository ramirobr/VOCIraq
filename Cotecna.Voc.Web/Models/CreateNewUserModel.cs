using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Web.Models
{
    public class CreateNewUserModel
    {
        /// <summary>
        ///  Gets or sets UserModel
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        ///  Gets or sets new password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        ///  Gets or sets new password confirmation
        /// </summary>
        public string ReNewPassword { get; set; }
        
    }
}