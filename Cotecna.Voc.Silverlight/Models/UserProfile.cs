using System.Linq;

namespace Cotecna.Voc.Business
{
    public partial class UserProfile
    {
        public int RoleId {
            get { return UserInRoles.Select(x => x.RoleId).FirstOrDefault(); }
        
        }
    }
}
