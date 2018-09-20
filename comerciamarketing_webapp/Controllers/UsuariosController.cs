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

                foreach (var item in usuarios) {
                    var registros_login = (from a in db.historial_conexiones where (a.ID_usuario == item.ID_usuario) select a).OrderByDescending(a => a.fecha_conexion);
                    if (registros_login.Count() > 0)
                    {
                        item.telefono = Convert.ToString(registros_login.Count());
                        item.contrasena = registros_login.FirstOrDefault().fecha_conexion.ToString();
                    }
                    else {
                        item.telefono = "0";
                        item.contrasena = "";

                    }

                }

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
                //excluimos el tipo de membresia con ID 6 ya que este es el asignado para DEMOS
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia.Where(c=> c.ID_tipomembresia != 6), "ID_tipomembresia", "descripcion");
                //excluimos ademas los roles de Demos
                ViewBag.ID_rol = new SelectList(db.Roles.Where(c => c.ID_rol != 6 && c.ID_rol != 7), "ID_rol", "descripcion");
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
            if (usuarios.contrasena == null)
            {
                usuarios.contrasena = "c0m2018";
            }

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
                //excluimos el tipo de membresia con ID 6 ya que este es el asignado para DEMOS
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia.Where(c => c.ID_tipomembresia != 6), "ID_tipomembresia", "descripcion", usuarios.ID_tipomembresia);
                //excluimos ademas los roles de Demos
                ViewBag.ID_rol = new SelectList(db.Roles.Where(c => c.ID_rol != 6 && c.ID_rol != 7), "ID_rol", "descripcion", usuarios.ID_rol);
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
                TempData["error"] = "An error was handled.  " + ex.Message + ". Please check if the user has resources assigned";
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
        public ActionResult User_registrationform([Bind(Include = "ID_usuario,correo,contrasena,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia")] Usuarios usuarios, string customer)
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

                try
                {
                    //Enviamos correo para notificar
                    dynamic email = new Email("email_userregistration");
                    email.To = "f.velasquez@limenainc.net";
                    email.From = "customercare@comerciamarketing.com";
                    email.CorreoActivar = usuarios.correo.ToString();
                    email.Empresa = customer;
                    email.NombreCliente = usuarios.nombre + " " + usuarios.apellido;

                    email.Send();

                    //FIN email
                }
                catch {

                }

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
            

                return View(db.Usuarios.Where(c =>c.ID_empresa == id_empresa.ID_empresa && c.activo ==false && c.ID_rol==1).ToList());


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
                //excluimos el tipo de membresia con ID 6 ya que este es el asignado para DEMOS
                ViewBag.ID_tipomembresia = new SelectList(db.Tipo_membresia.Where(c => c.ID_tipomembresia != 6), "ID_tipomembresia", "descripcion", usuarios.ID_tipomembresia);
                //excluimos ademas los roles de Demos
                ViewBag.ID_rol = new SelectList(db.Roles.Where(c => c.ID_rol != 6 && c.ID_rol != 7), "ID_rol", "descripcion", usuarios.ID_rol);
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
                TempData["exito"] = "User activated successfully.";


                try
                {
                    //Enviamos correo para notificar
                    dynamic email = new Email("email_confirmation");
                    email.To = usuarios.correo.ToString();
                    email.From = "customercare@comerciamarketing.com";
                    email.Nombrecliente = usuarios.nombre + " " + usuarios.apellido;
                    email.Correocliente = usuarios.correo;
                    email.Passwordcliente = usuarios.contrasena;
                    
                    email.Send();

                    //FIN email
                }
                catch
                {

                }


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

        public ActionResult Login_history(int? id, string modulo)
        {

            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.correo;

            var usuarios = (from e in db.Usuarios where (e.ID_usuario == id) select e).FirstOrDefault();
            ViewBag.CorreoUsuario = usuarios.correo;
            ViewBag.ID_empresaback = usuarios.ID_empresa;
            ViewBag.modulo = modulo;

            return View(db.historial_conexiones.Where(c => c.ID_usuario == id).OrderByDescending(c=>c.fecha_conexion).Take(20).ToList());


        }

        public ActionResult Details_loginhistory(int? id, string modulo)
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
                historial_conexiones historial = db.historial_conexiones.Find(id);
                if (historial == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ID_usuarioback = historial.ID_usuario;
                ViewBag.modulo = modulo;
                return View(historial);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }


        }

        //DEMOS ***************************
        public ActionResult Demo_users()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;


                var usuarios = db.Usuarios.Where(c => c.ID_empresa == 2 && c.ID_tipomembresia ==6).Include(u => u.Tipo_membresia).Include(u => u.Recursos_usuario).Include(u => u.Roles);

                foreach (var item in usuarios)
                {
                    var registros_login = (from a in db.historial_conexiones where (a.ID_usuario == item.ID_usuario) select a).OrderByDescending(a => a.fecha_conexion);
                    if (registros_login.Count() > 0)
                    {
                        item.telefono = Convert.ToString(registros_login.Count());
                        item.contrasena = registros_login.FirstOrDefault().fecha_conexion.ToString();
                    }
                    else
                    {
                        item.telefono = "0";
                        item.contrasena = "";

                    }

                    if (item.activo == false)
                    {
                        item.cargo = "Inactive";
                    }
                    else
                    {
                        item.cargo = "Active";
                    }
                    //Consultamos el estado del usuario DEMO
                    //utilizaremos la columna de cargo para mostrar el estado
                    //var datos_formulariow9 = (from a in db.user_form_w9 where (a.ID_usuario == item.ID_usuario) select a).FirstOrDefault();
                    //if (datos_formulariow9 != null)
                    //{
                    //    if (datos_formulariow9.iscomplete == false && datos_formulariow9.name == "")
                    //    {
                    //        item.cargo = "Waiting user to fill form";
                    //    }
                    //    else if (datos_formulariow9.iscomplete == false && datos_formulariow9.name != "")
                    //    {
                    //        item.cargo = "Waiting for approval";

                    //    }
                    //    else if (datos_formulariow9.iscomplete == true)
                    //    {
                    //        if (item.activo == false)
                    //        {
                    //            item.cargo = "Inactive";
                    //        }
                    //        else
                    //        {
                    //            item.cargo = "Active";
                    //        }

                    //    }

                    //}
                    //else {
                    //    if (item.ID_rol == 6)
                    //    {//6 es para Demo admin

                    //        if (item.activo == false)
                    //        {
                    //            item.cargo = "Inactive";
                    //        }
                    //        else
                    //        {
                    //            item.cargo = "Active";
                    //        }
                    //    }
                    //    else {
                    //        item.cargo = "No W9 form data was found";
                    //    }

                    //}
                }
              
                return View(usuarios.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // GET:
        public ActionResult Details_demoU(int? id)
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

                
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }


        }


        // GET:
        public ActionResult Create_demoU()
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

        // POST:
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_demoU([Bind(Include = "ID_usuario,correo,contrasena,ID_tipomembresia,ID_rol,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia,ID_empresa")] Usuarios usuarios)
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
            //predeterminados

            usuarios.ID_empresa = 2; //empresa sistema
            usuarios.fcreacion_usuario = DateTime.Now;
            usuarios.ID_tipomembresia = 6;
            usuarios.ID_rol = 7;
            usuarios.contrasena = "sinasignar2018";
            usuarios.activo =false;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuarios);
                db.SaveChanges();

                //user_form_w9 formulariow9 = new user_form_w9();

                //formulariow9.name = "";
                //formulariow9.business_name = "";
                //formulariow9.individual = false;
                //formulariow9.ccorporation = false;
                //formulariow9.scorporation = false;
                //formulariow9.partnership = false;
                //formulariow9.trust = false;
                //formulariow9.limited = false;
                //formulariow9.tax_classification = "";
                //formulariow9.other = false;
                //formulariow9.other_text = "";
                //formulariow9.exempt_payeecode = "";
                //formulariow9.exempt_fatcacode = "";
                //formulariow9.address = "";
                //formulariow9.city_state_zipcode = "";
                //formulariow9.list_account_nummber = "";
                //formulariow9.requestername_address = "";
                //formulariow9.social_securitynum = "";
                //formulariow9.employer_idnum = "";
                //formulariow9.signature = "";
                //formulariow9.sigdate = DateTime.Today;
                //formulariow9.iscomplete = false;

                //formulariow9.ID_usuario = usuarios.ID_usuario;

                //db.user_form_w9.Add(formulariow9);
                //db.SaveChanges();


                //try
                //{
                //    var usuario = (from a in db.Usuarios where (a.ID_usuario == usuarios.ID_usuario) select a).FirstOrDefault();
                //    //Enviamos correo para notificar
                //    dynamic email = new Email("newuser_alert");
                //    email.To = usuario.correo;
                //    email.From = "customercare@comerciamarketing.com";
                //    email.email = usuario.correo;
                //    email.password = usuario.contrasena;
                //    email.link = "http://internal.comerciamarketing.com/user_form_w9/edit/" + formulariow9.ID_form;
                    
                //    email.Send();

                //    //FIN email
                //}
                //catch
                //{

                //}



                TempData["exito"] = "User created successfully.";
                return RedirectToAction("Demo_users", "Usuarios");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Demo_users", "Usuarios");
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit_demoU(int? id)
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
        public ActionResult Edit_demoU([Bind(Include = "ID_usuario,correo,contrasena,ID_tipomembresia,ID_rol,fcreacion_usuario,activo,nombre,apellido,cargo,telefono,estados_influencia,ID_empresa")] Usuarios usuarios)
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

            //predeterminados

            usuarios.ID_empresa = 2; //empresa sistema
            usuarios.fcreacion_usuario = DateTime.Now;
            usuarios.ID_tipomembresia = 6;
            usuarios.ID_rol = 7;

            //usuarios.activo = true;


            if (ModelState.IsValid)
            {
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "User saved successfully.";
                return RedirectToAction("Demo_users", "Usuarios");
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Demo_users", "Usuarios");
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete_demoU(int? id)
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

                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home", null);
            }

        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete_demoU")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed_demoU(int id)
        {

            try
            {
                Usuarios usuarios = db.Usuarios.Find(id);
                var id_back = usuarios.ID_empresa;
                db.Usuarios.Remove(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User deleted successfully.";
                return RedirectToAction("Demo_users", "Usuarios");
            }
            catch (Exception ex)
            {
                Usuarios usuarios = db.Usuarios.Find(id);
                var id_back = usuarios.ID_empresa;
                TempData["error"] = "An error was handled.  " + ex.Message;
                return RedirectToAction("Demo_users", "Usuarios");
            }

        }


        //MERCHANDISING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRep(string nombre, string apellido, string correo, string telefono)
        {
            Usuarios usuarios = new Usuarios();

            usuarios.nombre = nombre;
            usuarios.apellido = apellido;
            usuarios.correo = correo;
            usuarios.telefono = telefono;


            if (usuarios.contrasena == null)
            {
                usuarios.contrasena = "c0m2018";
            }

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
            usuarios.cargo = "Representative";
            usuarios.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
            usuarios.ID_tipomembresia = 8;
            usuarios.ID_rol = 9;
            usuarios.fcreacion_usuario = DateTime.UtcNow;
            usuarios.activo = true;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                TempData["exito"] = "User created successfully.";
                return RedirectToAction("Representatives", "Home", null);
            }
            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("Representatives", "Home", null);
        }



    }
}
