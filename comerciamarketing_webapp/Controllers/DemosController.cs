using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using comerciamarketing_webapp.Models;

namespace comerciamarketing_webapp.Controllers
{
    public class DemosController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();

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
                db.Entry(demos).State = EntityState.Modified;
                db.SaveChanges();
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

    }
}
