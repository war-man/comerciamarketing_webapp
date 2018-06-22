using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Postal;

namespace comerciamarketing_webapp.Controllers
{
    public class DemosController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities CMKdb = new COM_MKEntities();

        // GET: Demos
        public ActionResult Index()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                if (datosUsuario.ID_rol == 6 || datosUsuario.ID_rol==1)
                {
                    var demos = db.Demos.OrderByDescending(d => d.visit_date).Include(d => d.Demo_state).Include(d => d.Forms);
                    ViewBag.rol = 6;
                    if (demos.Count() > 0) {

                        foreach (var item in demos)
                        {
                            var usuario = (from u in CMKdb.OCRDs where (u.CardCode == item.ID_usuario) select u).FirstOrDefault();
                            if (usuario == null)
                            {

                            }
                            else {
                                item.ID_Vendor = usuario.CardName;
                            }
                           

                        }

                    }



                    return View(demos.ToList());

                }
                else {
                    //Esto se iba a utilizar si se les daba usuario y registor 
                    var demos = db.Demos.Where(c=>c.ID_usuario == "noexisten2018").OrderByDescending(d => d.visit_date).Include(d => d.Demo_state).Include(d => d.Forms);
                    ViewBag.rol = 7;
                    return View();
                }



            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        // GET: Demos/Details/5
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
                Demos demos = db.Demos.Find(id);
                if (demos == null)
                {
                    return HttpNotFound();
                }
                return View(demos);

            }
            else
            {
                return RedirectToAction("Index","Home");
            }

        }

        // GET: Demos/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                ViewBag.ID_demostate = new SelectList(db.Demo_state, "ID_demostate", "sdescription");
                ViewBag.ID_form = new SelectList(db.Forms, "ID_form", "fdescription");

                //var usuarios = db.Usuarios.Where(s => s.ID_tipomembresia == 6 && s.ID_rol == 7).ToList();
                //IEnumerable<SelectListItem> selectList = from s in usuarios
                //                                         select new SelectListItem
                //                                         {
                //                                             Value = Convert.ToString(s.ID_usuario),
                //                                             Text = s.nombre.ToString() + " " + s.apellido.ToString() + " - " + s.correo.ToString()
                //                                         };



//USERS
                var usuariosdemo = CMKdb.OCRDs.Where(b => b.Series == 70 && b.CardName != null && b.CardName != "" && b.CardType=="s").OrderBy(b => b.CardName).ToList();

                IEnumerable<SelectListItem> selectList_usuarios = from st in usuariosdemo
                                                                  select new SelectListItem
                                                                  {
                                                                      Value = Convert.ToString(st.CardCode),
                                                                      Text = st.CardName.ToString() + " - " + st.E_Mail.ToString()
                                                         };
                ViewBag.ID_usuario = new SelectList(selectList_usuarios, "Value", "Text");



