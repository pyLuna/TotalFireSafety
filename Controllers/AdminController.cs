using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TotalFireSafety.Hubs;
using TotalFireSafety.Models;

/*
 * TODO
 * -LAGYAN NG SCROLL BAR BAWAT TABLERequisition
 * -YUNG IBA NASA CP MO CHECK MO JUST YOU
 */

namespace TotalFireSafety.Controllers
{
    public class AdminController : Controller
    {
        readonly APIRequestHandler api_req = new APIRequestHandler();
        #region SignalR
        private async Task SendNotif(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            await hubContext.Clients.All.SendAsync(message);
        }
        private async Task GroupNotif(string group, string message)
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
        #endregion

        // GET: Admin
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
            using (var _context = new nwTFSEntity())
            {
                var user = _context.Employees.Where(x => x.emp_no == emp_no).SingleOrDefault();
                if (user.ProfilePath != null)
                {
                    return user.ProfilePath;
                }
                return "~/images/profile.png";
            }
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
                    using (var _context = new nwTFSEntity())
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
        public async Task<ActionResult> InvReportPrint()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        public async Task<ActionResult> InvReportExport(int? id)
        {

            nwTFSEntity db = new nwTFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var empId = Session["emp_no"].ToString();
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Employees.Where(d => d.emp_no == id).ToList();
            ViewBag.Name = data.FirstOrDefault()?.emp_fname + " " + data.FirstOrDefault()?.emp_lname;
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
        public async Task<ActionResult> ProjectView(int? id, int? rep_no)
        {
            nwTFSEntity db = new nwTFSEntity();
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));

            var project = db.NewProjects.FirstOrDefault(p => p.proj_type_id == id);
            var lead = db.Employees.FirstOrDefault(e => e.emp_no == project.proj_emp_no);
            var report = db.NewReports.FirstOrDefault(r => r.rep_no == id);

            var viewModel = new ProjectReportViewModel
            {
                Project = project,
                Reports = db.NewReports.Where(r => r.rep_proj_id == id).ToList(),
                Manpowers = db.NewManpowers.Where(r => r.man_proj_id == id).ToList(),
                Attendances = db.Attendances.Where(r => r.atte_proj_id == id).ToList(),
                ReportImage = db.ReportImages.Where(r => r.img_proj_id == id).ToList(),

                // Initialize SelectedReport to the first report in the list
                SelectedReport = db.NewReports.FirstOrDefault(r => r.rep_proj_id == id)
            };

            ViewBag.ProjectID = project.proj_type_id;
            ViewBag.ProjectName = project.proj_name;
            ViewBag.Subject = project.proj_subject;
            ViewBag.Client = project.proj_client;
            ViewBag.Status = project.proj_status;
            ViewBag.LocationSite = project.proj_location;
            ViewBag.Startdate = project.proj_strDate?.ToString("MM/dd/yyyy");
            ViewBag.Enddate = project.proj_endDate?.ToString("MM/dd/yyyy");
            ViewBag.ProjectLead = $"{lead.emp_fname} {lead.emp_lname}";

            ViewBag.ReportDescription = report?.rep_description;
            ViewBag.ReportStats = report?.rep_stats;
            ViewBag.ReportDate = report?.rep_date.ToString("MM/dd/yyyy");
            ViewBag.ReportScope = report?.rep_scope;
            ViewBag.ReportNo = report?.rep_no;

            return View(viewModel);
        }

       /* public ActionResult GetReportDate(int rep_no)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Query the database to get the report for the given rep_no
            // Query the database to get the rep_date for the given rep_no
            var report = db.NewReports.SingleOrDefault(r => r.rep_no == rep_no);
            if (report == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            // Return the rep_date as a string
            return Content(report.rep_date.ToString("MM/dd/yyyy"));
        }*/

        [HttpPost]
        public ActionResult UpdateAttendance(Guid projectId, string timeOut)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Find the attendance record for the specified project ID
            var attendance = db.Attendances.FirstOrDefault(a => a.atte_id == projectId);

