using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;
using ResultatPartnerASAPI.BL;
using ResultatPartnerASAPI.Data;

namespace ResultatPartnerASAPI.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class APIAuthorize : ActionFilterAttribute
    {
        public APIAuthorize()
        {
            Enable = false;
            isAdmin = false;
        }
        public bool Enable { get; set; }
        public bool isAdmin { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (Enable)
                {
                    if (AuthorizeRequest(actionContext, isAdmin))
                    {
                        return;
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
            }
            catch (HttpResponseException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(Utility.Tostring(ex.Response.ReasonPhrase)),
                    ReasonPhrase = Utility.Tostring(ex.Response.ReasonPhrase),
                };
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                string methodName = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
                LogManager.writelog(methodName + " " + Utility.Tostring(ex.Message) + " " + Utility.Tostring(ex.InnerException));
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Format(ex.Message)),
                    ReasonPhrase = Utility.Tostring(ex.InnerException),
                };
                throw new HttpResponseException(resp);
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                LogManager.writelog("Error in Action Executed");
                throw new ArgumentNullException("actionExecutedContext");
            }
            else
            {
                actionExecutedContext.Response.Headers.Remove("Access-Control-Allow-Origin");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            base.OnActionExecuted(actionExecutedContext);
        }

        private bool AuthorizeRequest(HttpActionContext actionContext, bool isAdmin)
        {
            bool isTokenOk = false;
            //Write your code here to perform authorization
            try
            {
                string Token = Utility.GetRequestToken(actionContext.Request);
                if (!string.IsNullOrEmpty(Token))
                {
                    using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                    {
                        UserToken dbToken = dbContext.UserTokens.Where(x => x.Token == Token).FirstOrDefault();
                        if (dbToken != null && dbToken.UserTokenID > 0)
                        {
                            DateTime ExpiryDate = Convert.ToDateTime(dbToken.TokenExpiryDate);
                            if (ExpiryDate < DateTime.Now)
                            {
                                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                                {
                                    Content = new StringContent(string.Format("Invalid Token")),
                                    ReasonPhrase = "Invalid Token"
                                };
                                throw new HttpResponseException(resp);
                            }
                            else
                            {
                                isTokenOk = true;
                                if (isAdmin == true && dbToken.User.UserRole.Role == "Admin") { isTokenOk = true; }
                                else if (isAdmin == true)
                                {
                                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                                    {
                                        Content = new StringContent(string.Format("Invalid User")),
                                        ReasonPhrase = "Invalid user for access"
                                    };
                                    throw new HttpResponseException(resp);
                                }
                                return isTokenOk;
                            }
                        }
                        else
                        {
                            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                                Content = new StringContent(string.Format("Invalid Token")),
                                ReasonPhrase = "Invalid Token"
                            };
                            throw new HttpResponseException(resp);
                        }
                    }
                }
                else
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(string.Format("Invalid Token")),
                        ReasonPhrase = "Invalid Token"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            catch (HttpResponseException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Format("Invalid Token")),
                    ReasonPhrase = Utility.Tostring(ex.Response.ReasonPhrase),
                };
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                string methodName = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
                LogManager.writelog(methodName + ":" + actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + actionContext.ActionDescriptor.ActionName + " " + ": No Access");
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Format(ex.Message)),
                    ReasonPhrase = Utility.Tostring(ex.InnerException),
                };
                throw new HttpResponseException(resp);
            }
        }
        protected void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            string methodName = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            LogManager.writelog(methodName + ":" + actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + actionContext.ActionDescriptor.ActionName + " " + ": No Access");

            //Code to handle unauthorized request
            //actionContext.Response.Dispose();
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Format("Token Not Found")),
                ReasonPhrase = "Pass Token in header parameter"
            };
            throw new HttpResponseException(resp);
        }

    }
}