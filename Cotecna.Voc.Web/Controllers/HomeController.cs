using Cotecna.Voc.Web.Filters;
using Cotecna.Voc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;
using System.Configuration;
using System.Web.Security;

namespace Cotecna.Voc.Web.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class HomeController : Controller
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

        /// <summary>
        /// Display the login page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public PartialViewResult GetDisclaimer()
        {
            return PartialView("_Disclaimer");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Index(LoginModel model, string returnUrl)
        {
            //Validate the user name is an valid email
            if (!string.IsNullOrEmpty(model.UserName))
            {
                Regex regEx = new Regex("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");
                if (!regEx.IsMatch(model.UserName))
                    ModelState.AddModelError("", Resources.Common.EmailFormatNotValid);
            }
            //Validate user using Simple membership
            if (ModelState.IsValid)
            {
                var roles = (SimpleRoleProvider)Roles.Provider;
                    
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe) && (roles.GetRolesForUser(model.UserName).Contains("SuperAdmin") || roles.GetRolesForUser(model.UserName).Contains("Client")))
                {
                    using (UsersContext db = new UsersContext())
                    {
                        UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                        if (!user.IsActive)
                        {
                            ModelState.AddModelError("", Resources.Common.UserInactive);
                        }
                        else
                        {
                            if (user.IsDisclaimerAccepted.GetValueOrDefault())
                            {
                                Session.Add("persistencia", true);
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                ViewBag.IsDisclaimerAccepted = user.IsDisclaimerAccepted.GetValueOrDefault();
                                return View(model);
                            }
                        }
                    }                    
                }
                else
                {
                    // If we got this far, something failed, redisplay form
                    ModelState.AddModelError("", Resources.Common.WrongLogin);
                }
            }
            return View(model);            
        }

        public void AcceptDisclaimer()
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == User.Identity.Name);
                user.IsDisclaimerAccepted = true;
                db.SaveChanges();
            }
        }
        public void LogoutDisclaimer()
        {
            WebSecurity.Logout();
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Redirect to a specific url
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Certificate");
            }
        }

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
    }
}
