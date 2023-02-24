using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var userToken = Session["access_token"].ToString();
            var response = api_req.BarcodeGenerator(userToken, itemCode);

            JsonResult result = Json(response, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
            Response.ContentType = "application/json"; // Set the Content-Type header
            return result;
        }

        public ActionResult FindDataOf(string requestType)
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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
            //Count for employees
            TFSEntity db = new TFSEntity();
            int dataCount = db.Employees.Count(); // Replace "Data" with your model class name
            ViewBag.DataCount = dataCount;


            int Active = db.Status.Count(u => u.emp_no == 1);
            ViewBag.active = Active;

            //count for circle
            int inventoryCount = db.Inventories.Count(); // Replace "Data" with your model class name
            ViewBag.Inventory = inventoryCount;



            //count request
            int purchase = db.Requests.Count(x => x.request_type == "Purchase");
            ViewBag.Purchase = purchase;

            int entries = db.Requests.Count(x => x.request_type == "Purchase" && x.request_status == "pending");
            ViewBag.Entries = entries;

            int entrieses = db.Requests.Count(x => x.request_type == "Deployment" && x.request_status == "pending");
            ViewBag.Entrieses = entrieses;

            int supply = db.Requests.Count(x => x.request_type == "Supply");
            ViewBag.Supply = supply;

            int deployment = db.Requests.Count(x => x.request_type == "Deployment");
            ViewBag.Deployment = deployment;


            //chart
            var products = db.Inventories.ToList();

            var data = products.Select(p => new {
                Name = p.in_name,
                Quantity = int.Parse(new string(p.in_quantity.ToString().Where(char.IsDigit).ToArray())),
                Category = p.in_class
            }).ToList();



            ViewBag.Data = data;



            return View();
        }
        [HttpPost]
        public ActionResult AddItem1(Inventory item)
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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

        public ActionResult ExportRequest()
        {
            return View();
        }

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
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Requisition([System.Web.Http.FromBody] Request[] jsonData,[System.Web.Http.FromUri] string formType)
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var userToken = Session["access_token"].ToString();
            var newData = JsonConvert.SerializeObject(jsonData);
            //var serializedModel = JsonConvert.DeserializeObject<List<Request>>(newData);
            string uri = "";
            if(formType == "add")
            {
                uri = "Requests/Add";
            }

            var response = api_req.SetMethod(uri, userToken, newData);
            var json = JsonConvert.DeserializeObject(response);

            JsonResult result = Json(json, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
            Response.ContentType = "application/json"; // Set the Content-Type header
            return result;
        }

        public ActionResult Projects()
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
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