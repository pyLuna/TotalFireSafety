using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
//using System.Web.Http.Cors;
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
        private readonly Timer updateTimer;
        private readonly List<Basecount> bcItems;
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
            updateTimer = new Timer(async state =>
            {
                await AddItemsToBaseCount();
                await ResetRunningQuantity();
            }, null, dueTime, TimeSpan.FromDays(1));
        }

        private async Task ResetRunningQuantity()
        {
            using(var _context = new nwTFSEntity())
            {
                var allLimits = _context.Inv_Limits.ToList();

                foreach (var item in allLimits)
                {
                    item.running = 0;
                }
                _context.Entry(allLimits);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddItemsToBaseCount()
        {
            using (var _context = new nwTFSEntity())
            {
                var allItems = _context.Inventories.ToList();
                foreach (var item in allItems)
                {
                    var nbc = new Basecount()
                    {
                        bc_count = item.in_quantity,
                        bc_date = DateTime.Now,
                        bc_itemCode = item.in_code,
                        bc_id = Guid.NewGuid()
                    };
                    _context.Basecounts.Add(nbc);
                    await _context.SaveChangesAsync();
                }
            }
        }

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
        private Tuple<Guid, Boolean> Validate(string typeOf)
        {
            using (var _context = new nwTFSEntity())
            {
                var newGuid = Guid.NewGuid();
                switch (typeOf.ToLower())
                {
                    case "newitem":
                        var checkitem = _context.Inventories.Where(x => x.in_guid == newGuid).SingleOrDefault();
                        if (checkitem == null)
                        {
                            return new Tuple<Guid, Boolean>(newGuid, true);
                        }
                        else
                        {
                            return Validate("newitem");
                        }
                    case "done":
                        return new Tuple<Guid, Boolean>(newGuid, false);
                }
                return Validate("done");
            }
        }
        private async Task CreateNotif(string ntf_type, string req_id, string req_type, int? emp_no)
        {
            using (var _context = new nwTFSEntity())
            {
                Notification ntf = new Notification();
                switch (ntf_type)
                {
                    case "request":
                            var newGuid = Validate("newitem");
                        ntf = new Notification()
                        {
                            guid = newGuid.Item1,
                            emp_no = emp_no.Value,
                            ntf_title = "Request Approved",
                            ntf_content = "The " + req_type + " Request with Request ID " + req_id + " has been marked as Approved.",
                            ntf_date = DateTime.Now,
                            ntf_for = "",
                            ntf_type = ntf_type,
                            request_type = req_type,
                            request_type_id = int.Parse(req_id)
                        };
                        _context.Notifications.Add(ntf);
                        await _context.SaveChangesAsync();
                        break;
                    case "project":
                        break;
                }
            }
        }
        [HttpGet]
        public async Task<ActionResult> Notifications(int emp_id)
        {
            using (var _context = new nwTFSEntity())
            {
                var allNtfs = _context.Notifications.Where(x => x.emp_no == emp_id).ToList();

                using (var stream = new MemoryStream())
                {
                    var writer = new Utf8JsonWriter(stream);
                    writer.WriteStartArray();

                    foreach (var item in allNtfs)
                    {
                        writer.WriteStartObject();
                        //writer.WriteString("in_guid", item.in_guid);
                        writer.WriteString("ntf_title", item.ntf_title);
                        writer.WriteString("ntf_content", item.ntf_content);
                        writer.WriteString("ntf_date", item.ntf_date.ToString("MMMM dd, yyyy"));
                        writer.WriteString("ntf_type", item.ntf_type.Trim());
                        // add more properties as needed
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.Flush();
                    var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                    var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                    return Content(jsonString);
                }
            }
        }
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

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> SaveImage([System.Web.Http.FromBody] HttpPostedFileBase file)
        {
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 1)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
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
                    //inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    //result = Json(inv, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "report")
                {
                    uri = "Warehouse/Inventory/Updates";
                    response = await api_req.GetAllMethod(uri, userToken);
                    reports = JsonConvert.DeserializeObject<List<Inv_Update>>(response);
                    return Json(reports, JsonRequestBehavior.AllowGet);
                }
                if (requestType == "deleted")
                {
                    uri = "/Warehouse/Inventory/Archive";
                    response = await api_req.GetAllMethod(uri, userToken);
                    //inv = JsonConvert.DeserializeObject<List<Inventory>>(response);
                    //result = Json(inv, JsonRequestBehavior.AllowGet);
                }

                if (requestType == "requisition")
                {
                    uri = "/Requests/All";
                    response = await api_req.GetAllMethod(uri, userToken);
                    //requisition = JsonConvert.DeserializeObject<List<Request>>(response);
                    //result = Json(requisition, JsonRequestBehavior.AllowGet);
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

                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        #endregion
        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> InvReportPrint()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if(role ==3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }
        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> ExportReportNew(int? id)
        {
            var empId = Session["emp_no"].ToString();
            nwTFSEntity db = new nwTFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Requests.Where(d => d.request_type_id == id).ToList();

            return View(data);

        }

        //VIEW FOR PROJECT PROPOSAL
        public async Task<ActionResult> ProjectProposal(int? id)
        {
            var empId = Session["emp_no"].ToString();
            nwTFSEntity db = new nwTFSEntity();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            List<NewProposal> proposals = db.NewProposals.ToList();

            return View(proposals);



        }
        [HttpPost]
        public ActionResult Update(int propTypeId, string propDescription, string propStatus, string propSubject, string propManpower)
        {
            nwTFSEntity db = new nwTFSEntity();
            if (propTypeId != 0)
            {
                var proposal = db.NewProposals.Find(propTypeId); // Find the proposal by its ID
                if (proposal != null)
                {
                    // Update the proposal properties with the new values
                    proposal.prop_description = propDescription;
                    proposal.prop_status = propStatus;
                    proposal.prop_subject = propSubject;
                    proposal.prop_manpower = propManpower;

                    db.SaveChanges(); // Save changes to the database

                    return Json(new { success = true }); // Return success response to the client
                }
                else
                {
                    return Json(new { success = false, message = "Proposal not found." }); // Return error response to the client
                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid model state." }); // Return error response to the client
            }
        }

     /*   [HttpGet]
        public ActionResult GetProposalData(int propTypeId)
        {
            nwTFSEntity db = new nwTFSEntity();
            // Retrieve the proposal data based on the prop_type_id
            NewProposal proposal = db.NewProposals.FirstOrDefault(p => p.prop_type_id == propTypeId);

            if (proposal != null)
            {
                ViewBag.Proposal = proposal; // Store the proposal data in ViewBag
                return View();
            }
            else
            {
                ViewBag.Message = "Failed to retrieve proposal data.";
                return View("Error"); // Show an error view
            }
        }*/

        /*public ActionResult GetProposals()
        {
            nwTFSEntity db = new nwTFSEntity();

            var proposals = db.NewProposals.ToList();
            var data = proposals.Select(p => new
            {
                prop_description = p.prop_description,
                prop_status = p.prop_status
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }*/


        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> InvReportExport(int? id)
        {
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
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
        public ActionResult InventorylistExport(int? id)
        {
            /*var role = int.Parse(Session["system_role"].ToString());
            if (role == 4 || role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));    
            ViewBag.EmpId = empId;
            return View();*/
            nwTFSEntity db = new nwTFSEntity();
            var empId = Session["emp_no"].ToString();
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            var data = db.Employees.Where(d => d.emp_no == id).ToList();
            ViewBag.Name = data.FirstOrDefault()?.emp_fname + " " + data.FirstOrDefault()?.emp_lname;
            ViewBag.Position = data.FirstOrDefault()?.emp_position;
            ViewBag.Id = data.FirstOrDefault()?.emp_no;

            return View(data);
        }
        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> InvReorder()
        {
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        public async Task<ActionResult> ProjectsReport(int? id, int? rep_no)
        {

            nwTFSEntity db = new nwTFSEntity();
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));

            var report = db.NewReports.FirstOrDefault(r => r.rep_proj_id == id);



            ViewBag.ReportDescription = report?.rep_description;
            ViewBag.ReportStats = report?.rep_stats;
            ViewBag.ReportDate = report?.rep_date.ToString("MM/dd/yyyy");
            ViewBag.ReportScope = report?.rep_scope;
            ViewBag.ReportNo = report?.rep_no;


            ViewBag.ProjectName = report?.NewProject.proj_name;
            ViewBag.ProjectSubject = report?.NewProject.proj_subject;
            ViewBag.ProjectClient = report?.NewProject.proj_client;
            ViewBag.ProjectLead = report?.NewProject.proj_lead;
            ViewBag.ProjectLocation = report?.NewProject.proj_location;
            ViewBag.StartDate = report?.NewProject.proj_strDate;
            ViewBag.EndDate = report?.NewProject.proj_endDate;
            ViewBag.Status = report?.NewProject.proj_status;
            ViewBag.Engineer = report?.NewProject.proj_engineer_no;

            var reportImages = db.ReportImages
      .Where(r => !string.IsNullOrEmpty(r.img_image) && r.img_date == report.rep_date)
      .Select(r => r.img_image)
      .ToList();
            ViewBag.ImagePaths = reportImages;
            // Get multiple reports
            var reports = db.NewReports.ToList(); // Example: Fetch all reports from the database

            ViewBag.Reports = reports; // Pass the reports to ViewBag
            ViewBag.ReportId = id;




            return View();
        }


        //[System.Web.Mvc.Authorize(Roles = "admin,office")]
        public async Task<ActionResult> ProjectView(int? id, int? rep_no)
        {
            nwTFSEntity db = new nwTFSEntity();
            var empId = Session["emp_no"]?.ToString();

            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }

            ViewBag.ProfilePath = GetPath(int.Parse(empId));

            // Retrieve the session user's emp_no and position
            var sessionEmpNo = int.Parse(empId);
            var sessionUser = db.Employees.FirstOrDefault(e => e.emp_no == sessionEmpNo);
            var sessionUserPosition = sessionUser?.emp_position;

            var project = db.NewProjects.FirstOrDefault(p => p.proj_type_id == id);
            var lead = db.Employees.FirstOrDefault(e => e.emp_no == project.proj_emp_no);
            var Engineer = db.Employees.FirstOrDefault(e => e.emp_no == project.proj_engineer_no);

            // Check if rep_no is null, then use a default value or fallback behavior
            var reports = rep_no != null
                ? db.NewReports.FirstOrDefault(r => r.rep_no == rep_no)
                : db.NewReports.FirstOrDefault(r => r.rep_proj_id == id);

            var newProposal = db.NewProposals.FirstOrDefault(p => p.prop_type_id == project.proj_type_id);
            var Request = db.Requests.FirstOrDefault(q => q.request_proj_id == project.proj_type_id);
            ViewBag.Requests = db.Requests.Where(q => q.request_proj_id == project.proj_type_id).ToList();

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
            ViewBag.ProjectEngineer = $"{Engineer.emp_fname} {Engineer.emp_lname}";
            ViewBag.ProjectLeads = project.proj_lead;
            ViewBag.SessionUserPosition = sessionUserPosition;

            ViewBag.ReportDescription = reports?.rep_description;
            ViewBag.ReportStats = reports?.rep_stats;
            ViewBag.ReportDate = reports?.rep_date.ToString("MM/dd/yyyy");
            ViewBag.ReportScope = reports?.rep_scope;
            ViewBag.ReportNo = reports?.rep_no;

            ViewBag.ProposalStats = newProposal?.prop_status;
            ViewBag.ProposalStatus = newProposal?.prop_status;
            ViewBag.ProposalDescription = newProposal?.prop_description;

            // Retrieve the Employee object using the prop_emp_no value
            Employee engineer = null;
            if (newProposal != null && newProposal.prop_emp_no != null)
            {
                engineer = db.Employees.FirstOrDefault(e => e.emp_no == newProposal.prop_emp_no);
            }

            // Assign the emp_fname and emp_lname to ViewBag.ProposalEngineer
            ViewBag.ProposalEngineer = engineer?.emp_fname + " " + engineer?.emp_lname;
            ViewBag.ProposalEngineerID = newProposal?.prop_emp_no;
            ViewBag.ProposalMan = newProposal?.prop_manpower;
            ViewBag.ProposalSubject = newProposal?.prop_subject;
            ViewBag.NewProposal = newProposal;

           

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateProjectStatus(int projTypeId, string status)
        {
            if (status != "Cancelled")
            {
                nwTFSEntity db = new nwTFSEntity();
                // Retrieve the project and update the status based on proj_type_id
                var project = db.NewProjects.FirstOrDefault(p => p.proj_type_id == projTypeId);
                if (project != null)
                {
                    project.proj_status = status;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public async Task<ActionResult> ProjectExport(int? rep_no)
        {
            nwTFSEntity db = new nwTFSEntity();
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));

            var report = db.NewReports.FirstOrDefault(r => r.rep_no == rep_no);
           


            ViewBag.ReportDescription = report?.rep_description;
            ViewBag.ReportStats = report?.rep_stats;
            ViewBag.ReportDate = report?.rep_date.ToString("MM/dd/yyyy");
            ViewBag.ReportScope = report?.rep_scope;
            ViewBag.ReportNo = report?.rep_no;


            ViewBag.ProjectName = report?.NewProject.proj_name;
            ViewBag.ProjectSubject = report?.NewProject.proj_subject;
            ViewBag.ProjectClient = report?.NewProject.proj_client;
            ViewBag.ProjectLead = report?.NewProject.proj_lead;
            ViewBag.ProjectLocation = report?.NewProject.proj_location;
            ViewBag.StartDate = report?.NewProject.proj_strDate;
            ViewBag.EndDate = report?.NewProject.proj_endDate;
            ViewBag.Status = report?.NewProject.proj_status;

            var reportImages = db.ReportImages
         .Where(r => !string.IsNullOrEmpty(r.img_image) && r.img_date == report.rep_date) // Filter out null or empty image paths and match the image date with the report date
         .Select(r => r.img_image)
         .ToList();

            ViewBag.ImagePaths = reportImages;

          var attendanceList = db.Attendances
    .Include(a => a.NewManpower)
    .Where(a => a.NewManpower.man_proj_id == report.rep_proj_id && 
           (a.atte_timein != null || a.atte_timein == null) && 
           a.NewManpower.man_date == report.rep_date)
    .ToList();

ViewBag.AttendanceList = attendanceList;

            

            return View();
        }

      

        [HttpPost]
        public ActionResult SaveProposal(NewProposal proposal)
        {
            try
            {
                proposal.prop_id = Guid.NewGuid();
                proposal.prop_status = "Pending";

                using (var db = new nwTFSEntity())
                {
                    db.NewProposals.Add(proposal);
                    db.SaveChanges();
                }

                return Json(new { message = "Proposal saved successfully." });
            }
            catch (Exception ex)
            {
                return Content($"<script>alert('Error saving proposal: One propsal only please check the manpower button above'); window.location.href = '/Admin/Index';</script>");
            }
        }

        [HttpPost]
        public ActionResult UpdateProposal(NewProposal updatedProposal)
        {
            try
            {
                // Retrieve the existing proposal based on the updatedProposal.prop_type_id or any other identifier
                using (var db = new nwTFSEntity())
                {
                    var existingProposal = db.NewProposals.FirstOrDefault(p => p.prop_type_id == updatedProposal.prop_type_id);
                    if (existingProposal != null)
                    {
                        // Update the fields of the existing proposal with the updated values
                        existingProposal.prop_manpower = updatedProposal.prop_manpower;
                        existingProposal.prop_status = updatedProposal.prop_status;
                        existingProposal.prop_description = updatedProposal.prop_description;
                        existingProposal.prop_subject = updatedProposal.prop_subject;
                        existingProposal.prop_emp_no = updatedProposal.prop_emp_no;

                        db.SaveChanges();

                        return Json(new { message = "Proposal updated successfully." });
                    }
                    else
                    {
                        return Json(new { message = "Proposal not found." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error updating proposal: " + ex.Message });
            }
        }

      
        public ActionResult GetAttendanceByDate(DateTime rep_date)
        {
            nwTFSEntity db = new nwTFSEntity();
            var attendance = db.Attendances.Where(a => a.atte_timein == rep_date).ToList();
            return Json(attendance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAttendanceData()
        {
            nwTFSEntity db = new nwTFSEntity();
            var data = db.Attendances
                .Join(db.NewManpowers, a => a.atte_id, m => m.man_id, (a, m) => new { Attendance = a, Manpower = m })
                .Select(a => new
                {
                    Manpower = a.Manpower.man_name,
                    Position = a.Manpower.man_postition,
                    TimeIn = a.Attendance.atte_timein,
                    TimeOut = a.Attendance.atte_timeout
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAttendanceDatas()
        {
            nwTFSEntity db = new nwTFSEntity();
            var data = db.Attendances
                .Join(db.NewManpowers, a => a.atte_id, m => m.man_id, (a, m) => new { Attendance = a, Manpower = m })
                .Select(a => new
                {
                    Manpower = a.Manpower.man_name,
                    Position = a.Manpower.man_postition,
                    TimeIn = a.Attendance.atte_timein,
                    TimeOut = a.Attendance.atte_timeout
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReports(int rep_proj_id)
        {
            nwTFSEntity db = new nwTFSEntity();
            var reports = db.NewReports.Where(r => r.rep_proj_id == rep_proj_id).ToList();
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetReportDatas(int rep_no)
        {
            // Validate the rep_no parameter
            if (rep_no <= 0)
            {
                return Json(new { error = "Invalid rep_no parameter." }, JsonRequestBehavior.AllowGet);
            }

            nwTFSEntity db = new nwTFSEntity();
            var report = db.NewReports.FirstOrDefault(r => r.rep_no == rep_no);

            if (report == null)
            {
                return Json(new { error = "Report not found." }, JsonRequestBehavior.AllowGet);
            }

            // Format report data for JavaScript
            var reportData = new
            {
                rep_no = report.rep_no,
                rep_description = report.rep_description,
                rep_date = report.rep_date.ToString("yyyy-MM-dd"),
                rep_stats = report.rep_stats,
                rep_scope = report.rep_scope
            };

            return Json(reportData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportImages(DateTime rep_date, int? proj_type_id)
        {
            nwTFSEntity db = new nwTFSEntity();
            var images = db.ReportImages
                .Where(i => i.img_date.HasValue && DbFunctions.TruncateTime(i.img_date) == rep_date.Date
                    && (!proj_type_id.HasValue || i.img_proj_id == proj_type_id.Value))
                .Select(i => i.img_image)
                .ToList();
            return Json(images, JsonRequestBehavior.AllowGet);
        }


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

            return View();
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

        public ActionResult UpdateTimeOut(string manpowerName, int projectID)
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


        //[System.Web.Mvc.Authorize(Roles = "admin,office")]
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



        // for chart 2 / Item Supplies
        [HttpGet]
        public async Task<ActionResult> ItemSupplies()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var allReq = _context.Requests
                                .Where(x => x.request_type == "deployment")
                                //.OrderBy(x => x.request_date)
                                .Include("Inventory")
                                .ToList();

                    var groupedByCode = await Task.Run(() => allReq
                            .GroupBy(item => item.Inventory.in_code)
                            .Select(group => new
                            {
                                Code = group.Key,
                                TotalQuantity = group.Sum(item =>
                                {
                                    string quantityStr = item.request_item_quantity;
                                    int quantityInt = int.Parse(quantityStr.Split(' ')[0]);
                                    return quantityInt;
                                }),
                                TotalQuantityUnit = group.First().Inventory.in_quantity.Split(' ')[1],
                                TotalRequest = group.Count(),
                                Items = group.Select(item => new
                                {
                                    Name = item.Inventory.in_name,
                                    Size = item.Inventory.in_size
                                }),
                            })
                            .OrderByDescending(x => x.TotalQuantity)
                            .ThenBy(x => x.Code) // if there are ties in TotalQuantity, sort by Code ascending
                            .ToList()
                            .Select(group => new
                            {
                                Code = group.Code,
                                TotalQuantity = group.TotalQuantity.ToString() + " " + group.TotalQuantityUnit,
                                TotalRequest = group.TotalRequest,
                                Items = group.Items
                            })
                            );
                    var ave = groupedByCode.Average(x => x.TotalRequest);

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in groupedByCode)
                        {
                            writer.WriteStartObject();
                            writer.WriteString("Category", item.Code);
                            writer.WriteNumber("TotalRequest", item.TotalRequest);
                            writer.WriteNumber("Average", ave);
                            writer.WriteStartObject("Items");
                            writer.WriteNull("Class");
                            writer.WriteNull("Type");
                            writer.WriteString("Name", item.Items.First().Name);
                            writer.WriteString("Quantity", item.TotalQuantity);
                            writer.WriteString("Size", item.Items.First().Size);
                            writer.WriteEndObject();

                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());

                        return Content(jsonString, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                byte[] jsonBytes = Utf8Json.JsonSerializer.Serialize(ex);

                // Convert the byte array to a string
                string jsonString = Encoding.UTF8.GetString(jsonBytes);
                return Content(jsonString, "application/json");
            }
        }
        [HttpGet]
        //project insights
        public async Task<ActionResult> ProjectInsights()
        {
            using(var _context =  new nwTFSEntity())
            {
                var projs = _context.NewProjects.ToList();

                var upcoming = projs.Where(x => x.proj_status.Trim().ToLower() == "upcoming").Count();
                var ongoing = projs.Where(x => x.proj_status.Trim().ToLower() == "on-going").Count();
                var finished = projs.Where(x => x.proj_status.Trim().ToLower() == "finished").Count();
                var cancelled = projs.Where(x => x.proj_status.Trim().ToLower() == "cancelled").Count();
                var allProjs = projs.Count();

                var grouped = new
                {
                    Upcoming = upcoming,
                    OnGoing = ongoing,
                    Finished = finished,
                    Cancelled = cancelled,
                    AllProjects = allProjs
                };
                byte[] jsonBytes = Utf8Json.JsonSerializer.Serialize(grouped);

                // Convert the byte array to a string
                string jsonString = Encoding.UTF8.GetString(jsonBytes);
                return Content(jsonString, "application/json");
            }
        }

        //  for chart 3 / projects 
        public static int CalculateWorkingDays(DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue) return 0;

            int workingDays = 0;
            DateTime currentDay = start.Value;

            while (currentDay <= end.Value)
            {
                if (currentDay.DayOfWeek != DayOfWeek.Saturday && currentDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
                currentDay = currentDay.AddDays(1);
            }

            return workingDays;
        }
        [HttpGet]
        public async Task<ActionResult> AllProjects()
        {
            using(var _context = new nwTFSEntity())
            {
                var projs = _context.NewProjects
                            .Where(x => x.proj_status.Trim().ToLower() == "on-going")
                            .Include("Employee")
                            .Include("NewReports")
                            .ToList();

                var ong = projs.GroupBy(e => e.proj_emp_no)
                            .Select(p => new
                            {
                                Lead_ID = p.Key,
                                Projects = p.Select(proj => new
                                {
                                    Lead_Name = proj.Employee.emp_fname  + " " + proj.Employee.emp_lname,
                                    Name = proj.proj_name,
                                    Reports = proj.NewReports.Count(),
                                    Status = proj.proj_status,
                                    Start = proj.proj_strDate,
                                    End = proj.proj_endDate,
                                    WorkingDays = CalculateWorkingDays(proj.proj_strDate, proj.proj_endDate)
                                })
                            });
                // Serialize the object to a byte array
                byte[] jsonBytes = Utf8Json.JsonSerializer.Serialize(ong);

                // Convert the byte array to a string
                string jsonString = Encoding.UTF8.GetString(jsonBytes);
                return Content(jsonString, "application/json");
            }
        }



        // for chart 1 / Item Summary
        [HttpGet]
        public async Task<ActionResult> BaseResult([System.Web.Http.FromUri] string end, [System.Web.Http.FromUri] int diff)
        {
            using (var _context = new nwTFSEntity())
            {
                if (diff == 0)
                {
                    var res = ItemSum();
                    return Content(res, "application/json");
                }
                // Parse the end date string to a DateTime object
                DateTime endDate = DateTime.Parse(end);
                if (diff == 1)
                {
                    endDate = endDate.AddDays(-diff);
                }
                // Calculate the start date based on the end date and the diff parameter
                DateTime startDate = endDate.AddDays(-diff);

                //                var allBase1 = _context.Basecounts
                //    .Where(item => item.bc_date >= startDate && item.bc_date <= endDate)
                //    .ToList();
                //var allBase1Items = allBase1.First().Inventory;
                var allBase = _context.Basecounts
                    .Where(item => item.bc_date >= startDate && item.bc_date <= endDate)
                    //.Include("Inventory")
                    .ToList();

                var groupedByCode = await Task.Run(() => allBase
                            .GroupBy(item => item.Inventory.in_code)
                            .Select(group => new
                            {
                                Code = group.Key,
                                TotalQuantity = group.Sum(item =>
                                {
                                    string quantityStr = item.Inventory.in_quantity;
                                    int quantityInt = int.Parse(quantityStr.Split(' ')[0]);
                                    return quantityInt;
                                }).ToString() + " " + group.First().Inventory.in_quantity.Split(' ')[1],
                                Items = group.First() // Use the first item in the group to get other properties
                            })
                            .ToList()
                            );

                var groupedByCategory = await Task.Run(() => groupedByCode
                                        .GroupBy(item => item.Items.Inventory.in_category)
                                        .Select(group => new
                                        {
                                            Category = group.Key,
                                            Items = group.Select(item => new
                                            {
                                                Name = item.Items.Inventory.in_name,
                                                Quantity = item.TotalQuantity,
                                                Size = item.Items.Inventory.in_size,
                                                Class = item.Items.Inventory.in_class,
                                                Type = item.Items.Inventory.in_type
                                            })
                                        })
                                        .ToList()
                                        );
                // Serialize the object to a byte array
                byte[] jsonBytes = Utf8Json.JsonSerializer.Serialize(groupedByCategory);

                // Convert the byte array to a string
                string jsonString = Encoding.UTF8.GetString(jsonBytes);
                return Content(jsonString, "application/json");
            }
        }

        private string ItemSum()
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
                return serialize;
            }
        }

        [HttpGet]
        public async Task<ActionResult> DBResult()
        {

            using (var _context = new nwTFSEntity())
            {

                var users = _context.Employees.Count();
                var actives = _context.Status.Where(x => x.IsActive == 1 && x.IsUser == 1).Count();
                var deployment = _context.Requests
                    .Where(x => x.request_type.Trim().ToLower() == "deployment")
                    .GroupBy(x => x.request_type_id)
                    .Count();
                //var rec_deployment = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "deployment" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var rec_deployment = _context.Requests
                    .Where(x => x.request_type.Trim().ToLower() == "deployment" && x.request_status.Trim()
                    .ToLower() == "pending")
                    .GroupBy(x => x.request_type_id)
                    .Count();
                var supply = _context.Requests.Where(x => x.request_type.Trim()
                    .ToLower() == "supply")
                    .GroupBy(x => x.request_type_id)
                    .Count();
                //var rec_supply = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "supply" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var rec_supply = _context.Requests
                    .Where(x => x.request_type.Trim().ToLower() == "supply" && x.request_status.Trim().ToLower() == "pending")
                    .GroupBy(x => x.request_type_id)
                    .Count();
                var purchase = _context.Requests
                    .Where(x => x.request_type.Trim().ToLower() == "purchase")
                    .GroupBy(x => x.request_type_id)
                    .Count();
                //var rec_purchase = _context.Requests.Where(x => x.request_type.Trim().ToLower() == "purchase" && x.request_date.Day == (DateTime.Now.Day + 1)).Count();
                var rec_purchase = _context.Requests
                    .Where(x => x.request_type.Trim().ToLower() == "purchase" && x.request_status.Trim().ToLower() == "pending")
                    .GroupBy(x => x.request_type_id)
                    .Count();
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
        //[System.Web.Mvc.Authorize(Roles = "admin,office")]
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
            ViewBag.emp_position = "Project Lead";
            using (var db = new nwTFSEntity())
            {

                var projects = (
       from p in db.NewProjects
       join e1 in db.Employees on p.proj_emp_no equals e1.emp_no
       join e2 in db.Employees on p.proj_engineer_no equals e2.emp_no
       select new
       {
           proj_type_id = p.proj_type_id,
           proj_name = p.proj_name,
           emp_fname_lead = e1.emp_fname,
           emp_lname_lead = e1.emp_lname,
           emp_fname_engineer = e2.emp_fname,
           emp_lname_engineer = e2.emp_lname,
           proj_strDate = p.proj_strDate,
           proj_endDate = p.proj_endDate,
           proj_status = p.proj_status,
           proj_lead = p.proj_lead,
           proj_lead_id = p.proj_emp_no,
           proj_engineer_no = p.proj_engineer_no,
            proj_engineer_id = p.proj_engineer_no
       }
   ).ToList();

                var jsonProjects = projects.Select(p => new
                {
                    proj_type_id = p.proj_type_id,
                    proj_name = p.proj_name,
                    proj_lead = p.emp_fname_lead + ' ' + p.emp_lname_lead,
                    proj_strDate = p.proj_strDate.HasValue ? p.proj_strDate.Value.ToString("yyyy-MM-dd") : "",
                    proj_endDate = p.proj_endDate.HasValue ? p.proj_endDate.Value.ToString("yyyy-MM-dd") : "",
                    proj_status = p.proj_status,
                    project_leads = p.emp_fname_lead + ' ' + p.emp_lname_lead,
                    proj_lead_id = p.proj_lead_id,
                    proj_engineer_no = p.emp_fname_engineer + ' ' + p.emp_lname_engineer,
                    proj_engineer_id = p.proj_engineer_no
                });

                var serialize = JsonConvert.SerializeObject(jsonProjects);

                // Retrieve the session user's emp_no and position
                var sessionEmpNo = int.Parse(empId);
                var sessionUser = db.Employees.FirstOrDefault(e => e.emp_no == sessionEmpNo);
                var sessionUserPosition = sessionUser?.emp_position;
                ViewBag.SessionUserPosition = sessionUserPosition;


                ViewBag.Projects = new HtmlString(serialize);
            }
            return View();
        }



        public JsonResult GetEmployees()
        {
            nwTFSEntity db = new nwTFSEntity();

            var employees = db.Employees
                .Where(e => e.emp_position == "Project Lead")
                .Select(e => new
                {
                    e.emp_no,
                    e.emp_fname,
                    e.emp_lname,
                    projectCount = e.NewProjects.Count(p => p.proj_status == "On-going") // Count of ongoing projects
        })
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEngineer()
        {
            nwTFSEntity db = new nwTFSEntity();

            // Get the session emp_no value
            int sessionEmpNo = (int)Session["emp_no"]; // Replace with the actual session emp_no value

            var employees = db.Employees
                .Where(e => e.emp_position == "Engineer")
                .Select(e => new {
                    e.emp_no,
                    e.emp_fname,
                    e.emp_lname,
                    selected = (e.emp_no == sessionEmpNo) // Add the "selected" property
        })
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEngineers()
        {
            nwTFSEntity db = new nwTFSEntity();

            // Get the session emp_no value
            int sessionEmpNo = (int)Session["emp_no"]; // Replace with the actual session emp_no value

            var employees = db.Employees
                .Where(e => e.emp_position == "Engineer")
                .Select(e => new {
                    e.emp_no,
                    e.emp_fname,
                    e.emp_lname,
                    selected = (e.emp_no == sessionEmpNo) // Add the "selected" property
                })
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeses()
        {
            nwTFSEntity db = new nwTFSEntity();

            var employees = db.Employees
                .Where(e => e.emp_position == "Project Lead")
                .Select(e => new { e.emp_no, e.emp_fname, e.emp_lname })
                .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

       /* [HttpPost]
        public ActionResult SaveDataAndReports(List<Dictionary<string, string>> data, List<string> images, string rep_scope, string rep_date, string rep_stats, int rep_proj_id, string rep_description)
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
                manpower.man_proj_id = rep_proj_id; // Set the man_proj_id property to the provided value
                manpower.man_date = DateTime.Parse(rep_date);

                // Save the NewManpower object to the database
                db.NewManpowers.Add(manpower);
                db.SaveChanges();

                // Create a new Attendance object
                var attendance = new Attendance();
                attendance.atte_id = Guid.NewGuid();
                attendance.atte_proj_id = rep_proj_id;
                attendance.atte_timein = DateTime.Parse(row["atte_timein"].ToString());
                attendance.atte_timeout = attendance.atte_timeout;
                attendance.NewManpower = manpower;

                // Save the Attendance object to the database
                db.Attendances.Add(attendance);
                db.SaveChanges();
            }

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

            // convert string values to their corresponding data types
            DateTime date = DateTime.Parse(rep_date);

            // create a new report object
            NewReport report = new NewReport
            {
                rep_id = Guid.NewGuid(),
                rep_emp_no = (int)Session["emp_no"],
                rep_proj_id = rep_proj_id,
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
        }*/

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
                    project.proj_emp_no = int.Parse(project.proj_lead); // Set proj_emp_no to the selected value from proj_lead
                    project.proj_type_id = projTypeId;
                    db.NewProjects.Add(project);
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Project saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving the project: " + ex.Message });
            }
        }
        public async Task<ActionResult> ProjectList()
        {
            nwTFSEntity db = new nwTFSEntity();
            var projects = db.NewProjects.ToList();
            return View(projects);
        }

        public ActionResult GetEmployeeData()
        {
            nwTFSEntity db = new nwTFSEntity();
            var engineers = db.Employees.Where(e => e.emp_position == "Engineer")
                                         .Select(e => new { emp_no = e.emp_no, emp_fname = e.emp_fname, emp_lname = e.emp_lname })
                                         .ToList();
            return Json(engineers, JsonRequestBehavior.AllowGet);
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
                var role = int.Parse(Session["system_role"].ToString());
                if (role == 3)
                {
                    return RedirectToAction("Unauthorize", "Error");
                }
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
                await SendNotif("notification");
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
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
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
        // add quantity update (Inv_update)
        private async void QuantityUpdate(string code, int quant, string unit)
        {
                using (var _context = new nwTFSEntity())
                {
                var item = _context.Inventories.Where(x => x.in_code == code).SingleOrDefault();
                        var newGuid = Guid.NewGuid();

                        Inv_Update update = new Inv_Update()
                        {
                            update_id = newGuid,
                            update_date = DateTime.Now,
                            update_item_id = item.in_code,
                            update_quantity = quant.ToString() + " " + unit,
                            update_type = "quantity"
                        };
                        _context.Inv_Update.Add(update);
                        await _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        public async Task<ActionResult> AllUpdates([System.Web.Http.FromUri] string code)
        {
            using(var _context = new nwTFSEntity())
            {
                var updates = _context.Inv_Update.Where(x => x.update_item_id == code).ToList();

                if(updates == null)
                {
                    return Json(new { message = "no data" });
                }

                using (var stream = new MemoryStream())
                {
                    var writer = new Utf8JsonWriter(stream);
                    writer.WriteStartArray();

                    foreach (var item in updates)
                    {
                        writer.WriteStartObject();
                        //writer.WriteString("in_guid", item.in_guid);
                        writer.WriteString("code", item.update_item_id);
                        writer.WriteString("date", item.update_date?.ToString("MMMM dd, yyyy"));
                        writer.WriteString("quantity", item.update_quantity);
                        writer.WriteString("name", item.Inventory.in_name);
                        // add more properties as needed
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.Flush();
                    var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                    var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                    return Content(jsonString);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddQuantity([System.Web.Http.FromUri] string code, [System.Web.Http.FromUri] int quantity)
        {

            using(var _context = new nwTFSEntity())
            {
                var cd = code.ToString();
                var item = _context.Inventories.Where(x => x.in_code == code).SingleOrDefault();
                var quant = int.Parse(item.in_quantity.Split(' ')[0]) + quantity;
                item.in_quantity = quant + " " + item.in_quantity.Split(' ')[1];
                _context.Entry(item);
                await _context.SaveChangesAsync();
                QuantityUpdate(code,quantity, item.in_quantity.Split(' ')[1]);
            }

            Session["added"] = "Item quantity added successfully";
            return Json(new { message = "okay"});
        }

        [HttpPost]
        public async Task<ActionResult> AddItem2(Inventory item)
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
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
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
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
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
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
            await SendNotif("notification");
            //await SendMessage("notification");
            return Json("Item Deleted");
        }

        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> Inventory()
        {

            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
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
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
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
            return RedirectToAction("Inventory");
        }

        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public async Task<ActionResult> InvArchive()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            return View();
        }

        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        public ActionResult InventoryReport()
        {
            var role = int.Parse(Session["system_role"].ToString());
            if (role == 3)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            ViewBag.ProfilePath = GetPath(int.Parse(empId));
            ViewBag.EmpId = empId;
            return View();
        }

        //[System.Web.Mvc.Authorize(Roles = "admin,warehouse")]
        
        #endregion

        #region Users
        //[System.Web.Http.Authorize(Roles = "admin")]
        public async Task<ActionResult> Users()
        {
            var empId = Session["emp_no"]?.ToString();
            if (empId == null)
            {
                return RedirectToAction("Login", "Base");
            }
            var role = int.Parse(Session["system_role"].ToString());
            if (role != 1)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            //Session["editUser"] = null;
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
            var role = int.Parse(Session["system_role"].ToString());
            if (role != 1)
            {
                return RedirectToAction("Unauthorize", "Error");
            }
            var serializedModel = JsonConvert.SerializeObject(employee);
            var userToken = Session["access_token"].ToString();
            string uri = "";
            if (employee.formType == "add")
            {
                uri = "Admin/Employee/Add";
                //message = "Added!";
            }
            if (employee.formType == "edit")
            {
                uri = "Admin/Employee/Update";
                //message = "Updated!";
            }
            var response = api_req.SetMethod(uri, userToken, serializedModel);
            //ViewBag.Message = message;
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
            if (json is JObject jsonObject)
            {
                Session["errorresp"] = jsonObject.GetValue("Message").ToString();
            }
            else
            {
                Session["success"] = json.ToString();
            }
            //await SendNotif("notification");
            return RedirectToAction("Users");
        }

        //[System.Web.Mvc.Authorize(Roles = "admin")]
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
                //var json = JsonConvert.DeserializeObject(response);
                //JsonResult result = Json(response, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
                //Response.ContentType = "application/json"; // Set the Content-Type header
                ViewBag.ProfilePath = GetPath(int.Parse(empId));
                if (response == "BadRequest")
                {
                    return RedirectToAction("BadRequest", "Error");
                }
                else if (response == "InternalServerError")
                {
                    return RedirectToAction("InternalServerError", "Error");
                }
                else if (response == "Unauthorized")
                {
                    return RedirectToAction("Unauthorize", "Error");
                }
                else
                {
                    return Content(response, "application/json");
                }
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
            if(jsonData.Select(x => x.request_status).FirstOrDefault().Trim().ToLower() == "approved")
            {
                await CreateNotif("request", jsonData.Select(x => x.request_type_id ).FirstOrDefault().ToString(), jsonData.Select(x => x.request_type).FirstOrDefault().ToString(), jsonData.Select(x => x.request_employee_id).FirstOrDefault());
            }
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