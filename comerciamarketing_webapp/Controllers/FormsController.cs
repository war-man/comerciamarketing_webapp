﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using comerciamarketing_webapp.Models;
using Newtonsoft.Json;

namespace comerciamarketing_webapp.Controllers
{
    public class FormsController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities COM_MKdb = new COM_MKEntities();
        // GET: Forms
        public ActionResult Index()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                return View(db.Forms.ToList());



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        // GET: Forms/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Forms forms = db.Forms.Find(id);
                if (forms == null)
                {
                    return HttpNotFound();
                }
                return View(forms);



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Forms/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //ViewBag.ID_empresa = new SelectList(COM_MKdb.OCRDs.Where(c => c.Series == 2 || c.CardName != null), "CardCode", "CardName");
                ViewBag.vendors = (from b in COM_MKdb.OCRDs where (b.Series == 2 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type, "ID_formresourcetype", "fdescription");

                return View();



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }

        public class MyObj
        {
            public string id_resource { get; set; }
            public string fsource { get; set; }
            public string fdescription { get; set; }
        }
        // POST: Forms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_form,fdescription")] Forms forms, string nestable_output)
        {
            if (ModelState.IsValid)
            {
                //var detalles = JsonConvert.DeserializeObject<Forms_detailslist>(nestable_output);
                //var superdetalles = (from c in detalles.Details_list select c).ToList();
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(nestable_output);

                //var detalles = new JavaScriptSerializer().DeserializeObject(nestable_output);
                //context = new JavaScriptSerializer().Deserialize<SortedList<string, object>>(nestable_output);




                if (details.Count() > 0)
                {

                    //Guardamos formulario
                    db.Forms.Add(forms);
                    db.SaveChanges();
                    //Guardamos detalle de formulario con Original = true

                    int count = 1;
                    foreach (var item in details)
                    {
                        Forms_details detalle_nuevo = new Forms_details();
                        //colocamos 0 ya que esta seria la plantila
                        detalle_nuevo.ID_demo = 0;
                        //Se coloca true ya que con esto identificamos que es un item del template original
                        detalle_nuevo.original = true;

                        detalle_nuevo.ID_formresourcetype = Convert.ToInt32(item.id_resource);
                        detalle_nuevo.fsource = Convert.ToString(item.fsource);
                        detalle_nuevo.fdescription = Convert.ToString(item.fdescription);
                        detalle_nuevo.ID_form = forms.ID_form;

                        detalle_nuevo.fvalue = 0;
                        detalle_nuevo.obj_order = count;
                        detalle_nuevo.obj_group = 1;

                        count += 1;

                        db.Forms_details.Add(detalle_nuevo);
                        db.SaveChanges();

                    }


                    TempData["exito"] = "Form created successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["advertencia"] = "Something wrong happened, try again.";
                    return RedirectToAction("Index");
                }


            }


            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");
        }

