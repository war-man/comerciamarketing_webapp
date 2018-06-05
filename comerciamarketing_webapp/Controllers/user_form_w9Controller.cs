using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Mvc;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Online.SharePoint;
using Microsoft.SharePoint.Client;

namespace comerciamarketing_webapp.Controllers
{
    public class user_form_w9Controller : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();

        // GET: user_form_w9
        public ActionResult Index()
        {
            return View(db.user_form_w9.ToList());
        }

        // GET: user_form_w9/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_form_w9 user_form_w9 = db.user_form_w9.Find(id);
            if (user_form_w9 == null)
            {
                return HttpNotFound();
            }
            return View(user_form_w9);
        }

        // GET: user_form_w9/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: user_form_w9/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_form,name,business_name,individual,ccorporation,scorporation,partnership,trust,limited,tax_classification,other,other_text,exempt_payeecode,exempt_fatcacode,address,city_state_zipcode,list_account_nummber,requestername_address,social_securitynum,employer_idnum,signature,sigdate,iscomplete,ID_usuario")] user_form_w9 user_form_w9)
        {
            if (ModelState.IsValid)
            {
                db.user_form_w9.Add(user_form_w9);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user_form_w9);
        }

        // GET: user_form_w9/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_form_w9 user_form_w9 = db.user_form_w9.Find(id);
            if (user_form_w9 == null)
            {
                return HttpNotFound();
            }
            return View(user_form_w9);
        }

        // POST: user_form_w9/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_form,name,business_name,individual,ccorporation,scorporation,partnership,trust,limited,tax_classification,other,other_text,exempt_payeecode,exempt_fatcacode,address,city_state_zipcode,list_account_nummber,requestername_address,social_securitynum,employer_idnum,signature,sigdate,iscomplete,ID_usuario")] user_form_w9 user_form_w9)
        {
            if (ModelState.IsValid)
            {
                if (user_form_w9.iscomplete == true)
                {
                    TempData["advertencia"] = "The data in the form was approved, you cannot edit your personal information.";
                    return RedirectToAction("Edit", new { id = user_form_w9.ID_form });
                }
                else {
                    db.Entry(user_form_w9).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["exito"] = "Form saved successfully.";
                    return RedirectToAction("Edit", new { id = user_form_w9.ID_form });
                }

            }
            return View(user_form_w9);
        }

        // GET: user_form_w9/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_form_w9 user_form_w9 = db.user_form_w9.Find(id);
            if (user_form_w9 == null)
            {
                return HttpNotFound();
            }
            return View(user_form_w9);
        }

        // POST: user_form_w9/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user_form_w9 user_form_w9 = db.user_form_w9.Find(id);
            db.user_form_w9.Remove(user_form_w9);
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

        public ActionResult preview_w9form(int id)
        {
            var usuariow9form = (from a in db.user_form_w9 where (a.ID_usuario == id) select a).FirstOrDefault();

            var user_form_w9 = (from b in db.user_form_w9 where(b.ID_form ==usuariow9form.ID_form) select b).ToList();

            foreach (var item in user_form_w9) {

                string nuevo = "";
                string nuevo_employer_idnum = "";

                for (int i = 0; i < item.social_securitynum.Length; i++)
                {
                    if (nuevo == "")
                    {
                        nuevo = Convert.ToString(item.social_securitynum[i]);
                    }
                    else if (i == 3) {
                        nuevo = nuevo + "      " + Convert.ToString(item.social_securitynum[i]);
                    }
                    else if (i == 5)
                    {
                        nuevo = nuevo + "     " + Convert.ToString(item.social_securitynum[i]);
                    }
                    else
                    {
                        nuevo = nuevo + "  " + Convert.ToString(item.social_securitynum[i]);
                    }
                }
                //EMPLOYER ID NUM
                for (int i = 0; i < item.employer_idnum.Length; i++)
                {
                    if (nuevo_employer_idnum == "")
                    {
                        nuevo_employer_idnum = Convert.ToString(item.employer_idnum[i]);
                    }
                    else if (i == 2)
                    {
                        nuevo_employer_idnum = nuevo_employer_idnum + "      " + Convert.ToString(item.employer_idnum[i]);
                    }
                    else
                    {
                        nuevo_employer_idnum = nuevo_employer_idnum + "  " + Convert.ToString(item.employer_idnum[i]);
                    }
                }

                //foreach (char c in item.social_securitynum)
                //{                   
                //    if (nuevo == "") {
                //        nuevo = Convert.ToString(c);
                //    } else {
                //        nuevo = nuevo + "  " + Convert.ToString(c);
                //    }

                //}
                item.social_securitynum = nuevo;
                item.employer_idnum = nuevo_employer_idnum;
            }

            if (user_form_w9.Count() >0)

            {

                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("/Reportes"), "rptw9form.rpt"));



                rd.SetDataSource(user_form_w9);


                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();



                Response.AppendHeader("Content-Disposition", "inline; filename=" + "W9 Form.pdf " + "; ");





                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);



            }

            //Si no encontramos empleados

            else

            {



                return RedirectToAction("Index", "Usuarios", null);



            }

        }

   
        public ActionResult confirm_data(int? id)
        {
            var IDform = (from a in db.user_form_w9 where (a.ID_usuario == id) select a).FirstOrDefault();

            user_form_w9 user_form_w9 = db.user_form_w9.Find(IDform.ID_form);

            user_form_w9.iscomplete = true;

            if (ModelState.IsValid)
            {
                db.Entry(user_form_w9).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "User confirmed successfully.";
                return RedirectToAction("Demo_users", "Usuarios", null);

            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Demo_users", "Usuarios", null);
        }

        public ActionResult tosharepoint() {
            

            //FUNCIONA
            //using (ClientContext clientContext = new ClientContext("https://limenainc.sharepoint.com/sites/comercia"))
            //{
            //    var passWord = new SecureString();

            //    foreach (char c in "Fr@Nc!sC0@2018".ToCharArray()) passWord.AppendChar(c);

            //    clientContext.Credentials = new SharePointOnlineCredentials("f.velasquez@limenainc.net", passWord);

            //    Web web = clientContext.Web;

            //    clientContext.Load(web);
            //    try
            //    {

            //        clientContext.ExecuteQuery();
            //        ViewBag.hola = web.Title;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //        ViewBag.hola = e.Message;
            //    }
            //}

            //Nos conectamos al sitio de sharepoint

            using (ClientContext clientContext = new ClientContext("https://limenainc.sharepoint.com/sites/comercia"))
            {
                var passWord = new SecureString();

                foreach (char c in "Fr@Nc!sC0@2018".ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials("f.velasquez@limenainc.net", passWord);

                //Web web = clientContext.Web;
                //ListCollection collList = web.Lists;

                //clientContext.Load(collList);

                try
                {


                    // Seleccionamos la lista especifica. 
                    List customerdataList = clientContext.Web.Lists.GetByTitle("Customer Data");

                    ////PARA CONSULTAS [SELECT]
                    //// Luego hacemos una consulta limitando los resultados a 100


                    //CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                    //ListItemCollection items = customerdataList.GetItems(query);

                    //// Retornamos la lista de items en un  ListItemCollection  hacia un List.GetItems(Query). 
                    //clientContext.Load(items);
                    //clientContext.ExecuteQuery();
                    //foreach (ListItem listItem in items)
                    //{
                    //    // We have all the list item data. For example, Title. 
                    //    ViewBag.hola = ViewBag.hola + ", " + listItem["Title"];
                    //}


                    // We are just creating a regular list item, so we don't need to 
                    // set any properties. If we wanted to create a new folder, for 
                    // example, we would have to set properties such as 
                    // UnderlyingObjectType to FileSystemObjectType.Folder. 
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = customerdataList.AddItem(itemCreateInfo);
                    newItem["Title"] = "TIENDA DE PRUEBA FRAN";
                    newItem.Update();

                    clientContext.ExecuteQuery();

                    ViewBag.hola = "Tienda ingresada correctamente";
                    //clientContext.ExecuteQuery();

                    //foreach (List oList in collList)
                    //{
                    //    if (oList.Title == "Customer Data") {
                    //        ViewBag.hola = oList.Id + ", " + oList.Title + "" + oList.Created.ToString();
                    //    }

                    //    //Console.WriteLine("Title: {0} Created: {1}", oList.Title, oList.Created.ToString());
                    //}

                    //ViewBag.hola = web.Title;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    ViewBag.hola = e.Message;
                }
            }

            return View();

        }
    }
}
