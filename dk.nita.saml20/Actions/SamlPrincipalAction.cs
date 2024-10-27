using System.Web;
using System.Web.Security;
using dk.nita.saml20.identity;
using dk.nita.saml20.protocol;
using System.Security.Principal;
using dk.nita.saml20.Identity;
using dk.nita.saml20.Logging;
using System;

namespace dk.nita.saml20.Actions
{
    /// <summary>
    /// Sets the SamlPrincipal on the current http context
    /// </summary>
    public class SamlPrincipalAction : IAction
    {

        /// <summary>
        /// The default action name
        /// </summary>
        public const string ACTION_NAME = "SetSamlPrincipal";

        /// <summary>
        /// Action performed during login.
        /// </summary>
        /// <param name="handler">The handler initiating the call.</param>
        /// <param name="context">The current http context.</param>
        /// <param name="assertion">The saml assertion of the currently logged in user.</param>
        public void LoginAction(AbstractEndpointHandler handler, HttpContext context, Saml20Assertion assertion)
        {
            AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "BOOKMARK SamlPrincipalAction.LoginAction");
            Saml20SignonHandler signonhandler = (Saml20SignonHandler)handler;
            IPrincipal prince = Saml20Identity.InitSaml20Identity(assertion, signonhandler.RetrieveIDPConfiguration((string)context.Session[Saml20AbstractEndpointHandler.IDPTempSessionKey]));
            AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "BOOKMARK SamlPrincipalAction.LoginAction 2");
            if (prince == null)
            {
                AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "prince is null");
            }
            else {
                AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "Identity Name:" + prince.Identity?.Name);
            }
            Saml20PrincipalCache.AddPrincipal(prince);
            AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "BOOKMARK SamlPrincipalAction.LoginAction 3");
            try
            {
                FormsAuthentication.SetAuthCookie(prince.Identity.Name, false);
            }
            catch (Exception ex)
            {
                AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "EXCEPTION:" + ex.Message);
                throw (ex);
            }
            AuditLogging.logEntry(Direction.IN, Operation.LOGIN, "BOOKMARK SamlPrincipalAction.LoginAction 4");
        }

        /// <summary>
        /// Action performed during logout.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="IdPInitiated">During IdP initiated logout some actions such as redirecting should not be performed</param>
        public void LogoutAction(AbstractEndpointHandler handler, HttpContext context, bool IdPInitiated)
        {
            FormsAuthentication.SignOut();
            Saml20PrincipalCache.Clear();
        }

        private string _name;

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(_name) ? ACTION_NAME : _name;
            }
            set { _name = value; }
        }
    }
}
