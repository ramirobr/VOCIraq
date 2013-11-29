using Cotecna.Voc.Business;
using Cotecna.Voc.Web.Common;
using Cotecna.Voc.Web.Models;
using Cotecna.Voc.Web.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Cotecna.Voc.Web.Controllers
{
    public class ForgetPasswordController : Controller
    {
        #region fields
        private ExceptionManager _exceptionManager;
        #endregion

        #region properties

        #region ExceptionManager
        /// <summary>
        /// Used to handle exceptions and register them in the log file
        /// </summary>    
        public ExceptionManager ExceptionManager
        {
            get
            {
                IUnityContainer container = new UnityContainer();
                try
                {
                    if (_exceptionManager == null)
                    {

                        container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.
                                    EnterpriseLibraryCoreExtension>();
                        return container.Resolve<ExceptionManager>();
                    }
                    else
                    {
                        return _exceptionManager;
                    }
                }
                finally
                {
                    ((IDisposable)container).Dispose();
                }

            }
            set
            {
                _exceptionManager = value;
            }
        }
        #endregion

        #endregion

        #region overriden methods

        #region OnException
        /// <summary>
        /// Management the exceptions
        /// </summary>
        /// <param name="filterContext">ExceptionContext</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //Handle the exception with Enterprise Library (configured in Web.Config)
            //Search for this in the Web.config file <exceptionPolicies> tag
            this.ExceptionManager.HandleException(filterContext.Exception, "AllExceptionsPolicy");

            //Show basic Error view
            filterContext.ExceptionHandled = true;

            //Clear any data in the model as it wont be needed
            ViewData.Model = null;

            //Show basic Error view
            View("Error").ExecuteResult(this.ControllerContext);
        }
        #endregion

        #endregion

        #region ForgetPasswordRequest
        /// <summary>
        /// Display the view to request a password recovery
        /// </summary>
        /// <returns>ActionResult</returns>
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ForgetPasswordRequest()
        {
            return View("ResetPassword");
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Perform the request password recovery
        /// </summary>
        /// <param name="model">ResetPasswordModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            //the system validate if the user name have been written
            if (string.IsNullOrEmpty(model.UserName))
                ModelState.AddModelError("UserName", Resources.Common.ForgetPasswordEmailNotValid);

            //the system get the solution of the captcha
            var solution = Session[CaptchaController.FORGOTPASSWORD_CAPTCHAKEY];
            Session.Remove(CaptchaController.FORGOTPASSWORD_CAPTCHAKEY);//only valid once, remove inmediately

            //the system validate if the captcha code have been written correctly
            if (string.Compare(solution.ToString(), model.CaptchaValue, true) != 0 || solution == null)
                ModelState.AddModelError("isHuman", Resources.Common.ForgetPasswordCaptchaNotValid);

            //if not exist errors, the system continues with the process
            if (ModelState.IsValid)
            {
                //the system gets the user information
                var user = model.GetUser(model.UserName);
                //if the user exist, the system continues with the process
                if (user != null)
                {

                    //Generate a temporal password
                    string tempPassword = System.Web.Security.Membership.GeneratePassword(12, 1);
                    //Encrypt the temporal password
                    var passwordEncrypted = EncryptionHelper.EncryptAes(tempPassword);
                    model.SaveTemporalPassword(user.UserName, passwordEncrypted);

                    //the system reads the template
                    string messageBody = System.IO.File.ReadAllText(Server.MapPath("~/Templates/PasswordResetMessageRequest.html"));

                    //the system sets the encryted url
                    DateTime now = DateTime.Now;
                    string enc = "zi=" + System.Web.HttpUtility.UrlEncode(EncryptionHelper.EncryptAes(model.UserName + "&&" + now.Year + "&&" + now.Month + "&&" + now.Day + "&&" + now.Hour + "&&" + now.Minute));

                    //the system fills the template
                    messageBody = messageBody.Replace("{TEMPORAL_PASSWORD}", tempPassword);
                    messageBody = messageBody.Replace("{INSERT ENCRYPTED}", enc);

                    //the system send an email to the user, with the necessary information for reset the password
                    EmailManagement.SendEmail(model.UserName, messageBody, Resources.Common.ForgetPasswordSubjectEmail, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);
                    //the system shows a confirmation message
                    return View("ResetPasswordConfirmation");
                }
                else
                {
                    //if the user not exist, the system will show an error message
                    ModelState.AddModelError("userNotExist", Resources.Common.ForgetPasswordUserNotFound);
                    ViewBag.HasErrors = true;
                    return View("ResetPassword", model);
                }
            }
            else
            {
                //if the validation process is not completed, the system will show all errors.
                ViewBag.HasErrors = true;
                return View("ResetPassword", model);
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Show the view ChangePassword
        /// </summary>
        /// <param name="zi">Url parameter</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ChangePassword(string zi)
        {
            DateTime now = DateTime.Now;
            DateTime sentTime;
            string userName = "";
            try
            {
                //read the parameters from url
                string dec = EncryptionHelper.DecryptAes(zi);
                string[] separator = { "&&" };
                string[] data = dec.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                //get the information
                userName = data[0];
                sentTime = new DateTime(Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), Convert.ToInt32(data[5]), 0);

                //validate if is valid
                TimeSpan t = now - sentTime;
                if (t.TotalDays > 3)
                    return View("Expired");

                if (VerifyResetPassword(sentTime, userName))
                    return View("Expired");

                //if all is ok, the system shows change password view
                ChangePasswordModel model = new ChangePasswordModel();
                model.UserName = userName;

                return View("ChangePassword", model);

            }
            catch (Exception)
            {
                return View("Unavailable");
            }
        }
        #endregion

        #region SaveChangePassword
        /// <summary>
        /// Save the new password
        /// </summary>
        /// <param name="model">ChangePasswordModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult SaveChangePassword(ChangePasswordModel model)
        {
            ValidateChangePassword(model);

            //if there are no errors, the system will continue with the process
            if (ModelState.IsValid)
            {
                //change the password.
                if (ResetPassword(model.UserName, model.ReNewPassword))
                {
                    string userEmail = model.UserName;

                    //the system reads the template
                    string messageBody = System.IO.File.ReadAllText(Server.MapPath("~/Templates/PasswordChangedConfirmationTemplate.html"));

                    //the system send an email to the user, with the necessary information for reset the password
                    EmailManagement.SendEmail(userEmail, messageBody, Resources.Common.ForgetPasswordSubjectConfirEmail, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    return View("ChangePasswordConfirmation");
                }
                else
                {
                    ModelState.AddModelError("", Resources.Common.ForgetPasswordGeneralError);
                    return View("ChangePassword", model);
                }
            }
            else
            {
                //if exist errors, the system will display the errors.
                return View("ChangePassword", model);
            }

        }

        private void ValidateChangePassword(ChangePasswordModel model)
        {

            RegexUtilities util = new RegexUtilities();

            //the system validates old password
            if (string.IsNullOrEmpty(model.OldPassword))
                ModelState.AddModelError("OldPassRequired", Resources.Common.OldPassRequired);
            if (model.OldPassword.Length < model.MinLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MinLegthPassword);
            //The system validates new password
            if (string.IsNullOrEmpty(model.NewPassword))
                ModelState.AddModelError("NewPassRequired", Resources.Common.NewPassRequired);
            if (model.NewPassword.Length < model.MinLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MinLegthPassword);
            //the system validates new password
            if (string.IsNullOrEmpty(model.ReNewPassword))
                ModelState.AddModelError("ReNewPassWordRequired", Resources.Common.ReNewPassWordRequired);
            if (model.ReNewPassword.Length < model.MinLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MinLegthPassword);
            //the system validates if the new password and the ReNewPassword are equals
            if (model.NewPassword != model.ReNewPassword)
                ModelState.AddModelError("", Resources.Common.NotIqualPasswordValidation);
            //validate the temporary password
            if (!CompareTemporalPassword(model.UserName, model.OldPassword))
                ModelState.AddModelError("", "The temporary password is not correct");
            if (!util.IsValidCotecnaPasswordFormat(model.NewPassword))
                ModelState.AddModelError("", Resources.Common.FormatPasswordValidation);
            if (!util.IsValidCotecnaPasswordFormat(model.ReNewPassword))
                ModelState.AddModelError("", Resources.Common.FormatPasswordValidation);
        }
        #endregion

        #region VerifyResetPassword
        /// <summary>
        /// Verify if is valid the current url
        /// </summary>
        /// <param name="sentTime">Change password date</param>
        /// <param name="userName">User name</param>
        /// <returns>bool</returns>
        private bool VerifyResetPassword(DateTime sentTime, string userName)
        {
            bool result = false;
            using (Business.UsersContext context = new Business.UsersContext())
            {
                var query = (from membership in context.Memberships
                             join user in context.UserProfiles on membership.UserId equals user.UserId
                             where user.UserName == userName
                             select membership).FirstOrDefault();

                TimeSpan wasChangedAlready = sentTime.ToUniversalTime() - query.PasswordChangedDate.GetValueOrDefault();
                result = wasChangedAlready.TotalDays < 0;
            }
            return result;
        }
        #endregion

        #region CompareTemporalPassword
        /// <summary>
        /// Compare the temporal password entered by the user with the one saved in the database
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="temporalPassword">Temporal password entered by the user</param>
        /// <returns>True when it is the same than the one of the database; otherwise false</returns>
        private bool CompareTemporalPassword(string userName, string temporalPassword)
        {
            Business.UserProfile currentUser = null;
            using (Business.UsersContext context=new Business.UsersContext())
            {
                currentUser = context.UserProfiles.FirstOrDefault(x => x.UserName == userName);
            }
            string tempPasswordSaved = EncryptionHelper.DecryptAes(currentUser.TemporalPassword);
            if (String.Equals(tempPasswordSaved, temporalPassword, StringComparison.Ordinal))
                return true;
            else
                return false;
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Resets the user's password using the secret answer.
        /// </summary>
        /// <param name="userName">The user's clientEmail.</param>
        /// <param name="newPassword">The new password defined by the user.</param>
        /// <returns>True is the password has been successfully reset.</returns>
        private bool ResetPassword(string userName, string newPassword)
        {
            string resetToken = WebSecurity.GeneratePasswordResetToken(userName);
            bool result = WebSecurity.ResetPassword(resetToken, newPassword);
            if (result)
            {
                using (Business.UsersContext context = new Business.UsersContext())
                {
                    var currentUser = context.UserProfiles.FirstOrDefault(x => x.UserName == userName);
                    currentUser.IsDisclaimerAccepted = false;
                    context.SaveChanges();
                }
            }
            return result;
        }
        #endregion

    }
}
