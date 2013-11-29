using System.Collections.Generic;
using System.Security.Principal;

namespace Cotecna.Voc.Business
{
    public class VocUser: IPrincipal
    {
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; }
        public string Name { get; set; }
        public string OfficeName { get; set; }
        public string OfficeCode { get; set; }
        public string CountryCode { get; set; }
        public int OfficeId { get; set; }
        public int? EntryPointId { get; set; }
        public string FilePath { get; set; }
        public int PageSize { get; set; }
        public bool  HasSignature { get; set; }
        public bool IsEntryPointLo { get; set; }
        public bool IsRoUser { get; set; }
        public bool CanSync { get; set; }

        public VocUser()
        {
            Roles = new List<string>();
        }
        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool IsInRole(params UserRoleEnum[] roleNames)
        {
            bool right = false;
            foreach (var roleName in roleNames)
            {
                right = Roles.Contains(roleName.ToString());
                if (right == true)
                    break;
            }
            return right;
        }

        public IIdentity Identity
        {
            get { return new WindowsIdentity(Name); }
        }

    }

    public enum UserRoleEnum : int
    {
        Admin = 1,
        SuperAdmin = 2,
        Issuer = 3,
        Coordinator = 4,
        Client = 5,
        BorderAgent = 6,
        LOAdmin = 7,
        Supervisor = 8

    }

}
