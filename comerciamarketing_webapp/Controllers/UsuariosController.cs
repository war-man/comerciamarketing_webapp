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
    public class UsuariosController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();

        // GET: Usuarios
        public ActionResult Index()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;

                var usuarios = db.Usuarios.Include(u => u.Tipo_membresia).Include(u => u.Recursos_usuario);

                return View(usuarios.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia, "ID_tipomembresia", "descripcion");
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_usuario,correo,usuario,contrasena,ID_clienteSAP,NOM_clienteSAP,ID_tipomembresia,contador_visitas,fultima_visita,fcreacion_usuario,activo")] Usuarios usuarios)
        {
            if (usuarios.ID_clienteSAP == null)
            {
                usuarios.ID_clienteSAP = "";
            }
            if (usuarios.NOM_clienteSAP == null)
            {
                usuarios.NOM_clienteSAP = "";
            }

            usuarios.contador_visitas = 0;
            usuarios.fcreacion_usuario = DateTime.Now;
            usuarios.fultima_visita = DateTime.Now;
            usuarios.activo = true;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User created successfully.";
                return RedirectToAction("Index");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia, "ID_tipomembresia", "descripcion", usuarios.ID_tipomembresia);
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_usuario,correo,usuario,contrasena,ID_clienteSAP,NOM_clienteSAP,ID_tipomembresia,contador_visitas,fultima_visita,fcreacion_usuario,activo")] Usuarios usuarios)
        {
            if (usuarios.ID_clienteSAP == null) {
                usuarios.ID_clienteSAP = "";
            }
            if (usuarios.NOM_clienteSAP == null) {
                usuarios.NOM_clienteSAP = "";
            }
            if (ModelState.IsValid)
            {
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "User saved successfully.";
                return RedirectToAction("Index");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index");
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.usuario;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Usuarios usuarios = db.Usuarios.Find(id);
                db.Usuarios.Remove(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User deleted successfully.";
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
    }
}
