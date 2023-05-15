using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
//using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web.Http;
using TotalFireSafety.Models;
using ZXing.QrCode;

namespace TotalFireSafety.Controllers
{
    public class APIController : ApiController
    {
        //private readonly IHubContext<MyHub> _hubContext;

        //public APIController(IHubContext<MyHub> hubContext)
        //{
        //    _hubContext = hubContext;
        //}
        //private void NotifyClients()
        //{
        //     _hubContext.Clients.All.Send("updated");
        //}
        /*
         *  TO DO
         *  --AYUSIN ANG GetAll ng mga APIs
         */
        private Tuple<Guid, Boolean> Validate(string typeOf)
        {
            using (var _context = new nwTFSEntity())
            {
                var newGuid = Guid.NewGuid();
                switch (typeOf.ToLower())
                {
                    case "requests":
                        var checkId = _context.Requests.Where(x => x.request_id == newGuid).SingleOrDefault();

                        if (checkId == null)
                        {
                            return new Tuple<Guid, Boolean>(newGuid, true);
                        }
                        else
                        {
                            return Validate("requests");
                        }
                    case "additem":
                        var item = _context.Inv_Update.Where(x => x.update_id == newGuid).SingleOrDefault();
                        if (item == null)
                        {
                            return new Tuple<Guid, Boolean>(newGuid, true);
                        }
                        else
                        {
                            return Validate("additem");
                        }
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

        #region Status
        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Status")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public async Task<IHttpActionResult> GetAllEmployeeStatus()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var statuses = _context.Status.Select(x => x).ToList();
                    var employees = _context.Employees.Select(x => x).ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in statuses)
                        {
                            writer.WriteStartObject();
                            writer.WriteNumber("emp_no", item.emp_no);
                            writer.WriteNumber("IsActive", item.IsActive);
                            writer.WriteNumber("IsLocked", item.IsLocked);
                            writer.WriteNumber("IsUser", item.IsUser);
                            writer.WriteStartObject("Employee");
                            writer.WriteNumber("emp_no", item.Employee.emp_no);
                            writer.WriteString("emp_hiredDate", item.Employee.emp_hiredDate?.ToString("dd-MM-yyyyTHH:mm:ss"));
                            writer.WriteString("FormattedDate", item.Employee.emp_hiredDate?.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("emp_contact", item.Employee.emp_contact);
                            writer.WriteString("emp_fname", item.Employee.emp_fname);
                            writer.WriteString("emp_lname", item.Employee.emp_lname);
                            writer.WriteString("emp_mname", item.Employee.emp_name);
                            writer.WriteEndObject();

                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);

                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Status/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatusById(int id)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<Status>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Status/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddStatus(Status items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new nwTFSEntity())
                {
                    var roleExist = _context.Status.Where(x => x.emp_no == items.emp_no).SingleOrDefault();

                    if (roleExist == null)
                    {
                        _context.Status.Add(items);
                        _context.SaveChanges();
                        return Ok("Credential Added");
                    }
                    return BadRequest("Please verify your data");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Status/Edit")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatus(Status item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Status.Where(x => x.emp_no == item.emp_no).SingleOrDefault();

                    if (role == null)
                    {
                        return BadRequest("ID not found.");
                    }

                    role = new Status()
                    {
                        emp_no = item.emp_no,
                        IsActive = item.IsActive,
                        IsLocked = item.IsLocked
                    };
                    _context.Entry(role);
                    _context.SaveChanges();
                    return Ok("Status Updated");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Status/Delete")]  //  Update Employee Route
        [HttpPost]
        public async Task<IHttpActionResult> DeleteStatus(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("No ID found");
                }
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Status.Where(x => x.emp_no == id).SingleOrDefault();

                    if (role == null)
                    {
                        return BadRequest("Employee Not Found");
                    }

                    //role.status = "archived"; // edit db
                    _context.Entry(role);
                    _context.SaveChanges();
                    return Ok("Status Deleted");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Credentials
        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Credentials")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public async Task<IHttpActionResult> GetAllEmployeeCredentials()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var roles = _context.Credentials.Select(x => x).ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in roles)
                        {
                            writer.WriteStartObject();
                            writer.WriteNumber("emp_no", item.emp_no);
                            writer.WriteString("username", item.username);
                            writer.WriteString("password", item.password);
                            writer.WriteStartObject("Employee");
                            writer.WriteNumber("emp_no", item.Employee.emp_no);
                            writer.WriteString("emp_hiredDate", item.Employee.emp_hiredDate?.ToString("dd-MM-yyyyTHH:mm:ss"));
                            writer.WriteString("FormattedDate", item.Employee.emp_hiredDate?.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("emp_contact", item.Employee.emp_contact);
                            writer.WriteString("emp_fname", item.Employee.emp_fname);
                            writer.WriteString("emp_lname", item.Employee.emp_lname);
                            writer.WriteString("emp_mname", item.Employee.emp_name);
                            writer.WriteEndObject();

                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Credentials/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCredentialsById(int id)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<Credential>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Credentials/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCredentials(Credential items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new nwTFSEntity())
                {
                    var roleExist = _context.Credentials.Where(x => x.emp_no == items.emp_no).SingleOrDefault();

                    if (roleExist == null)
                    {
                        _context.Credentials.Add(items);
                        _context.SaveChanges();
                        return Ok("Credential Added");
                    }
                    return BadRequest("Please verify your data");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Credentials/Edit")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCredential(Credential item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == item.emp_no).SingleOrDefault();

                    if (role == null)
                    {
                        return BadRequest("ID not found.");
                    }

                    role = new Credential()
                    {
                        emp_no = item.emp_no,
                        username = item.username,
                        password = item.password
                    };
                    _context.Entry(role);
                    _context.SaveChanges();
                    return Ok("Credential Updated");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Credentials/Delete")]  //  Update Employee Route
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCredential(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("No ID found");
                }
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == id).SingleOrDefault();

                    if (role == null)
                    {
                        return BadRequest("Employee Not Found");
                    }

                    //role.status = "archived"; // edit db
                    _context.Entry(role);
                    _context.SaveChanges();
                    return Ok("Role Deleted");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Roles
        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Roles")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public async Task<IHttpActionResult> GetAllEmployeeRoles()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var roles = _context.Roles.Select(x => x).ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in roles)
                        {
                            writer.WriteStartObject();
                            writer.WriteNumber("emp_no", item.emp_no);
                            writer.WriteNumber("role", item.role1);
                            writer.WriteStartObject("Employee");
                            writer.WriteNumber("emp_no", item.Employee.emp_no);
                            writer.WriteString("emp_hiredDate", item.Employee.emp_hiredDate?.ToString("dd-MM-yyyyTHH:mm:ss"));
                            writer.WriteString("FormattedDate", item.Employee.emp_hiredDate?.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("emp_contact", item.Employee.emp_contact);
                            writer.WriteString("emp_fname", item.Employee.emp_fname);
                            writer.WriteString("emp_lname", item.Employee.emp_lname);
                            writer.WriteString("emp_mname", item.Employee.emp_name);
                            writer.WriteEndObject();

                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Roles/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRolesById(int id)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Roles.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<Role>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Roles/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddRoles(Role items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new nwTFSEntity())
                {
                    var roleExist = _context.Roles.Where(x => x.emp_no == items.emp_no).SingleOrDefault();

                    if (roleExist == null)
                    {
                        _context.Roles.Add(items);
                        _context.SaveChanges();
                        return Ok("Roles Added");
                    }
                    return BadRequest("Please verify your data");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Roles/Edit")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateRoles(Role item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new nwTFSEntity())
                {
                    var role = _context.Roles.Where(x => x.emp_no == item.emp_no).SingleOrDefault();

                    if (role == null)
                    {
                        return BadRequest("ID not found.");
                    }

                    role = new Role()
                    {
                        emp_no = item.emp_no,
                        role1 = item.role1
                    };
                    _context.Entry(role);
                    _context.SaveChanges();
                    return Ok("Role Updated");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        //[Route("Admin/Roles/Delete")]  //  Update Employee Route
        //[HttpPost]
        //public async Task<IHttpActionResult> DeleteRole(int? id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return BadRequest("No ID found");
        //        }
        //        using (var _context = new nwTFSEntity())
        //        {
        //            var role = _context.Roles.Where(x => x.emp_no == id).SingleOrDefault();

        //            if(role == null)
        //            {
        //                return BadRequest("Employee Not Found");
        //            }

        //            role.status = "archived";
        //            _context.Entry(role);
        //            _context.SaveChanges();
        //            return Ok("Role Deleted");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        #endregion

        #region Employees
        [System.Web.Http.Authorize]
        [Route("Admin/Employee")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public async Task<IHttpActionResult> GetAllEmployee()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var roles = _context.Employees.Include("Credential")
                                .Include("Role")
                                .Include("Status")
                                .ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in roles)
                        {
                            writer.WriteStartObject();
                            writer.WriteNumber("emp_no", item.emp_no);
                            writer.WriteString("emp_position", item.emp_position);
                            writer.WriteNumber("emp_no", item.emp_no);
                            writer.WriteString("emp_hiredDate", item.emp_hiredDate?.ToString("dd-MM-yyyyTHH:mm:ss"));
                            writer.WriteString("FormattedDate", item.emp_hiredDate?.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("emp_contact", item.emp_contact);
                            writer.WriteString("emp_fname", item.emp_fname);
                            writer.WriteString("emp_lname", item.emp_lname);
                            writer.WriteString("emp_name", item.emp_name);
                            if (item.Credential != null)
                            {
                                writer.WriteStartObject("Credential");
                                writer.WriteNumber("emp_no", item.Credential.emp_no);
                                writer.WriteString("username", item.Credential.username);
                                writer.WriteString("password", item.Credential.password);
                                writer.WriteEndObject();
                            }
                            else
                            {
                                writer.WriteStartObject("Credential");
                                writer.WriteNull("emp_no");
                                writer.WriteNull("username");
                                writer.WriteNull("password");
                                writer.WriteEndObject();
                            }
                            if (item.Role != null)
                            {
                                writer.WriteStartObject("Role");
                                writer.WriteNumber("emp_no", item.Role.emp_no);
                                writer.WriteNumber("role", item.Role.role1);
                                writer.WriteEndObject();
                            }
                            else
                            {
                                writer.WriteStartObject("Role");
                                writer.WriteNull("emp_no");
                                writer.WriteNull("role");
                                writer.WriteEndObject();
                            }
                            if (item.Status != null)
                            {
                                writer.WriteStartObject("Status");
                                writer.WriteNumber("emp_no", item.Status.emp_no);
                                writer.WriteNumber("IsActive", item.Status.IsActive);
                                writer.WriteNumber("IsLocked", item.Status.IsLocked);
                                writer.WriteNumber("IsUser", item.Status.IsUser);
                                writer.WriteEndObject();
                            }
                            else
                            {
                                writer.WriteStartObject("Status");
                                writer.WriteNull("emp_no");
                                writer.WriteNull("IsActive");
                                writer.WriteNull("IsLocked");
                                writer.WriteNull("IsUser");
                                writer.WriteEndObject();
                            }
                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Employee/{val}")]  //  Get Employee by any value Route
        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeById(string val)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    int _id = 0;
                    var isNumeric = int.TryParse(val, out _);
                    if (isNumeric)
                    {
                        _id = int.Parse(val);
                    }

                    var user = _context.Employees.Where(x => x.emp_fname == val || x.emp_no == _id || x.emp_hiredDate.ToString().Contains(val)).SingleOrDefault();

                    if (user != null)
                    {
                        var _serialize = JsonConvert.SerializeObject(user, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                        var _deserialize = JsonConvert.DeserializeObject(_serialize);
                        return Ok(_deserialize);
                    }
                    return BadRequest("Employee Not Found");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Employee/Add")]  //  Add Employee Route
        [HttpPost]
        public async Task<IHttpActionResult> AddEemployee(Employee _emp)
        {
            try
            {
                if (_emp != null)
                {
                    using (var _context = new nwTFSEntity())
                    {
                        var employee = _context.Employees.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var cre = _context.Credentials.Where(x => x.username == _emp.Credential.username).FirstOrDefault();
                        if (employee != null)
                        {
                            return BadRequest("ID is already in used");
                        }
                        if (cre != null)
                        {
                            return BadRequest("Username is already in used");
                        }
                        _context.Employees.Add(_emp);
                        _context.SaveChanges();
                        return Ok("Employee details added successfully");
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin")]
        [Route("Admin/Employee/Update")]  //  Update Employee Route
        [HttpPost]
        public async Task<IHttpActionResult> UpdateEmployee(Employee _emp)
        {
            try
            {
                if (_emp != null)
                {
                    using (var _context = new nwTFSEntity())
                    {
                        var emp = _context.Employees.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var roles = _context.Roles.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var status = _context.Status.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var creds = _context.Credentials.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        if (emp != null)
                        {
                            emp.emp_no = _emp.emp_no;
                            emp.emp_fname = _emp.emp_fname;
                            emp.emp_name = _emp.emp_name;
                            emp.emp_lname = _emp.emp_lname;
                            emp.emp_contact = _emp.emp_contact;
                            emp.emp_hiredDate = _emp.emp_hiredDate;
                            emp.emp_position = _emp.emp_position;
                            roles.emp_no = _emp.emp_no;
                            roles.role1 = _emp.Role.role1;
                            status.emp_no = _emp.emp_no;
                            status.IsActive = _emp.Status.IsActive;
                            status.IsLocked = _emp.Status.IsLocked;
                            creds.emp_no = _emp.emp_no;
                            creds.username = _emp.Credential.username;
                            creds.password = _emp.Credential.password;

                            _context.Entry(creds);
                            _context.Entry(status);
                            _context.Entry(roles);
                            _context.Entry(emp);
                            _context.SaveChanges();
                            return Ok("Employee Added");
                        }
                        return BadRequest("tite");
                    }
                }
                return BadRequest("tite11");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
        #endregion

        #region Inventory

        private async Task<IHttpActionResult> UpdateNewItem(Inventory item)
        {
            using (var _context = new nwTFSEntity())
            {
                try
                {
                    var newGUID = GetNewGuid("AddItem");


                    Inv_Update update = new Inv_Update()
                    {
                        update_id = newGUID,
                        update_date = DateTime.Now,
                        update_item_id = item.in_code,
                        update_quantity = item.in_quantity,
                        update_type = "newitem"
                    };
                    _context.Inv_Update.Add(update);
                    _context.SaveChanges();

                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
                return Ok();
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "warehouse,admin,owner")]
        [Route("Warehouse/Inventory/Updates")]
        [HttpGet]
        public async Task<IHttpActionResult> AddedItemInfo()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var invItems = _context.Inventories.Select(x => x).ToList();
                    var reports = _context.Inv_Update.Select(x => x).ToList();
                    var newList = new List<Inv_Update>();

                    foreach (var item in reports)
                    {
                        var itemReports = invItems.Where(x => x.in_code == item.update_item_id).SingleOrDefault();
                        var newReport = new Inv_Update()
                        {
                            update_id = item.update_id,
                            update_item_id = item.update_item_id,
                            update_quantity = item.update_quantity,
                            update_date = item.update_date,
                            update_type = item.update_type,

                            Inventory = itemReports?.in_code == item.update_item_id ? new Inventory
                            {
                                in_code = itemReports.in_code,
                                in_name = itemReports.in_name,
                                in_category = itemReports.in_category,
                                in_type = itemReports.in_type,
                                in_size = itemReports.in_size,
                                in_status = itemReports.in_status,
                                in_class = itemReports.in_class
                            } : null
                        };
                        newList.Add(newReport);
                    }

                    var _jsonSerialized = JsonConvert.SerializeObject(newList, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    //  Deserialize the serialized json format to remove the escape characters like \ 
                    var _jsonDeserialized = JsonConvert.DeserializeObject<List<Inv_Update>>(_jsonSerialized);

                    return Ok(_jsonDeserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "warehouse,admin,owner")]
        [Route("Warehouse/Inventory/Status")]
        [HttpPost]
        public async Task<IHttpActionResult> RestoreItem(Inventory val)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var item = _context.Inventories.Where(x => x.in_code == val.in_code).FirstOrDefault();
                    if (item == null)
                    {
                        return BadRequest();
                    }
                    item.in_status = "active";
                    item.in_arch_date = null;
                    _context.SaveChanges();
                    _context.Entry(item);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "warehouse,admin,owner")]
        [Route("Warehouse/Inventory/Archive")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllArchivedItem()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    //var _jsonSerialized = JsonConvert.SerializeObject(items, Formatting.None, new JsonSerializerSettings()
                    //{
                    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //});
                    //  Deserialize the serialized json format to remove the escape characters like \ 
                    //var _jsonDeserialized = JsonConvert.DeserializeObject<List<Inventory>>(_jsonSerialized);

                    var items = _context.Inventories.Where(x => x.in_status == "archived").ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in items)
                        {
                            var arch_date = item.in_arch_date?.ToString("MMMM dd, yyyy");
                            writer.WriteStartObject();
                            //writer.WriteString("in_guid", item.in_guid);
                            writer.WriteString("in_code", item.in_code);
                            writer.WriteString("in_name", item.in_name);
                            writer.WriteString("FormattedDate", arch_date);
                            writer.WriteString("in_quantity", item.in_quantity);
                            writer.WriteString("in_category", item.in_category);
                            writer.WriteString("in_type", item.in_type);
                            writer.WriteString("in_size", item.in_size);
                            writer.WriteString("in_type", item.in_status);
                            writer.WriteString("in_remarks", item.in_remarks.Trim());
                            writer.WriteString("in_class", item.in_class);
                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.Authorize]
        [Route("Warehouse/Inventory")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllItem()
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var items = _context.Inventories
                                .Include("Inv_Limits")
                                .Where(x => x.in_status.Trim() != "archived")
                                .ToList();
                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in items)
                        {
                            writer.WriteStartObject();
                            //writer.WriteString("in_guid", item.in_guid);
                            writer.WriteString("in_code", item.in_code);
                            writer.WriteString("in_name", item.in_name);
                            writer.WriteString("in_dateAdded", item.in_dateAdded);
                            writer.WriteString("in_quantity", item.in_quantity);
                            writer.WriteString("in_category", item.in_category);
                            writer.WriteString("in_type", item.in_type);
                            writer.WriteString("in_size", item.in_size);
                            writer.WriteString("in_status", item.in_status);
                            writer.WriteString("in_remarks", item.in_remarks.Trim());
                            writer.WriteString("in_class", item.in_class);
                            writer.WriteStartObject("Limits");
                            if(item.Inv_Limits != null)
                            {
                                writer.WriteNumber("maximum", item.Inv_Limits.maximum);
                                writer.WriteNumber("minimum", item.Inv_Limits.minimum);
                                writer.WriteNumber("reorder", item.Inv_Limits.reorder);
                                writer.WriteNumber("running", item.Inv_Limits.running ?? 0);
                            }
                            else
                            {
                                writer.WriteNull("maximum");
                                writer.WriteNull("minimum");
                                writer.WriteNull("reorder");
                                writer.WriteNull("running");
                            }
                            writer.WriteEndObject();
                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "warehouse,admin,owner")]
        [Route("Warehouse/Inventory/{val}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetItemByValue(string val)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    //  Check if the value of val is an integer
                    int id = 0;
                    var isNumeric = int.TryParse(val, out _); // return boolean
                    if (isNumeric)
                    {
                        id = int.Parse(val); // parse val into integer
                    }
                    var items = _context.Inventories.Where(x => x.in_code.Contains(val) || x.in_name.Contains(val) || x.in_category.Contains(val) || x.in_type.Contains(val) && x.in_status != "archived").ToList();

                    if (items != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var writer = new Utf8JsonWriter(stream);
                            writer.WriteStartArray();

                            foreach (var item in items)
                            {
                                writer.WriteStartObject();
                                //writer.WriteString("in_guid", item.in_guid);
                                writer.WriteString("in_code", item.in_code);
                                writer.WriteString("in_name", item.in_name);
                                writer.WriteString("in_dateAdded", item.in_dateAdded);
                                writer.WriteString("in_quantity", item.in_quantity);
                                writer.WriteString("in_category", item.in_category);
                                writer.WriteString("in_type", item.in_type);
                                writer.WriteString("in_size", item.in_size);
                                writer.WriteString("in_type", item.in_status);
                                writer.WriteString("in_remarks", item.in_remarks.Trim());
                                writer.WriteString("in_class", item.in_class);
                                // add more properties as needed
                                writer.WriteEndObject();
                            }

                            writer.WriteEndArray();
                            writer.Flush();
                            var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                            var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                            return Ok(_jsonDeserialized);
                            //var _SerializedJson = JsonConvert.SerializeObject(user, Formatting.None, new JsonSerializerSettings()
                            //{
                            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            //});
                            //var _DeserializedJson = JsonConvert.DeserializeObject(_SerializedJson);
                            //return Ok(_DeserializedJson);
                        }
                    }
                    return BadRequest("Item Not Found");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddItem(Inventory _item)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var newGuid = GetNewGuid("newitem");
                    if (_item != null)
                    {

                        var quantity = int.Parse(_item.in_quantity.Split(' ')[0]);

                        if (quantity >= 100)
                        {
                            _item.in_remarks = "overstock";
                        }
                        else if (quantity <= 99 && quantity >= 50)
                        {
                            _item.in_remarks = "standard";
                        }
                        else if (quantity <= 49 && quantity >= 30)
                        {
                            _item.in_remarks = "average";
                        }
                        else if (quantity <= 29 && quantity >= 10)
                        {
                            _item.in_remarks = "reorder";
                        }
                        else if (quantity <= 9)
                        {
                            _item.in_remarks = "critical";
                        }

                        var response = UpdateNewItem(_item);
                        _item.in_guid = newGuid;
                        _context.Inventories.Add(_item);
                        _context.SaveChanges();


                        return Ok("Item Added");
                    }
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Edit")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateItem(Inventory _item)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    /*  Check if _item is null
                     *  I use this than ModelState.IsValid because it throws Exception when there is a null
                     */
                    if (_item != null)
                    {
                        var itemDB = _context.Inventories.Where(x => x.in_code == _item.in_code || x.in_code == _item.formType).SingleOrDefault();
                        var ups = _context.Inv_Update.Where(x => x.update_item_id == _item.in_code || x.update_item_id == _item.formType).ToList();
                        var reqs = _context.Requests.Where(x => x.request_item == _item.in_code || x.request_item == _item.formType).ToList();

                        if (itemDB != null)
                        {

                            var quantity = int.Parse(_item.in_quantity.Split(' ')[0]);

                            if (quantity >= 100)
                            {
                                itemDB.in_remarks = "overstock";
                            }
                            else if (quantity <= 99 && quantity >= 50)
                            {
                                itemDB.in_remarks = "standard";
                            }
                            else if (quantity <= 49 && quantity >= 30)
                            {
                                itemDB.in_remarks = "average";
                            }
                            else if (quantity <= 29 && quantity >= 10)
                            {
                                itemDB.in_remarks = "reorder";
                            }
                            else if (quantity <= 9)
                            {
                                itemDB.in_remarks = "critical";
                            }

                            //  Convert _item into itemDB
                            itemDB.in_name = _item.in_name;
                            itemDB.in_category = _item.in_category;
                            itemDB.in_type = _item.in_type;
                            itemDB.in_size = _item.in_size;
                            itemDB.in_quantity = _item.in_quantity;
                            await UpdateNewItem(itemDB);
                            //if(_item.formType != null)
                            //{
                            //    itemDB.in_code = _item.in_code;
                            //    foreach (var item in ups)
                            //    {
                            //        item.update_item_id = _item.in_code;
                            //    }
                            //    foreach (var item in reqs)
                            //    {
                            //        item.request_item = _item.in_code;
                            //    }
                            //}
                            _context.Entry(itemDB);
                            //_context.Entry(reqs);
                            //_context.Entry(ups);
                            await _context.SaveChangesAsync();     //  Save to Database


                            return Ok("Process Completed!");
                        }
                        return BadRequest();
                    }
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.AuthorizeAttribute(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Delete")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteItem(Inventory codeToFind)
        {
            try
            {
                var item = codeToFind.in_code;
                using (var _context = new nwTFSEntity())
                {
                    var _item = _context.Inventories.Where(x => x.in_code == item).SingleOrDefault();
                    if (_item != null)
                    {
                        _item.in_status = "archived";
                        _item.in_arch_date = DateTime.Now;
                        _context.Entry(_item);
                        _context.SaveChanges();


                        return Ok("Item Deleted Successfully!");
                    }
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Barcode")]
        [HttpPost]
        // Barcorde Generator
        public async Task<IHttpActionResult> GenerateBarcode(string value)
        {
            Byte[] byteArray;
            var width = 350; // width of the Qr Code
            var height = 100; // height of the Qr Code
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(value);

            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to stream as PNG
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }

            // convert byte array to base64-encoded string
            string base64String = Convert.ToBase64String(byteArray);

            return Ok(base64String);
        }
        #endregion

        #region Request
        [System.Web.Http.AuthorizeAttribute]
        [Route("Requests/All")]
        [HttpGet]
        public async Task<IHttpActionResult> AllRequests()
        {
            try
            {

                using (var _context = new nwTFSEntity())
                {
                    var roles = _context.Requests
                        .Include("Employee")
                        .Include("Inventory")
                        .ToList();

                    using (var stream = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(stream);
                        writer.WriteStartArray();

                        foreach (var item in roles)
                        {
                            string newID = "";
                            if (item.request_type.ToLower() == "purchase")
                            {
                                newID = "PUR" + item.request_type_id;
                            }
                            if (item.request_type.ToLower() == "deploy" || item.request_type.ToLower() == "deployment")
                            {
                                newID = "DEP" + (item.request_type_id);
                            }
                            if (item.request_type.ToLower() == "supply")
                            {
                                newID = "SUP" + item.request_type_id;
                            }
                            writer.WriteStartObject();
                            writer.WriteString("request_id", item.request_id);
                            writer.WriteString("request_type", item.request_type);
                            writer.WriteString("request_date", item.request_date);
                            writer.WriteString("Id", newID);
                            writer.WriteString("FormattedDate", item.request_date.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("request_employee_id", int.Parse(item.request_employee_id.ToString()));
                            writer.WriteString("request_item", item.request_item);
                            writer.WriteString("request_item_quantity", item.request_item_quantity);
                            writer.WriteString("request_status", item.request_status.Trim());
                            writer.WriteNumber("request_type_id", item.request_type_id);
                            writer.WriteString("request_type_status", item.request_type_status);
                            writer.WriteStartObject("Employee");
                            writer.WriteNumber("emp_no", item.Employee.emp_no);
                            writer.WriteString("emp_hiredDate", item.Employee.emp_hiredDate?.ToString("dd-MM-yyyyTHH:mm:ss"));
                            writer.WriteString("FormattedDate", item.Employee.emp_hiredDate?.ToString("MMMM dd, yyyy"));
                            writer.WriteNumber("emp_contact", item.Employee.emp_contact);
                            writer.WriteString("emp_fname", item.Employee.emp_fname);
                            writer.WriteString("emp_lname", item.Employee.emp_lname);
                            writer.WriteString("emp_mname", item.Employee.emp_name);
                            writer.WriteString("emp_position", item.Employee.emp_position);
                            writer.WriteEndObject();
                            writer.WriteStartObject("Inventory");
                            writer.WriteString("in_code", item.Inventory.in_code);
                            writer.WriteString("in_category", item.Inventory.in_category);
                            writer.WriteString("in_class", item.Inventory.in_class);
                            writer.WriteString("in_name", item.Inventory.in_name);
                            writer.WriteString("in_quantity", item.Inventory.in_quantity);
                            writer.WriteString("in_size", item.Inventory.in_size);
                            writer.WriteString("in_type", item.Inventory.in_type);
                            writer.WriteEndObject();

                            // add more properties as needed
                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.Flush();
                        var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                        var _jsonDeserialized = JsonConvert.DeserializeObject(jsonString);
                        return Ok(_jsonDeserialized);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public Guid GetNewGuid(string typeOf)
        {
            var newId = Validate(typeOf);
            if (newId.Item2)
            {
                return newId.Item1;
            }
            else
            {
                return GetNewGuid(typeOf);
            }
        }

        [System.Web.Http.AuthorizeAttribute]
        [Route("Requests/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddRequests(List<Request> req)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    if (req == null)
                    {
                        return BadRequest("Please check your data.");
                    }
                    foreach (var item in req)
                    {
                        var checkId = GetNewGuid("requests");
                        item.request_id = checkId;
                        Request newReqs = new Request()
                        {
                            request_date = item.request_date,
                            request_employee_id = item.request_employee_id,
                            request_id = item.request_id,
                            request_item = item.request_item,
                            request_item_quantity = item.request_item_quantity,
                            request_status = "On Process",
                            request_type = item.request_type,
                            request_type_id = item.request_type_id,
                            request_type_status = "Active",
                            request_proj_id = item.request_proj_id
                        };
                        _context.Requests.Add(newReqs);
                        _context.SaveChanges();
                        
                    }
                    return Ok("Requests Added Successfully!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AuthorizeAttribute]
        [Route("Requests/Edit")]
        [HttpPost]
        public async Task<IHttpActionResult> EditRequests(List<Request> req)
        {
            try
            {
                using (var _context = new nwTFSEntity())
                {
                    var maxId = _context.Requests.Max(x => x.request_type_id);
                    foreach (var item in req)
                    {
                        var request = _context.Requests.Where(x => x.request_id == item.request_id).SingleOrDefault();
                        if (request == null)
                        {
                            return BadRequest("Request Not Found");
                        }
                        // duplicate supply as deployment
                        if (item.request_status.Trim().ToLower() == "approved" && item.request_type.Trim().ToLower() == "supply")
                        {
                            var newGuid = Validate("requests");
                            Request newReq = new Request()
                            {
                                request_type = "Deployment",
                                request_status = "On Process",
                                request_date = DateTime.Now,
                                request_employee_id = item.request_employee_id,
                                request_id = newGuid.Item1,
                                request_item = item.request_item,
                                request_item_quantity = item.request_item_quantity,
                                request_type_id = maxId + 1,
                                request_type_status = "Active"
                            };
                           
                            _context.Requests.Add(newReq);
                            await _context.SaveChangesAsync();
                        }

                        if(item.request_status.Trim().ToLower() == "approved" && item.request_type.Trim().ToLower() == "deployment")
                        {
                            var _item = _context.Inventories.Where(x => x.in_code == item.request_item).SingleOrDefault();

                            var itemQuant = int.Parse(_item.in_quantity.Split(' ')[0]);
                            var reqQuant = int.Parse(item.request_item_quantity.Split(' ')[0]);
                            var unit = item.request_item_quantity.Split(' ')[1];

                            _item.in_quantity = (itemQuant - reqQuant) + " " + unit;
                            //_item.Inv_Limits.running = _item.Inv_Limits.running - reqQuant;

                            //var inv = _context.Inventories.Where(x => item.request_item == x.in_code).SingleOrDefault();
                            var runn = _item.Inv_Limits?.running;
                            var quant = int.Parse(item.request_item_quantity.Split(' ')[0]);

                            var result = 0;
                            if (runn == null)
                            {
                                result = int.Parse(item.request_item_quantity.Split(' ')[0]);
                            }
                            else
                            {
                                result = runn + quant ?? 0;
                            }
                            _item.Inv_Limits.running = result;

                            _context.Entry(_item);
                            await _context.SaveChangesAsync();
                        }

                        request.request_status = item.request_status;
                        request.request_type = item.request_type;
                        request.request_item = item.request_item;
                        request.request_item_quantity = item.request_item_quantity;
                        request.request_type_status = item.request_type_status;
                        _context.Entry(request);
                        _context.SaveChanges();
                    }
                    return Ok("Requests Updated Successfully!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //public async Task<IHttpActionResult> DeleteRequests(Guid req)
        //{
        //    try
        //    {
        //        using (var _context = new nwTFSEntity())
        //        {
        //            var request = _context.Requests.Where(x => x.request_id == req).SingleOrDefault();
        //            if (request == null)
        //            {
        //                return BadRequest("Request Not Found");
        //            }
        //            request.request_status = "archived";
        //            _context.Entry(request);
        //            _context.SaveChanges();
        //            return Ok("Requests Deleted Successfully!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
        #endregion

       //[AllowAnonymous]
       //[HttpGet]
       // [Route("EditInventory")]
       // public async Task<IHttpActionResult> EditInv()
       // {
       //     try
       //     {
       //         using (var _context = new nwTFSEntity())
       //         {
       //             var allItems = _context.Inventories.ToList();

       //             foreach (var item in allItems)
       //             {
       //                 var quantity = int.Parse(item.in_quantity.Split(' ')[0]);

       //                 if (quantity >= 100)
       //                 {
       //                     item.in_remarks = "overstock";
       //                 }
       //                 else if (quantity <= 99 && quantity >= 50)
       //                 {
       //                     item.in_remarks = "standard";
       //                 }
       //                 else if (quantity <= 49 && quantity >= 30)
       //                 {
       //                     item.in_remarks = "average";
       //                 }
       //                 else if (quantity <= 29 && quantity >= 10)
       //                 {
       //                     item.in_remarks = "reorder";
       //                 }
       //                 else if (quantity <= 9)
       //                 {
       //                     item.in_remarks = "critical";
       //                 }
       //                 _context.Entry(item);
       //                 await _context.SaveChangesAsync();
       //             }
       //                 return Ok("Successful");
       //             //return BadRequest();
       //         }
       //     }
       //     catch(Exception ex)
       //     {
       //         return InternalServerError(ex);
       //     }
       // }
    }
}