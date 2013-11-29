using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Cotecna.Voc.Web.Filters;
using Cotecna.Voc.Web.Models;
using System.Text.RegularExpressions;
using Cotecna.Voc.Business;
using Cotecna.Voc.Web.Properties;
using Cotecna.Voc.Web.Common;
using System.IO;

namespace Cotecna.Voc.Web.Controllers
{
    public class AccountController : BaseController
    {
        /// <summary>
        /// Show the index view
        /// </summary>
        /// <returns>ActionResult</returns>
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Index()
        {
            UserListModel model = new UserListModel();
            model.SearchUsers(1, string.Empty, string.Empty);
            return View(model);
        }

        /// <summary>
        /// Show the change password view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            return View(model);
        }

        /// <summary>
        /// Perform change password operation
        /// </summary>
        /// <param name="model">ChangePasswordModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult SaveChangePassword(ChangePasswordModel model)
        {
            ValidateChangePassword(model);

            if (ModelState.IsValid)
            {
                bool result = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                if (result)
                {
                    string userEmail = User.Identity.Name;
                    //the system reads the template
                    string messageBody = System.IO.File.ReadAllText(Server.MapPath("~/Templates/PasswordChangedConfirmationTemplate.html"));

                    //the system send an email to the user, with the necessary information for reset the password
                    EmailManagement.SendEmail(userEmail, messageBody, Resources.Common.ForgetPasswordSubjectConfirEmail, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    return RedirectToAction("Index", "Certificate");
                }
                else
                {
                    ModelState.AddModelError("", Resources.Common.ForgetPasswordGeneralError);
                    return View("ChangePassword", model);
                }
            }
            else
            {
                return View(model);
            }
        }

        /// <summary>
        /// Validate change password
        /// </summary>
        /// <param name="model"></param>
        private void ValidateChangePassword(ChangePasswordModel model)
        {
            RegexUtilities util = new RegexUtilities();
            //the system validates old password
            if (string.IsNullOrEmpty(model.OldPassword))
                ModelState.AddModelError("OldPassRequired", Resources.Common.OldPassRequired);

            //The system validates new password
            if (string.IsNullOrEmpty(model.NewPassword))
                ModelState.AddModelError("NewPassRequired", Resources.Common.NewPassRequired);
            if (model.NewPassword.Length < model.MinLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MinLegthPassword);
            if (model.NewPassword.Length > model.MaxLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MaxLegthPassword);
            if (!util.IsValidCotecnaPasswordFormat(model.NewPassword))
                ModelState.AddModelError("", Resources.Common.FormatPasswordValidation);


            //the system validates new password
            if (string.IsNullOrEmpty(model.ReNewPassword))
                ModelState.AddModelError("ReNewPassWordRequired", Resources.Common.ReNewPassWordRequired);
            if (model.ReNewPassword.Length < model.MinLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MinLegthPassword);
            if (model.ReNewPassword.Length > model.MaxLenghtPassword)
                ModelState.AddModelError("", Resources.Common.MaxLegthPassword);
            if (!util.IsValidCotecnaPasswordFormat(model.ReNewPassword))
                ModelState.AddModelError("", Resources.Common.FormatPasswordValidation);

            //the system validates if the new password and the ReNewPassword are equals
            if (model.NewPassword != model.ReNewPassword)
                ModelState.AddModelError("", Resources.Common.NotIqualPasswordValidation);
        }

        /// <summary>
        /// Get the list of users with filter and pagination
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <returns>PartialViewResult</returns>
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public PartialViewResult SearchUsers(int pageNumber, string firstName, string lastName)
        {
            UserListModel model = new UserListModel();
            model.SearchUsers(pageNumber,firstName,lastName);
            return PartialView("_UserList", model.SearchResult);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult CreateUser()
        {
            UserModel model = new UserModel();
            model.ScreenOpenMode = (int) OpenMode.New;
            return View("User", model);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult EditUser(string userEmail)
        {
            UserModel model = new UserModel();
            model.SearchUser(userEmail);
            model.ScreenOpenMode = (int) OpenMode.Edit;
            return View("User", model);
        }


        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult SaveNewUser(UserModel user)
        {
            try
            {
                user.ValidadUser(this);

                //Validate user using Simple membership
                if (ModelState.IsValid)
                {
                    if (user.ScreenOpenMode == (int)OpenMode.New)
                    {
                    string answer = WebSecurity.CreateUserAndAccount(user.Email, user.NewPassword);

                    if (string.IsNullOrEmpty(answer))
                    {
                        Roles.AddUserToRole(user.Email, user.SelectedRole);


                        using (Cotecna.Voc.Business.UsersContext context = new Cotecna.Voc.Business.UsersContext())
                        {
                            Cotecna.Voc.Business.UserProfile currentUser = context.UserProfiles.FirstOrDefault(x => x.UserName == user.Email);
                            currentUser.FirstName = user.FirstName;
                            currentUser.LastName = user.LastName;
                            currentUser.IsActive = user.Active;
                            currentUser.IsInternalUser = false;
                            context.SaveChanges();
                        }
                    }
                    }
                    else
                    {
                        string[] roles = Roles.GetRolesForUser(user.Email);
                        Roles.RemoveUserFromRoles(user.Email, roles);
                        Roles.AddUserToRole(user.Email, user.SelectedRole);

                        //Changing password
                        if (!string.IsNullOrEmpty(user.NewPassword))
                        {
                            var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;
                            var us = membership.GetUser(user.Email, true);
                            string temppassword = us.ResetPassword();
                            WebSecurity.ChangePassword(user.Email, temppassword, user.NewPassword);
                        }

                        //Editing parameters
                        using (Cotecna.Voc.Business.UsersContext context = new Cotecna.Voc.Business.UsersContext())
                        {
                            Cotecna.Voc.Business.UserProfile currentUser = context.UserProfiles.FirstOrDefault(x => x.UserName == user.Email);
                            currentUser.FirstName = user.FirstName;
                            currentUser.LastName = user.LastName;
                            currentUser.IsActive = user.Active;
                            context.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index", "Account");
                }                
                
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }

            return View("User", user);
            
           
        }


        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public void ExportToExcel(string firstName, string lastName)
        {
            UserListModel model = new UserListModel();
            model.SearchUsers(1, firstName, lastName, true);
            MemoryStream report = ExcelManagement.GenerateUserReport(model.SearchResult.Collection, Server.MapPath("~/Images/Logo-Voc-Iraq.png"));
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "UserReport" + currentDateTime + ".xlsx");
            Session.Add("UserReport", result);
        }

        /// <summary>
        /// Download the report
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public FileStreamResult DownloadExcelReport()
        {
            MicrosoftExcelStreamResult result = Session["UserReport"] as MicrosoftExcelStreamResult;
            Session.Remove("UserReport");
            return result;
        }
        
        #region Helpers

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
