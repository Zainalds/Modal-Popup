using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triton.CRM.Models;
using Triton.Model.CRM.Tables;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.CRM.Controllers
{
    public class CustomerController : Controller
    {
        [HttpGet]
        public IActionResult ContactList()
        {
            return View(new CustomerSearchModel { CustomerAdditionalContactsList = new List<CustomerAdditionalContacts>()});
        }

        [HttpPost]
        public async Task<IActionResult> ContactList(string accountcode)
        {
            var model = new CustomerSearchModel();

            try
            {

                model.CustomerAdditionalContactsList = await CustomerAdditionalContactsService.GetCustomerAdditionalContactsByAccountCode(accountcode);
                model.Customers = await CustomerService.GetCRMCustomerByAccountCode(accountcode);
                model.ErrorMessage = "There are no contacts";

                return View(model);

            }
            catch
            {
                model.ErrorMessage = "There are no contacts";
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult Create(int customerID, string accountCode, string Businessname, string errormessage, string cell, string tel, string email)
        {
            var model = new CustomerSearchModel();
            /*model.Customers = await CustomerService.GetCrmIDCustomerByAccountCode(accountcode);
            model.Customers = await CustomerService.GetCRMCustomerByAccountCode(accountcode);*/
            model.CustomerID = customerID;
            model.ErrorMessage = errormessage;
            model.AccountCode = accountCode;
            model.CustomerName = Businessname;
            model.Cell = cell;
            model.Tel = tel;
            model.Email = email;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerSearchModel model)
        {
            CustomerSearchModel csm = new CustomerSearchModel();


            if (model.Cell == null)
            {
                model.Cell = "0";
            }
            if (model.Tel == null)
            {
                model.Tel = "0";
            }
            if (model.Email == null)
            {
                model.Email = "0";
            }

            csm.CustomerAdditionalContactsList = await CustomerAdditionalContactsService.GetCustomerAdditionalEmailCellTelByID(model.Cell, model.Tel, model.Email, model.CustomerID);
            if(csm.CustomerAdditionalContactsList.Count > 0)
            {
                var cell = (from x in await CustomerAdditionalContactsService.GetCustomerAdditionalEmailCellTelByID(model.Cell, model.Tel, model.Email, model.CustomerID)
                            select x.Cell).FirstOrDefault();

                var tel = (from x in await CustomerAdditionalContactsService.GetCustomerAdditionalEmailCellTelByID(model.Cell, model.Tel, model.Email, model.CustomerID)
                            select x.Tel).FirstOrDefault();

                var email = (from x in await CustomerAdditionalContactsService.GetCustomerAdditionalEmailCellTelByID(model.Cell, model.Tel, model.Email, model.CustomerID)
                           select x.Email).FirstOrDefault();

                csm.ErrorMessage = "";
                return RedirectToAction("Create", "Customer", new { customerID = model.CustomerID, accountCode = model.AccountCode, Businessname = model.CustomerName, errormessage = csm.ErrorMessage, cell = cell, tel = tel, email = email });
            }
            else
            {

                await CustomerAdditionalContactsService.InsertCustomerAdditionalContactsAsync(model);

                return RedirectToAction("Message", "Home", new { type = Service.Utils.StringHelper.Types.SaveSuccess });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(int CustomerAdditionalContactID, string accountCode, string Businessname)
        {
            var model = new CustomerSearchModel();
            model.CustomerAdditionalContacts = await CustomerAdditionalContactsService.GetCustomerAdditionalContactsByID(CustomerAdditionalContactID);
            model.AccountCode = accountCode;
            model.CustomerName = Businessname;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CustomerSearchModel model)
        {
            var csm = new CustomerSearchModel();

            csm.CustomerAdditionalContactID = model.CustomerAdditionalContacts.CustomerAdditionalContactID;
            csm.CustomerID = model.CustomerAdditionalContacts.CustomerID;
            csm.Name = model.CustomerAdditionalContacts.Name;
            csm.Position = model.CustomerAdditionalContacts.Position;
            csm.Tel = model.CustomerAdditionalContacts.Tel;
            csm.Cell = model.CustomerAdditionalContacts.Cell;
            csm.Email = model.CustomerAdditionalContacts.Email;
            csm.Proposal = model.CustomerAdditionalContacts.Proposal;
            csm.OpsContact = model.CustomerAdditionalContacts.OpsContact;

            await CustomerAdditionalContactsService.UpdateCustomerAdditionalContactsAsync(csm);
            return RedirectToAction("Message", "Home", new { type = Service.Utils.StringHelper.Html.UpdateRecordSuccessMessage });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int CustomerAdditionalContactID)
        {
            var csm = new CustomerSearchModel();
            csm.CustomerAdditionalContacts = await CustomerAdditionalContactsService.GetCustomerAdditionalContactsByID(CustomerAdditionalContactID);
            return PartialView("Delete", csm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CustomerSearchModel model)
        {
            ViewBag.model = model;

            var csm = new CustomerSearchModel();

            csm.CustomerAdditionalContactID = model.CustomerAdditionalContacts.CustomerAdditionalContactID;
            csm.CustomerID = model.CustomerAdditionalContacts.CustomerID;
            csm.Name = model.CustomerAdditionalContacts.Name;
            csm.Position = model.CustomerAdditionalContacts.Position;
            csm.Tel = model.CustomerAdditionalContacts.Tel;
            csm.Cell = model.CustomerAdditionalContacts.Cell;
            csm.Email = model.CustomerAdditionalContacts.Email;
            csm.Proposal = model.CustomerAdditionalContacts.Proposal;
            csm.OpsContact = model.CustomerAdditionalContacts.OpsContact;
            csm.DeletedOn = System.DateTime.Now;
            csm.DeletedByUserID = model.DeletedByUserID;


            await CustomerAdditionalContactsService.UpdateCustomerAdditionalContactsAsync(csm);

            return RedirectToAction("Message", "Home", new { type = Service.Utils.StringHelper.Types.DeleteSuccess });
        }
    }
}
