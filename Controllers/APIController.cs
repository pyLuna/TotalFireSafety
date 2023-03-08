using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TotalFireSafety.Models;
using System.IO;
using ZXing;
using ZXing.QrCode;
//using System.Web.Mvc;
using System.Drawing;

namespace TotalFireSafety.Controllers
{
    public class APIController : ApiController
    {
        private Tuple<Guid, Boolean> Validate(string typeOf)
        {
            using (var _context = new TFSEntity())
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
                    case "done":
                        return new Tuple<Guid, Boolean>(newGuid, false);
                }
                return Validate("done");
            }
        }
        #region Status
        [Authorize(Roles = "admin")]
        [Route("Admin/Status")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public IHttpActionResult GetAllEmployeeStatus()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var roles = _context.Status.Select(x => x).ToList();

                    var _jsonSerialized = JsonConvert.SerializeObject(roles, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    var employee = new Employee();

                    var _Deserialized = JsonConvert.DeserializeObject<List<Status>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Status/{id}")]
        [HttpGet]
        public IHttpActionResult GetStatusById(int id)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Status>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Status/Add")]
        [HttpPost]
        public IHttpActionResult AddStatus(Status items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new TFSEntity())
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

        [Authorize(Roles = "admin")]
        [Route("Admin/Status/Edit")]
        [HttpPost]
        public IHttpActionResult UpdateStatus(Status item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new TFSEntity())
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

        [Authorize(Roles = "admin")]
        [Route("Admin/Status/Delete")]  //  Update Employee Route
        [HttpPost]
        public IHttpActionResult DeleteStatus(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("No ID found");
                }
                using (var _context = new TFSEntity())
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
        [Authorize(Roles = "admin")]
        [Route("Admin/Credentials")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public IHttpActionResult GetAllEmployeeCredentials()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var roles = _context.Roles.Select(x => x).ToList();


                    var _jsonSerialized = JsonConvert.SerializeObject(roles, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Credential>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Credentials/{id}")]
        [HttpGet]
        public IHttpActionResult GetCredentialsById(int id)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var role = _context.Credentials.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Credential>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Credentials/Add")]
        [HttpPost]
        public IHttpActionResult AddCredentials(Credential items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new TFSEntity())
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

        [Authorize(Roles = "admin")]
        [Route("Admin/Credentials/Edit")]
        [HttpPost]
        public IHttpActionResult UpdateCredential(Credential item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new TFSEntity())
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

        [Authorize(Roles = "admin")]
        [Route("Admin/Credentials/Delete")]  //  Update Employee Route
        [HttpPost]
        public IHttpActionResult DeleteCredential(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("No ID found");
                }
                using (var _context = new TFSEntity())
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
        [Authorize(Roles = "admin")]
        [Route("Admin/Roles")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public IHttpActionResult GetAllEmployeeRoles()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var roles = _context.Roles.Select(x => x).ToList();


                    var _jsonSerialized = JsonConvert.SerializeObject(roles, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Role>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Roles/{id}")]
        [HttpGet]
        public IHttpActionResult GetRolesById(int id)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var role = _context.Roles.Where(x => x.emp_no == id).SingleOrDefault();

                    var _jsonSerialized = JsonConvert.SerializeObject(role, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Role>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Roles/Add")]
        [HttpPost]
        public IHttpActionResult AddRoles(Role items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest("Please verify your data");
                }
                using (var _context = new TFSEntity())
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

            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Roles/Edit")]
        [HttpPost]
        public IHttpActionResult UpdateRoles(Role item)
        {
            try
            {
                if(item == null)
                {
                    return BadRequest("Please verify your data.");
                }
                using (var _context = new TFSEntity())
                    {
                        var role = _context.Roles.Where(x => x.emp_no == item.emp_no).SingleOrDefault();

                        if(role == null)
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

        [Authorize(Roles = "admin")]
        [Route("Admin/Roles/Delete")]  //  Update Employee Route
        [HttpPost]
        public IHttpActionResult DeleteRole(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("No ID found");
                }
                using (var _context = new TFSEntity())
                {
                    var role = _context.Roles.Where(x => x.emp_no == id).SingleOrDefault();

                    if(role == null)
                    {
                        return BadRequest("Employee Not Found");
                    }

                    role.status = "archived";
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
        
        #region Employees
        [Authorize(Roles = "admin")]
        [Route("Admin/Employee")]  //  Get all Employee Route
        [HttpGet]
        // GET api/<controller>
        public IHttpActionResult GetAllEmployee()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var users = _context.Employees.Select(x => x).ToList();
                    var status = _context.Status.Select(x => x).ToList();
                    var roles = _context.Roles.Select(x => x).ToList();
                    var creds = _context.Credentials.Select(x => x).ToList();

                    var newList = new List<Employee>();

                    foreach (var item in users)
                    {
                        var newCreds = creds.Where(x => x.emp_no == item.emp_no).SingleOrDefault();
                        var newRoles = roles.Where(x => x.emp_no == item.emp_no).SingleOrDefault();
                        var newStatus = status.Where(x => x.emp_no == item.emp_no).SingleOrDefault();
                        var user = new Employee()
                        {
                            emp_no = item.emp_no,
                            emp_contact = item.emp_contact,
                            emp_hiredDate = item.emp_hiredDate,
                            emp_name = item.emp_name,
                            emp_position = item.emp_position,

                            Credential = newCreds?.emp_no == item.emp_no? new Credential { 
                                emp_no = newCreds.emp_no,
                                username = newCreds.username,
                                password = newCreds.password
                            } : null,
                            Role = newRoles?.emp_no == item.emp_no ? new Role
                            {
                                emp_no = newRoles.emp_no,
                                status = newRoles.status,
                                role1 = newRoles.role1,
                                date = newRoles.date
                            } : null,
                            Status = newStatus?.emp_no == item.emp_no ? new Status
                            {
                                emp_no = newStatus.emp_no,
                                IsActive = newStatus.IsActive,
                                IsLocked = newStatus.IsLocked
                            } : null
                        };
                        newList.Add(user);
                    }

                    var _jsonSerialized = JsonConvert.SerializeObject(newList, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<Employee>>(_jsonSerialized);
                    return Ok(_Deserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Employee/{val}")]  //  Get Employee by any value Route
        [HttpGet]
        public IHttpActionResult GetEmployeeById(string val)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    int _id = 0;
                    var isNumeric = int.TryParse(val, out _);
                    if (isNumeric)
                    {
                        _id = int.Parse(val);
                    }

                    var user = _context.Employees.Where(x => x.emp_name == val || x.emp_no == _id || x.emp_hiredDate.ToString().Contains(val)).SingleOrDefault();

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

        [Authorize(Roles = "admin")]
        [Route("Admin/Employee/Add")]  //  Add Employee Route
        [HttpPost]
        public IHttpActionResult AddEemployee(Employee _emp)
        {
            try
            {
                if (_emp != null)
                {
                    using (var _context = new TFSEntity())
                    {
                        var employee = _context.Employees.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        if (employee == null)
                        {
                            Employee emp = new Employee()
                            {
                                emp_no = _emp.emp_no,
                                emp_contact = _emp.emp_contact,
                                emp_hiredDate = _emp.emp_hiredDate,
                                emp_name = _emp.emp_name,
                                emp_position = _emp.emp_position
                            };

                            _context.Employees.Add(_emp);
                            _context.SaveChanges();
                            return Ok("Employee Added");
                        }
                        return BadRequest("ID is already in used.");
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("Admin/Employee/Update")]  //  Update Employee Route
        [HttpPost]
        public IHttpActionResult UpdateEmployee(Employee _emp)
        {
            try
            {
                if (_emp != null)
                {
                    using (var _context = new TFSEntity())
                    {
                        var emp = _context.Employees.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var roles = _context.Roles.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var status = _context.Status.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        var creds = _context.Credentials.Where(x => x.emp_no == _emp.emp_no).SingleOrDefault();
                        if (emp != null)
                        {
                            emp.emp_no = _emp.emp_no;
                            emp.emp_name = _emp.emp_name;
                            emp.emp_contact = _emp.emp_contact;
                            emp.emp_hiredDate = _emp.emp_hiredDate;
                            emp.emp_position = _emp.emp_position;

                            roles.role1 = _emp.Role.role1;
                            status.IsActive = _emp.Status.IsActive;
                            status.IsLocked = _emp.Status.IsLocked;
                            creds.username = _emp.Credential.username;
                            creds.password = _emp.Credential.password;

                            _context.Entry(creds);
                            _context.Entry(status);
                            _context.Entry(roles);
                            _context.Entry(emp);
                            _context.SaveChanges();
                            return Ok("Employee Added");
                        }
                        return BadRequest();
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
        #endregion

        #region Inventory

        private IHttpActionResult UpdateNewItem(Inventory item)
        {
            using (var _context = new TFSEntity())
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
                catch(Exception ex)
                {
                    return InternalServerError(ex);
                }
                return Ok();
            }
        }
        [Authorize(Roles = "warehouse,admin")]
        [Route("Warehouse/Inventory/Updates")]
        [HttpGet]
        public IHttpActionResult AddedItemInfo()
        {
            try
            {
                using (var _context = new TFSEntity())
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
        [Authorize(Roles = "warehouse,admin")]
        [Route("Warehouse/Inventory/Status")]
        [HttpPost]
        public IHttpActionResult RestoreItem(Inventory val)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var item = _context.Inventories.Where(x => x.in_code == val.in_code).FirstOrDefault();
                    if(item == null)
                    {
                        return BadRequest();
                    }
                    item.in_status = "active";
                    item.in_arch_date = null;
                    _context.Entry(item);
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Authorize(Roles = "warehouse,admin")]
        [Route("Warehouse/Inventory/Archive")]
        [HttpGet]
        public IHttpActionResult GetAllArchivedItem()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var items = _context.Inventories.Where(x => x.in_status == "archived");

                    var _jsonSerialized = JsonConvert.SerializeObject(items, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    //  Deserialize the serialized json format to remove the escape characters like \ 
                    var _jsonDeserialized = JsonConvert.DeserializeObject<List<Inventory>>(_jsonSerialized);

                    return Ok(_jsonDeserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Authorize(Roles = "warehouse,admin")]
        [Route("Warehouse/Inventory")]
        [HttpGet]
        public IHttpActionResult GetAllItem()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var items = _context.Inventories.Where(x => x.in_status != "archived");
                    List<Inventory> newList = new List<Inventory>();

                    foreach (var item in items)
                    {
                        Inventory newItems = new Inventory()
                        {
                            in_code = item.in_code,
                            in_category = item.in_category,
                            in_arch_date = item.in_arch_date,
                            in_class = item.in_class,
                            in_dateAdded = item.in_dateAdded,
                            in_name = item.in_name,
                            in_quantity = item.in_quantity,
                            in_size = item.in_size,
                            in_status = item.in_status,
                            in_type = item.in_type
                        };
                        newList.Add(newItems);
                    }

                    var _jsonSerialized = JsonConvert.SerializeObject(newList, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    //  Deserialize the serialized json format to remove the escape characters like \ 
                    var _jsonDeserialized = JsonConvert.DeserializeObject<List<Inventory>>(_jsonSerialized);

                    return Ok(_jsonDeserialized);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Authorize(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/{val}")]
        [HttpGet]
        public IHttpActionResult GetItemByValue(string val)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    //  Check if the value of val is an integer
                    int id = 0;
                    var isNumeric = int.TryParse(val, out _); // return boolean
                    if (isNumeric)
                    {
                        id = int.Parse(val); // parse val into integer
                    }
                    var user = _context.Inventories.Where(x => x.in_code.Contains(val) || x.in_name.Contains(val) || x.in_category.Contains(val) || x.in_type.Contains(val) && x.in_status != "archived");

                    if (user != null)
                    {
                        var _SerializedJson = JsonConvert.SerializeObject(user, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                        var _DeserializedJson = JsonConvert.DeserializeObject(_SerializedJson);
                        return Ok(_DeserializedJson);
                    }
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Authorize(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Add")]
        [HttpPost]
        public IHttpActionResult AddItem(Inventory _item)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    if (_item != null)
                    {
                         var response = UpdateNewItem(_item);
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
        [Authorize(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Edit")]
        [HttpPost]
        public IHttpActionResult UpdateItem(Inventory _item)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    /*  Check if _item is null
                     *  I use this than ModelState.IsValid because it throws Exception when there is a null
                     */
                    if (_item != null)
                    {
                        var itemDB = _context.Inventories.Where(x => x.in_code == _item.in_code).SingleOrDefault();
                        if (itemDB != null)
                        {
                            //  Convert _item into itemDB
                            itemDB.in_name = _item.in_name;
                            itemDB.in_category = _item.in_category;
                            itemDB.in_type = _item.in_type;
                            itemDB.in_size = _item.in_size;
                            itemDB.in_quantity = _item.in_quantity;
                            UpdateNewItem(itemDB);
                            _context.Entry(itemDB);
                            _context.SaveChanges();     //  Save to Database
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
        [Authorize(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Delete")]
        [HttpPost]
        public IHttpActionResult DeleteItem(Inventory codeToFind)
        {
            try
            {
                var item = codeToFind.in_code;
                using (var _context = new TFSEntity())
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

        [Authorize(Roles = "admin,warehouse")]
        [Route("Warehouse/Inventory/Barcode")]
        [HttpPost]
        // Barcorde Generator
        public IHttpActionResult GenerateBarcode(string value)
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
        [Authorize]
        [Route("Requests/All")]
        [HttpGet]
        public IHttpActionResult AllRequests()
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var requests = _context.Requests.Select(x => x).ToList();
                    var employee = _context.Employees.Select(x => x).ToList();
                    var inventory = _context.Inventories.Select(x => x).ToList();
                    var newRequest = new List<Request>();
                    foreach (var item in requests)
                    {
                        var newInv = inventory.Where(x => x.in_code == item.request_item).SingleOrDefault();
                        var newEmployee = employee.Where(x => x.emp_no == item.request_employee_id).SingleOrDefault();
                        var req = new Request()
                        {
                            request_id = item.request_id,
                            request_type = item.request_type,
                            request_date = item.request_date,
                            request_employee_id = item.request_employee_id,
                            request_item = item.request_item,
                            request_item_quantity = item.request_item_quantity,
                            request_status = item.request_status.Trim(' '),
                            request_type_id = item.request_type_id,
                            Employee = newEmployee?.emp_no == item.request_employee_id ? new Employee
                            {
                                emp_no = newEmployee.emp_no,
                                emp_contact = newEmployee.emp_contact,
                                emp_hiredDate = newEmployee.emp_hiredDate,
                                emp_name = newEmployee.emp_name,
                                emp_position = newEmployee.emp_position,
                            } : null,
                            Inventory = newInv?.in_code == item.request_item ? new Inventory
                            {
                                in_code = newInv.in_code,
                                in_category = newInv.in_category,
                                in_class = newInv.in_class,
                                in_name = newInv.in_name,
                                in_quantity = newInv.in_quantity,
                                in_size = newInv.in_size,
                                in_type = newInv.in_type
                            } : null
                        };
                        newRequest.Add(req);
                    }
                    var _jsonSerialized = JsonConvert.SerializeObject(newRequest, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    //  Deserialize the serialized json format to remove the escape characters like \ 
                    var _jsonDeserialized = JsonConvert.DeserializeObject<List<Request>>(_jsonSerialized);
                    return Ok(_jsonDeserialized);
                }
            }
            catch(Exception ex)
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

        [Authorize]
        [Route("Requests/Add")]
        [HttpPost]
        public IHttpActionResult AddRequests(List<Request> req)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    if(req == null)
                    {
                        return BadRequest("Please check your data.");
                    }
                    foreach (var item in req)
                    {
                        var checkId = GetNewGuid("requests");
                        item.request_id = checkId;
                        Request newReqs = new Request() {
                            request_date = item.request_date,
                            request_employee_id = item.request_employee_id,
                            request_id = item.request_id,
                            request_item = item.request_item,
                            request_item_quantity = item.request_item_quantity,
                            request_status = "pending",
                            request_type = item.request_type,
                            request_type_id = item.request_type_id
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

        [Authorize]
        [Route("Requests/Edit")]
        [HttpPost]
        public IHttpActionResult EditRequests(List<Request> req)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    foreach (var item in req) { 
                        var request = _context.Requests.Where(x => x.request_id == item.request_id).SingleOrDefault();
                        if (request == null)
                        {
                            return BadRequest("Request Not Found");
                        }
                        request.request_status = item.request_status;
                        request.request_type = item.request_type;
                        request.request_item = item.request_item;
                        request.request_item_quantity = item.request_item_quantity;
                        _context.Entry(request);
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

        public IHttpActionResult DeleteRequests(Guid req)
        {
            try
            {
                using (var _context = new TFSEntity())
                {
                    var request = _context.Requests.Where(x => x.request_id == req).SingleOrDefault();
                    if (request == null)
                    {
                        return BadRequest("Request Not Found");
                    }
                    request.request_status = "archived";
                    _context.Entry(request);
                    _context.SaveChanges();
                    return Ok("Requests Deleted Successfully!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}