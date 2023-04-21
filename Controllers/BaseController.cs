﻿using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using TotalFireSafety.Hubs;
using TotalFireSafety.Models;


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
            if (Session["emp_no"] != null)
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
                _client.BaseAddress = new Uri(requestHandler.BaseDomain());
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var _result = dict.GetValues(_creds);

                //  Initiate Request
                var postTask = _client.PostAsync("/Authenticate/Login", new FormUrlEncodedContent(_result));
                postTask.Wait();

                //  GET result from API
                var result = postTask.Result;
                using (var _context = new nwTFSEntity())
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var tokenresponse = result.Content.ReadAsStringAsync().Result;
                        var token = JsonConvert.DeserializeObject<TokenCatcher>(tokenresponse);
                        var _user = _context.Credentials.Where(x => x.username == creds.username && x.password == creds.password).SingleOrDefault();
                        if (_user != null)
                        {
                            var _actuser = _context.Employees.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                            var _roles = _context.Roles.Where(x => x.emp_no == _user.emp_no).SingleOrDefault();
                            Session["access_token"] = token.access_token;
                            Session["emp_no"] = _user.emp_no;
                            Session["system_role"] = _roles.role1;
                            Session["name"] = _actuser.emp_fname;
                            Session["position"] = _actuser.emp_position;
                        }
                        //var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                        //hubContext.Groups.Add("test",HttpContext.Current.Session.SessionID.ToString());
                        return RedirectToAction("Dashboard", "Admin");
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