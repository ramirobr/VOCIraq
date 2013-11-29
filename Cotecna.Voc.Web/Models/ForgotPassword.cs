using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Web.Models
{
    public class ResetPasswordModel
    {
        /// <summary>
        /// Gets or sets UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets CaptchaValue
        /// </summary>
        public string CaptchaValue { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ResetPasswordModel()
        {
            UserName = string.Empty;
            CaptchaValue = string.Empty;
        }

        /// <summary>
        /// Search and retrieve the user
        /// </summary>
        /// <param name="email">The name of the user</param>
        /// <returns>UserProfile</returns>
        public Business.UserProfile GetUser(string email)
        {
            Cotecna.Voc.Business.UserProfile user = null;
            using (Business.UsersContext context = new Business.UsersContext())
            {
                user = context.UserProfiles.FirstOrDefault(x => x.UserName == email);
            }
            return user;
        }

        /// <summary>
        /// Save the temporal password in the bdd
        /// </summary>
        /// <param name="email">User name</param>
        /// <param name="temporalPassword">Temporal password</param>
        public void SaveTemporalPassword(string email,string temporalPassword)
        {
            Cotecna.Voc.Business.UserProfile user = null;
            using (Business.UsersContext context = new Business.UsersContext())
            {
                user = context.UserProfiles.FirstOrDefault(x => x.UserName == email);
                user.TemporalPassword = temporalPassword;
                context.SaveChanges();
            }
        }
    }

    public class ChangePasswordModel
    {
        /// <summary>
        /// Gets Min Lenght Password
        /// </summary>
        public int MinLenghtPassword
        {
            get
            {
                return Cotecna.Voc.Web.Properties.Settings.Default.MinLenghtPassword;
            }
        }
        /// <summary>
        /// Gets Max Lenght Password
        /// </summary>
        public int MaxLenghtPassword
        {
            get
            {
                return Cotecna.Voc.Web.Properties.Settings.Default.MaxLenghtPassword;
            }
        }

        /// <summary>
        /// Gets or sets OldPassword
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// Gets or sets NewPassword
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// Gets or sets ReNewPassword
        /// </summary>
        public string ReNewPassword { get; set; }
        /// <summary>
        /// Gets or sets UserName
        /// </summary>
        public string UserName { get; set; }
    }
}