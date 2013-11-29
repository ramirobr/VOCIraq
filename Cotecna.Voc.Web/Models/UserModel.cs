using Cotecna.Voc.Web.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Cotecna.Voc.Web.Models;
using System.Web.Mvc;
using Cotecna.Voc.Web.Common;



namespace Cotecna.Voc.Web.Models
{
    /// <summary>
    /// Enum to sign type of user
    /// </summary>
    public enum OpenMode: int
    {
        New = 1,
        Edit = 2
    }

    public class UserListModel
    {

        /// <summary>
        /// Gets or sets results
        /// </summary>
        public PaginatedList<UserModel> SearchResult { get; set; }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        public UserListModel()
        {
            SearchResult = new PaginatedList<UserModel>();
        }

        /// <summary>
        /// Retrieve the list of users with filters and pagination
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="isExport">Is or not export to excel</param>
        public void SearchUsers(int pageNumber, string firstName, string lastName, bool isExport = false)
        {
            //Get the current index
            int currentIndex = (pageNumber - 1) * SearchResult.PageSize;
            using (Cotecna.Voc.Business.UsersContext context = new Cotecna.Voc.Business.UsersContext())
            {
                //Get the list of users
                var query = (from user in context.UserProfiles
                             join userInRole in context.UserInRoles on user.UserId equals userInRole.UserId
                             join role in context.Roles on userInRole.RoleId equals role.RoleId
                             where user.IsInternalUser == false
                             select new
                             {
                                 Email = user.UserName,
                                 user.FirstName,
                                 user.LastName,
                                 IsActive = user.IsActive,
                                 RoleName = role.RoleName
                             });

                //add first name filter
                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(data => data.FirstName.Contains(firstName));
                //add last name filter
                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(data => data.LastName.Contains(lastName));
                //get the total of itmes
                SearchResult.TotalCount = query.Count();
                //set page number
                SearchResult.Page = pageNumber;

                if (!isExport)
                {
                    foreach (var item in query.OrderBy(x => x.FirstName).Skip(currentIndex).Take(SearchResult.PageSize).ToList())
                    {
                        //build the result
                        SearchResult.Collection.Add(new UserModel
                        {
                            Email = item.Email,
                            FullName = item.FirstName + " " + item.LastName,
                            IsActive = item.IsActive.GetValueOrDefault()
                            ? Resources.Common.Yes : Resources.Common.No,
                            Role = item.RoleName
                        });
                    }
                }
                else
                {
                    foreach (var item in query.OrderBy(x => x.FirstName).ToList())
                    {
                        //build the result
                        SearchResult.Collection.Add(new UserModel
                        {
                            Email = item.Email,
                            FullName = item.FirstName + " " + item.LastName,
                            IsActive = item.IsActive.GetValueOrDefault()
                            ? Resources.Common.Yes : Resources.Common.No,
                            Role = item.RoleName
                        });
                    }
                }
                //get the number of pages
                SearchResult.NumberOfPages = (int)Math.Ceiling((double)SearchResult.TotalCount / (double)SearchResult.PageSize);

            }
        }
    }

    public class UserModel
    {
        #region Constructores
        /// <summary>
        /// Constructor 
        /// </summary>
        public UserModel()
        {

        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public UserModel(string email, string firstname, string lastname, bool isActive, string selectedrole)
        {
            Email = email;
            FirstName = firstname;
            LastName = lastname;
            Active = isActive;
            SelectedRole = selectedrole;
        } 
        #endregion

        #region Properties
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
        /// Gets or sets the Emails
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///  Gets or sets first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///  Gets or sets last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public string IsActive { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public bool Active
        {
            get
            {
                if (IsActive == null)
                {
                    return false;
                }
                else if (String.Equals(IsActive.ToUpper(), "TRUE"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    IsActive = "TRUE";
                }
                else
                {
                    IsActive = "FALSE";
                }
            }
        }

        /// <summary>
        /// Gets or sets the Role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///  Gets or sets new password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        ///  Gets or sets new password confirmation
        /// </summary>
        public string ReNewPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SelectedRole { get; set; }

        /// <summary>
        /// Gets or sets Open mode screen
        /// </summary>
        public int ScreenOpenMode { get; set; } 
        #endregion

        #region Methods

        /// <summary>
        /// Method to validate the user fields
        /// </summary>
        /// <param name="c"></param>
        public void ValidadUser(Controller c)
        {
            ValidateEmail(c);

            ValidateFirstName(c);

            ValidateLastName(c);

            ValidatePassword(c);

            ValidateRole(c);

        }

        /// <summary>
        /// Method to validate email
        /// </summary>
        /// <param name="c"></param>
        public void ValidateEmail(Controller c)
        {
            //email
            if (!string.IsNullOrEmpty(Email))
            {
                //Regex regEx = new Regex("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");
                //if (!regEx.IsMatch(Email))

                RegexUtilities util = new RegexUtilities();
                if (!util.IsValidEmail(Email))
                    c.ModelState.AddModelError("", Resources.Common.EmailFormatNotValid);
            }
            else
            {
                c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.Email));
            }
        }

