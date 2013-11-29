using Cotecna.Voc.Business;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Web;
using System.Web.Caching;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Cotecna.Voc.Silverlight.Web.Properties;
using System.IO;
using System.Web.Hosting;

namespace Cotecna.Voc.Silverlight.Web.Services
{
    /// <summary>
    /// Service for managing authentication and roles
    /// </summary>
    [EnableClientAccess]
    public class AuthenticationDomainService : DomainService
    {
        UsersContext userContext = new UsersContext();
        /// <summary>
        /// Get windows user 
        /// </summary>
        /// <returns></returns>
        [Invoke]
        public string GetWindowsUser()
        {
            string userName = HttpContext.Current.User.Identity.Name;
            if (userName.Contains("\\"))
            {
                userName = userName.Replace("\\", @"\");
            }
            return userName;
        }

        /// <summary>
        /// Perform login authentication and return the information of the user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>VocUser</returns>
        [Invoke]
        public VocUser PerformAuthentication(string userName)
        {
            VocUser user = null;
            using (UsersContext userContext = new UsersContext())
            {
                //verify if exist a user with this userName
                var currentUser = userContext.UserProfiles.FirstOrDefault(membershipUser => membershipUser.UserName == userName);
                if (currentUser != null && currentUser.IsActive == true)
                {
                    VocEntities businessContext = AuthenticationPartOne(userName, ref user, userContext, currentUser);

                    AuthenticationPartTwo(user, currentUser, businessContext);

                    if (HttpContext.Current.User == null)
                        HttpContext.Current.User = user;

                    if (HttpRuntime.Cache.Get("LoggedUser" + userName) != null)
                    {
                        HttpRuntime.Cache.Remove("LoggedUser" + userName);
                        HttpRuntime.Cache.Add("LoggedUser" + userName, user, null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                        return user;
                    }

                    if (HttpRuntime.Cache.Get("LoggedUser" + userName) == null)
                    {
                        HttpRuntime.Cache.Add("LoggedUser" + userName, user, null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                        return user;

                    }
                }
            }
            return user;
        }

        private static void AuthenticationPartTwo(VocUser user, UserProfile currentUser, VocEntities businessContext)
        {
            if (user.EntryPointId.HasValue)
            {
                var userEntryPoint = businessContext.EntryPoints.FirstOrDefault(x => x.EntryPointId == user.EntryPointId.Value);
                if (userEntryPoint != null)
                {
                    if (userEntryPoint.IsLo.HasValue)
                        user.IsEntryPointLo = userEntryPoint.IsLo.Value;
                }
            }
            if (currentUser.OfficeId.HasValue)
            {
                var userOffice = businessContext.Offices.FirstOrDefault(of => of.OfficeId == currentUser.OfficeId);

                if (userOffice != null)
                {
                    user.OfficeCode = userOffice.OfficeCode;
                    user.OfficeName = userOffice.OfficeName;
                    user.CountryCode = userOffice.CountryCode;
                    user.OfficeId = userOffice.OfficeId;
                    user.CanSync = !string.IsNullOrEmpty(userOffice.ServerName) && !string.IsNullOrEmpty(userOffice.DatabaseName);
                }
            }
            if (string.IsNullOrEmpty(user.FilePath))
            {
                user.FilePath = Properties.Settings.Default.PathDocument;
                if (!user.FilePath.EndsWith("\\"))
                    user.FilePath += "\\";
            }
        }

        private static VocEntities AuthenticationPartOne(string userName, ref VocUser user, UsersContext userContext, UserProfile currentUser)
        {
            VocEntities businessContext = new VocEntities();
            var userRoles = (from users in userContext.UserProfiles
                             join userInRole in userContext.UserInRoles on users.UserId equals userInRole.UserId
                             join roles in userContext.Roles on userInRole.RoleId equals roles.RoleId
                             where users.UserId == currentUser.UserId
                             select roles.RoleName).ToList();

            user = new VocUser
            {
                Roles = userRoles,
                Name = userName,
                DisplayName = currentUser.FirstName + " " + currentUser.LastName,
                EntryPointId = currentUser.EntryPointId,
                FilePath = currentUser.FilePath,
                HasSignature = currentUser.SignatureFile != null,
                PageSize = Convert.ToInt32(Cotecna.Voc.Silverlight.Web.Properties.Settings.Default.PageSize),
                IsEntryPointLo = false,
                IsRoUser = currentUser.IsRoUser.GetValueOrDefault()
            };
            return businessContext;
        }

        /// <summary>
        /// Get the list of roles for a user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>List of roles</returns>
        [Invoke]
        public List<string> GetRolesForUser(string userName)
        {
            List<string> userRoles = null;
            using (UsersContext userContext = new UsersContext())
            {
                userRoles = (from users in userContext.UserProfiles
                             join userInRole in userContext.UserInRoles on users.UserId equals userInRole.UserId
                             join roles in userContext.Roles on userInRole.RoleId equals roles.RoleId
                             where users.UserName == userName
                             select roles.RoleName).ToList();
            }


            return userRoles;
        }

        /// <summary>
        /// Verifiy if a user has a specific role
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="roleName">Role name</param>
        /// <returns>bool</returns>
        [Invoke]
        public bool IsUserInRole(string userName, string roleName)
        {
            bool isInRole = false;
            using (UsersContext userContext = new UsersContext())
            {
                isInRole = (from users in userContext.UserProfiles
                            join userInRole in userContext.UserInRoles on users.UserId equals userInRole.UserId
                            join roles in userContext.Roles on userInRole.RoleId equals roles.RoleId
                            where users.UserName == userName && roles.RoleName == roleName
                            select roles.RoleName).Any();
            }
            return isInRole;
        }

        /// <summary>
        /// Get signature by user 
        /// </summary>
        /// <returns></returns>
        public static byte[] GetSignatureByUser(string userName)
        {
            byte[] signature = null;
            using (UsersContext userContext = new UsersContext())
            {
                signature = userContext.UserProfiles.Where(x => x.UserName == userName).Select(x => x.SignatureFile).FirstOrDefault();
            }
            return signature;
        }



        /// <summary>
        /// Get Voc user 
        /// </summary>
        /// <returns>List of users</returns>        
        public IQueryable<UserProfile> GetUsers(string firstname, string lastname, int? officeId, int? entryPointId, int? roleId, List<string> userRoles)
        {
            userContext.Configuration.AutoDetectChangesEnabled = true;
            IQueryable<UserProfile> userQuery = userContext.UserProfiles.Include("UserInRoles");

            if (roleId != null && roleId != 0)
            {
                userQuery = userQuery
                    .Join(userContext.UserInRoles, u => u.UserId, r => r.UserId, (u, r) => new { User = u, Role = r })
                    .Where(x => x.Role.RoleId == roleId).Select(x => x.User);
            }else
                if (userRoles.Contains(UserRoleEnum.Admin.ToString()))
                {
                    userQuery = userQuery
                        .Join(userContext.UserInRoles, u => u.UserId, r => r.UserId, (u, r) => new { User = u, Role = r })
                        .Where(x => x.Role.RoleId == (int)UserRoleEnum.Admin || x.Role.RoleId == (int)UserRoleEnum.Coordinator || x.Role.RoleId == (int)UserRoleEnum.Issuer)
                        .Select(x => x.User);
                }
                else
                    if (userRoles.Contains(UserRoleEnum.LOAdmin.ToString()))
                    {
                        userQuery = userQuery
                            .Join(userContext.UserInRoles, u => u.UserId, r => r.UserId, (u, r) => new { User = u, Role = r })
                            .Where(x => x.Role.RoleId == (int)UserRoleEnum.LOAdmin || x.Role.RoleId == (int)UserRoleEnum.BorderAgent)
                            .Select(x => x.User);
                    }

            if (!string.IsNullOrEmpty(firstname))
                userQuery = userQuery.Where(x => x.FirstName.Contains(firstname));
            if (!string.IsNullOrEmpty(lastname))
                userQuery = userQuery.Where(x => x.LastName.Contains(lastname));
            if (officeId!=null && officeId!=0)
                userQuery = userQuery.Where(x => x.OfficeId == officeId);
            if (entryPointId!=null && entryPointId!=0)
                userQuery = userQuery.Where(x => x.EntryPointId == entryPointId);

            return userQuery.Where(x=> x.IsInternalUser==true);           
        }

        public UserProfile GetUserByUserId(int userId)
        {
            return userContext.UserProfiles.Find(userId);
        }

        public UserInRole GetUserInRoleByUserId(int userId)
        {
            return userContext.UserInRoles.Where(x=> x.UserId==userId).FirstOrDefault();
        }

        /// <summary>
        /// Validate if user already exist 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        [Invoke]
        public bool UserAleradyExist(int userId, string userName, int? officeId, int? entryPointId, int? roleId)
        {
            bool result=false;
            if (userId == 0)
                result = userContext.UserProfiles.Any(x => x.UserName == userName);
            else
                result = userContext.UserProfiles.Any(x => x.UserName == userName && x.UserId != userId);

            return result;
        }

        /// <summary>
        /// Get the number of rows in the query
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">Query</param>
        /// <returns>Int</returns>
        protected override int Count<T>(IQueryable<T> query)
        {
            return query.Count();
        }

        /// <summary>
        /// Get the information of the user retrieved of the active directory
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">Domain name</param>
        /// <returns>UserProfile</returns>
        [Invoke]
        public UserProfile GetUserInformationActiveDirectory(string userName,string domain)
        {
            UserProfile userInformation = null;
            using (DirectoryEntry root = new DirectoryEntry("LDAP://" + domain))
            {
                using(DirectorySearcher search = new DirectorySearcher(root))
                {
                    search.Filter = "(&(objectClass=user)(sAMAccountName=" + userName + "))";
                    //Find all users
                    
                    SearchResult result = search.FindOne();
                    if (result != null)
                    {
                        DirectoryEntry user = result.GetDirectoryEntry();
                        //verify that the account is a user
                        if (user != null && user.SchemaClassName == "user")
                        {
                            userInformation = new UserProfile
                            {
                                FirstName = GetProperty(result, "givenname"),
                                LastName = GetProperty(result, "sn"),
                                Email = GetProperty(result, "mail"),
                            };
                        }
                    }
                }
            }
            return userInformation;
        }

        /// <summary>
        /// Get the user information, searching in the Active Directory based on the email info
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Invoke]
        public UserProfile GetUserInformationByEmail(string email)
        {
            List<Domain> currentDomains = GetActiveDirectoryDomains();
            foreach (Domain domain in currentDomains)
            {
                using (DirectoryEntry root = domain.GetDirectoryEntry())
                {
                    using (DirectorySearcher search = new DirectorySearcher(root))
                    {
                        search.Filter = "(&(objectClass=user)(mail=" + email + "))";
                        //Find all users

                        SearchResult result = search.FindOne();
                        if (result != null)
                        {
                            DirectoryEntry user = result.GetDirectoryEntry();
                            //verify that the account is a user
                            if (user != null && user.SchemaClassName == "user")
                            {
                                UserProfile userInformation = new UserProfile
                                {
                                    FirstName = GetProperty(result, "givenname"),
                                    LastName = GetProperty(result, "sn"),
                                    UserName = GetProperty(result, "sAMAccountName"),
                                };
                                // concatenating the domain name to the user name (e.g. AMERICA/ecuiouser)
                                userInformation.UserName = userInformation.UserName.Insert(0, GetDomainFriendlyName(domain.Name) + @"\");

                                return userInformation;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Lists all domains on the Active Directory
        /// </summary>
        /// <returns></returns>
        protected List<Domain> GetActiveDirectoryDomains()
        {
            List<Domain> domainList = new List<Domain>();

            using (Forest forest = Forest.GetCurrentForest())
            {
                foreach (Domain domain in forest.Domains)
                {
                    domainList.Add(domain);
                }
            }

            return domainList;
        }

        /// <summary>
        /// Get the domain name without .LOC
        /// </summary>
        /// <param name="domainName">The full domain name</param>
        /// <returns></returns>
        /// <remarks>e.g. AMERICA instead of AMERICA.LOC</remarks>
        protected string GetDomainFriendlyName(string domainName)
        {
            string[] domainNodes = domainName.Split('.');
            string domainFriendlyName = domainNodes[0];

            return domainFriendlyName;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static string GetProperty(SearchResult searchResult, string propertyName)
        {
            //return data from properties if exist else string empty
            return searchResult.Properties.Contains(propertyName) ? searchResult.Properties[propertyName][0].ToString() : string.Empty;
        }

        #region Error handling
        private ExceptionManager exceptionManager;
        private LogWriter logWriter;
        /// <summary>
        /// Used to handle exceptions and register them in the log file
        /// </summary>    
        public ExceptionManager ExceptionManager
        {
            get
            {
                if (exceptionManager == null)
                {
                    IUnityContainer container = new UnityContainer();
                    container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.EnterpriseLibraryCoreExtension>();
                    return container.Resolve<ExceptionManager>();
                }
                else
                    return exceptionManager;
            }
            set
            {
                exceptionManager = value;
            }
        }

        /// <summary>
        /// Used to log entries in the log file
        /// </summary>
        public LogWriter LogWriter
        {
            get
            {
                if (logWriter == null)
                {
                    IUnityContainer container = new UnityContainer();
                    container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.EnterpriseLibraryCoreExtension>();
                    return container.Resolve<LogWriter>();
                }
                else
                    return logWriter;
            }
            set
            {
                logWriter = value;
            }
        }

        /// <summary>
        /// Writes a message in the Log file
        /// </summary>
        /// <param name="message"></param>
        protected void WriteLog(string message)
        {
            //In Web.config there is a listener "Monitor Rolling Flat File Trace Listener"
            LogWriter.Write(message, "Monitor");
        }



        protected override void OnError(DomainServiceErrorInfo errorInfo)
        {
            //Do log and error handling
            //Handle the exception with Enterprise Library (configured in Web.Config)
            //Search for this in the Web.config file <exceptionPolicies> tag

            if (!System.Diagnostics.Debugger.IsAttached)
                this.ExceptionManager.HandleException(errorInfo.Error, "AllExceptionsPolicy");

            base.OnError(errorInfo);
        }

        #endregion

        #region Persistance


        #region UserProfile Persistance
        [Delete]
        public void DeleteUserProfile(UserProfile userProfile)
        {
            throw new NotImplementedException("Not supported");
        }

        [Insert]
        public void InsertUserProfile(UserProfile userProfile)
        {
            Insert(userProfile);
        }

        [Update]
        public void UpdateUserProfile(UserProfile userProfile)
        {
            Update(userProfile);
        }

        #endregion

        #region UserInRole Persistance
        [Delete]
        public void DeleteUserInRole(UserInRole userInRole)
        {
            Delete(userInRole);
        }

        [Insert]
        public void InsertUserInRole(UserInRole userInRole)
        {
            if (userInRole.UserId != 0)
                Insert(userInRole);
        }

        [Update]
        public void UpdateUserInRole(UserInRole userInRole)
        {
            Update(userInRole);
        }

        #endregion


        /// <summary>
        /// Generic Insert Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            userContext.Set<TEntity>().Add(entity);
            userContext.SaveChanges();
        }

        /// <summary>
        /// Generic Update Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected void Update<TEntity>(TEntity entity) where TEntity : class
        {
            userContext.Set<TEntity>().Attach(entity);
            userContext.Entry(entity).State = EntityState.Modified;
            userContext.SaveChanges();
        }

        /// <summary>
        /// Generic Delete Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            userContext.Entry(entity).State = EntityState.Deleted;
            userContext.SaveChanges();
        }


        #endregion

        #region Export to Excel
        /// <summary>
        /// Method to export a file excel with users with the current filters
        /// </summary>
        [Invoke]
        public string ExportUsersToExcel(string firstname, string lastname, int? officeId, int? entryPointId, int? roleId, List<string> userRoles)
        {
            List<UserProfile> usersList = GetUsers(firstname, lastname, officeId, entryPointId, roleId, userRoles).OrderBy(x => x.UserId).ToList();

            MemoryStream report = ExcelManagement.GenerateReport<UserProfile>(usersList, "Users", typeof(UserProfile), HostingEnvironment.MapPath("~/Images/Logo-Voc-Iraq.png"));
            report.Position = 0;

            string userName = HttpContext.Current.User.Identity.Name;
            userName = userName.Replace("\\", "");
            userName += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            userName = userName.Replace("/", "_");
            userName = userName.Replace(":", "_");
            string NameDocument = "Users" + userName + ".xlsx";
            string folder = GetSourcePathExcelFile();
            string fullPath = folder + @"\" + NameDocument ;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            File.WriteAllBytes(fullPath, report.ToArray());

            return NameDocument;

            
        }

        /// <summary>
        /// Gets the string path to save the exported excel files
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="office"></param>
        /// <returns></returns>
        public static string GetSourcePathExcelFile()
        {
            return Path.Combine(Properties.Settings.Default.PathDocument, Properties.Settings.Default.ExcelFolder).Replace("/", "\\");
        }
        #endregion



        
    }    
}