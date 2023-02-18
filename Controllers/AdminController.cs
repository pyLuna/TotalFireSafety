using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TotalFireSafety.Models;


namespace TotalFireSafety.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        readonly APIRequestHandler api_req = new APIRequestHandler();
        [HttpPost]
        public ActionResult GetBarcode(string itemCode)
        {
            var userToken = Session["access_token"].ToString();
            var response = api_req.BarcodeGenerator(userToken, itemCode);

            JsonResult result = Json(response, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
            Response.ContentType = "application/json"; // Set the Content-Type header
            return result;
        }

        public ActionResult FindDataOf(string requestType)
        {
            try
            {
                var userToken = Session["access_token"].ToString();
                List<Request> requisition = new List<Request>();
                List<Inventory> inv = new List<Inventory>();
                string uri = "", response = "";
                JsonResult result = null;
                if (requestType == "inventory")
                {
                    uri = "/Warehouse/Inventory";
                    response = api_req.GetAllMethod(uri, userToken);
                    inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    result = Json(inv, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "requisition") {
                    uri = "/Requests/All";
                    response = api_req.GetAllMethod(uri, userToken);
                    requisition = JsonConvert.DeserializeObject<List<Request>>(response);
                    result = Json(requisition, JsonRequestBehavior.AllowGet);
                }

                Response.ContentType = "application/json"; // Set the Content-Type header
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public ActionResult Dashboard()
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddItem1(Inventory item)
        {
            var serializedModel = JsonConvert.SerializeObject(item);
            var userToken = Session["access_token"].ToString();
            string uri;

            if( item.formType == "add")
            {
                uri = "Warehouse/Inventory/Add";
            }
            else
            {
                uri = "Warehouse/Inventory/Edit";
            }

            var response = api_req.SetMethod(uri, userToken, serializedModel);

            if (response == "BadRequest" || response == "InternalServerError")
            {
                ViewBag.Added = response.ToString();
                return RedirectToAction("Inventory");
            }
            var json = JsonConvert.DeserializeObject(response);
            Session["edit"] = json.ToString();
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public ActionResult DeleteItem(string item)
        {
            var addCode = new Inventory()
            {
                in_code = item
            };
            var serializedModel = JsonConvert.SerializeObject(addCode);
            var userToken = Session["access_token"].ToString();

            var response = api_req.SetMethod("Warehouse/Inventory/Delete", userToken, serializedModel);

            if (response == "BadRequest" || response == "InternalServerError")
            {
                return Json("error");
            }
            return Json("Item Deleted");
        }

        //  Inventory
        public ActionResult Inventory()
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            if (Session["edit"] == null)
            {
                Session["edit"] = "pending";
            }
            return View();
        }

        //  Users
        public ActionResult Users()
        {
            Session["editUser"] = null;
            return View();
        }
       
        //private Tuple<string,string> setUri(int isEdit)
        //{
        //    string uri,message;
        //    if(isEdit == 1)
        //    {
        //        uri = "Admin/Employee/Update";
        //        message = "Updated!";
        //    }
        //    else
        //    {
        //        uri = "Admin/Employee/Add";
        //        message = "Added!";
        //    }
        //    return new Tuple<string, string> (uri,message);
        //}

        [HttpPost]
        public ActionResult Users(Employee employee)
        {
            if(Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }

            var serializedModel = JsonConvert.SerializeObject(employee);
            var userToken = Session["access_token"].ToString();
            string uri = "", message = "";
            if (employee.formType == "add")
            {
                uri = "Admin/Employee/Add";
                message = "Added!";
            }
            if (employee.formType == "edit")
            {
                uri = "Admin/Employee/Update";
                message = "Updated!";
            }
            var response = api_req.SetMethod(uri, userToken, serializedModel);
            ViewBag.Message = message;
            if (response == "BadRequest" || response == "InternalServerError")
            {
                ViewBag.Response = response.ToString();
                return View();
            }
            var json = JsonConvert.DeserializeObject(response);
            ViewBag.Success = json.ToString();
            Session["editUser"] = 0;
            return View();
        }

        public ActionResult SearchEmployee()
        {
            try
            {
                var userToken = Session["access_token"].ToString();
                var response = api_req.GetAllMethod("/Admin/Employee", userToken);

                var json = JsonConvert.DeserializeObject<List<Employee>>(response);

                JsonResult result = Json(json, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
                Response.ContentType = "application/json"; // Set the Content-Type header
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        
        public ActionResult Requisition()
        {
            return View();
        }

        public ActionResult Projects()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Logout()
        {

            // DONT FORGET TO CLEAR SESSIONS, TOKENS AND OTHERS
            Session.Clear();
            return RedirectToAction("Login", "Base");
        }

    }
}