using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using PhoneBookApp.Models;

namespace PhoneBookApp.Controllers
{
    public class PhoneBookController : Controller
    {
        private PhoneBookDBEntities db = new PhoneBookDBEntities();

        // GET: PhoneBook
        public ActionResult Index()
        {
            return View(db.PhoneBooks.ToList());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, PhoneBook phoneBook)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/files"), fileName);
                    file.SaveAs(path);


                    db.PhoneBooks.AddRange(GetBusinessCardList(file.FileName));
                    db.SaveChanges();
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View(db.PhoneBooks.ToList());
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View(db.PhoneBooks.ToList());
            }

        }

        // GET: PhoneBook/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhoneBook phoneBook = db.PhoneBooks.Find(id);
            if (phoneBook == null)
            {
                return HttpNotFound();
            }
            return View(phoneBook);
        }

        // GET: PhoneBook/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhoneBook/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Gender,Date_of_birth,Email,Phone,Photo,Address")] PhoneBook phoneBook, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                db.PhoneBooks.Add(phoneBook);
                db.SaveChanges();
                string path = "";
                if (imgFile.FileName != null)
                {
                    path = "~/uploadedPhotos/" + phoneBook.Name + ".jpeg";
                    imgFile.SaveAs(Server.MapPath(path));

                    Image image = Image.FromFile(Server.MapPath(path));

                    byte[] imageArray = imageToByteArray(image);
                    string base64Image = Convert.ToBase64String(imageArray);
                    phoneBook.Photo = base64Image;
                    db.Entry(phoneBook).State = EntityState.Modified;
                    db.SaveChanges();

                    
                }
                return RedirectToAction("Index");
            }

            return View(phoneBook);
        }

        // GET: PhoneBook/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhoneBook phoneBook = db.PhoneBooks.Find(id);
            if (phoneBook == null)
            {
                return HttpNotFound();
            }
            return View(phoneBook);
        }

        // POST: PhoneBook/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Gender,Date_of_birth,Email,Phone,Photo,Address")] PhoneBook phoneBook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phoneBook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phoneBook);
        }

        // GET: PhoneBook/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhoneBook phoneBook = db.PhoneBooks.Find(id);
            if (phoneBook == null)
            {
                return HttpNotFound();
            }
            return View(phoneBook);
        }

        // POST: PhoneBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PhoneBook phoneBook = db.PhoneBooks.Find(id);
            db.PhoneBooks.Remove(phoneBook);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public byte[] imageToByteArray(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        private List<PhoneBook> GetBusinessCardList(string fileName)
        {
            List<PhoneBook> businessCards = new List<PhoneBook>();

            //ReadCSV
            string path = Path.Combine(Server.MapPath("~/files"), fileName);

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var businessCard = csv.GetRecord<PhoneBook>();
                    businessCards.Add(businessCard);
                }
            }
            return businessCards;
        }

        [HttpGet]
        public void DownloadBusinessCard(string id)
        {
            PhoneBook phoneBook = db.PhoneBooks.Find(id);

            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Name\",\"Gender\",\"DOB\",\"Email\",\"Phone\",\"Photo\",\"Address\"");
            Response.ClearContent();

            Response.AddHeader("content-disposition", $"attachment;filename={phoneBook.Name}BusinessCard.csv");
            Response.ContentType = "text/csv";

            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"", 
                                        phoneBook.Name,
                                        phoneBook.Gender,
                                        phoneBook.Date_of_birth,
                                        phoneBook.Email,
                                        phoneBook.Phone,
                                        phoneBook.Photo,
                                        phoneBook.Address));
                                           

            Response.Write(sw.ToString());

            Response.End();

            
        }
    }
}
