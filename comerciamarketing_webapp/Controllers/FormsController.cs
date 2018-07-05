using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using Postal;

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
                ViewBag.vendors = (from b in COM_MKdb.OCRDs where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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
            public string fvalue { get; set; }
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

                        if (Convert.ToInt32(item.id_resource) == 6) //6 es el ID de del recurso de input_text
                        {
                            detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                        }

                        
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
                ViewBag.vendors = (from b in COM_MKdb.OCRDs where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
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

                        if (Convert.ToInt32(item.id_resource) == 6) //6 es el ID de del recurso de input_text
                        {
                            detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                        }


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
                            //if (item.text == null || item.text == "")
                            //{


                            //}
                            //else {
                            //    var image_name = detail.fsource;

                            //    detail.fsource = "";

                            //    db.Entry(detail).State = EntityState.Modified;
                            //    db.SaveChanges();

                            //    //luego, eliminamos la url
                            //    string fullPath = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), image_name.Replace("~/Content/images/ftp_demo/", ""));
                            //    if (System.IO.File.Exists(fullPath))
                            //    {
                            //        System.IO.File.Delete(fullPath);
                            //    }
                            //}


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

                //asignamos check out
                var fecha = (from f in values where (f.id == "999999999") select f).FirstOrDefault();
                demo.end_date = Convert.ToDateTime(fecha.text);
                db.Entry(demo).State = EntityState.Modified;
                db.SaveChanges();

                //Enviamos el correo al usuario
                SendDemoResume(Convert.ToInt32(id_demo));

                //Enviamos el correo a los mail contacts
                SendDemoResumeMC(Convert.ToInt32(id_demo));


                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Warning" });

        }

        public void SendDemoResume(int? id)
        {

            var demo_header = (from a in db.Demos where (a.ID_demo == id) select a).ToList();

            var id_empresa = "";

            if (demo_header.Count > 0)

            {
                var nombretienda = "";
                foreach (var item in demo_header)
                {
                    //nombre de usuario
                    var usuario = (from u in COM_MKdb.OCRDs where (u.CardCode == item.ID_usuario) select u).FirstOrDefault();
                    item.ID_Vendor = usuario.CardName;
                    item.end_date = item.end_date.ToLocalTime();

                    //nombre de tienda
                    var tiendita = (from u in COM_MKdb.OCRDs where (u.CardCode == item.ID_Store) select u).FirstOrDefault();
                    nombretienda = tiendita.CardName;

                    id_empresa = item.ID_Vendor;
                }

                //Existen datos
                //Buscamos los detalles

                var demo_details = (from b in db.Forms_details where (b.ID_demo == id && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details
                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        .Select(cl => new Forms_details
                        {
                            ID_details = cl.First().ID_details,
                            ID_formresourcetype = cl.First().ID_formresourcetype,
                            fsource = cl.First().fsource,
                            fdescription = cl.First().fdescription,
                            fvalue = cl.Sum(c => c.fvalue),
                            ID_form = cl.First().ID_form,
                            ID_demo = cl.First().ID_demo,
                            original = cl.First().original,
                            obj_order = cl.First().obj_order,
                            obj_group = cl.First().obj_group
                        }).ToList();


                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));


                //Obtenemos el nombre de las marcas o brands por cada articulo
                var listadeItems = (from d in db.Forms_details where (d.ID_demo == id && d.ID_formresourcetype == 3) select d).ToList();

                var oitm = (from h in COM_MKdb.OITMs select h).ToList();
                var omrc = (from i in COM_MKdb.OMRC select i).ToList();
                foreach (var itemd in listadeItems)
                {

                    itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                }

                var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                var brandstoshow = "";
                int count = 0;
                foreach (var items in brands)
                {
                    if (count == 0)
                    {
                        brandstoshow = items.fdescription.ToString();
                    }
                    else
                    {
                        brandstoshow += ", " + items.fdescription.ToString();
                    }
                    count += 1;
                }
                //*******************************

                demo_header[0].vendor = brandstoshow;


                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(result);

                //Verificamos si existen fotos en el demo (MAX 4 fotos)
                var fotos = (from c in db.Forms_details where (c.ID_demo == id && c.ID_formresourcetype == 5) select c).ToList();

                int fotosC = fotos.Count();




                if (fotosC == 4)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }
                    if (fotos[3].fsource == "")
                    {
                        rd.SetParameterValue("urlimg4", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                    }


                }
                else if (fotosC == 3)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 2)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 1)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }

                    rd.SetParameterValue("urlimg2", "");

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else
                {

                    rd.SetParameterValue("urlimg1", "");
                    rd.SetParameterValue("urlimg2", "");
                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");
                }


                //Verificams si existe firma electronica
                var firma = (from d in db.Forms_details where (d.ID_demo == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                //Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume; ");



                //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                //stream.Seek(0, SeekOrigin.Begin);



                //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                //PARA ENVIAR POR CORREO

                try

                {
                    //limpiamos el directorio

                    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                    foreach (FileInfo file in di.GetFiles())

                    {

                        file.Delete();

                    }

                    foreach (DirectoryInfo dir in di.GetDirectories())

                    {

                        dir.Delete(true);

                    }

                }

                catch (Exception e)

                {

                    var mensaje = e.ToString();

                }

                var path2 = "";
                var filename = "Demo resume " + "" + ".pdf";
                path2 = Path.Combine(filePathOriginal, filename);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                //PdfDocument doc = new PdfDocument();
                //doc.LoadFromFile(path);
                ////Contamos numero total de paginas
                //int indexpages = doc.Pages.Count;

                //Image img = doc.SaveAsImage(0);
                //var imagename = "Accounts Receivable Report " + seller.SalesRepresentative + ".jpg";
                //pathimage = Path.Combine(filePathOriginal, imagename);
                //img.Save(pathimage);
                //doc.Close();


                //Enviamos correo al usuario
                try
                {
                    var data = demo_header.FirstOrDefault();
                    var usuario = (from u in COM_MKdb.OCRDs where (u.CardCode == data.ID_usuario) select u).FirstOrDefault();


                    dynamic email = new Email("DemoResume");
                    email.To = usuario.E_Mail.ToString();
                    email.From = "customercare@comerciamarketing.com";
                    email.Subject = "DEMO IN " + nombretienda;
                    email.Attach(new Attachment(path2));
                    //email.Body = imagename;
                    //return new EmailViewResult(email);

                    email.Send();
                }

                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            else
            {

            }
        }

        public void SendDemoResumeMC(int? id)
        {

            var demo_header = (from a in db.Demos where (a.ID_demo == id) select a).ToList();

            var id_empresa = "";

            if (demo_header.Count > 0)

            {
                var nombretienda = "";
                foreach (var item in demo_header)
                {
                    //nombre de usuario
                    var usuario = (from u in COM_MKdb.OCRDs where (u.CardCode == item.ID_usuario) select u).FirstOrDefault();
                    item.ID_Vendor = usuario.CardName;
                    item.end_date = item.end_date.ToLocalTime();

                    //nombre de tienda
                    var tiendita = (from u in COM_MKdb.OCRDs where (u.CardCode == item.ID_Store) select u).FirstOrDefault();
                    nombretienda = tiendita.CardName;

                    id_empresa = item.ID_Vendor;

                    item.check_in = item.check_in.AddHours(-(Convert.ToDouble(item.extra_hours)));
                }

                //Existen datos
                //Buscamos los detalles

                var demo_details = (from b in db.Forms_details where (b.ID_demo == id && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details
                        .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        .Select(cl => new Forms_details
                        {
                            ID_details = cl.First().ID_details,
                            ID_formresourcetype = cl.First().ID_formresourcetype,
                            fsource = cl.First().fsource,
                            fdescription = cl.First().fdescription,
                            fvalue = cl.Sum(c => c.fvalue),
                            ID_form = cl.First().ID_form,
                            ID_demo = cl.First().ID_demo,
                            original = cl.First().original,
                            obj_order = cl.First().obj_order,
                            obj_group = cl.First().obj_group
                        }).ToList();


                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));

                //Obtenemos el nombre de las marcas o brands por cada articulo
                var listadeItems = (from d in db.Forms_details where (d.ID_demo == id && d.ID_formresourcetype == 3) select d).ToList();

                var oitm = (from h in COM_MKdb.OITMs select h).ToList();
                var omrc = (from i in COM_MKdb.OMRC select i).ToList();
                foreach (var itemd in listadeItems)
                {

                    itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                }

                var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                var brandstoshow = "";
                int count = 0;
                foreach (var items in brands)
                {
                    if (count == 0)
                    {
                        brandstoshow = items.fdescription.ToString();
                    }
                    else
                    {
                        brandstoshow += ", " + items.fdescription.ToString();
                    }
                    count += 1;
                }
                //*******************************

                demo_header[0].vendor = brandstoshow;

                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(result);

                //Verificamos si existen fotos en el demo (MAX 4 fotos)
                var fotos = (from c in db.Forms_details where (c.ID_demo == id && c.ID_formresourcetype == 5) select c).ToList();

                int fotosC = fotos.Count();




                if (fotosC == 4)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }
                    if (fotos[3].fsource == "")
                    {
                        rd.SetParameterValue("urlimg4", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                    }


                }
                else if (fotosC == 3)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }
                    if (fotos[2].fsource == "")
                    {
                        rd.SetParameterValue("urlimg3", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                    }

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 2)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }
                    if (fotos[1].fsource == "")
                    {
                        rd.SetParameterValue("urlimg2", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                    }

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else if (fotosC == 1)
                {
                    if (fotos[0].fsource == "")
                    {
                        rd.SetParameterValue("urlimg1", "");
                    }
                    else
                    {
                        rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                    }

                    rd.SetParameterValue("urlimg2", "");

                    rd.SetParameterValue("urlimg3", "");

                    rd.SetParameterValue("urlimg4", "");

                }
                else
                {

                    rd.SetParameterValue("urlimg1", "");
                    rd.SetParameterValue("urlimg2", "");
                    rd.SetParameterValue("urlimg3", "");
                    rd.SetParameterValue("urlimg4", "");
                }


                //Verificams si existe firma electronica
                var firma = (from d in db.Forms_details where (d.ID_demo == id && d.ID_formresourcetype == 9) select d).ToList();

                int firmaC = firma.Count();




                if (firmaC == 1)
                {

                    string data = firma[0].fsource;
                    if (data != "")
                    {
                        var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        var binData = Convert.FromBase64String(base64Data);

                        using (var streamf = new MemoryStream(binData))
                        {

                            Bitmap myImage = new Bitmap(streamf);

                            // Assumes myImage is the PNG you are converting
                            using (var b = new Bitmap(myImage.Width, myImage.Height))
                            {
                                b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(myImage, 0, 0);
                                }

                                // Now save b as a JPEG like you normally would

                                var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                                b.Save(path, ImageFormat.Jpeg);


                                rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                            }



                        }
                    }
                    else
                    {
                        rd.SetParameterValue("urlimgsign", "");

                    }

                }
                else
                {
                    rd.SetParameterValue("urlimgsign", "");
                }


                var filePathOriginal = Server.MapPath("/Reportes/pdf");

                Response.Buffer = false;

                Response.ClearContent();

                Response.ClearHeaders();


                //PARA VISUALIZAR
                //Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume; ");



                //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                //stream.Seek(0, SeekOrigin.Begin);



                //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                //PARA ENVIAR POR CORREO

                try

                {
                    //limpiamos el directorio

                    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                    foreach (FileInfo file in di.GetFiles())

                    {

                        file.Delete();

                    }

                    foreach (DirectoryInfo dir in di.GetDirectories())

                    {

                        dir.Delete(true);

                    }

                }

                catch (Exception e)

                {

                    var mensaje = e.ToString();

                }

                var path2 = "";
                var filename = "Demo resume " + "" + ".pdf";
                path2 = Path.Combine(filePathOriginal, filename);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                //PdfDocument doc = new PdfDocument();
                //doc.LoadFromFile(path);
                ////Contamos numero total de paginas
                //int indexpages = doc.Pages.Count;

                //Image img = doc.SaveAsImage(0);
                //var imagename = "Accounts Receivable Report " + seller.SalesRepresentative + ".jpg";
                //pathimage = Path.Combine(filePathOriginal, imagename);
                //img.Save(pathimage);
                //doc.Close();



                //Enviamos correo a los mail contacts
                var contacts = (from s in db.Usuarios where (s.Empresas.ID_SAP == id_empresa && s.ID_tipomembresia == 7) select s).ToList();

                if (contacts.Count > 0)
                {
                    foreach (var item in contacts)
                    {
                        if (item.correo != null)
                        {
                            try
                            {

                                dynamic email = new Email("DemoResume");
                                email.To = item.correo;
                                email.From = "customercare@comerciamarketing.com";
                                email.Subject = "DEMO IN " + nombretienda;
                                email.Attach(new Attachment(path2));
                                //email.Body = imagename;
                                //return new EmailViewResult(email);

                                email.Send();
                            }

                            catch (Exception e)
                            {
                                Console.WriteLine("{0} Exception caught.", e);
                            }

                        }
                    }

                }



            }
            else
            {

            }
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


                        Image TargetImg = Image.FromStream(file.InputStream, true, true);
                        try
                        {
                            if (TargetImg.PropertyIdList.Contains(0x0112))
                            {
                                int rotationValue = TargetImg.GetPropertyItem(0x0112).Value[0];
                                switch (rotationValue)
                                {
                                    case 1: // landscape, do nothing
                                        break;

                                    case 8: // rotated 90 right
                                            // de-rotate:
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                                        break;

                                    case 3: // bottoms up
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                                        break;

                                    case 6: // rotated 90 left
                                        TargetImg.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                                        break;
                                }
                            }
                        }
                        catch {

                        }


                        using (Image watermark = Image.FromFile(Server.MapPath("~/Content/images/Logo_watermark.png")))
                        using (Graphics g = Graphics.FromImage(TargetImg))
                        {

                            Image thumb = watermark.GetThumbnailImage((TargetImg.Width / 2), (TargetImg.Height / 3), null, IntPtr.Zero);

                            var destX = (TargetImg.Width / 2 - thumb.Width / 2);
                            var destY = (TargetImg.Height / 2 - thumb.Height / 2);

                            g.DrawImage(watermark, new Rectangle(destX,
                                        destY,
                                        TargetImg.Width / 2,
                                        TargetImg.Height / 4));


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
                lstproduct = (dbmk.OITMs.Where(x => x.U_customerCM == vendoriD)).ToList<OITM>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check_in(string ID_demo, string check_in)
        {
            try
            {
                Demos demo = db.Demos.Find(Convert.ToInt32(ID_demo));
                if (demo != null) {
                    demo.ID_demostate = 2;
                    demo.check_in = Convert.ToDateTime(check_in);
                    db.Entry(demo).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Fail" });
            }
            catch {
                return Json(new { Result = "Error" });
            }


           
        }
    }
}