            // Update the atte_timeout property with the specified time
            if (attendance != null)
            {
                attendance.atte_timeout = DateTime.Parse(timeOut);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetReportData(int rep_no)
        {
            nwTFSEntity db = new nwTFSEntity();
            // retrieve report data using the rep_no parameter
            var report = db.NewReports.FirstOrDefault(r => r.rep_no == rep_no);

            // return the report data as a JSON result
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAttendance(string manpowerName, int projectID)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Get the Attendance record for the specified manpower and project
            var attendance = db.Attendances.FirstOrDefault(a => a.NewManpower.man_name == manpowerName && a.atte_proj_id == projectID);

            if (attendance != null && attendance.atte_timeout == null)
            {
                // Update the Attendance record with the current time as the TimeOut value
                attendance.atte_timeout = DateTime.Now;
                db.Entry(attendance).State = EntityState.Modified;
                db.SaveChanges();

                // Return the new TimeOut value to the AJAX request
                return Content(attendance.atte_timeout.Value.ToString("hh:mm tt"));
            }
            else
            {
                // Return an error message if the Attendance record was not found or already has a TimeOut value
                return Content("Error: No Attendance record found or TimeOut value already exists.");
            }
        }

        public ActionResult EditProject(int projId)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Fetch the project from the database
            var project = db.NewProjects.SingleOrDefault(p => p.proj_type_id == projId);


            // If project is not found, return a 404 error
            if (project == null)
            {
                return HttpNotFound();
            }

            // Create a new instance of the view model and populate it with data from the database
            var viewModel = new NewProject
            {

                proj_type_id = project.proj_type_id,
                proj_lead = project.proj_lead,
                proj_status = project.proj_status,
                proj_strDate = project.proj_strDate,
                proj_endDate = project.proj_endDate
            };

            // Render the edit project view with the view model
            return View(viewModel);
        }

        // POST: Admin/EditProject
        [HttpPost]
        public ActionResult EditProject(int projId, string projName, string projSubject, string projClient, string projLocation, string projLead, string projStatus, string projStartDate, string projEndDate)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Fetch the project from the database
            var project = db.NewProjects.SingleOrDefault(p => p.proj_type_id == projId);

            // If project is not found, return a 404 error
            if (project == null)
            {
                return HttpNotFound();
            }
            DateTime startDate = DateTime.Parse(projStartDate);
            DateTime endDate = DateTime.Parse(projEndDate);

            // Update the project data with the new values
            project.proj_name = projName;
            project.proj_subject = projSubject;
            project.proj_client = projClient;
            project.proj_location = projLocation;
            project.proj_lead = projLead;
            project.proj_status = projStatus;
            project.proj_strDate = startDate;
            project.proj_endDate = endDate;

            // Save changes to the database
            db.SaveChanges();

            // Return a success response
            return Json(new { success = true });
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


        //public AdminController()
        //{
        //    // Schedule the update to run once per day
        //    var updateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
        //    var dueTime = updateTime - DateTime.Now;
        //    if (dueTime < TimeSpan.Zero)
        //    {
        //        dueTime = TimeSpan.Zero;
        //    }

