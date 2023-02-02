using TotalFireSafety.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;


namespace TotalFireSafety.Controllers
{
    public class BaseController : Controller
    {
        readonly Dictionary dict = new Dictionary();
        readonly APIRequestHandler requestHandler = new APIRequestHandler();


        // GET: Base
        [HttpGet]
        public ActionResult Login()
        {
            if(Session["emp_no"] != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(Credential creds)
        {
            using (var _client = new HttpClient())
            {
                //  Migrate data from Credential model to LoginCredential Model for smooth transition of transaction from API
                var _creds = new LoginCredential()
                {
                    username = creds.username,
                    password = creds.password,
                    grant_type = "password"
                };

                //  API Address
                _client.BaseAddress = new Uri(requestHandler.BaseDomain);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var _result = dict.GetValues(_creds);

                //  Initiate Request
                var postTask = _client.PostAsync("/Authenticate/Login", new FormUrlEncodedContent(_result));
                postTask.Wait();

                //  GET result from API
                var result = postTask.Result;
                using (var _context = new TFSEntity())
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var tokenresponse = result.Content.ReadAsStringAsync().Result;
                        var token = JsonConvert.DeserializeObject<TokenCatcher>(tokenresponse);
                        var _user = _context.Credentials.Where(x => x.username == creds.username && x.password == creds.password).SingleOrDefault();
                        Session["access_token"] = token.access_token;
                        Session["emp_no"] = _user.emp_no;
                        return RedirectToAction("Dashboard","Admin");
                    }
                    else
                    {
                        var response = result.Content.ReadAsStringAsync().Result;
                        var respDeserialize = JsonConvert.DeserializeObject<ErrorHandler>(response);
                        ViewBag.Error = respDeserialize.error_description;
                        return View();
                    }
                }
            }
        }
    }
}