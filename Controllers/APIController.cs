using TotalFireSafety.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TotalFireSafety.Controllers
{
    public class APIController : ApiController
    {
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

                    List<NewEmployeeModel> emp = users.Select(x => new NewEmployeeModel
                    {
                        emp_no = x.emp_no,
                        emp_contact = x.emp_contact,
                        emp_hiredDate = x.emp_hiredDate,
                        emp_name = x.emp_name,
                        emp_position = x.emp_position
                    }).ToList();

                    var _jsonSerialized = JsonConvert.SerializeObject(emp, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    var _Deserialized = JsonConvert.DeserializeObject<List<NewEmployeeModel>>(_jsonSerialized);
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
                    var user = _context.Employees.Where(x => x.emp_name == val || x.emp_no == _id || x.emp_hiredDate == val).SingleOrDefault();

                    if (user != null)
                    {
                        NewEmployeeModel nem = new NewEmployeeModel()
                        {
                            emp_no = user.emp_no,
                            emp_contact = user.emp_contact,
                            emp_hiredDate = user.emp_hiredDate,
                            emp_name = user.emp_name,
                            emp_position = user.emp_position
                        };
                        var _serialize = JsonConvert.SerializeObject(nem, Formatting.None, new JsonSerializerSettings()
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
                        if (emp != null)
                        {
                            emp.emp_no = _emp.emp_no;
                            emp.emp_name = _emp.emp_name;
                            emp.emp_contact = _emp.emp_contact;
                            emp.emp_hiredDate = _emp.emp_hiredDate;
                            emp.emp_position = _emp.emp_position;

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
                    var items = _context.Inventories.Select(x => x);
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