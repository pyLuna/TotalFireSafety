using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TotalFireSafety.Hubs;
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

        private async Task SendNotif(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            await hubContext.Clients.All.SendAsync(message);
        }
        private async Task GroupNotif(string group,string message)
        {
            //var act = Session["system_role"].ToString();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            await hubContext.Clients.Group(group).SendAsyncGroup(message);
        }
        private async Task SendMessage(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            await hubContext.Clients.All.receiveMessage(message);
        }
        // GET: Admin
        readonly APIRequestHandler api_req = new APIRequestHandler();
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
        #region Others
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
        public async Task<ActionResult> InvReportPrint()
        {
            return View();
        }
        public async Task<ActionResult> InvReportExport(int? id)
        {
            
            TFSEntity db = new TFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var empId = Session["emp_no"].ToString();
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Employees.Where(d => d.emp_no == id).ToList();
            ViewBag.Name = data.FirstOrDefault()?.emp_name;
            ViewBag.Position = data.FirstOrDefault()?.emp_position;
            ViewBag.Id = data.FirstOrDefault()?.emp_no;

            return View(data);
        }
        public async Task<ActionResult> InvReorder()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SaveImage([System.Web.Http.FromBody] HttpPostedFileBase file)
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
        [HttpGet]
        public async Task<ActionResult> FindDataOf(string requestType)
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
                    response = await api_req.GetAllMethod(uri, userToken);
                    inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    result = Json(inv, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "report")
                {
                    uri = "Warehouse/Inventory/Updates";
                    response = await api_req.GetAllMethod(uri, userToken);
                    reports = JsonConvert.DeserializeObject<List<Inv_Update>>(response);
                    result = Json(reports, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "deleted")
                {
                    uri = "/Warehouse/Inventory/Archive";
                    response = await api_req.GetAllMethod(uri, userToken);
                    inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    result = Json(inv, JsonRequestBehavior.AllowGet);
                }

                if (requestType == "requisition")
                {
                    uri = "/Requests/All";
                    response = await api_req.GetAllMethod(uri, userToken);
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
        public async Task<ActionResult> ProjectView()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        public async Task<ActionResult> ProjectAdd()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }


        private Timer updateTimer;

        public AdminController()
        {
            // Schedule the update to run once per day
            var updateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            var dueTime = updateTime - DateTime.Now;
            if (dueTime < TimeSpan.Zero)
            {
                dueTime = TimeSpan.Zero;
            }

            // Create and start the timer
            this.updateTimer = new Timer(async state =>
            {
                await this.Dashboard();
            }, null, dueTime, TimeSpan.FromDays(1));
        }

        public async Task<ActionResult> Dashboard()
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

            int stats = db.Inventories.Count(x => x.in_status == "Active" && x.in_status == "Active");
            ViewBag.archive = stats;



            //count request
            int purchase = db.Requests.Count(x => x.request_type == "Purchase");
            ViewBag.Purchase = purchase;

            int entries = db.Requests.Count(x => x.request_type == "Purchase" && x.request_status == "Pending");
            ViewBag.Entries = entries;

            int entrieses = db.Requests.Count(x => x.request_type == "Deployment" && x.request_status == "Pending");
            ViewBag.Entrieses = entrieses;

            int Sup = db.Requests.Count(x => x.request_type == "Supply" && x.request_status == "Pending");
            ViewBag.Sup = Sup;

            int supply = db.Requests.Count(x => x.request_type == "Supply");
            ViewBag.Supply = supply;

            int deployment = db.Requests.Count(x => x.request_type == "Deployment");
            ViewBag.Deployment = deployment;

    
            //chart
            var products = db.Inv_Update.ToList();
            var update = db.Basecounts.ToList();
            var Request = db.Requests.ToList();

            /* List<Inventory> newList = new List<Inventory>();
             foreach(var item in products)
             {
                 var check = update.Where(x => x.update_item_id == item.in_code).ToList();
                 Inventory newInv = new Inventory()
                 {

                     in_category = item.in_category,
                     in_class = item.in_class,
                     in_code = item.in_code,
                     in_dateAdded = item.in_dateAdded,
                     in_name = item.in_name,
                     in_quantity = item.in_quantity,
                     in_size = item.in_size,
                     in_type = item.in_type,
                     Inv_Update = check*//* == null ? check : null*//*
                 };
                 newList.Add(newInv);
             }*/

            /* .SelectMany(p => p.Inv_Update.DefaultIfEmpty(), (p, u) => new {
                 Name = p.in_name,
                 Quantity = (u == null ? 0 : int.Parse(new string(u.update_quantity.ToString().Where(char.IsDigit).ToArray()))) +
                  (p == null ? 0 : int.Parse(new string(p.in_quantity.ToString().Where(char.IsDigit).ToArray()))),
                 Class = p.in_class,
                 Category = p.in_category
             })
             .GroupBy(d => d.Name)
             .Select(g => new {
                 Name = g.Key,
                 Quantity = g.Sum(x => x.Quantity),
                 Class = g.FirstOrDefault()?.Class,
                 Category = g.FirstOrDefault()?.Category
             })
             .ToList();*/
            var data = update
     .GroupBy(g => new { g.Inventory.in_code, g.Inventory.in_name, g.Inventory.in_size })
     .Select(g => new {
         Name = g.Key.in_name,
         Quantity = g == null ? 0 : int.Parse(new string(g.First().bc_count.ToString().Where(char.IsDigit).ToArray())),
         Size = g.First().Inventory.in_size,
         Category = g.First().Inventory.in_category,
         Class = g.First().Inventory.in_class,
         Code = g.Select(x => x.Inventory.in_code).ToList(),
         Date = g.First().bc_date != null ? ((DateTime)g.First().bc_date).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : null
     })
     .ToList();

            ViewBag.Data = data;

            //chart 2
            var data1 = Request
    .Where(g => g.request_type == "Deployment")
    .GroupBy(g => new { g.request_type, g.Inventory.in_name })
    .Select(g => new {
        Name1 = g.Key.in_name,
        Quantity1 = g.Sum(x => {
            var quantity1 = new string(x.request_item_quantity.ToString().Where(char.IsDigit).ToArray());
            return string.IsNullOrEmpty(quantity1) ? 0 : int.Parse(quantity1);
        }),
        Type = g.First().Inventory.in_code,
        Code = g.Select(x => x.request_type).ToList(),
        Date = g.First().request_date.ToString("MM/dd/yyyy")
    })
    .ToList();

            ViewBag.Chart = data1;
            ViewBag.ProfilePath = GetPath(int.Parse(empId));


            // Include previous data from Inv_Update in the calculation
            var inventories = await db.Inventories.ToListAsync();
            var invUpdates = await db.Inv_Update.ToListAsync();

            // Calculate the sum of in_quantity and update_quantity for each item code
            var itemCodeSumDict = new Dictionary<string, int>();
            foreach (var inventory in inventories)
            {
                if (int.TryParse(inventory.in_quantity.Split(' ')[0], out int quantity))
                {
                    if (!itemCodeSumDict.ContainsKey(inventory.in_code))
                    {
                        itemCodeSumDict[inventory.in_code] = 0;
                    }
                    itemCodeSumDict[inventory.in_code] += quantity;
                }
            }
            foreach (var invUpdate in invUpdates)
            {
                if (int.TryParse(invUpdate.update_quantity.Split(' ')[0], out int quantity))
                {
                    if (!itemCodeSumDict.ContainsKey(invUpdate.update_item_id))
                    {
                        itemCodeSumDict[invUpdate.update_item_id] = 0;
                    }
                    itemCodeSumDict[invUpdate.update_item_id] += quantity;
                }
            }

            // Include previous data from Inv_Update in the calculation
            var prevInvUpdateIds = invUpdates.Select(iu => iu.update_id).ToList();
            var prevInvUpdates = await db.Inv_Update
                .Where(iu => !prevInvUpdateIds.Contains(iu.update_id) && !itemCodeSumDict.Keys.Contains(iu.update_item_id))
                .ToListAsync();
            foreach (var invUpdate in prevInvUpdates)
            {
                if (int.TryParse(invUpdate.update_quantity, out int quantity))
                {
                    if (!itemCodeSumDict.ContainsKey(invUpdate.update_item_id))
                    {
                        itemCodeSumDict[invUpdate.update_item_id] = 0;
                    }
                    itemCodeSumDict[invUpdate.update_item_id] += quantity;
                }
            }

            var currentDate = DateTime.Now.Date;
            foreach (var itemCodeSum in itemCodeSumDict)
            {
                var existingRecord = db.Basecounts.FirstOrDefault(bc => bc.bc_itemCode == itemCodeSum.Key && bc.bc_date == currentDate);
                if (existingRecord != null)
                {
                    existingRecord.bc_count = itemCodeSum.Value.ToString() + " pcs";
                }
                else
                {
                    Basecount bc = new Basecount
                    {
                        bc_id = Guid.NewGuid(),
                        bc_itemCode = itemCodeSum.Key,
                        bc_date = currentDate,
                        bc_count = itemCodeSum.Value.ToString() + " pcs"
                    };
                    db.Basecounts.Add(bc);
                }
            }
            await db.SaveChangesAsync();

            return View();
        }



        public async Task<ActionResult> Projects()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        //public async Task<ActionResult> ProjectAdd()
        //{
        //    var empId = Session["emp_no"]?.ToString();
        //    if (empId == null)
        //    {
        //        return RedirectToAction("Login", "Base");
        //    }
        //    ViewBag.ProfilePath = GetPath(int.Parse(empId));
        //    return View();
        //}

        #region Inventory
        [HttpPost]
        public async Task<ActionResult> RestoreItem([System.Web.Http.FromUri] string itemCode)
        {
            try
            {
                var act = Session["emp_no"];
                Session["active"] = act;
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
                await SendNotif(Session["emp_no"].ToString());
                await SendNotif("notification");
                return result;
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetBarcode(string itemCode)
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
        public async Task<ActionResult> AddItem2(Inventory item)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var serializedModel = JsonConvert.SerializeObject(item);
            var userToken = Session["access_token"].ToString();
            string uri, message;

            if (item.formType == "add")
            {
                uri = "Warehouse/Inventory/Add";
                message = "Item has added successfully";
            }
            else
            {
                uri = "Warehouse/Inventory/Edit";
                message = "Item has updated successfully";
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
            Session["added"] = message;
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public async Task<ActionResult> AddItem1(Inventory item)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var serializedModel = JsonConvert.SerializeObject(item);
            var userToken = Session["access_token"].ToString();
            string uri,message;

            if( item.formType == "add")
            {
                uri = "Warehouse/Inventory/Add";
                message = "Item has added successfully";
            }
            else
            {
                uri = "Warehouse/Inventory/Edit";
                message = "Item has updated successfully";
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
            Session["added"] = message;
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItem(string item)
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
                return RedirectToAction("BadRequest", "Error");
            }
            if (response == "InternalServerError")
            {
                return RedirectToAction("InternalServerError", "Error");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            await SendNotif(Session["emp_no"].ToString());
            await SendMessage("notification");
            return Json("Item Deleted");
        }

        public async Task<ActionResult> Inventory()
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
            //Session["added"] = null;
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            ViewBag.EmpId = empId;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Inventory(Inventory items,string type)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var serializedModel = JsonConvert.SerializeObject(items);
            var userToken = Session["access_token"].ToString();
            string uri,message;
            if (type == "add")
            {
                uri = "Warehouse/Inventory/Add";
                message = "Item has added successfully";
            }
            else
            {
                uri = "Warehouse/Inventory/Edit";
                message = "Item has updated successfully";
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
            Session["added"] = message;
            //ViewBag.Added = "Item has updated successfully";
            return View();
        }

        public async Task<ActionResult> InvArchive()
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
            ViewBag.EmpId = empId;
            return View();
        }

        public ActionResult InventorylistExport()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            ViewBag.EmpId = empId;
            return View();
        }
        #endregion

        #region Users
        public async Task<ActionResult> Users()
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
        public async Task<ActionResult> Users(Employee employee)
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

        public async Task<ActionResult> SearchEmployee()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            try
            {
                var userToken = Session["access_token"].ToString();
                var response = await api_req.GetAllMethod("/Admin/Employee", userToken);
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
        public async Task<ActionResult> ExportRequest(int? id)
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
        public async Task<ActionResult> Requisition()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Requisition([System.Web.Http.FromBody] Request[] jsonData,[System.Web.Http.FromUri] string formType)
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
            ViewBag.Message = message;
            return result;
        }
        #endregion

        public async Task<ActionResult> Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Base");
        }
    }
}