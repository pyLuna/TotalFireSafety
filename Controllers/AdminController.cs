using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using TotalFireSafety.Models;
using System.Collections.Generic;

namespace TotalFireSafety.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        readonly APIRequestHandler api_req = new APIRequestHandler();

        public ActionResult FindDataOf()
        {
            try
            {
                var userToken = Session["access_token"].ToString();
                var response = api_req.GetAllMethod("/Warehouse/Inventory", userToken);

                var json = JsonConvert.DeserializeObject<List<Inventory>>(response);
                json.RemoveAll(x => x.in_status == "archived");

                JsonResult result = Json(json, JsonRequestBehavior.AllowGet); // return the value as JSON and allow Get Method
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
            if(Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddItem(Inventory item)
        {
            var serializedModel = JsonConvert.SerializeObject(item);
            var userToken = Session["access_token"].ToString();
            var response = api_req.AddMethod("Warehouse/Inventory/Edit", userToken, serializedModel);

            if (response == "BadRequest" || response == "InternalServerError")
            {
                ViewBag.Added = response.ToString();
                return View();
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

            var response = api_req.DeleteMethod("Warehouse/Inventory/Delete", userToken, serializedModel);

            if (response == "BadRequest" || response == "InternalServerError")
            {
                //ViewBag.Removed = response.ToString();
                return Json("error");
            }
            //var json = JsonConvert.DeserializeObject(response);
            //ViewBag.Removed = "Item details has deleted in the list!";
            //Session["removed"] = "Item details has deleted in the list!";
            return Json("Item Deleted");
        }

        //  Inventory
        public ActionResult Inventory()
        {
            if (Session["emp_no"] == null)
            {
                return RedirectToAction("Login", "Base");
            }
            return View();
        }
        //  Users
        public ActionResult Users()
        {
            return View();
        }
        public ActionResult AddUsers()
        {
            return View();
        }

        //  Purchasing
        #region
        public ActionResult Purchasing()
        {
            return View();
        }
        public ActionResult AddPurchase()
        {
            return View();
        }
        public ActionResult ExportPurchase()
        {
            return View();
        }
        public ActionResult PrintPurchase()
        {
            return View();
        }
        #endregion

        public ActionResult PurchaseRequisition()
        {
            return View();
        }

        //  Deployment Requisition
        #region
        public ActionResult DeploymentRequisition()
        {
            return View();
        }
        public ActionResult AddDeploy()
        {
            return View();
        }
        public ActionResult ExportDeploy()
        {
            return View();
        }
        public ActionResult PrintDeploy()
        {
            return View();
        }
        #endregion

        //  Supply
        #region
        public ActionResult SupplyRequisition()
        {
            return View();
        }
        public ActionResult AddSupply()
        {
            return View();
        }
        public ActionResult PrintSupply()
        {
            return View();
        }
        public ActionResult ExportSupply()
        {
            return View();
        }
        #endregion

        public ActionResult Projects()
        {
            return View();
        }

        public ActionResult Logout()
        {

            // DONT FORGET TO CLEAR SESSIONS, TOKENS AND OTHERS

            return RedirectToAction("Login", "Base");
        }

    }
}