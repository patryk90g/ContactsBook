using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactsBook.Domain;
using ContactsBook.Domain.Common;
using PagedList;

namespace ContactsBook.Controllers
{
    /// <summary>
    /// contact controller class
    /// </summary>
    public class ContactController : Controller
    {
        /// <summary>
        /// represents application db context 
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contact
        /// <summary>
        /// show listing of contacts
        /// </summary>
        /// <param name="sortOrder"> represents order of sorting </param>
        /// <param name="page"> represents number of page </param>
        public async Task<ActionResult> Index(string sortOrder, int? page)
        {
           
            List<Contact> contacts = db.Contacts.Where(model => model.Active).ToList();
            if (ViewBag.LastNameOrder == null) ViewBag.LastNameOrder = "LastName";
            switch (sortOrder)
            {
                case "FirstName":
                    contacts = contacts.OrderBy(model => model.FirstName).ToList();
                    break;
                case "LastName":
                    contacts = contacts.OrderBy(model => model.LastName).ToList();
                    ViewBag.LastNameOrder = "LastNameDesc";
                    break;
                case "LastNameDesc":
                    contacts = contacts.OrderByDescending(model => model.LastName).ToList();
                    ViewBag.LastNameOrder = "LastName";
                    break;
                case "PhoneNumber":
                    contacts = contacts.OrderBy(model => model.PhoneNumber).ToList();
                    break;
                case "Email":
                    contacts = contacts.OrderBy(model => model.Email).ToList();
                    break;
            }
            var pageNumber = page ?? 1;
            ContactsBook.Models.ContactsViewModel viewModel = new Models.ContactsViewModel() { Contacts = contacts.ToPagedList(pageNumber, 399) };

            int numberOfContacts = db.Contacts.Where(model => model.Active).Count();
            int numberOfFriends = db.Contacts.Where(model => model.IsFriend && model.Active).Count();
            viewModel.NumberOfContacts = numberOfContacts;
            viewModel.NumberOfFriends = numberOfFriends;
            return View(viewModel);
        }

        // POST: Contact
        /// <summary>
        /// show listing of contacts, based on recevied form
        /// </summary>
        /// <param name="form"> represents form from users </param>
        /// <param name="page"> represents number of page </param>
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form, int? page)

        {

            
            //form["lastName"];

            var pageNumber = page ?? 1;
            ContactsBook.Models.ContactsViewModel viewModel = new Models.ContactsViewModel() { Contacts = db.Contacts.Where(model => model.Active).ToList().ToPagedList(pageNumber, 1) };
            if (!String.IsNullOrEmpty(form["lastName"]))
            {
                string name = form["lastName"];
                viewModel.Contacts = db.Contacts.Where(model => model.LastName.Contains(name) && model.Active).ToList().ToPagedList(pageNumber, 1);
            }

            int numberOfContacts = db.Contacts.Where(model => model.Active).Count();
            int numberOfFriends = db.Contacts.Where(model => model.IsFriend && model.Active).Count();
            viewModel.NumberOfContacts = numberOfContacts;
            viewModel.NumberOfFriends = numberOfFriends;

           

            return View(viewModel);


        }
        /// <summary>
        ///  delete all contact displayed on the page
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DeleteAll()
        {
            if (TempData["ContactsToRemove"] != null)
            {
                foreach(var contact in (PagedList.IPagedList<Contact>)TempData["ContactsToRemove"])
                {
                    var toDelete = db.Contacts.FirstOrDefault(model => model.Id == contact.Id);
                    toDelete.Active = false;
                    db.SaveChanges();     
                }
                
            } 

            return RedirectToAction("Index"); 
        }

        // GET: Contact/Details/5
        /// <summary>
        /// show details of contact
        /// </summary>
        /// <param name="id"> represents ID of contact </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contact/Create
        /// <summary>
        /// show form to create new contact
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// create new contact based on form
        /// </summary>
        /// <param name="contact"> represents data of new contact </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,PhoneNumber,Email,Address,City,Zip,IsFriend,Active")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.Id = Guid.NewGuid();
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Contact/Edit/5
        /// <summary>
        /// show form edit of contact
        /// </summary>
        /// <param name="id">represents ID of contact</param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// edit of contact based on form
        /// </summary>
        /// <param name="contact"> represents data edit of contact </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,PhoneNumber,Email,Address,City,Zip,IsFriend,Active")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        /// <summary>
        /// show form delete of contact
        /// </summary>
        /// <param name="id">represents ID of contact</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        /// <summary>
        /// delete of contact
        /// </summary>
        /// <param name="id">represents ID of contact</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            contact.Active = false;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// dispose aplication db context
        /// </summary>
        /// <param name="disposing">represents dispose state</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
