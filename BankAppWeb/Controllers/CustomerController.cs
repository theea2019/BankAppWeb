using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Bank.Commons.Concretes.Helpers;
using Bank.Commons.Concretes.Logger;
using BankAppWeb.BankAppCustomerService;
using Customers = Bank.Models.Concretes.Customers;
using Transactions = Bank.Models.Concretes.Transactions;

namespace BankAppWeb.Controllers
{
    public class CustomerController : Controller
    {
        
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id)
        {
            return View(SelectCustomerByID(id));
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                    if (InsertCustomer(collection["CustomerName"],collection["CustomerSurname"],collection["CustomerPasskey"],decimal.Parse(collection["Balance"]),byte.Parse(collection["BalanceType"])))
                       return RedirectToAction("ListAll");

                return View();
            }
            catch(Exception ex)
            {
                LogHelper.Log(LogTarget.File,ExceptionHelper.ExceptionToString(ex),true);
                return View();
            }
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                return View(SelectCustomerByID(id));
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        // POST: Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                if (UpdateCustomer(id, collection["CustomerName"],collection["CustomerSurname"],collection["CustomerPasskey"],decimal.Parse(collection["Balance"]),byte.Parse(collection["BalanceType"])))
                    return RedirectToAction("Index");

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                if (DeleteCustomer(id))
                    return RedirectToAction("ListAll");
                return RedirectToAction("ListAll");
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Operation failed!", ex);
            }
        }

        public ActionResult ListAll()
        {
            try
            {
                IList<Customers> customers = ListAllCustomers().ToList();
                return View(customers);
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        #region PRIVATE METHODS
        
        private bool InsertCustomer(string name, string surname, string passkey, decimal balance, byte balancetype, bool isActive = true)
        {
            try
            {
                using (var customerSoapClient = new CustomerWebServiceSoapClient()) {
                    return customerSoapClient.InsertCustomer(new BankAppCustomerService.Customers()
                    {
                        CustomerName = name,
                        CustomerSurname = surname,
                        CustomerPasskey = passkey,
                        Balance = balance,
                        BalanceType = balancetype,
                        isActive = true
                    });
                 }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        private bool UpdateCustomer(int id, string name, string surname, string passkey, decimal balance, byte balancetype, bool isActive = true)
        {
            try
            {

                using (var customerSoapClient = new CustomerWebServiceSoapClient())
                {
                    return customerSoapClient.InsertCustomer(new BankAppCustomerService.Customers()
                    {
                        CustomerID = id,
                        CustomerName = name,
                        CustomerSurname = surname,
                        CustomerPasskey = passkey,
                        Balance = balance,
                        BalanceType = balancetype,
                        isActive = isActive
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        private bool DeleteCustomer(int ID)
        {
            try
            {
                using (var customerSoapClient = new CustomerWebServiceSoapClient())
                {
                    return customerSoapClient.DeleteCustomer(ID);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        private List<Customers> ListAllCustomers()
        {
            try
            {
                using (var customerSoapClient = new CustomerWebServiceSoapClient())
                {
                    List<Customers> customers = new List<Customers>();
                    foreach (var responsedCustomer in customerSoapClient.SelectAllCustomers().OrderBy(x => x.CustomerID).ToList())
                    {
                        Customers castedCustomer = new Customers()
                        {
                            CustomerID = responsedCustomer.CustomerID,
                            CustomerName = responsedCustomer.CustomerName,
                            CustomerSurname = responsedCustomer.CustomerSurname,
                            CustomerPasskey = responsedCustomer.CustomerPasskey,
                            Balance = responsedCustomer.Balance,
                            BalanceType = responsedCustomer.BalanceType,
                            isActive = responsedCustomer.isActive
                        };
                        customers.Add(castedCustomer);
                    }
                    return customers;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, ExceptionHelper.ExceptionToString(ex), true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        private Customers SelectCustomerByID(int ID)
        {
            try
            {
                using (var customerSoapClient = new CustomerWebServiceSoapClient())
                {
                    Customers castedCustomer = null;
                    BankAppCustomerService.Customers responsedCustomer = customerSoapClient.SelectCustomerById(ID);
                    if (responsedCustomer != null)
                    {
                        castedCustomer = new Customers()
                        {
                            CustomerID = responsedCustomer.CustomerID,
                            CustomerName = responsedCustomer.CustomerName,
                            CustomerSurname = responsedCustomer.CustomerSurname,
                            CustomerPasskey = responsedCustomer.CustomerPasskey,
                            Balance = responsedCustomer.Balance,
                            BalanceType = responsedCustomer.BalanceType,
                            isActive = responsedCustomer.isActive
                        };
                        List<Transactions> castedTransactions = new List<Transactions>();
                        foreach (var responsedTransaction in responsedCustomer.Transactions)
                        {
                            castedTransactions.Add(new Transactions()
                            {
                                TransactorAccountNumber = responsedTransaction.TransactorAccountNumber,
                                TransactionDate = responsedTransaction.TransactionDate,
                                TransactionID = responsedTransaction.TransactionID,
                                RecieverAccountNumber = responsedTransaction.RecieverAccountNumber,
                                TransactionAmount = responsedTransaction.TransactionAmount,
                                Customer = castedCustomer,
                                isSuccess = responsedTransaction.isSuccess
                            });
                        }

                        castedCustomer.Transactions.AddRange(castedTransactions);
                    }

                    return castedCustomer;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File,ExceptionHelper.ExceptionToString(ex),true);
                throw new Exception("Customer doesn't exists.");
            }
        }

        #endregion
    }
}