        /// <summary>
        /// Method to validate First Name
        /// </summary>
        /// <param name="c"></param>
        public void ValidateFirstName(Controller c)
        {
            //first name
            if (string.IsNullOrEmpty(FirstName))
            {
                c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.FirstName));
            }
        }

        /// <summary>
        /// Method to validate Last Name
        /// </summary>
        /// <param name="c"></param>
        public void ValidateLastName(Controller c)
        {
            //Last name
            if (string.IsNullOrEmpty(LastName))
            {
                c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.LastName));
            }
        }

        /// <summary>
        /// Method to validate Password
        /// </summary>
        /// <param name="c"></param>
        public void ValidatePassword(Controller c)
        {
            ValidateNewPassword(c);

            ValidateReNewPassword(c);
        }

        /// <summary>
        /// Method to validate New Password
        /// </summary>
        /// <param name="c"></param>
        private void ValidateNewPassword(Controller c)
        {
            //new password
            if (!string.IsNullOrEmpty(NewPassword))
            {

                if (NewPassword.Length < Cotecna.Voc.Web.Properties.Settings.Default.MinLenghtPassword ||
                    NewPassword.Length > Cotecna.Voc.Web.Properties.Settings.Default.MaxLenghtPassword)
                {
                    c.ModelState.AddModelError("", Resources.Common.ShortPasswordValidation);
                }
                else
                {
                    RegexUtilities util = new RegexUtilities();
                    if (!util.IsValidCotecnaPasswordFormat(NewPassword))
                        c.ModelState.AddModelError("", Resources.Common.FormatPasswordValidation);
                   
                }
            }
            else
            {
                if (ScreenOpenMode == (int)OpenMode.New)
                {
                    c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.Password));
                }
            }
        }

        /// <summary>
        /// Method to validate new password confirmation
        /// </summary>
        /// <param name="c"></param>
        private void ValidateReNewPassword(Controller c)
        {
            //renew password
            if (!string.IsNullOrEmpty(ReNewPassword))
            {
                if (NewPassword.Length < Cotecna.Voc.Web.Properties.Settings.Default.MinLenghtPassword ||
                    NewPassword.Length > Cotecna.Voc.Web.Properties.Settings.Default.MaxLenghtPassword)
                {
                    c.ModelState.AddModelError("", Resources.Common.ShortPasswordValidation);
                }
                else
                {
                    if (!String.Equals(ReNewPassword, NewPassword))
                        c.ModelState.AddModelError("", Resources.Common.NotIqualPasswordValidation);
                }
            }
            else
            {
                if (ScreenOpenMode == (int)OpenMode.New)
                {
                    c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.ConfirmNewPassword));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void ValidateRole(Controller c)
        {
            //Role
            if (string.IsNullOrEmpty(SelectedRole))
            {
                c.ModelState.AddModelError("", string.Format(Resources.Common.SpecificMadatoryField, Resources.Common.Role));
            }
        }

        /// <summary>
        /// Retrieve the user
        /// </summary>
        /// <param name="email"></param>
        public void SearchUser(string email)
        {
            //Get the current index
            using (Cotecna.Voc.Business.UsersContext context = new Cotecna.Voc.Business.UsersContext())
            {
                //Get the list of users
                var query = (from user in context.UserProfiles
                             join userInRole in context.UserInRoles on user.UserId equals userInRole.UserId
                             join role in context.Roles on userInRole.RoleId equals role.RoleId
                             where user.UserName == email
                             select new UserModel
                             {
                                 Email = user.UserName,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 Active = user.IsActive.HasValue ? user.IsActive.Value : false,
                                 SelectedRole = role.RoleName
                             });

                UserModel us = query.SingleOrDefault();
                Charge(us);

            }
        }

        /// <summary>
        /// Method to charge properties from other UserModel
        /// </summary>
        /// <param name="user"></param>
        public void Charge(UserModel user)
        {
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.IsActive = user.IsActive;
            this.SelectedRole = user.SelectedRole;

        } 
        #endregion

    }
}