        //    // Create and start the timer
        //    this.updateTimer = new Timer(async state =>
        //    {
        //        await this.Dashboard();
        //    }, null, dueTime, TimeSpan.FromDays(1));
        //}
        [HttpGet]
        public async Task<ActionResult> BaseResult()
        {
            using (var _context = new nwTFSEntity())
            {
                var allBase = _context.Basecounts.OrderByDescending(item => item.bc_date).ToList();
                //List<Basecount> allBase = _context.Basecounts.OrderByDescending(item => item.bc_date).ToList();
                var serialize = JsonConvert.SerializeObject(allBase);
                var deserialize = JsonConvert.DeserializeObject<Basecount>(serialize);
                return Json(deserialize, JsonRequestBehavior.AllowGet);
                //var allBase = _context.Basecounts.Select(x => x).ToList();
                ////var allItems = _context.Inventories.Select(x => x).ToList();
                //var allItems = _context.Inventories.Where(x => x.in_status != "archived").ToList();

                //var newBase = new List<Basecount>();

                //foreach (var item in allBase)
                //{
                //    var newInv = allItems.Where(x => item.bc_itemCode == x.in_code).SingleOrDefault();
                //    Basecount count = new Basecount()
                //    {
                //        bc_id = item.bc_id,
                //        bc_count = item.bc_count,
                //        bc_date = item.bc_date,
                //        Inventory = newInv?.in_code == item.bc_itemCode ? new Inventory
                //        {
                //            in_name = newInv.in_name,
                //            in_category = newInv.in_category,
                //            in_class = newInv.in_class,
                //            in_quantity = newInv.in_quantity,
                //            in_size = newInv.in_size,
                //            in_type = newInv.in_type,
                //        } : null
                //    };
                //    newBase.Add(count);
                //}
                //var groupedItems = newBase.GroupBy(item => item.Inventory?.in_category ?? null)
                //    .Where(group => group.Key != null);

                //var jsonItems = groupedItems.Select(group =>
                //                new
                //                {
                //                    Category = group.Key,
                //                    Items = group.Select(item => new
                //                    {
                //                        Name = item.Inventory?.in_name,
                //                        Quantity = item.bc_count,
                //                        Size = item.Inventory?.in_size,
                //                        Class = item.Inventory?.in_class,
                //                        Type = item.Inventory?.in_type,
                //                        FormattedDate = item.FormattedDate,
                //                        Date = item.bc_date
                //                    })
                //                });

                //var serialize = JsonConvert.SerializeObject(jsonItems);
                ////var deserialize = JsonConvert.DeserializeObject<Object>(serialize);
                //return Json(serialize, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<ActionResult> DBResult()
        {

            using (var _context = new nwTFSEntity())
            {

                var users = _context.Employees.Count();
                var actives = _context.Status.Where(x => x.IsActive == 1 && x.IsUser == 1).Count();
                var deployment = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "deployment").Count();
                var rec_deployment = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "deployment" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var supply = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "supply").Count();
                var rec_supply = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "supply" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var purchase = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "purchase").Count();
                var rec_purchase = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "purchase" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var allItems = _context.Inventories.Count();
                var crit_items = _context.Inventories.Where(x => x.in_remarks.Trim().ToLower() == "critical" && x.in_status.Trim().ToLower() != "archived").Count();
                var model = new DashboardModel()
                {
                    users = users,
                    active_users = actives,
                    deployment = deployment,
                    rec_deployment = rec_deployment,
                    supply = supply,
                    rec_supply = rec_supply,
                    rec_purchase = rec_purchase,
                    purchase = purchase,
                    items = allItems,
                    crit_items = crit_items
                };
                var serialize = JsonConvert.SerializeObject(model);
                var deserialize = JsonConvert.DeserializeObject<DashboardModel>(serialize);
                return Json(deserialize, JsonRequestBehavior.AllowGet);
            }

        }
        // today's summary
        [HttpGet]
        public async Task<ActionResult> ItemSummary()
        {
            using (var _context = new nwTFSEntity())
            {
                var allItems = _context.Inventories.Where(x => x.in_status.Trim().ToLower() != "archived").ToList();
                var groupedItems = allItems.GroupBy(item => item.in_category);

                var jsonItems = groupedItems.Select(group =>
                new
                {
                    Category = group.Key,
                    Items = group.Select(item => new
                    {
                        Name = item.in_name,
                        Quantity = item.in_quantity,
                        Size = item.in_size,
                        Class = item.in_class,
                        Type = item.in_type
                    })
                });

                var serialize = JsonConvert.SerializeObject(jsonItems);
                //var deserialize = JsonConvert.DeserializeObject<Object>(serialize);
                return Json(serialize, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> Dashboard()
        {

            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Projects()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            ViewBag.EmpId = empId;
            using (var db = new nwTFSEntity())
            {

                var projects = (
                    from p in db.NewProjects
                    join e in db.Employees on p.proj_emp_no equals e.emp_no
                    select new
                    {
                        proj_type_id = p.proj_type_id,
                        proj_name = p.proj_name,
                        emp_fname = e.emp_fname,
                        emp_lname = e.emp_lname,
                        proj_strDate = p.proj_strDate,
                        proj_endDate = p.proj_endDate,
                        proj_status = p.proj_status
                    }
                ).ToList();
                var jsonProjects = projects.Select(p => new
                {
                    proj_type_id = p.proj_type_id,
                    proj_name = p.proj_name,
                    proj_lead = p.emp_fname + ' ' + p.emp_lname,
                    proj_strDate = p.proj_strDate.HasValue ? p.proj_strDate.Value.ToString("yyyy-MM-dd") : "",
                    proj_endDate = p.proj_endDate.HasValue ? p.proj_endDate.Value.ToString("yyyy-MM-dd") : "",
                    proj_status = p.proj_status
                });
                var serialize = JsonConvert.SerializeObject(jsonProjects);
                ViewBag.Projects = new HtmlString(serialize);
            }
            return View();
        }



        public async Task<JsonResult> GetEmployees(int empId)
        {
            nwTFSEntity db = new nwTFSEntity();

            var employees = await db.Employees
                .Where(e => e.emp_no == empId)
                .Select(e => new { e.emp_no, e.emp_fname, e.emp_lname })
                .ToListAsync();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveData(List<Dictionary<string, string>> data, int man_proj_id, string man_date)
        {
            nwTFSEntity db = new nwTFSEntity();
            foreach (var row in data)
            {
                // Create a new NewManpower object
                var manpower = new NewManpower();
                manpower.man_emp_no = (int?)Session["emp_no"];
                manpower.man_id = Guid.NewGuid();
                manpower.man_name = row["man_name"];
                manpower.man_postition = row["man_position"];
                manpower.man_proj_id = man_proj_id; // Set the man_proj_id property to the provided value
                manpower.man_date = DateTime.Parse(man_date);

                // Save the NewManpower object to the database
                db.NewManpowers.Add(manpower);
                db.SaveChanges();

                // Create a new Attendance object
                var attendance = new Attendance();
                attendance.atte_id = Guid.NewGuid();
                attendance.atte_proj_id = man_proj_id;
                attendance.atte_timein = DateTime.Parse(row["atte_timein"].ToString());
                attendance.atte_timeout = attendance.atte_timeout;
                attendance.NewManpower = manpower;

                // Save the Attendance object to the database
                db.Attendances.Add(attendance);
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult SaveProject(List<NewProject> projectList)
        {
            nwTFSEntity db = new nwTFSEntity();
            try
            {
                foreach (NewProject project in projectList)
                {
                    Random rand = new Random();
                    int projTypeId = rand.Next(1000, 9999); // Generate a random 4-digit integer
                    project.proj_id = Guid.NewGuid();
                    project.proj_emp_no = (int)Session["emp_no"];
                    project.proj_type_id = projTypeId;
                    db.NewProjects.Add(project);
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<ActionResult> ProjectList()
        {
            nwTFSEntity db = new nwTFSEntity();
            var projects = db.NewProjects.ToList();
            return View(projects);
        }

        [HttpPost]
        public ActionResult SaveReport(string rep_scope, string rep_date, string rep_stats, string rep_proj_id, string rep_description)
        {
            // convert string values to their corresponding data types
            DateTime date = DateTime.Parse(rep_date);
            int projectId = int.Parse(rep_proj_id);
            // create a new report object
            NewReport report = new NewReport
            {
                rep_id = Guid.NewGuid(),
                rep_emp_no = (int)Session["emp_no"],
                rep_proj_id = projectId,
                rep_scope = rep_scope,
                rep_stats = rep_stats,
                rep_date = date,
                rep_description = rep_description
            };
            try
            {
                using (var context = new nwTFSEntity())
                {
                    context.NewReports.Add(report);
                    context.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return Json(new { success = false, message = "Validation failed." });
            }
        }

            [HttpPost]
        public ActionResult SaveReportImages(List<string> images, string rep_date, int rep_proj_id)
        {
            nwTFSEntity db = new nwTFSEntity();

            foreach (var image in images)
            {
                var reportImage = new ReportImage
                {
                    img_id = Guid.NewGuid(),
                    img_proj_id = rep_proj_id,
                    img_date = DateTime.Parse(rep_date)
                };

                // get the filename from the base64 string
                var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var fileName = $"{reportImage.img_id}.jpg";
                var filePath = Path.Combine(Server.MapPath("~/ReportImg"), fileName);

                // convert base64 string to byte array and save to server
                var imageData = Convert.FromBase64String(base64Data);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    stream.Write(imageData, 0, imageData.Length);
                }

                // assign filename to img_image property and save to database
                reportImage.img_image = fileName;
                db.ReportImages.Add(reportImage);
                db.SaveChanges();
            }

            return Json(new { success = true });
        }


        /*        PUBLIC ASYNC TASK<ACTIONRESULT> PROJECTADD()
                {
                    VAR EMPID = SESSION["EMP_NO"]?.TOSTRING();
                    IF(EMPID == NULL)
                    {
                        RETURN REDIRECTTOACTION("LOGIN", "BASE");
                    }
                    VIEWBAG.PROFILEPATH = GETPATH(INT.PARSE(EMPID));
                    RETURN VIEW();
                }*/

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
                //await SendNotif("notification");
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
        public async Task<ActionResult> Inventory(Inventory items, string type)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var serializedModel = JsonConvert.SerializeObject(items);
            var userToken = Session["access_token"].ToString();
            string uri, message;
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
            if (empId == null)
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
            await SendNotif("notification");
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
            nwTFSEntity db = new nwTFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Requests.Where(d => d.request_type_id == id && d.request_status == "Pending").ToList();
            ViewBag.ActiveCount = data.Count(); // Count the number of items in data where request_type_status is "Active"

            return View(data);
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
        public async Task<ActionResult> Requisition([System.Web.Http.FromBody] Request[] jsonData, [System.Web.Http.FromUri] string formType)
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
            string uri = "", message = "";
            if (formType == "add")
            {
                message = "Request has been accepted";
                uri = "Requests/Add";
            }
            else
            {
                if (formType == "approved")
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
            await SendNotif("notification");
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