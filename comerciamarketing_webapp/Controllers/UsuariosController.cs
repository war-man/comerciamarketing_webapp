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
        public ActionResult Index(int? ID_Empresa)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.ID_empresa = ID_Empresa;

                var usuarios = db.Usuarios.Where(c=>c.ID_empresa == ID_Empresa).Include(u => u.Tipo_membresia).Include(u => u.Recursos_usuario).Include(u => u.Roles);
                //var usuarios= (from c in db.Usuarios where(c.ID_empresa == ID_Empresa))
                return View(usuarios.ToList());
            }
            else
            {
                return RedirectToAction("Index","Home", null);
            }

        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
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
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ID_empresaback = usuarios.ID_empresa;
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }


        }

        // GET: Usuarios/Create
        public ActionResult Create(int? ID_empresa)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia, "ID_tipomembresia", "descripcion");
                ViewBag.ID_rol = new SelectList(db.Roles, "ID_rol", "descripcion");
                ViewBag.ID_empresa = ID_empresa;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_usuario,correo,contrasena,ID_tipomembresia,ID_rol,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia,ID_empresa")] Usuarios usuarios)
        {
            if (usuarios.cargo == null)
            {
                usuarios.cargo = "";
            }
            if (usuarios.telefono == null)
            {
                usuarios.telefono = "";
            }
            if (usuarios.estados_influencia == null)
            {
                usuarios.estados_influencia = "";
            }

            usuarios.fcreacion_usuario = DateTime.Now;

            usuarios.activo = true;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User created successfully.";
                return RedirectToAction("Index","Usuarios", new { ID_Empresa = usuarios.ID_empresa });
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index", "Usuarios", new { ID_Empresa = usuarios.ID_empresa });
        }

        // GET: Usuarios/Edit/5
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
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia, "ID_tipomembresia", "descripcion", usuarios.ID_tipomembresia);
                ViewBag.ID_rol = new SelectList(db.Roles, "ID_rol", "descripcion", usuarios.ID_rol);
                ViewBag.ID_empresa = new SelectList(db.Empresas, "ID_empresa", "nombre", usuarios.ID_empresa);

                ViewBag.ID_empresaback = usuarios.ID_empresa;

                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_usuario,correo,contrasena,ID_tipomembresia,ID_rol,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia,ID_empresa")] Usuarios usuarios)
        {
            if (usuarios.cargo == null)
            {
                usuarios.cargo = "";
            }
            if (usuarios.telefono == null)
            {
                usuarios.telefono = "";
            }
            if (usuarios.estados_influencia == null)
            {
                usuarios.estados_influencia = "";
            }

            if (ModelState.IsValid)
            {
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "User saved successfully.";
                return RedirectToAction("Index", "Usuarios", new { ID_Empresa = usuarios.ID_empresa });
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Index", "Usuarios", new { ID_Empresa = usuarios.ID_empresa });
        }

        // GET: Usuarios/Delete/5
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
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ID_empresaback = usuarios.ID_empresa;
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
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
                var id_back = usuarios.ID_empresa;
                db.Usuarios.Remove(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User deleted successfully.";
                return RedirectToAction("Index", "Usuarios", new { ID_Empresa = id_back });
            }
            catch (Exception ex) {
                Usuarios usuarios = db.Usuarios.Find(id);
                var id_back = usuarios.ID_empresa;
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Index", "Usuarios", new { ID_Empresa = id_back });
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

        // GET: Usuarios/Create
        public ActionResult User_registrationform()
        {
                
                return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult User_registrationform([Bind(Include = "ID_usuario,correo,contrasena,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia")] Usuarios usuarios)
        {
            if (usuarios.cargo == null)
            {
                usuarios.cargo = "";
            }
            if (usuarios.telefono == null)
            {
                usuarios.telefono = "";
            }
            if (usuarios.estados_influencia == null)
            {
                usuarios.estados_influencia = "";
            }
            usuarios.ID_rol = 1;
            usuarios.ID_tipomembresia = 2; //por defecto

            usuarios.fcreacion_usuario = DateTime.Now;
            //IMPORTANTE AQUI
            usuarios.activo = false;
            var id_empresadefault = (from e in db.Empresas where (e.nombre == "SISTEMA") select e).FirstOrDefault();
            usuarios.ID_empresa = id_empresadefault.ID_empresa;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                TempData["exito"] = "Data was saved successfully.";
                return RedirectToAction("User_success", "Usuarios");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Pre_user", "Usuarios");
        }

        public ActionResult User_success()
        {

            return View();
        }

        //PARA USUARIOS EN ESPERA

        public ActionResult Waiting_list()
        {

                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                    var id_empresa = (from b in db.Empresas where (b.nombre == "SISTEMA") select b).FirstOrDefault();
            

                return View(db.Usuarios.Where(c =>c.ID_empresa == id_empresa.ID_empresa).ToList());


        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit_waiting(int? id)
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
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia, "ID_tipomembresia", "descripcion", usuarios.ID_tipomembresia);
                ViewBag.ID_rol = new SelectList(db.Roles, "ID_rol", "descripcion", usuarios.ID_rol);
                ViewBag.ID_empresa = new SelectList(db.Empresas.Where(c=>c.nombre!="SISTEMA"), "ID_empresa", "nombre", usuarios.ID_empresa);

                ViewBag.ID_empresaback = usuarios.ID_empresa;

                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_waiting([Bind(Include = "ID_usuario,correo,contrasena,ID_tipomembresia,ID_rol,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia,ID_empresa")] Usuarios usuarios)
        {
            if (usuarios.cargo == null)
            {
                usuarios.cargo = "";
            }
            if (usuarios.telefono == null)
            {
                usuarios.telefono = "";
            }
            if (usuarios.estados_influencia == null)
            {
                usuarios.estados_influencia = "";
            }

            if (ModelState.IsValid)
            {
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "User saved successfully.";
                return RedirectToAction("Waiting_list", "Usuarios");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Waiting_list", "Usuarios");
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete_waiting(int? id)
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
                Usuarios usuarios = db.Usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ID_empresaback = usuarios.ID_empresa;
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete_waiting")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed_waiting(int id)
        {

            try
            {
                Usuarios usuarios = db.Usuarios.Find(id);
                var id_back = usuarios.ID_empresa;
                db.Usuarios.Remove(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User deleted successfully.";
                return RedirectToAction("Waiting_list", "Usuarios");
            }
            catch (Exception ex)
            {
                Usuarios usuarios = db.Usuarios.Find(id);
                var id_back = usuarios.ID_empresa;
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Waiting_list", "Usuarios");
            }

        }
    }
}
