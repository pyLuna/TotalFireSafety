using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TotalFireSafety.Models;

namespace TotalFireSafety
{
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        private readonly nwTFSEntity _context = new nwTFSEntity();

        //User Validation
        #region Roles and Status
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
                case 4:
                    return "owner";
            }
            return "";
        }
        #endregion

        private Credential Hash(string uname, string pword)
        {
            using (var context = new nwTFSEntity())
            {
                var md5 = MD5.Create();
                var users = context.Credentials.Select(x => x).ToList();
                var userResult = new Credential();

                byte[] inputBytes = Encoding.UTF8.GetBytes(pword);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                foreach (var user in users)
                {
                    byte[] userBytes = Encoding.UTF8.GetBytes(user.password);
                    byte[] resultByte = md5.ComputeHash(userBytes);

                    if (hashBytes.SequenceEqual(resultByte))
                    {
                        userResult = new Credential()
                        {
                            username = user.username,
                            password = user.password,
                            emp_no = user.emp_no
                        };
                    }
                }
                return userResult;
            }
        }

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
            using (var db = new nwTFSEntity()) //init database connection
            {
                try
                {
                    //find user
                    var _user = Hash(context.UserName, context.Password);
                    //verify role
                    if (_user.username != context.UserName)
                    {
                        context.SetError("invalid_grant", "Username or Password is incorrect");
                        return;
                    }
                    //find roles
                    var _role = db.Roles.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                    //find status
                    var _status = db.Status.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                    string authorization = VerifyRole(_role.role1);
                    if (!IsActive(_status.IsActive))
                    {
                        context.SetError("invalid_grant", "Inactive Account. Contact Admin");
                        return;
                    }

                    if (IsLocked(_status.IsLocked))
                    {
                        context.SetError("invalid_grant", "Locked Account. Contact Admin");
                        return;
                    }
                    identity.AddClaim(new Claim(ClaimTypes.Role, authorization));
                    identity.AddClaim(new Claim("username", _user.username));
                    identity.AddClaim(new Claim(ClaimTypes.Name, authorization));
                    context.Validated(identity);
                    return;
                }
                catch (Exception ex)
                {
                    context.SetError("invalid_grant", ex.ToString());
                    return;
                }
            }
        }
    }
}