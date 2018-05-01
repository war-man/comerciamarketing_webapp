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
    public class EmpresasController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();

        // GET: Empresas
        public ActionResult Index()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                var empresas = (from a in db.Empresas where(a.nombre != "SISTEMA")select a).ToList();

                foreach (var item in empresas) {
                    item.giro = (from b in db.Usuarios where (b.ID_empresa == item.ID_empresa) select b).Count().ToString();

                    if (item.giro == "" || item.giro == null){
                        item.giro = "0";
                    }
                }


                return View(empresas);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

           
        }

        // GET: Empresas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresas empresas = db.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // GET: Empresas/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }
            
        }

        // POST: Empresas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_empresa,nombre,giro,ID_SAP")] Empresas empresas)
        {
            if (ModelState.IsValid)
            {
                empresas.giro = "";
                db.Empresas.Add(empresas);
                db.SaveChanges();
                TempData["exito"] = "Customer created successfully.";
                return RedirectToAction("Index");
            }

            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index", "Empresas");
        }

        // GET: Empresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["IDusuario"] != null)
            {


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Empresas empresas = db.Empresas.Find(id);
                if (empresas == null)
                {
                    return HttpNotFound();
                }

                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                return View(empresas);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Empresas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_empresa,nombre,giro,ID_SAP")] Empresas empresas)
        {
            if (ModelState.IsValid)
            {
                empresas.giro = "";
                db.Entry(empresas).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "Customer saved successfully.";
                return RedirectToAction("Index");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index", "Empresas");
        }

        // GET: Empresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["IDusuario"] != null)
            {


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Empresas empresas = db.Empresas.Find(id);
                if (empresas == null)
                {
                    return HttpNotFound();
                }


                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;

                return View(empresas);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            try
            {
                Empresas empresas = db.Empresas.Find(id);
                db.Empresas.Remove(empresas);
                db.SaveChanges();

                TempData["exito"] = "User deleted successfully.";
                return RedirectToAction("Index", "Empresas");
            }
            catch (Exception ex)
            {
                Empresas empresas = db.Empresas.Find(id);
                var id_back = empresas.ID_empresa;
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Index", "Empresas");
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
