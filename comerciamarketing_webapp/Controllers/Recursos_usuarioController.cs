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
            var recursos_usuario = db.Recursos_usuario.Where(c=> c.ID_usuario == id).Include(r => r.Usuarios);
            return View(recursos_usuario.ToList());
        }

        // GET: Recursos_usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
            if (recursos_usuario == null)
            {
                return HttpNotFound();
            }
            return View(recursos_usuario);
        }

        // GET: Recursos_usuario/Create
        public ActionResult Create()
        {
            ViewBag.ID_usuario = new SelectList(db.Usuarios, "ID_usuario", "correo");
            return View();
        }

        // POST: Recursos_usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_recursousuario,descripcion,url,fultima_actualizacion,ID_usuario,tipo_recurso")] Recursos_usuario recursos_usuario)
        {
            if (ModelState.IsValid)
            {
                db.Recursos_usuario.Add(recursos_usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_usuario = new SelectList(db.Usuarios, "ID_usuario", "correo", recursos_usuario.ID_usuario);
            return View(recursos_usuario);
        }

        // GET: Recursos_usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
            if (recursos_usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_usuario = new SelectList(db.Usuarios, "ID_usuario", "correo", recursos_usuario.ID_usuario);
            return View(recursos_usuario);
        }

        // POST: Recursos_usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_recursousuario,descripcion,url,fultima_actualizacion,ID_usuario,tipo_recurso")] Recursos_usuario recursos_usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recursos_usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_usuario = new SelectList(db.Usuarios, "ID_usuario", "correo", recursos_usuario.ID_usuario);
            return View(recursos_usuario);
        }

        // GET: Recursos_usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
            if (recursos_usuario == null)
            {
                return HttpNotFound();
            }
            return View(recursos_usuario);
        }

        // POST: Recursos_usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recursos_usuario recursos_usuario = db.Recursos_usuario.Find(id);
            db.Recursos_usuario.Remove(recursos_usuario);
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
    }
}
