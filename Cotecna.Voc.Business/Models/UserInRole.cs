
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(UserInRoleMetadata))]
    public partial class UserInRole
    {

        private sealed class UserInRoleMetadata
        {
            [Key]
            public int RoleId { get; set; }

            [Key]
            public int UserId { get; set; }

            [Include]
            [Association("UserInRole_UserProfile", "UserId", "UserId")]
            public UserProfile UserProfile { get; set; }

        }
    }
}
