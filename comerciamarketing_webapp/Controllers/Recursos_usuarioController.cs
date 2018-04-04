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
    public class Recursos_usuarioController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();

        // GET: Recursos_usuario
        public ActionResult Index(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                ViewBag.id_customer = id;
                var id_back= (from b in db.Usuarios where (b.ID_usuario == id) select b).FirstOrDefault();
                ViewBag.ID_empresaback = id_back.ID_empresa;
                var recursos_usuario = db.Recursos_usuario.Where(c=> c.ID_usuario == id).Include(r => r.Usuarios).Include(r => r.tipo_recurso);
                return View(recursos_usuario.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: Recursos_usuario/Create
        public ActionResult Create(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                ViewBag.ID_tiporecurso = new SelectList(db.tipo_recurso, "ID_tiporecurso", "descripcion");
            ViewBag.ID_usuario = id;
            return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Recursos_usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_recursousuario,descripcion,url,fultima_actualizacion,ID_usuario,ID_tiporecurso")] Recursos_usuario recursos_usuario)
        {
            if (ModelState.IsValid)
            {
                recursos_usuario.fultima_actualizacion = DateTime.Now;
                db.Recursos_usuario.Add(recursos_usuario);
                db.SaveChanges();
                TempData["exito"] = "Resource created successfully.";
                return RedirectToAction("Index","Recursos_usuario", new { id = recursos_usuario.ID_usuario });
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            ViewBag.id_customer = recursos_usuario.ID_usuario;
            return RedirectToAction("Index", "Recursos_usuario", new { id = recursos_usuario.ID_usuario });
        }

        // GET: Recursos_usuario/Edit/5
        public ActionResult Edit(int? id)
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
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
            if (recursos_usuario == null)
            {
                return HttpNotFound();
            }
                ViewBag.ID_tiporecurso = new SelectList(db.tipo_recurso, "ID_tiporecurso", "descripcion", recursos_usuario.ID_tiporecurso);

                ViewBag.id_customer = recursos_usuario.ID_usuario;
            return View(recursos_usuario);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        // POST: Recursos_usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_recursousuario,descripcion,url,fultima_actualizacion,ID_usuario,ID_tiporecurso")] Recursos_usuario recursos_usuario)
        {
            if (ModelState.IsValid)
            {
                recursos_usuario.fultima_actualizacion = DateTime.Now;
                db.Entry(recursos_usuario).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "Resource saved successfully.";
                return RedirectToAction("Index", "Recursos_usuario", new { id = recursos_usuario.ID_usuario });
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            ViewBag.id_customer = recursos_usuario.ID_usuario;
            return RedirectToAction("Index", "Recursos_usuario", new { id = recursos_usuario.ID_usuario });
        }

        // GET: Recursos_usuario/Delete/5
        public ActionResult Delete(int? id)
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
                Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
                if (recursos_usuario == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_customer = recursos_usuario.ID_usuario;
                return View(recursos_usuario);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }

        }

        // POST: Recursos_usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            try
            {
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
                var id_customer = recursos_usuario.ID_usuario;
            db.Recursos_usuario.Remove(recursos_usuario);
            db.SaveChanges();
                TempData["exito"] = "Resource deleted successfully.";
                return RedirectToAction("Index", "Recursos_usuario", new { id = id_customer });
            }
            catch (Exception ex)
            {
                Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Index", "Recursos_usuario", new { id = recursos_usuario.ID_usuario });
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
