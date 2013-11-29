using System.Linq;

namespace Cotecna.Voc.Business
{
    public sealed partial class VocUser
    {
        private bool? _coordinatorRightsPlusSuperior;
        private bool? _issuerRightsPlusSuperior;

        public bool IsInRole(string roleName)
        {
            return Roles.Contains(roleName);
        }

        public bool IsInRole(params UserRoleEnum[] roleNames)
        {
            bool right = false;
            foreach (var roleName in roleNames)
            {
                right=Roles.Contains(roleName.ToString());
                if (right == true)
                    break;
            }
            return right;
        }

        public bool IsCoordinatorOrSuperior
        {
            get
            {
                if (_coordinatorRightsPlusSuperior.HasValue == false)
                    _coordinatorRightsPlusSuperior = IsInRole(UserRoleEnum.Coordinator, UserRoleEnum.Issuer, UserRoleEnum.Admin, UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor);
                return _coordinatorRightsPlusSuperior.Value;
            }
        }

        public bool IsIssuerOrSuperior
        {
            get
            {
                if (_issuerRightsPlusSuperior.HasValue == false)
                    _issuerRightsPlusSuperior = IsInRole(UserRoleEnum.Issuer, UserRoleEnum.Admin, UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor);
                return _issuerRightsPlusSuperior.Value;
            }
        }

        public bool IsBorderAgentOrSuperior
        {
            get
            {
                return IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin, UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor);
            }
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
