using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TotalFireSafety.Models;

/*
 * TODO
 * -LAGYAN NG SCROLL BAR BAWAT TABLE
 * -YUNG IBA NASA CP MO CHECK MO JUST YOU
 */

namespace TotalFireSafety.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        readonly APIRequestHandler api_req = new APIRequestHandler();
        #region Others
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
        protected string GetPath(int emp_no)
        {
            using(var _context = new TFSEntity())
            {
                var user = _context.Employees.Where(x => x.emp_no == emp_no).SingleOrDefault();
                if(user.ProfilePath != null)
                {
                    return user.ProfilePath;
                }
                return "~/images/profile.png";
            }
        }
        [HttpPost]
        public ActionResult SaveImage([System.Web.Http.FromBody] HttpPostedFileBase file)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            // Check if a file was uploaded
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Store the file path in the database as a string using Entity Framework
                    using (var _context = new TFSEntity())
                    {
                        // Get the filename and extension of the uploaded file
                        string filename = Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(filename);
                        // Generate a unique filename
                        string uniqueFilename = Guid.NewGuid().ToString() + extension;
                        // Save the uploaded file to a folder on the server
                        string folderPath = "~/Uploads/Images/";
                        string serverFolderPath = HostingEnvironment.MapPath(folderPath);
                        string serverFilePath = Path.Combine(serverFolderPath, uniqueFilename);
                        file.SaveAs(serverFilePath);
                        var Id = int.Parse(empId);
                        var user = _context.Employees.Where(x => x.emp_no == Id).SingleOrDefault();
                        var image = folderPath + uniqueFilename;
                        user.ProfilePath = image;
                        _context.Entry(user);
                        _context.SaveChanges();
                        ViewBag.ProfilePath = GetPath(int.Parse(empId));
                        return Json("Success");
                        //C: \Users\Lucas Eli\Source\Repos\TotalFireSafety\Uploads\Images\
                    }
                }
                ViewBag.ProfilePath = GetPath(int.Parse(empId));
                return Json("File is Empty");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", ex);
            }
        }

        public ActionResult FindDataOf(string requestType)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            try
            {
                var userToken = Session["access_token"].ToString();
                List<Request> requisition = new List<Request>();
                List<Inv_Update> reports = new List<Inv_Update>();
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
                if (requestType == "report")
                {
                    uri = "Warehouse/Inventory/Updates";
                    response = api_req.GetAllMethod(uri, userToken);
                    reports = JsonConvert.DeserializeObject<List<Inv_Update>>(response);
                    result = Json(reports, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "deleted")
                {
                    uri = "/Warehouse/Inventory/Archive";
                    response = api_req.GetAllMethod(uri, userToken);
                    inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    result = Json(inv, JsonRequestBehavior.AllowGet);
                }

                if (requestType == "requisition")
                {
                    uri = "/Requests/All";
                    response = api_req.GetAllMethod(uri, userToken);
                    requisition = JsonConvert.DeserializeObject<List<Request>>(response);
                    result = Json(requisition, JsonRequestBehavior.AllowGet);
                }
                if (response == "BadRequest")
                {
                    //return Json("error", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("BadRequest", "Error");
                }
                if (response == "InternalServerError")
                {
                    return RedirectToAction("InternalServerError", "Error");
                    //return Json("error", JsonRequestBehavior.AllowGet);
                }
                Response.ContentType = "application/json"; // Set the Content-Type header
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        #endregion

        public ActionResult Dashboard()
        {
            var empId = Session["emp_no"]?.ToString();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            //Count for employees
            TFSEntity db = new TFSEntity();
            int dataCount = db.Employees.Count(); // Replace "Data" with your model class name
            ViewBag.DataCount = dataCount;


            int Active = db.Status.Count(u => u.IsActive == 1);
            ViewBag.active = Active;

            //count for circle
            int inventoryCount = db.Inventories.Count(); // Replace "Data" with your model class name
            ViewBag.Inventory = inventoryCount;



            //count request
            int purchase = db.Requests.Count(x => x.request_type == "Purchase");
            ViewBag.Purchase = purchase;

            int entries = db.Requests.Count(x => x.request_type == "Purchase" && x.request_status == "pending");
            ViewBag.Entries = entries;

            int entrieses = db.Requests.Count(x => x.request_type == "Deploy" && x.request_status == "pending");
            ViewBag.Entrieses = entrieses;

            int Sup = db.Requests.Count(x => x.request_type == "Supply" && x.request_status == "pending");
            ViewBag.Sup = Sup;

            int supply = db.Requests.Count(x => x.request_type == "Supply");
            ViewBag.Supply = supply;

            int deployment = db.Requests.Count(x => x.request_type == "Deploy");
            ViewBag.Deployment = deployment;


            //chart
            var products = db.Inventories.ToList();

            var data = products.Select(p => new {
                Name = p.in_name,
                Quantity = int.Parse(new string(p.in_quantity.ToString().Where(char.IsDigit).ToArray())),
                Date = DateTime.ParseExact(p.in_dateAdded, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture).Year,
                Date1 = DateTime.ParseExact(p.in_dateAdded, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture).Month,
                Class = p.in_class,
                Category = p.in_category
            }).ToList();



            ViewBag.Data = data;

            //chart 2
            var data1 = products.Select(g => new {
                Name1 = g.in_name,
                Quantity1 = int.Parse(new string(g.in_quantity.ToString().Where(char.IsDigit).ToArray())),
            }).ToList();

            ViewBag.Chart = data1;


            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        public ActionResult Projects()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        public ActionResult ProjectAdd()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        #region Inventory
        [HttpPost]
        public ActionResult RestoreItem([System.Web.Http.FromUri] string itemCode)
        {
            try
            {
                var empId = Session["emp_no"]?.ToString();
                if (empId == null)
                {
                    return RedirectToAction("Login", "Base");
                }
                var userToken = Session["access_token"]?.ToString();
                ViewBag.ProfilePath = GetPath(int.Parse(empId));
                var addCode = new Inventory()
                {
                    in_code = itemCode
                };
                var codeToFind = JsonConvert.SerializeObject(addCode);
                var uri = "/Warehouse/Inventory/Status";
                var response = api_req.SetMethod(uri, userToken, codeToFind);
                if (response == "BadRequest")
                {
                    //return Json("error", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("BadRequest", "Error");
                }
                if (response == "InternalServerError")
                {
                    return RedirectToAction("InternalServerError", "Error");
                    //return Json("error", JsonRequestBehavior.AllowGet);
                }
                var json = JsonConvert.DeserializeObject(response);
                JsonResult result = Json("Ok", JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetBarcode(string itemCode)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var userToken = Session["access_token"].ToString();
            var response = api_req.BarcodeGenerator(userToken, itemCode);
            JsonResult result = Json(response, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
            if (response == "BadRequest")
            {
                //return Json("error", JsonRequestBehavior.AllowGet);
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
            Response.ContentType = "application/json"; // Set the Content-Type header
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return result;
        }

        [HttpPost]
        public ActionResult AddItem1(Inventory item)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
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

            if (response == "BadRequest")
            {
                //return Json("error", JsonRequestBehavior.AllowGet);
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
            var json = JsonConvert.DeserializeObject(response);
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            Session["edit"] = json.ToString();
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public ActionResult DeleteItem(string item)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
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
            if (response == "BadRequest")
            {
                //return Json("error", JsonRequestBehavior.AllowGet);
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return Json("Item Deleted");
        }

        public ActionResult Inventory()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            if (Session["edit"] == null)
            {
                Session["edit"] = "pending";
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        public ActionResult InvArchive()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        public ActionResult InventoryReport()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        #endregion

        #region Users
        public ActionResult Users()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            Session["editUser"] = null;
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        [HttpPost]
        public ActionResult Users(Employee employee)
        {
            var empId = Session["emp_no"]?.ToString();
            if(empId == null)
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
            if (response == "BadRequest")
            {
                //return Json("error", JsonRequestBehavior.AllowGet);
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
            var json = JsonConvert.DeserializeObject(response);
            ViewBag.Success = json.ToString();
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            Session["editUser"] = 0;
            return View();
        }

        public ActionResult SearchEmployee()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
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
                ViewBag.ProfilePath = GetPath(int.Parse(empId));
                if (response == "BadRequest")
                {
                    //return Json("error", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("BadRequest", "Error");
                }
                if (response == "InternalServerError")
                {
                    return RedirectToAction("InternalServerError", "Error");
                    //return Json("error", JsonRequestBehavior.AllowGet);
                }
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        #endregion

        #region Requisition
        public ActionResult Requisition()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        public ActionResult ExportRequest(int? id)
        {
            var empId = Session["emp_no"].ToString();
            TFSEntity db = new TFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Requests.Where(d => d.request_type_id == id).ToList();

            return View(data);


            //var datas = db.Requests.Where(d => d.request_type_id == id).FirstOrDefault();
            //return View(datas);
        }
        [HttpPost]
        public ActionResult Requisition([System.Web.Http.FromBody] Request[] jsonData,[System.Web.Http.FromUri] string formType)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            Session["message"] = null;
            var userToken = Session["access_token"].ToString();
            var newData = JsonConvert.SerializeObject(jsonData);
            //var serializedModel = JsonConvert.DeserializeObject<List<Request>>(newData);
            string uri = "",message="";
            if(formType == "add")
            {
                message = "Request has been accepted";
                uri = "Requests/Add";
            }
            else
            {
                if(formType == "approved")
                {
                    message = "Request Approved";
                }
                else
                {
                    message = "Request Declined";
                }
                uri = "Requests/Edit";
            }
            var response = api_req.SetMethod(uri, userToken, newData);
            if (response == "BadRequest")
            {
                //return Json("error", JsonRequestBehavior.AllowGet);
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
            var json = JsonConvert.DeserializeObject(response);
            JsonResult result = Json(json, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
            Response.ContentType = "application/json"; // Set the Content-Type header
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            Session["message"] = message;
            return result;
        }
        #endregion

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Base");
        }
    }
}