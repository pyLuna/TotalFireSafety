using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TotalFireSafety.Models;

namespace TotalFireSafety.Controllers
{
    public class APIController : ApiController
    {
        //  Status
        #region
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
        //  Credentials
        #region
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
        //  Roles
        #region
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
        //  EMPLOYEE API
        #region
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
                    //List<NewEmployeeModel> emp = users.Select(x => new NewEmployeeModel
                    //{
                    //    emp_no = x.emp_no,
                    //    emp_contact = x.emp_contact,
                    //    emp_hiredDate = x.emp_hiredDate,
                    //    emp_name = x.emp_name,
                    //    emp_position = x.emp_position
                    //}).ToList();

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
                            //Credential = creds.Where(x => x.emp_no == item.emp_no).SingleOrDefault(),
                            //Role = roles.Where(x => x.emp_no == item.emp_no).SingleOrDefault(),
                            //Status = status.Where(x => x.emp_no == item.emp_no).SingleOrDefault()

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
                        //NewEmployeeModel nem = new NewEmployeeModel()
                        //{
                        //    emp_no = user.emp_no,
                        //    emp_contact = user.emp_contact,
                        //    emp_hiredDate = user.emp_hiredDate,
                        //    emp_name = user.emp_name,
                        //    emp_position = user.emp_position
                        //};
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

        //  INVENTORY API
        #region
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
                    /*  Serialize items into JSON Format 
                     *  example
                     *  {
                     *      "in_code" : 1234
                     *      "in_name" : "Fire Extinguisher"
                     *      "etc"     : "etc"
                     *  }
                     */


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
                        _context.Inventories.Add(_item);
                        _context.SaveChanges();

                        return Ok("Item Added");
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
                            _context.Entry(_item);
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
        #endregion
    }
}