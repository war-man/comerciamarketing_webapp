using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using comerciamarketing_webapp.Models;
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
                    var demos = db.Demos.Include(d => d.Demo_state).Include(d => d.Forms).Include(d => d.Usuarios);
                    ViewBag.rol = 6;
                    return View(demos.ToList());

                }
                else {
                    var demos = db.Demos.Where(c=>c.ID_usuario == ID).Include(d => d.Demo_state).Include(d => d.Forms).Include(d => d.Usuarios);
                    ViewBag.rol = 7;
                    return View(demos.ToList());
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

                var usuarios = db.Usuarios.Where(s => s.ID_tipomembresia == 6 && s.ID_rol == 7).ToList();
                IEnumerable<SelectListItem> selectList = from s in usuarios
                                                         select new SelectListItem
                                                         {
                                                             Value = Convert.ToString(s.ID_usuario),
                                                             Text = s.nombre.ToString() + " " + s.apellido.ToString() + " - " + s.correo.ToString()
                                                         };


                ViewBag.ID_usuario = new SelectList(selectList, "Value", "Text");
                ViewBag.ID_Vendor = new SelectList(CMKdb.OCRDs.Where(b => b.Series == 2 && b.CardName != null && b.CardName != "").OrderBy(b => b.CardName), "CardCode", "CardName");
                


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
                    var usuario = (from a in db.Usuarios where (a.ID_usuario == demos.ID_usuario) select a).FirstOrDefault();
                    //Enviamos correo para notificar
                    dynamic email = new Email("newdemo_alert");
                    email.To = usuario.correo;
                    email.From = "customercare@comerciamarketing.com";
                    email.Vendor = demos.vendor;
                    email.Date = Convert.ToDateTime(demos.visit_date).ToLongDateString();
                    email.Time = Convert.ToDateTime(demos.visit_date).ToLongTimeString();
                    email.Place = demos.store;

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
                    var usuario = (from a in db.Usuarios where (a.ID_usuario == demos.ID_usuario) select a).FirstOrDefault();

                    //Enviamos correo para notificar
                    dynamic email = new Email("editdemo_alert");
                    email.To = usuario.correo;
                    email.From = "customercare@comerciamarketing.com";
                    email.Vendor = demos.vendor;
                    email.Date = Convert.ToDateTime(demos.visit_date).ToLongDateString();
                    email.Time = Convert.ToDateTime(demos.visit_date).ToLongTimeString();
                    email.Place = demos.store;

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
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

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
                    return RedirectToAction("Index");
                }

                if (demos.ID_demostate == 3)
                {
                    demos.ID_demostate = 2;
                    
                    db.Entry(demos).State = EntityState.Modified;
                    db.SaveChanges();
                }

                ViewBag.id_demo = demos.ID_demo;
                return View(Forms_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Form_templatepreview(int? id_demo, int? id_form)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                var Forms_details = db.Forms_details.Where(c => c.ID_demo == id_demo && c.ID_form == id_form).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order).Include(d => d.form_resource_type).Include(d => d.Forms);

                var vendor = (from b in db.Demos where (b.ID_demo == id_demo) select b).FirstOrDefault();
                ViewBag.vendor = vendor.vendor.ToString();


                return View(Forms_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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

    }
}
