using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(UserProfileMetadata))]
    public partial class UserProfile
    {

        private sealed class UserProfileMetadata
        {
            [Key]
            public int UserId { get; set; }

            [Include]
            [Association("UserProfile_UserInRole", "UserId", "UserId")]
            public IEnumerable<UserInRole> UserInRoles { get; set; }
        }
    }
}