//VENDORS
                ViewBag.ID_Vendor = new SelectList(CMKdb.OCRDs.Where(b => b.Series == 61 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName");
                


                var store = CMKdb.OCRDs.Where(b => b.Series == 68 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName).ToList();
                IEnumerable<SelectListItem> selectList_stores = from st in store
                                                         select new SelectListItem
                                                         {
                                                             Value = Convert.ToString(st.CardCode),
                                                             Text = st.CardName.ToString() + ", " + st.MailAddres.ToString() + ", " + st.MailCity.ToString() + ", " + st.MailZipCod.ToString()
                                                         };

                ViewBag.ID_Store = new SelectList(selectList_stores, "Value", "Text");

                return View();

            }
            else
            {
                return RedirectToAction("Index","Home");
            }


        }

        // POST: Demos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_demo,ID_Vendor,vendor,ID_Store,store,visit_date,ID_usuario,ID_demostate,comments,ID_form")] Demos demos)
        {
            if (ModelState.IsValid)
            {
                demos.ID_demostate = 3;
                demos.comments = "";
                demos.end_date = demos.visit_date;
                db.Demos.Add(demos);
                db.SaveChanges();

                //Guardamos el detalle
                var detalles_acopiar = (from a in db.Forms_details where (a.ID_form == demos.ID_form && a.original == true) select a).ToList();

                foreach (var item in detalles_acopiar) {
                    Forms_details nuevodetalle = new Forms_details();
                    nuevodetalle = item;
                    nuevodetalle.original = false;
                    nuevodetalle.ID_demo = demos.ID_demo;

                    db.Forms_details.Add(nuevodetalle);
                    db.SaveChanges();
                }

                try
                {
                    var usuario = (from a in CMKdb.OCRDs where (a.CardCode == demos.ID_usuario) select a).FirstOrDefault();
                    //Enviamos correo para notificar
                    dynamic email = new Email("newdemo_alert");
                    email.To = usuario.E_Mail.ToString();
                    email.From = "customercare@comerciamarketing.com";
                    email.Vendor = demos.vendor;
                    email.Date = Convert.ToDateTime(demos.visit_date).ToLongDateString();
                    email.Time = Convert.ToDateTime(demos.visit_date).ToLongTimeString();
                    email.Place = demos.store;
                    email.link = "http://internal.comerciamarketing.com/Demos/Form_template?id_demo=" + demos.ID_demo + Server.HtmlDecode("&") + "id_form=" + demos.ID_form;
                    email.enddate = Convert.ToDateTime(demos.visit_date).AddDays(1).ToLongDateString();
                    email.Send();

                    //FIN email
                }
                catch
                {

                }


                TempData["exito"] = "Demo created successfully.";
                return RedirectToAction("Index");
            }

            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");
        }

        // GET: Demos/Edit/5
        public ActionResult Edit(int? id)
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
                Demos demos = db.Demos.Find(id);
                if (demos == null)
                {
                    return HttpNotFound();
                }
                if (demos.ID_demostate == 4)
                {
                    TempData["advertencia"] = "This demo is finished, you can't modify it.";
                    return RedirectToAction("Index");
                }
                ViewBag.ID_demostate = new SelectList(db.Demo_state, "ID_demostate", "sdescription", demos.ID_demostate);
                //ViewBag.ID_form = new SelectList(db.Forms, "ID_form", "fdescription", demos.ID_form);

                var usuarios = db.Usuarios.Where(s => s.ID_tipomembresia == 6 && s.ID_rol == 7).ToList();
                IEnumerable<SelectListItem> selectList = from s in usuarios
                                                         select new SelectListItem
                                                         {
                                                             Value = Convert.ToString(s.ID_usuario),
                                                             Text = s.nombre.ToString() + " " + s.apellido.ToString() + " - " + s.correo.ToString()
                                                         };


                //ViewBag.ID_usuario = new SelectList(selectList, "Value", "Text", demos.ID_usuario);
                var store = CMKdb.OCRDs.Where(b => b.Series == 68 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName).ToList();
                IEnumerable<SelectListItem> selectList_stores = from st in store
                                                                select new SelectListItem
                                                                {
                                                                    Value = Convert.ToString(st.CardCode),
                                                                    Text = st.CardName.ToString() + ", " + st.MailAddres.ToString() + ", " + st.MailCity.ToString() + ", " + st.MailZipCod.ToString()
                                                                };

                ViewBag.ID_Store = new SelectList(selectList_stores, "Value", "Text", demos.ID_Store);

                ViewBag.ID_Vendor = new SelectList(CMKdb.OCRDs.Where(b => b.Series == 2 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName",demos.ID_Vendor);

                return View(demos);

            }
            else
            {
                return RedirectToAction("Index","Home");
            }

        }

        // POST: Demos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_demo,ID_Vendor,vendor,ID_Store,store,visit_date,ID_usuario,ID_demostate,comments,ID_form")] Demos demos)
        {
            if (ModelState.IsValid)
            {
                if (demos.comments == "" || demos.comments == null) {
                    demos.comments = "";
                }
                demos.end_date = demos.visit_date;
                db.Entry(demos).State = EntityState.Modified;
                db.SaveChanges();


                try
                {
                    var usuario = (from a in CMKdb.OCRDs where (a.CardCode == demos.ID_usuario) select a).FirstOrDefault();

                    //Enviamos correo para notificar
                    dynamic email = new Email("editdemo_alert");
                    email.To = usuario.E_Mail.ToString();
                    email.From = "customercare@comerciamarketing.com";
                    email.Vendor = demos.vendor;
                    email.Date = Convert.ToDateTime(demos.visit_date).ToLongDateString();
                    email.Time = Convert.ToDateTime(demos.visit_date).ToLongTimeString();
                    email.Place = demos.store;
                    email.link = "http://internal.comerciamarketing.com/Demos/Form_template?id_demo=" + demos.ID_demo + Server.HtmlDecode("&") + "id_form=" + demos.ID_form;
                    email.enddate = Convert.ToDateTime(demos.visit_date).AddDays(1).ToLongDateString();
                    email.Send();

                    //FIN email
                }
                catch
                {

                }

                TempData["exito"] = "Demo saved successfully.";
                return RedirectToAction("Index");
            }

            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");
        }

        // GET: Demos/Delete/5
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
                Demos demos = db.Demos.Find(id);
                if (demos == null)
                {
                    return HttpNotFound();
                }

                if (demos.ID_demostate == 4)
                {
                    TempData["advertencia"] = "This demo is finished, you can't delete it.";
                    return RedirectToAction("Index");
                }
                return View(demos);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Demos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
        
                Demos demos = db.Demos.Find(id);
                db.Demos.Remove(demos);
                db.SaveChanges();

                //Eliminamos el detalle que genero el demo en Forms_details
                var lista_eliminar = (from c in db.Forms_details where (c.ID_demo == id && c.original ==false) select c).ToList();

                foreach (var item in lista_eliminar) {
                    Forms_details detailstodelete = db.Forms_details.Find(item.ID_details);
                    db.Forms_details.Remove(detailstodelete);
                    db.SaveChanges();

                }

                TempData["exito"] = "Demo deleted successfully.";
                return RedirectToAction("Index");
            }
            catch(Exception ex) {
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

        public ActionResult Form_template(int? id_demo, int? id_form)
        {
            //if (Session["IDusuario"] != null)
            //{
            //    int ID = Convert.ToInt32(Session["IDusuario"]);
            //    var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            //    ViewBag.usuario = datosUsuario.correo;
            //    ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                var Forms_details = db.Forms_details.Where(c => c.ID_demo == id_demo && c.ID_form ==id_form).OrderBy(c=>c.obj_group).ThenBy(c=> c.obj_order).Include(d => d.form_resource_type).Include(d => d.Forms);

                var vendor = (from b in db.Demos where (b.ID_demo == id_demo) select b).FirstOrDefault();
                ViewBag.vendor = vendor.vendor.ToString();
                

                Demos demos = db.Demos.Find(id_demo);
                if (demos == null)
                {
                    return HttpNotFound();
                }
                //Finalizado  o cancelado
                if (demos.ID_demostate == 4 || demos.ID_demostate == 1)
                {
                    TempData["advertencia"] = "This demo is finished.";
                int? id_demop = id_demo;
                int? id_formp = id_form;
                    return RedirectToAction("Form_templatepreview",new{ id_demo = id_demop, id_form = id_formp });
                }

            DateTime start = Convert.ToDateTime(demos.visit_date).Date;
            DateTime end = Convert.ToDateTime(demos.visit_date).AddDays(1).Date;

            DateTime today = DateTime.Today.Date;

            if (today >= start && today <= end)
            {
                if (demos.ID_demostate == 3)
                {
                    demos.ID_demostate = 2;

                    db.Entry(demos).State = EntityState.Modified;
                    db.SaveChanges();
                }

                ViewBag.id_demo = demos.ID_demo;
                return View(Forms_details.ToList());
            }
            else {
                TempData["advertencia"] = "This demo is not available.";
                int? id_demop = id_demo;
                int? id_formp = id_form;
                return RedirectToAction("Form_templatepreview", new { id_demo = id_demop, id_form = id_formp });
            }




            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
        }
        public ActionResult Form_templatepreview(int? id_demo, int? id_form)
        {
            //if (Session["IDusuario"] != null)
            //{
            //    int ID = Convert.ToInt32(Session["IDusuario"]);
            //    var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            //    ViewBag.usuario = datosUsuario.correo;
            //    ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                var Forms_details = db.Forms_details.Where(c => c.ID_demo == id_demo && c.ID_form == id_form).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order).Include(d => d.form_resource_type).Include(d => d.Forms);

                var vendor = (from b in db.Demos where (b.ID_demo == id_demo) select b).FirstOrDefault();
                ViewBag.vendor = vendor.vendor.ToString();


                return View(Forms_details.ToList());


            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
        }

        [HttpPost]
        public ActionResult demo_logsave(demo_log datosusuario)
        {
            if (datosusuario != null)
            {
                try
                {
                    if (datosusuario.ip == null)
                    {
                        datosusuario.ip = "";
                    }
                    if (datosusuario.hostname == null)
                    {
                        datosusuario.hostname = "";
                    }
                    if (datosusuario.typeh == null)
                    {
                        datosusuario.typeh = "";
                    }
                    if (datosusuario.continent_name == null)
                    {
                        datosusuario.continent_name = "";
                    }
                    if (datosusuario.country_code == null)
                    {
                        datosusuario.country_code = "";
                    }
                    if (datosusuario.country_name == null)
                    {
                        datosusuario.country_name = "";
                    }
                    if (datosusuario.region_code == null)
                    {
                        datosusuario.region_code = "";
                    }
                    if (datosusuario.region_name == null)
                    {
                        datosusuario.region_name = "";
                    }
                    if (datosusuario.city == null)
                    {
                        datosusuario.city = "";
                    }
                    if (datosusuario.latitude == null)
                    {
                        datosusuario.latitude = "";
                    }
                    if (datosusuario.longitude == null)
                    {
                        datosusuario.longitude = "";
                    }

                    datosusuario.fecha_conexion = DateTime.Now;

                    db.demo_log.Add(datosusuario);
                    db.SaveChanges();

                }
                catch
                {

                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult Demo_log(int? id_demo)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                return View(db.demo_log.Where(c => c.ID_demo == id_demo).OrderByDescending(c => c.fecha_conexion).ToList());



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult Details_demolog(int? id)
        {

            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
               demo_log historial = db.demo_log.Find(id);
                if (historial == null)
                {
                    return HttpNotFound();
                }
                ViewBag.latitude = historial.latitude;
                ViewBag.longitude = historial.longitude;

                return View(historial);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }


        }

        

        public ActionResult PreviewDemoResumeByCustomer(string id)
        {
            DateTime today_init_hour = DateTime.Today;
            DateTime today_end_hour = DateTime.Today.AddHours(23).AddMinutes(58);

            var total_demos = (from e in db.Demos where (e.ID_Vendor == id && e.visit_date >= today_init_hour && e.visit_date <= today_end_hour) select e).ToList();

            if (total_demos.Count > 0)
            {
                //Recuperamos los IDS de las demos en el dia especifico y del customer especifico
                int[] demo_ids = (from f in db.Demos where (f.ID_Vendor == id && (f.visit_date >= today_init_hour && f.visit_date <= today_end_hour)) select f.ID_demo).ToArray();

                //Existen datos
                //Buscamos los detalles

                var demo_details_items = (from b in db.Forms_details where (demo_ids.Contains(b.ID_demo) && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details_items
                                        .GroupBy(l => new {ID_formresourcetype = l.ID_formresourcetype , fsource = l.fsource })
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


                if (result.Count > 0)

                {

                    //CAMBIAMOS LOS DATOS A LOS QUE NECESITAMOS DESDE SAP COMERCIA
                    /*
                     STORE = NOMBRE DE TIENDA
                     ID_STORE = ESTADO
                     VENDOR = CIUDAD
                     ID_VENDOR = UNIDADES VENDIDAS
                     
                     */
                     //VARIABLE PARA TOTAL DE HORAS DEMO
                    TimeSpan totaldemohours = new TimeSpan(0,0,0);

                    foreach (var item in total_demos) {
                        var store_details = (from CM in CMKdb.OCRDs where (CM.CardCode == item.ID_Store) select CM).FirstOrDefault();

                        if (store_details == null)
                        {
                            item.store = "NOT FOUND";
                            item.ID_Store = item.ID_Store + ": NOT FOUND" ;
                            item.vendor = "NOT FOUND";

                            decimal sumLineTotal = (from s in db.Forms_details where (s.ID_demo == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);

                            DateTime dt = item.visit_date;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);


                            totaldemohours = totaldemohours + ts;
                        }
                        else {
                            item.store = store_details.CardName;
                            item.ID_Store = store_details.State2;
                            item.vendor = store_details.MailCity;

                            decimal sumLineTotal = (from s in db.Forms_details where (s.ID_demo == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);


                            DateTime dt = item.visit_date;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);


                            totaldemohours = totaldemohours + ts;
                            
                        }

                    }

                    ReportDocument rd = new ReportDocument();

                    rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemoDailyResume_v2.rpt"));



                    rd.SetDataSource(total_demos);

                    rd.Subreports[0].SetDataSource(result);
                    // rd.Subreports[1].SetDataSource(total_demos);

                    rd.SetParameterValue("totaldemohours", totaldemohours.ToString(@"hh\:mm"));

                    var filePathOriginal = Server.MapPath("/Reports/pdfReports");

                    Response.Buffer = false;

                    Response.ClearContent();

                    Response.ClearHeaders();


                    //PARA VISUALIZAR
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Daily Resume; ");



                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);



                    return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                    //PARA ENVIAR POR CORREO

                    //try

                    //{



                    //    //limpiamos el directorio

                    //    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);



                    //    foreach (FileInfo file in di.GetFiles())

                    //    {

                    //        file.Delete();

                    //    }

                    //    foreach (DirectoryInfo dir in di.GetDirectories())

                    //    {

                    //        dir.Delete(true);

                    //    }



                    //}

                    //catch (Exception e)

                    //{

                    //    var mensaje = e.ToString();

                    //}







                    //var filename = "Returns and Allowances Report " + seller.SalesRepresentative + " " + DateTime.Now.ToString("dddd").ToUpper() + ".pdf";

                    //path = Path.Combine(filePathOriginal, filename);

                    //rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);



                    //PdfDocument doc = new PdfDocument();

                    //doc.LoadFromFile(path);

                    //Image img = doc.SaveAsImage(0);

                    //var imagename = "Returns and Allowances Report " + seller.SalesRepresentative + " " + DateTime.Now.ToString("dddd").ToUpper() + ".jpg";

                    //pathimage = Path.Combine(filePathOriginal, imagename);

                    //img.Save(pathimage);

                    //doc.Close();





                    ////Para enviar correos

                    //try

                    //{

                    //    dynamic email = new Email("EmailMWRCI");

                    //    email.To = seller.Email.ToString();

                    //    email.From = Config.Email;

                    //    email.Subject = "Returns and Allowances Report | " + seller.SalesRepresentative + " " + DateTime.Now.ToString("dddd").ToUpper();

                    //    email.Cc = destinatariosCC.ToString();

                    //    email.CcSupervisor = seller.Supervisor.ToString();

                    //    email.Body = imagename;

                    //    //return new EmailViewResult(email);



                    //    email.Send();

                    //}



                    //    catch (Exception e)

                    //    {

                    //        Console.WriteLine("{0} Exception caught.", e);

                    //    }
                }
                else {
                    TempData["advertencia"] = "No data to send.";
                    return RedirectToAction("Index", "Empresas", null);
                }
                }
                else
            {
                TempData["advertencia"] = "No Demos assigned for today.";
                return RedirectToAction("Index", "Empresas", null);
            }
        }

        public ActionResult SendDemoResumeByCustomer(string id)
        {
            DateTime today_init_hour = DateTime.Today;
            DateTime today_end_hour = DateTime.Today.AddHours(11).AddMinutes(58);

            var total_demos = (from e in db.Demos where (e.ID_Vendor == id && e.visit_date >= today_init_hour && e.visit_date <= today_end_hour) select e).ToList();

            if (total_demos.Count > 0)
            {
                //Recuperamos los IDS de las demos en el dia especifico y del customer especifico
                int[] demo_ids = (from f in db.Demos where (f.ID_Vendor == id && (f.visit_date >= today_init_hour || f.visit_date <= today_end_hour)) select f.ID_demo).ToArray();

                //Existen datos
                //Buscamos los detalles

                var demo_details_items = (from b in db.Forms_details where (demo_ids.Contains(b.ID_demo) && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                var result = demo_details_items
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


                if (result.Count > 0)

                {

                    //CAMBIAMOS LOS DATOS A LOS QUE NECESITAMOS DESDE SAP COMERCIA
                    /*
                     STORE = NOMBRE DE TIENDA
                     ID_STORE = ESTADO
                     VENDOR = CIUDAD
                     ID_VENDOR = UNIDADES VENDIDAS
                     
                     */
                    //VARIABLE PARA TOTAL DE HORAS DEMO
                    TimeSpan totaldemohours = new TimeSpan(0, 0, 0);

                    foreach (var item in total_demos)
                    {
                        var store_details = (from CM in CMKdb.OCRDs where (CM.CardCode == item.ID_Store) select CM).FirstOrDefault();

                        if (store_details == null)
                        {
                            item.store = "NOT FOUND";
                            item.ID_Store = item.ID_Store + ": NOT FOUND";
                            item.vendor = "NOT FOUND";

                            decimal sumLineTotal = (from s in db.Forms_details where (s.ID_demo == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);

                            DateTime dt = item.visit_date;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);


                            totaldemohours = totaldemohours + ts;
                        }
                        else
                        {
                            item.store = store_details.CardName;
                            item.ID_Store = store_details.State2;
                            item.vendor = store_details.MailCity;

                            decimal sumLineTotal = (from s in db.Forms_details where (s.ID_demo == item.ID_demo && s.ID_formresourcetype == 3) select s.fvalue).Sum();

                            item.ID_Vendor = Convert.ToString(sumLineTotal);


                            DateTime dt = item.visit_date;
                            DateTime dt2 = item.end_date;
                            TimeSpan ts = (dt2 - dt);


                            totaldemohours = totaldemohours + ts;

                        }

                    }

                    ReportDocument rd = new ReportDocument();

                    rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemoDailyResume_v2.rpt"));



                    rd.SetDataSource(total_demos);

                    rd.Subreports[0].SetDataSource(result);
                    // rd.Subreports[1].SetDataSource(total_demos);

                    rd.SetParameterValue("totaldemohours", totaldemohours.ToString(@"hh\:mm"));

                    var filePathOriginal = Server.MapPath("/Reportes/pdf");

                    Response.Buffer = false;

                    Response.ClearContent();

                    Response.ClearHeaders();


                    //PARA VISUALIZAR
                    //Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Daily Resume; ");



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


                    //Para enviar correos
                    try
                    {
                        var contactos = (from u in db.Usuarios where (u.ID_tipomembresia == 7) select u).ToList();

                        foreach (var item in contactos) {
                            dynamic email = new Email("DemoResumeTotal");
                            email.To = item.correo;
                            email.From = "customercare@comerciamarketing.com";
                            email.Subject = "Demos daily summary " + today_init_hour.ToShortDateString();
                            email.Attach(new Attachment(path2));
                            //email.Body = imagename;
                            //return new EmailViewResult(email);


                            email.Send();

                        }


                        TempData["exito"] = "All emails were successfully sent.";
                        return RedirectToAction("Index", "Empresas", null);
                    }

                    catch (Exception e)
                    {
                      
                        TempData["advertenca"] = "Something went wrong, please try again. " + e.Message;
                        return RedirectToAction("Index", "Empresas", null);
                    }

                }
                else
                {
                    TempData["advertencia"] = "No data to send.";
                    return RedirectToAction("Index", "Empresas", null);
                }
            }
            else
            {
                TempData["advertencia"] = "No Demos assigned for today.";
                return RedirectToAction("Index", "Empresas", null);
            }
        }

        public ActionResult PreviewSendDemoResume(int? id)
        {

            var demo_header = (from a in db.Demos where (a.ID_demo == id) select a).ToList();



            if (demo_header.Count > 0)

            {

                foreach (var item in demo_header)
                {
                    var usuario = (from u in CMKdb.OCRDs where (u.CardCode == item.ID_usuario) select u).FirstOrDefault();
                    if (usuario == null)
                    {

                    }
                    else {
                        item.ID_Vendor = usuario.CardName;
                    }
                   

                    item.end_date = item.end_date.ToLocalTime();

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



                rd.SetDataSource(demo_header);

                rd.Subreports[0].SetDataSource(demo_details);

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
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Resume; ");



                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);



                return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                ////PARA ENVIAR POR CORREO

                //try

                //{
                //    //limpiamos el directorio

                //    System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                //    foreach (FileInfo file in di.GetFiles())

                //    {

                //        file.Delete();

                //    }

                //    foreach (DirectoryInfo dir in di.GetDirectories())

                //    {

                //        dir.Delete(true);

                //    }

                //}

                //catch (Exception e)

                //{

                //    var mensaje = e.ToString();

                //}

                //var path2 = "";
                //var filename = "Demo resume " + "" + ".pdf";
                //path2 = Path.Combine(filePathOriginal, filename);
                //rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);

                ////PdfDocument doc = new PdfDocument();
                ////doc.LoadFromFile(path);
                //////Contamos numero total de paginas
                ////int indexpages = doc.Pages.Count;

                ////Image img = doc.SaveAsImage(0);
                ////var imagename = "Accounts Receivable Report " + seller.SalesRepresentative + ".jpg";
                ////pathimage = Path.Combine(filePathOriginal, imagename);
                ////img.Save(pathimage);
                ////doc.Close();


                ////Para enviar correos
                //try
                //{
                //    var data = demo_header.FirstOrDefault();


                //    dynamic email = new Email("DemoResume");
                //    email.To = data.Usuarios.correo;
                //    email.From = "customercare@comerciamarketing.com";
                //    email.Subject = "Demo summary " + data.visit_date.ToShortDateString();
                //    email.Attach(new Attachment(path2));
                //    //email.Body = imagename;
                //    //return new EmailViewResult(email);





                //    email.Send();
                //}

                //catch (Exception e)
                //{
                //    Console.WriteLine("{0} Exception caught.", e);
                //}

            }
            else
            {
                return RedirectToAction("Index", "Empresas", null);
            }
        }
    }
}
