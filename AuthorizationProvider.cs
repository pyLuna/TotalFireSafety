using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TotalFireSafety.Models;

namespace TotalFireSafety
{
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        //User Validation
        #region
        private bool IsLocked(int value)
        {
            switch (value)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    return false;
            }
        }
        private bool IsActive(int value)
        {
            switch (value)
            {
                case 1:
                    return true;
                case 0:
                    return false;
                default:
                    return false;
            }
        }
        private string VerifyRole(int _id)
        {
            switch (_id)
            {
                case 1:
                    return "admin";
                case 2:
                    return "warehouse";
                case 3:
                    return "office";
            }
            return "";
        }
        #endregion
        //validate Authentication
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        //Verify user and grant access --- token creation on this process
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //set token as "bearer"
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            using (var _context = new TFSEntity()) //init database connection
            {
                //find user
                var _user = _context.Credentials.Where(x => x.username.Equals(context.UserName,System.StringComparison.CurrentCulture) && x.password.Equals(context.Password, System.StringComparison.CurrentCulture)).SingleOrDefault();
                //var _user = _context.Credentials.Where(x => string.Equals(x.username,context.UserName, System.StringComparison.CurrentCulture) && string.Equals(x.password, context.Password, System.StringComparison.CurrentCulture)).FirstOrDefault();
                //verify role
                if (_user != null)
                {
                    //find roles
                    //var _role = _context.Roles.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                    var _role = _context.Roles.Where(x => x.emp_no.Equals(_user.emp_no)).SingleOrDefault();
                    //find status
                    var _status = _context.Status.Where(x => x.emp_no.Equals(_user.emp_no)).SingleOrDefault();
                    //var _status = _context.Status.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                    string authorization = VerifyRole(_role.role1);
                    if (IsActive(_status.IsActive))
                    {
                        if (!IsLocked(_status.IsLocked))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, authorization));
                            identity.AddClaim(new Claim("username", _user.username));
                            identity.AddClaim(new Claim(ClaimTypes.Name, authorization));
                            context.Validated(identity);
                            return;
                        }
                        context.SetError("invalid_grant", "Locked Account. Contact Admin");
                        return;
                    }
                    context.SetError("invalid_grant", "Inactive Account. Contact Admin");
                    return;
                }
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }

        }
    }
}