        // GET: Forms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                Forms forms = db.Forms.Find(id);
                if (forms == null)
                {
                    return HttpNotFound();
                }

                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                //ViewBag.ID_empresa = new SelectList(COM_MKdb.OCRDs.Where(b => b.Series == 2 || b.CardName != null).OrderBy(b=> b.CardName), "CardCode", "CardName");
                ViewBag.vendors = (from b in COM_MKdb.OCRDs where (b.Series == 2 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type, "ID_formresourcetype", "fdescription");

                
                ViewBag.lista_objetos = (from a in db.Forms_details where (a.ID_form == id && a.original == true) select a).OrderBy(a=>a.obj_order).Include(a => a.form_resource_type).ToList();


                return View(forms);



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_form,fdescription")] Forms forms, string nestable_output)
        {
            if (ModelState.IsValid)
            {
                //var detalles = JsonConvert.DeserializeObject<Forms_detailslist>(nestable_output);
                //var superdetalles = (from c in detalles.Details_list select c).ToList();
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(nestable_output);

                //var detalles = new JavaScriptSerializer().DeserializeObject(nestable_output);
                //context = new JavaScriptSerializer().Deserialize<SortedList<string, object>>(nestable_output);




                if (details.Count() > 0)
                {

                    //Guardamos formulario
                    db.Entry(forms).State = EntityState.Modified;
                    db.SaveChanges();

                    var objetos_ant = (from b in db.Forms_details where (b.ID_form == forms.ID_form && b.original == true) select b).ToList();

                    foreach (var object_item in objetos_ant) {

                        Forms_details detailstodelete = db.Forms_details.Find(object_item.ID_details);
                        db.Forms_details.Remove(detailstodelete);
                        db.SaveChanges();
                    }



                    //Guardamos detalle de formulario con Original = true

                    int count = 1;
                    foreach (var item in details)
                    {
                        Forms_details detalle_nuevo = new Forms_details();
                        //colocamos 0 ya que esta seria la plantila
                        detalle_nuevo.ID_demo = 0;
                        //Se coloca true ya que con esto identificamos que es un item del template original
                        detalle_nuevo.original = true;

                        detalle_nuevo.ID_formresourcetype = Convert.ToInt32(item.id_resource);
                        detalle_nuevo.fsource = Convert.ToString(item.fsource);
                        detalle_nuevo.fdescription = Convert.ToString(item.fdescription);
                        detalle_nuevo.ID_form = forms.ID_form;

                        detalle_nuevo.fvalue = 0;
                        detalle_nuevo.obj_order = count;
                        detalle_nuevo.obj_group = 1;

                        count += 1;

                        db.Forms_details.Add(detalle_nuevo);
                        db.SaveChanges();

                    }


                    TempData["exito"] = "Form saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["advertencia"] = "Something wrong happened, try again.";
                    return RedirectToAction("Index");
                }


            }


            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");

        }

        // GET: Forms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Forms forms = db.Forms.Find(id);
                if (forms == null)
                {
                    return HttpNotFound();
                }
                return View(forms);



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Eliminamos el detalle que genero el demo en Forms_details
                var lista_eliminar = (from c in db.Forms_details where (c.ID_form == id && c.original == true) select c).ToList();

                foreach (var item in lista_eliminar)
                {
                    Forms_details detailstodelete = db.Forms_details.Find(item.ID_details);
                    db.Forms_details.Remove(detailstodelete);
                    db.SaveChanges();

                }



                Forms forms = db.Forms.Find(id);
                db.Forms.Remove(forms);
                db.SaveChanges();



                TempData["exito"] = "Form deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Index");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Addnewform()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                ViewBag.ID_empresa = new SelectList(db.Empresas.Where(c => c.nombre != "SISTEMA"), "ID_empresa", "nombre");
                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type, "ID_formresourcetype", "fdescription");

                return View();



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public class MyObj_formtemplate
        {
            public string id { get; set; }
            public string text { get; set; }
        }
        public JsonResult Save_templateformdata(List<MyObj_formtemplate> values)
        {

            if (values.Count > 0) {

                foreach (var item in values) {
                    Forms_details detail = db.Forms_details.Find(Convert.ToInt32(item.id));
                    if (detail == null)
                    {

                    }
                    else {
                        if (detail.form_resource_type.fdescription == "Product" || detail.form_resource_type.fdescription == "Product_sample")
                        {
                            if (item.text == "" || item.text == null) { item.text = "0"; }
                            detail.fvalue = Convert.ToInt32(item.text);

                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        else if (detail.form_resource_type.fdescription == "Picture")
                        {
                            if (item.text == null || item.text == "")
                            {


                            }
                            else {
                                var image_name = detail.fsource;

                                detail.fsource = "";

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();

                                //luego, eliminamos la url
                                string fullPath = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), image_name.Replace("~/Content/images/ftp_demo/", ""));
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }


                        }
                        else if (detail.form_resource_type.fdescription == "Input_text" || detail.form_resource_type.fdescription == "Electronic_signature") {

                            if (item.text == "" || item.text == null){item.text = "";}

                            detail.fsource = item.text;

                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else {
                            //No hacemos nada porque no esta registrado
                        }

                    }


                }

                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Warning" });

        }
        public JsonResult Finish_templateformdata(List<MyObj_formtemplate> values)
        {

            if (values.Count > 0)
            {

                foreach (var item in values)
                {
                    Forms_details detail = db.Forms_details.Find(Convert.ToInt32(item.id));
                    if (detail == null)
                    {

                    }
                    else
                    {
                        if (detail.form_resource_type.fdescription == "Product" || detail.form_resource_type.fdescription == "Product_sample")
                        {
                            if (item.text == "" || item.text == null) { item.text = "0"; }
                            detail.fvalue = Convert.ToInt32(item.text);

                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        else if (detail.form_resource_type.fdescription == "Picture")
                        {

                            if (item.text == null || item.text == "")
                            {


                            }
                            else
                            {
                                var image_name = detail.fsource;

                                detail.fsource = "";

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();

                                //luego, eliminamos la url
                                string fullPath = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), image_name.Replace("~/Content/images/ftp_demo/", ""));
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                        }
                        else if (detail.form_resource_type.fdescription == "Input_text" || detail.form_resource_type.fdescription == "Electronic_signature")
                        {

                            if (item.text == "" || item.text == null) { item.text = ""; }

                            detail.fsource = item.text;

                            db.Entry(detail).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            //No hacemos nada porque no esta registrado
                        }

                    }


                }

                //Cambiamos  el estado del demo
                var id_detailform = (from a in values select a).FirstOrDefault();
                Forms_details detail_id = db.Forms_details.Find(Convert.ToInt32(id_detailform.id));

                var id_demo = detail_id.ID_demo;

                Demos demo = db.Demos.Find(Convert.ToInt32(id_demo));
                demo.ID_demostate = 4;
                demo.end_date = DateTime.UtcNow;
                db.Entry(demo).State = EntityState.Modified;
                db.SaveChanges();



                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Warning" });

        }

        [HttpPost]  
        public ActionResult UploadFiles(string id)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }


                        // Adding watermark to the image and saving it into the specified folder!!!!

                        //Image image = Image.FromStream(file.InputStream, true, true);


                        using (Image TargetImg = Image.FromStream(file.InputStream, true, true))
                        using (Image watermark = Image.FromFile(Server.MapPath("~/Content/images/Logo_watermark.png")))
                        using (Graphics g = Graphics.FromImage(TargetImg))
                        {

                            Image thumb = watermark.GetThumbnailImage((TargetImg.Width), (TargetImg.Height/2), null, IntPtr.Zero);

                            var destX = (TargetImg.Width / 2 - thumb.Width / 2);
                            var destY = (TargetImg.Height / 2 - thumb.Height / 2);

                            g.DrawImage(thumb, new Rectangle(destX,
                                        destY,
                                        thumb.Width,
                                        thumb.Height));


                            // display a clone for demo purposes
                            //pb2.Image = (Image)TargetImg.Clone();
                            Image imagenfinal = (Image)TargetImg.Clone();
                            var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                            imagenfinal.Save(path, ImageFormat.Jpeg);

                        }
                        

                        //fname = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                        //file.SaveAs(fname);

      





                        //Luego guardamos la url en la db
                        Forms_details detail = db.Forms_details.Find(Convert.ToInt32(id));
                        detail.fsource = "~/Content/images/ftp_demo/" + file.FileName;

                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public ActionResult Form_templatepreview(int? id_form)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                var Forms_details = db.Forms_details.Where(c => c.ID_form == id_form && c.original==true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order).Include(d => d.form_resource_type).Include(d => d.Forms);




                return View(Forms_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Getproducts(string vendorID)
        {
            List<OITM> lstproduct = new List<OITM>();
            string vendoriD = vendorID;
            using (COM_MKEntities dbmk = new COM_MKEntities())
            {
                lstproduct = (dbmk.OITMs.Where(x => x.CardCode == vendoriD)).ToList<OITM>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
