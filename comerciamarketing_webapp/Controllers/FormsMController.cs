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
using System.Web.Script.Serialization;
using comerciamarketing_webapp.Models;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using Postal;

namespace comerciamarketing_webapp.Controllers
{
    public class FormsMController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities COM_MKdb = new COM_MKEntities();
  
        // GET: FormsMs
        public ActionResult Index()
        {
            var formsM = db.FormsM.Include(f => f.ActivitiesM);

            ViewBag.ID_activity = new SelectList(db.ActivitiesM, "ID_activity", "description");
            ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type, "ID_formresourcetype", "fdescription");
            ViewBag.vendors = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();

            return View(formsM.ToList());
        }

        // GET: FormsMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormsM formsM = db.FormsM.Find(id);
            if (formsM == null)
            {
                return HttpNotFound();
            }
            return View(formsM);
        }

        // GET: FormsMs/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                ViewBag.ID_activity = new SelectList(db.ActivitiesM, "ID_activity", "description");
                ViewBag.ID_formresourcetype = new SelectList(db.form_resource_type, "ID_formresourcetype", "fdescription");

                ViewBag.vendors = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home", null);
            }
          
        }

        //CLASE PARA ALMACENAR OBJETOS
        public class MyObj
        {
            public string id_resource { get; set; }
            public string fsource { get; set; }
            public string fdescription { get; set; }
            public string fvalue { get; set; }
            public int idkey { get; set; }
            public int parent { get; set; }
            public IList<MyObj> children { get; set; }
        }

        public int order2=0;
        public string nest = "";
        public void Savelist(IList<MyObj> parentList, int formid, int parent_id=0,int order= 0) {
            foreach (var item in parentList) {
                //AUMENTAMOS EL CONTADOR PARA ORDENAMIENTO
                order2++;
                // GUARDAMOS EL DETALLE PRINCIPAL O EL NODO HIJO

                FormsM_details detalle_nuevo = new FormsM_details();

                if (parent_id != 0)
                {
                    Console.WriteLine("GUARDANDO HIJO. PADRE ID: " + parent_id + ". ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = parent_id;
                    item.parent = parent_id;
                }
                else
                {
                    Console.WriteLine("GUARDANDO PADRE. ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = 0;
                    item.parent = 0;
                }
                detalle_nuevo.ID_formresourcetype = Convert.ToInt32(item.id_resource);
                detalle_nuevo.fsource = Convert.ToString(item.fsource);
                detalle_nuevo.fvalueText = "";
                if (Convert.ToInt32(item.id_resource) == 6) {
                    detalle_nuevo.fsource = "";
                    detalle_nuevo.fvalueText = Convert.ToString(item.fsource);
                }
             
                detalle_nuevo.fdescription = Convert.ToString(item.fdescription);
                detalle_nuevo.fvalue = 0;
                detalle_nuevo.fvalueDecimal = 0;

                detalle_nuevo.ID_formM = formid;
                //colocamos 0 ya que esta seria la plantila
                detalle_nuevo.ID_visit = 0;
                //Se coloca true ya que con esto identificamos que es un item del template original
                detalle_nuevo.original = true;
                //Colocamos numero de orden
                detalle_nuevo.obj_order = order2;
                //Colocamos grupo si tiene
                detalle_nuevo.obj_group = 0;
                //Colocamos ID generado por editor
                detalle_nuevo.idkey = order2;
                detalle_nuevo.query1 = "";
                detalle_nuevo.query2 = "";
                detalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
                //Guardando por tipo de recurso
                if (Convert.ToInt32(item.id_resource) == 6) //6 es el ID de del recurso de input_text para el tipo de comentario
                                                            //Categorias
                {
                    detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                }
                if (Convert.ToInt32(item.id_resource) == 24) //24 es el ID de del recurso de Column para el tipo de datos
                                                             //Tipo de datos
                {
                    detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                }
                db.FormsM_details.Add(detalle_nuevo);
                db.SaveChanges();

                //FIN NODO PADRE O HIJO
                //VERIFICAMOS SI TIENE NODOS HIJOS Y REALIZAMOS UN BUCLE A NUESTRO CONSTRUCTOR
                if (item.children != null) {
                    Savelist(item.children, formid, order2, order2);

                }



            }
            //Le asignamos los padres eh hijos al codigo para leerlo posteriormente
            JavaScriptSerializer js = new JavaScriptSerializer();
            nest = js.Serialize(parentList);


        }

        public void SavelistDemo(IList<MyObj> parentList, int formid, int parent_id = 0, int order = 0)
        {
            foreach (var item in parentList)
            {
                //AUMENTAMOS EL CONTADOR PARA ORDENAMIENTO
                order2++;
                // GUARDAMOS EL DETALLE PRINCIPAL O EL NODO HIJO

                FormsM_details detalle_nuevo = new FormsM_details();

                if (parent_id != 0)
                {
                    Console.WriteLine("GUARDANDO HIJO. PADRE ID: " + parent_id + ". ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = parent_id;
                    item.parent = parent_id;
                }
                else
                {
                    Console.WriteLine("GUARDANDO PADRE. ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = 0;
                    item.parent = 0;
                }
                detalle_nuevo.ID_formresourcetype = Convert.ToInt32(item.id_resource);
                detalle_nuevo.fsource = Convert.ToString(item.fsource);
                detalle_nuevo.fvalueText = "";
                if (Convert.ToInt32(item.id_resource) == 6)
                {
                    detalle_nuevo.fsource = "";
                    detalle_nuevo.fvalueText = Convert.ToString(item.fsource);
                }

                detalle_nuevo.fdescription = Convert.ToString(item.fdescription);
                detalle_nuevo.fvalue = 0;
                detalle_nuevo.fvalueDecimal = 0;

                detalle_nuevo.ID_formM = formid;
                //colocamos 0 ya que esta seria la plantila
                detalle_nuevo.ID_visit = 0;
                //Se coloca true ya que con esto identificamos que es un item del template original
                detalle_nuevo.original = true;
                //Colocamos numero de orden
                detalle_nuevo.obj_order = order2;
                //Colocamos grupo si tiene
                detalle_nuevo.obj_group = 1; //DEMOS MODULE
                //Colocamos ID generado por editor
                detalle_nuevo.idkey = order2;
                detalle_nuevo.query1 = "";
                detalle_nuevo.query2 = "";
                detalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
                //Guardando por tipo de recurso
                if (Convert.ToInt32(item.id_resource) == 6) //6 es el ID de del recurso de input_text para el tipo de comentario
                                                            //Categorias
                {
                    detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                }

                db.FormsM_details.Add(detalle_nuevo);
                db.SaveChanges();

                //FIN NODO PADRE O HIJO
                //VERIFICAMOS SI TIENE NODOS HIJOS Y REALIZAMOS UN BUCLE A NUESTRO CONSTRUCTOR
                if (item.children != null)
                {
                    SavelistDemo(item.children, formid, order2, order2);

                }



            }
            //Le asignamos los padres eh hijos al codigo para leerlo posteriormente
            JavaScriptSerializer js = new JavaScriptSerializer();
            nest = js.Serialize(parentList);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForm([Bind(Include = "ID_form,name,description, ID_activity,active")] FormsM formsM, string nestable_output, string action, string possibleID)
        {

            
            formsM.active = true;
            formsM.query1 = nestable_output;
            formsM.query2 = "";
            formsM.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
            if (ModelState.IsValid)
            {
                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(nestable_output);

                

                int idfinal = 0;
                
                if (details.Count() > 0)
                {
                    ////Guardamos formulario
                    //Determinamos si es un update o un create
                    if (action == "update")
                    {
                        FormsM formsMi = db.FormsM.Find(Convert.ToInt32(possibleID));

                        formsMi.name = formsM.name;
                        formsMi.description = formsM.description;
                        formsMi.ID_activity = formsM.ID_activity;
                        formsMi.query1 = nestable_output;
                        formsMi.query2 = "";
                        formsMi.ID_empresa = formsM.ID_empresa;


                        db.Entry(formsMi).State = EntityState.Modified;
                        db.SaveChanges();

                   
                        idfinal = formsMi.ID_form;

                        //Eliminamos el detalle
                        var detalles = (from t in db.FormsM_details where (t.ID_formM == formsMi.ID_form && t.original == true) select t).ToList();

                        foreach (var detalle in detalles) {
                            FormsM_details formsMDet = db.FormsM_details.Find(detalle.ID_details);
                            db.FormsM_details.Remove(formsMDet);
                            db.SaveChanges();
                        }


                    }
                    else if(action == "create")
                    {
                        db.FormsM.Add(formsM);
                        db.SaveChanges();

                        idfinal = formsM.ID_form;
                    }


                    ////Guardamos detalle de formulario con Original = true
                    //Llamamos el constructor el cual consiste en un bucle "infinito" en base a los nodos padre eh hijos que contenta el array de objetos del editor
                    //del formulario
                    order2 = 0;

                    if (formsM.ID_activity == 2)
                    {
                        Savelist_retail(details, idfinal);
                    }
                    //else if (formsM.ID_activity == 4)
                    //{
                    //    SavelistDemo(details, idfinal);
                    //}
                    else {
                        Savelist(details, idfinal);
                    }

                 


                    FormsM formsMlast = db.FormsM.Find(idfinal);

                    formsMlast.query2 = nest;

                    db.Entry(formsMlast).State = EntityState.Modified;
                    db.SaveChanges();

                    ////***********************************************

                    TempData["exito"] = "Form created successfully.";
                    return RedirectToAction("FormsM","Home",null);
                }
                else
                {
                    TempData["advertencia"] = "No data found, try again.";
                    return RedirectToAction("FormsM", "Home", null);
                }


            }


            TempData["advertencia"] = "Something wrong happened, try again.";
            return RedirectToAction("FormsM", "Home", null);
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string idFormD)
        {
            try
            {
                int IDform = Convert.ToInt32(idFormD);

                FormsM form = db.FormsM.Find(IDform);
                db.FormsM.Remove(form);
                db.SaveChanges();

                var detalle = (from d in db.FormsM_details where (d.ID_formM == IDform && d.original==true) select d).ToList();
                foreach (var det in detalle) {
                    FormsM_details detf = db.FormsM_details.Find(det.ID_details);
                    db.FormsM_details.Remove(detf);
                    db.SaveChanges();

                }


                TempData["exito"] = "Form deleted successfully.";
                return RedirectToAction("FormsM", "Home", null);
            }
            catch (Exception ex)
            {
                TempData["advertencia"] = "Something wrong happened, try again. " + ex.Message;
                return RedirectToAction("FormsM", "Home", null);
            }
        }

        public void Savelist_retail(IList<MyObj> parentList, int formid, int parent_id = 0, int order = 0)
        {
            var mainlist = parentList;
            var listaColumnas = (from a in parentList where (a.id_resource == "24") select a).ToList();
            foreach (var item in parentList)
            {
                //AUMENTAMOS EL CONTADOR PARA ORDENAMIENTO
                order2++;
                // GUARDAMOS EL DETALLE PRINCIPAL O EL NODO HIJO

                FormsM_details detalle_nuevo = new FormsM_details();

                if (parent_id != 0)
                {
                    Console.WriteLine("GUARDANDO HIJO. PADRE ID: " + parent_id + ". ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = parent_id;
                    item.parent = parent_id;
                }
                else
                {
                    Console.WriteLine("GUARDANDO PADRE. ID: " + item.idkey + "description: " + item.fsource);
                    //Si es nodo hijo se coloca el idkey del padre
                    detalle_nuevo.parent = 0;
                    item.parent = 0;
                }
                detalle_nuevo.ID_formresourcetype = Convert.ToInt32(item.id_resource);
                detalle_nuevo.fsource = Convert.ToString(item.fsource);
                detalle_nuevo.fvalueText = "";
                if (Convert.ToInt32(item.id_resource) == 6)
                {
                    detalle_nuevo.fsource = "";
                    detalle_nuevo.fvalueText = Convert.ToString(item.fsource);
                }

                detalle_nuevo.fdescription = Convert.ToString(item.fdescription);
                detalle_nuevo.fvalue = 0;
                detalle_nuevo.fvalueDecimal = 0;

                detalle_nuevo.ID_formM = formid;
                //colocamos 0 ya que esta seria la plantila
                detalle_nuevo.ID_visit = 0;
                //Se coloca true ya que con esto identificamos que es un item del template original
                detalle_nuevo.original = true;
                //Colocamos numero de orden
                detalle_nuevo.obj_order = order2;
                //Colocamos grupo si tiene
                detalle_nuevo.obj_group = 0;
                //Colocamos ID generado por editor
                detalle_nuevo.idkey = order2;
                detalle_nuevo.query1 = "";
                detalle_nuevo.query2 = "";
                detalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;
                //Guardando por tipo de recurso
                if (Convert.ToInt32(item.id_resource) == 6) //6 es el ID de del recurso de input_text para el tipo de comentario
                                                            //Categorias
                {
                    detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                }
                if (Convert.ToInt32(item.id_resource) == 24) //24 es el ID de del recurso de Column para el tipo de datos
                                                            //Tipo de datos
                {
                    detalle_nuevo.fvalue = Convert.ToInt32(item.fvalue);
                }
                db.FormsM_details.Add(detalle_nuevo);
                db.SaveChanges();

                //FIN NODO PADRE O HIJO
                //VERIFICAMOS SI TIENE NODOS HIJOS Y REALIZAMOS UN BUCLE A NUESTRO CONSTRUCTOR
                if (item.children != null)
                {
                    Savelist(item.children, formid, order2, order2);

                }

                //if (Convert.ToInt32(item.id_resource) == 3) { //PARA PRODUCTO
                //    foreach (var itemColumna in listaColumnas)
                //    {
                //        //AUMENTAMOS EL CONTADOR PARA ORDENAMIENTO
                //        order2++;
                //        if (itemColumna.fvalue == "16") {
                //            //Multiple choice
                //            FormsM_details Subdetalle_nuevo = new FormsM_details();

                //            Subdetalle_nuevo.parent = detalle_nuevo.idkey;
                //            Subdetalle_nuevo.ID_formresourcetype = 16;
                //            Subdetalle_nuevo.fsource = "";
                //            Subdetalle_nuevo.fvalueText = "";

                //            Subdetalle_nuevo.fdescription = Convert.ToString(itemColumna.fdescription);
                //            Subdetalle_nuevo.fvalue = 0;
                //            Subdetalle_nuevo.fvalueDecimal = 0;

                //            Subdetalle_nuevo.ID_formM = formid;
                //            //colocamos 0 ya que esta seria la plantila
                //            Subdetalle_nuevo.ID_visit = 0;
                //            //Se coloca true ya que con esto identificamos que es un item del template original
                //            Subdetalle_nuevo.original = true;
                //            //Colocamos numero de orden
                //            Subdetalle_nuevo.obj_order = order2;
                //            //Colocamos grupo si tiene
                //            Subdetalle_nuevo.obj_group = 0;
                //            //Colocamos ID generado por editor
                //            Subdetalle_nuevo.idkey = order2;
                //            Subdetalle_nuevo.query1 = "";
                //            Subdetalle_nuevo.query2 = "";
                //            Subdetalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;

                //            db.FormsM_details.Add(Subdetalle_nuevo);
                //            db.SaveChanges();
                //        }
                //        else if (itemColumna.fvalue == "21")
                //        {
                //            //Currency
                //            FormsM_details Subdetalle_nuevo = new FormsM_details();

                //            Subdetalle_nuevo.parent = detalle_nuevo.idkey;
                //            Subdetalle_nuevo.ID_formresourcetype = 21;
                //            Subdetalle_nuevo.fsource = "";
                //            Subdetalle_nuevo.fvalueText = "";

                //            Subdetalle_nuevo.fdescription = Convert.ToString(itemColumna.fdescription);
                //            Subdetalle_nuevo.fvalue = 0;
                //            Subdetalle_nuevo.fvalueDecimal = 0;

                //            Subdetalle_nuevo.ID_formM = formid;
                //            //colocamos 0 ya que esta seria la plantila
                //            Subdetalle_nuevo.ID_visit = 0;
                //            //Se coloca true ya que con esto identificamos que es un item del template original
                //            Subdetalle_nuevo.original = true;
                //            //Colocamos numero de orden
                //            Subdetalle_nuevo.obj_order = order2;
                //            //Colocamos grupo si tiene
                //            Subdetalle_nuevo.obj_group = 0;
                //            //Colocamos ID generado por editor
                //            Subdetalle_nuevo.idkey = order2;
                //            Subdetalle_nuevo.query1 = "";
                //            Subdetalle_nuevo.query2 = "";
                //            Subdetalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;

                //            db.FormsM_details.Add(Subdetalle_nuevo);
                //            db.SaveChanges();


                //        }
                //    }

                //}


            }
            //Le asignamos los padres eh hijos al codigo para leerlo posteriormente
            JavaScriptSerializer js = new JavaScriptSerializer();
            nest = js.Serialize(parentList);


        }

        // GET: FormsMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormsM formsM = db.FormsM.Find(id);
            if (formsM == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_activity = new SelectList(db.ActivitiesM, "ID_activity", "description", formsM.ID_activity);
            return View(formsM);
        }

        // POST: FormsMs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_form,name,description,ID_activity,active")] FormsM formsM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formsM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_activity = new SelectList(db.ActivitiesM, "ID_activity", "description", formsM.ID_activity);
            return View(formsM);
        }

        // GET: FormsMs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormsM formsM = db.FormsM.Find(id);
            if (formsM == null)
            {
                return HttpNotFound();
            }
            return View(formsM);
        }

        // POST: FormsMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FormsM formsM = db.FormsM.Find(id);
            db.FormsM.Remove(formsM);
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

        public ActionResult GetDynamicProducts(string activityID, string ID_customer)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                try
                {
                    HttpCookie aCookieCorreo = Request.Cookies["correo"];
                    HttpCookie aCookiePassword = Request.Cookies["pass"];

                    var correo = Server.HtmlEncode(aCookieCorreo.Value).ToString();
                    var pass = Server.HtmlEncode(aCookiePassword.Value).ToString();

                    using (var db = new dbComerciaEntities())
                    {
                        Session["activeUser"] = (from c in db.Usuarios where (c.correo == correo && c.contrasena == pass) select c).FirstOrDefault();
                    }

                    activeuser = Session["activeUser"] as Usuarios;

                    if (activeuser != null)
                    {

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { access = false });
                    }
                }
                catch
                {
                    return RedirectToAction("Index", "Home", new { access = false });
                }
            }

            try
            {
                int idact = Convert.ToInt32(activityID);
                List<OITM> lstproduct = new List<OITM>();
                string vendoriD = ID_customer;
                using (COM_MKEntities dbmk = new COM_MKEntities())
                {
                    lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD)).ToList<OITM>();
                }

                ActivitiesM act = (from actd in db.ActivitiesM where (actd.ID_activity == idact) select actd).FirstOrDefault();
                var countItems = (from a in db.FormsM_details where (a.ID_visit == idact) select a).Count();

                var nuevacuenta = countItems + 4;

                foreach (var item in lstproduct)
                {
                    try
                    {
                        FormsM_details detalle_nuevo = new FormsM_details();


                        detalle_nuevo.ID_formresourcetype = 3;
                        detalle_nuevo.fsource = item.ItemCode;
                        detalle_nuevo.fdescription = item.ItemName;
                        detalle_nuevo.fvalue = 0;
                        detalle_nuevo.fvalueDecimal = 0;
                        detalle_nuevo.fvalueText = "";
                        detalle_nuevo.ID_formM = act.ID_form;

                        detalle_nuevo.ID_visit = idact;
                        detalle_nuevo.original = false;
                        //Colocamos numero de orden
                        detalle_nuevo.obj_order = nuevacuenta;
                        //Colocamos grupo si tiene
                        detalle_nuevo.obj_group = 0;
                        //Colocamos ID generado por editor
                        detalle_nuevo.idkey = nuevacuenta;
                        detalle_nuevo.query1 = "";
                        detalle_nuevo.query2 = "";
                        detalle_nuevo.parent = 0;
                        detalle_nuevo.ID_empresa = Convert.ToInt32(activeuser.ID_empresa);



                        db.FormsM_details.Add(detalle_nuevo);
                        db.SaveChanges();
                        nuevacuenta++;
                    }
                    catch (Exception ex){
                        var error = ex.Message;
                        
                    }

                }

                FormsM_details lastitem = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems -2)) select a).FirstOrDefault();

                lastitem.obj_order = nuevacuenta + 1;
                lastitem.idkey = nuevacuenta + 1;
                db.Entry(lastitem).State = EntityState.Modified;
                db.SaveChanges();
                nuevacuenta++;
                FormsM_details lastitem2 = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems -1)) select a).FirstOrDefault();

                lastitem2.obj_order = nuevacuenta + 1;
                lastitem2.idkey = nuevacuenta + 1;
                db.Entry(lastitem2).State = EntityState.Modified;
                db.SaveChanges();
                nuevacuenta++;
                FormsM_details lastitem3 = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == countItems) select a).FirstOrDefault();

                lastitem3.obj_order = nuevacuenta + 1;
                lastitem3.idkey = nuevacuenta + 1;
                db.Entry(lastitem3).State = EntityState.Modified;
                db.SaveChanges();

                var customer = (from cust in COM_MKdb.OCRD where (cust.CardCode == ID_customer) select cust).FirstOrDefault();
                act.ID_customer = ID_customer;
                act.Customer = customer.CardName;
                db.Entry(act).State = EntityState.Modified;
                db.SaveChanges();


                string result = "Success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch {
                string result = "Error";
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            
        }

        public ActionResult GetDynamicProductsByBrand(string activityID, string ID_customer, string ID_brand)
        {

            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                try
                {
                    HttpCookie aCookieCorreo = Request.Cookies["correo"];
                    HttpCookie aCookiePassword = Request.Cookies["pass"];

                    var correo = Server.HtmlEncode(aCookieCorreo.Value).ToString();
                    var pass = Server.HtmlEncode(aCookiePassword.Value).ToString();

                    using (var db = new dbComerciaEntities())
                    {
                        Session["activeUser"] = (from c in db.Usuarios where (c.correo == correo && c.contrasena == pass) select c).FirstOrDefault();
                    }

                    activeuser = Session["activeUser"] as Usuarios;

                    if (activeuser != null)
                    {

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { access = false });
                    }
                }
                catch
                {
                    return RedirectToAction("Index", "Home", new { access = false });
                }
            }

            try
            {
                int idact = Convert.ToInt32(activityID);
                List<OITM> lstproduct = new List<OITM>();
                string vendoriD = ID_customer;
                int brand = Convert.ToInt32(ID_brand);
                using (COM_MKEntities dbmk = new COM_MKEntities())
                {
                    lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD && x.FirmCode == brand)).ToList<OITM>();
                }

                ActivitiesM act = (from actd in db.ActivitiesM where (actd.ID_activity == idact) select actd).FirstOrDefault();
                var countItems = (from a in db.FormsM_details where (a.ID_visit == idact) select a).Count();

                var nuevacuenta = countItems + 2;

                foreach (var item in lstproduct)
                {
                    try
                    {
                        FormsM_details detalle_nuevo = new FormsM_details();


                        detalle_nuevo.ID_formresourcetype = 3;
                        detalle_nuevo.fsource = item.ItemCode;
                        detalle_nuevo.fdescription = item.ItemName;
                        detalle_nuevo.fvalue = 0;
                        detalle_nuevo.fvalueDecimal = 0;
                        detalle_nuevo.fvalueText = "";
                        detalle_nuevo.ID_formM = act.ID_form;

                        detalle_nuevo.ID_visit = idact;
                        detalle_nuevo.original = false;
                        //Colocamos numero de orden
                        detalle_nuevo.obj_order = nuevacuenta;
                        //Colocamos grupo si tiene
                        detalle_nuevo.obj_group = 0;
                        //Colocamos ID generado por editor
                        detalle_nuevo.idkey = nuevacuenta;
                        detalle_nuevo.query1 = "";
                        detalle_nuevo.query2 = "";
                        detalle_nuevo.parent = 0;
                        detalle_nuevo.ID_empresa = Convert.ToInt32(activeuser.ID_empresa);



                        db.FormsM_details.Add(detalle_nuevo);
                        db.SaveChanges();
                        nuevacuenta++;

                        FormsM_details detalle_nuevo2 = new FormsM_details();


                        detalle_nuevo2.ID_formresourcetype = 22;
                        detalle_nuevo2.fsource = "Expiration Date";
                        detalle_nuevo2.fdescription = "";
                        detalle_nuevo2.fvalue = 0;
                        detalle_nuevo2.fvalueDecimal = 0;
                        detalle_nuevo2.fvalueText = "";
                        detalle_nuevo2.ID_formM = act.ID_form;

                        detalle_nuevo2.ID_visit = idact;
                        detalle_nuevo2.original = false;
                        //Colocamos numero de orden
                        detalle_nuevo2.obj_order = nuevacuenta;
                        //Colocamos grupo si tiene
                        detalle_nuevo2.obj_group = 0;
                        //Colocamos ID generado por editor
                        detalle_nuevo2.idkey = nuevacuenta;
                        detalle_nuevo2.query1 = "";
                        detalle_nuevo2.query2 = "";
                        detalle_nuevo2.parent = 0;
                        detalle_nuevo2.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;



                        db.FormsM_details.Add(detalle_nuevo2);
                        db.SaveChanges();
                        nuevacuenta++;

                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;

                    }

                }

                //FormsM_details lastitem = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems - 2)) select a).FirstOrDefault();

                //lastitem.obj_order = nuevacuenta + 1;
                //lastitem.idkey = nuevacuenta + 1;
                //db.Entry(lastitem).State = EntityState.Modified;
                //db.SaveChanges();
                //nuevacuenta++;
                FormsM_details lastitem2 = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems - 1)) select a).FirstOrDefault();

                lastitem2.obj_order = nuevacuenta + 1;
                lastitem2.idkey = nuevacuenta + 1;
                db.Entry(lastitem2).State = EntityState.Modified;
                db.SaveChanges();
                nuevacuenta++;
                FormsM_details lastitem3 = (from a in db.FormsM_details where (a.ID_visit == idact && a.idkey == countItems) select a).FirstOrDefault();

                lastitem3.obj_order = nuevacuenta + 1;
                lastitem3.idkey = nuevacuenta + 1;
                db.Entry(lastitem3).State = EntityState.Modified;
                db.SaveChanges();

                var customer = (from cust in COM_MKdb.OCRD where (cust.CardCode == ID_customer) select cust).FirstOrDefault();
                act.ID_customer = ID_customer;
                act.Customer = customer.CardName;
                db.Entry(act).State = EntityState.Modified;
                db.SaveChanges();


                string result = "Success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                string result = "Error";
                return Json(result, JsonRequestBehavior.AllowGet);
            }



        }

        //GET PRODUCTS, SAMPLES, GIFT, DISPLAYS, FORM DATA, GET BRANDS, GET PRODUCT LINE, GET ENEMYS

        public ActionResult Getproducts(string vendorID)
        {
            List<OITM> lstproduct = new List<OITM>();
            string vendoriD = vendorID;
            using (COM_MKEntities dbmk = new COM_MKEntities())
            {
                lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD)).OrderBy(x=>x.ItemName).ToList<OITM>();
            }

            foreach (var item in lstproduct) {

                item.ItemName = item.ItemName.Replace("\'", "");
            }




            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getsamples(string vendorID)
        {
            List<OITM> lstproduct = new List<OITM>();
            string vendoriD = vendorID;
            using (COM_MKEntities dbmk = new COM_MKEntities())
            {
                lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD && x.ItmsGrpCod == 107)).ToList<OITM>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getgifts(string vendorID)
        {
            List<OITM> lstproduct = new List<OITM>();
            string vendoriD = vendorID;
            using (COM_MKEntities dbmk = new COM_MKEntities())
            {
                lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD && x.ItmsGrpCod == 108)).ToList<OITM>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getdisplays(string vendorID)
        {
            List<OITM> lstproduct = new List<OITM>();
            string vendoriD = vendorID;
            using (COM_MKEntities dbmk = new COM_MKEntities())
            {
                lstproduct = (dbmk.OITM.Where(x => x.U_CustomerCM == vendoriD && x.ItemCode.Contains("DIS%"))).ToList<OITM>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(lstproduct);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Getformdata(string IDform)
        {
            FormsM formsM = db.FormsM.Find(Convert.ToInt32(IDform));

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = formsM.query1;

            //Deserealizamos  los datos
            JavaScriptSerializer js = new JavaScriptSerializer();
            MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

            return Json(details, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Getbrands(string customerID, string idvisit)
        {
            try
            {
                if (customerID != null)
                {
                    //List<view_CMKEditorB> lstbrands = new List<view_CMKEditorB>();

                    //using (COM_MKEntities dbmk = new COM_MKEntities())
                    //{
                    //lstbrands = (dbmk.view_CMKEditorB.Where(x => x.U_CustomerCM == customerID)).ToList<view_CMKEditorB>();

                    int IDV = Convert.ToInt32(idvisit);
                    var lstbrands = COM_MKdb.view_CMKEditorB
                            .Where(i => i.U_CustomerCM == customerID)
                            .Select(i => new brands { FirmCode = i.FirmCode.ToString(), FirmName = i.FirmName, isselected = false, Customer = "" })
                            .Distinct()
                            .OrderByDescending(i => i.FirmName)
                            .ToList();

                    var itemselectbrand = (from br in db.FormsM_details where (br.ID_formresourcetype == 13 && br.ID_visit == IDV) select br).FirstOrDefault();
                    foreach (var item in lstbrands)
                    {
                        if (item.FirmCode.ToString() == itemselectbrand.fvalueText)
                        {
                            item.isselected = true;
                        }

                    }

                    //}
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(lstbrands);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);

        }
            
        

        public ActionResult Getproductline(string brandID, string idvisit)
        {
            //List<view_CMKEditorB> lstproductline = new List<view_CMKEditorB>();

            //using (COM_MKEntities dbmk = new COM_MKEntities())
            //{
            //    lstproductline = (dbmk.view_CMKEditorB.Where(x => x.FirmCode.ToString() == brandID)).ToList<view_CMKEditorB>();
            //}
            if (brandID != null) {
                var lstproductline = COM_MKdb.view_CMKEditorB
        .Where(i => i.Id_subcategory != null && i.FirmCode.ToString() == brandID)
        .Select(i => new productline { Id_subcategory = i.Id_subcategory, SubCategory = i.SubCategory, isselected = false, Brand = "" })
        .Distinct()
        .OrderByDescending(i => i.SubCategory)
        .ToList();

                int IDV = Convert.ToInt32(idvisit);
                var itemselectbrand = (from br in db.FormsM_details where (br.ID_formresourcetype == 14 && br.ID_visit == IDV) select br).FirstOrDefault();
                if (itemselectbrand != null)
                {
                    foreach (var item in lstproductline)
                    {
                        if (item.Id_subcategory.ToString() == itemselectbrand.fvalueText)
                        {
                            item.isselected = true;
                        }

                    }
                }
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lstproductline);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);

        }
        public ActionResult Getbrandcompetitors(string brandID, string idvisit)
        {
            if (brandID != null)
            {
                var lstbrand = db.Brand_competitors
        .Where(i => i.ID_brand == brandID)
        .Select(i => new brandcompetitor { Id_brandc = i.ID_competitor.ToString(), namec = i.Name, isselected = false, Brand = "" })
        .Distinct()
        .OrderByDescending(i => i.namec)
        .ToList();

                int IDV = Convert.ToInt32(idvisit);
                var itemselectbrand = (from br in db.FormsM_details where (br.ID_formresourcetype == 15 && br.ID_visit == IDV) select br).FirstOrDefault();
                if (itemselectbrand != null)
                {
                    foreach (var item in lstbrand)
                    {
                        if (item.Id_brandc.ToString() == itemselectbrand.fvalueText)
                        {
                            item.isselected = true;
                        }

                    }
                }
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lstbrand);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);

        }
        public ActionResult Template_preview(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;


                    FormsM formsM = db.FormsM.Find(id);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO


                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    

                    //Cargamos las marcas
                    //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                    //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                    //    .Distinct()
                    //    .OrderByDescending(i => i.FirmName)
                    //    .ToList();

                    //ViewBag.brands = brandlist;

                    //Cargamos las lineas de procuctos
                    //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                    //.Where(i => i.Id_subcategory != null)
                    //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                    //.Distinct()
                    //.OrderByDescending(i => i.SubCategory)
                    //.ToList();

                    //ViewBag.productline = productlinelist;

                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_details
                                                                    where (item.ID_formM == id && item.parent == 0 && item.original == true)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in db.FormsM_details
                                                                  where (item.ID_formM == id && item.original == true)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///

                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = "0";
                    ViewBag.idvisita = "0";

                    ViewBag.details = categoriasListActivities;

                    ViewBag.detailssql = (from a in db.FormsM_details where (a.ID_formM == id && a.original == true) select a).ToList();




                    return View();
                

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                //return View(FormsM_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Template_preview2(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;


                    FormsM formsM = db.FormsM.Find(id);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();
                //Cargamos las marcas
                //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                //    .Distinct()
                //    .OrderByDescending(i => i.FirmName)
                //    .ToList();

                //ViewBag.brands = brandlist;

                //Cargamos las lineas de procuctos
                //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                //.Where(i => i.Id_subcategory != null)
                //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                //.Distinct()
                //.OrderByDescending(i => i.SubCategory)
                //.ToList();

                //ViewBag.productline = productlinelist;

                //NUEVO
                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_details
                                                                    where (item.parent == 0 && item.ID_formM==id && item.original == true)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in db.FormsM_details
                                                                  where (item.ID_formM ==id && item.original == true)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                var showbuttondynamic = (from item in db.FormsM_details
                                         where (item.ID_formM == id && item.ID_formresourcetype == 11)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {

                    ViewBag.mostrarboton = 1; //Lo mostramos
                }
                else
                {
                    ViewBag.mostrarboton = 0; //Lo ocultamos
                }

                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = 0;
                    ViewBag.idvisita = 0;

                    ViewBag.details = categoriasListActivities;






                    return View();



            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Template_preview4(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                FormsM formsM = db.FormsM.Find(Convert.ToInt32(id));

                //LISTADO DE CLIENTES
                var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                ViewBag.customers = customers.ToList();


                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.details = details;






                return View();
                //return View(FormsM_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult ActivityresumeC(int? id,string id_customer, string modulo, string ID_Customer, string brand, string fstartd, string fendd)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
            ViewBag.username = activeuser.nombre + " " + activeuser.apellido;

            ViewBag.id_customer = id_customer;

            var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

            if (activity == null)
            {
                return RedirectToAction("Main", "Home");
            }
            else
            {
                //Fechas
                DateTime filtrostartdate;
                DateTime filtroenddate;

                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var saturday = sunday.AddDays(6).AddHours(23);



                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }
                //
                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();
                FormsM formsM = db.FormsM.Find(activity.ID_form);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                if (activity.Customer != "")
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }
                else
                {
                    if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {

                        var customers = (from b in COM_MKdb.OCRD where (datosUsuario.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                }

                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();


                //NUEVO
                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0)
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                                      ).OrderBy(a => a.obj_order).ToList();


                List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).OrderBy(a => a.obj_order).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_formresourcetype == 3)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {

                    ViewBag.mostrarboton = 1;
                }
                else
                {
                    ViewBag.mostrarboton = 0;
                }
                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.idvisitareal = activity.ID_visit;
                ViewBag.idvisita = activity.ID_activity;

                ViewBag.details = categoriasListActivities;

                ViewBag.detailssql = FormsMDet;

                Session["detailsForm"] = FormsMDet;
                VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                ViewBag.storename = visitsM.store;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                return View();
            }

        }


        public ActionResult ActivityresumeR(int? id, string modulo, string ID_Customer, string brand)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
            ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
            var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

            if (activity == null)
            {
                return RedirectToAction("Main", "Home");
            }
            else
            {

                FormsM formsM = db.FormsM.Find(activity.ID_form);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                if (activity.Customer != "")
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }
                else
                {
                    if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {

                        var customers = (from b in COM_MKdb.OCRD where (datosUsuario.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                }

                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();


                //NUEVO
                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0)
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                                      ).OrderBy(a => a.obj_order).ToList();


                List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).OrderBy(a => a.obj_order).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_formresourcetype == 3)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {

                    ViewBag.mostrarboton = 1;
                }
                else
                {
                    ViewBag.mostrarboton = 0;
                }
                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.idvisitareal = activity.ID_visit;
                ViewBag.idvisita = activity.ID_activity;

                ViewBag.details = categoriasListActivities;

                ViewBag.detailssql = FormsMDet;

                Session["detailsForm"] = FormsMDet;
                VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                ViewBag.storename = visitsM.store;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                return View();
            }

        }


        public ActionResult Activityresume(int? id, string modulo, string ID_Customer, string brand)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
            ViewBag.username = activeuser.nombre + " " + activeuser.apellido;
            var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

            if (activity == null)
            {
                return RedirectToAction("Main", "Home");
            }
            else
            {

                FormsM formsM = db.FormsM.Find(activity.ID_form);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                if (activity.Customer != "")
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }
                else
                {
                    if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {

                        var customers = (from b in COM_MKdb.OCRD where (datosUsuario.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                }

                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();


                //NUEVO
                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0)
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                                      ).OrderBy(a => a.obj_order).ToList();


                List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).OrderBy(a => a.obj_order).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_formresourcetype == 3)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {

                    ViewBag.mostrarboton = 1;
                }
                else
                {
                    ViewBag.mostrarboton = 0;
                }
                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.idvisitareal = activity.ID_visit;
                ViewBag.idvisita = activity.ID_activity;

                ViewBag.details = categoriasListActivities;

                ViewBag.detailssql = FormsMDet;

                Session["detailsForm"] = FormsMDet;
                VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                ViewBag.storename = visitsM.store;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                return View();
            }

        }
        public ActionResult Activity_ea(string codigo)
        {

            if (codigo == "" || codigo == null || codigo.Length <8)
            {
                TempData["advertencia"] = "Invalid or expired code. Please check the activity data in your email.";
                return RedirectToAction("Internal", "Home", null);
            }
            else
            {
                //devolvemos el codigo asignado en base a buscar el codigo introducido

                //ID_de la actividad ACCMK00
                var solocodigo = codigo.Substring(7);
                int id = 0;
                try
                {
                    id = Convert.ToInt32(solocodigo);
                }
                catch {
                    TempData["advertencia"] = "Invalid or expired code. Please check the activity data in your email.";
                    return RedirectToAction("Internal", "Home", null);
                }
               


                var activity = (from v in db.Demos where (v.ID_demo == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    TempData["advertencia"] = "Invalid or expired code. Please check the activity data in your email.";
                    return RedirectToAction("Internal", "Home");
                }
                else
                {
                    //Verificamos si no esta finalizada
                    if (activity.ID_demostate  ==4) {
                        TempData["exito"] = "This demo is finished.";
                        return RedirectToAction("Internal", "Home");
                    }

                    //Verificamos si esta en el periodo activo

                    DateTime start = Convert.ToDateTime(activity.visit_date).Date;
                    DateTime end = Convert.ToDateTime(activity.visit_date).AddDays(1).Date;
                    DateTime today = DateTime.Today.Date;
                    if (today >= start && today <= end)
                    {
                        int mostrarCheckin = 0;

                        if (activity.ID_demostate == 3)
                        {
                            mostrarCheckin = 1;
                        }

                        ViewBag.checkin = mostrarCheckin;
                    }
                    else
                    {
                        TempData["advertencia"] = "This demo is not available.";
                        return RedirectToAction("Internal", "Home");
                    }

                    FormsM formsM = db.FormsM.Find(activity.ID_formM);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.vendor != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_Vendor) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                    //Cargamos las marcas
                    //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                    //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                    //    .Distinct()
                    //    .OrderByDescending(i => i.FirmName)
                    //    .ToList();

                    //ViewBag.brands = brandlist;

                    //Cargamos las lineas de procuctos
                    //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                    //.Where(i => i.Id_subcategory != null)
                    //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                    //.Distinct()
                    //.OrderByDescending(i => i.SubCategory)
                    //.ToList();

                    //ViewBag.productline = productlinelist;

                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_detailsDemos
                                                                    where (item.parent == 0 && item.ID_visit == activity.ID_demo && item.original == false)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in db.FormsM_detailsDemos
                                                                  where (item.ID_visit == activity.ID_demo && item.original == false)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///

                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisita = activity.ID_demo;

                    ViewBag.details = categoriasListActivities;

                    ViewBag.detailssql = (from a in db.FormsM_detailsDemos where (a.ID_visit == activity.ID_demo && a.original == false) select a).ToList();



                    Session["detailsForm"] = (from f in db.FormsM_detailsDemos where (f.ID_visit == id) select f).ToList();
                    ViewBag.storename = activity.store;
                    ViewBag.address = activity.address + ", " + activity.state + ", " + activity.city + ", " + activity.zipcode;
                    return View();

                }


            }
     
        }

        public ActionResult Activity(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;
                var iscopy="";
                if (id == null) {
                    //Asignamos el id a 
                    if (Request.Cookies["IDactivitycopy"] == null)
                    {
                            HttpCookie aCookie = new HttpCookie("IDactivitycopy");
                            aCookie.Value = "copy";
                            aCookie.Expires = DateTime.Now.AddMonths(3);
                            Response.Cookies.Add(aCookie);
                            iscopy = "copy";
                    }
                    else {

                        var cookie = Request.Cookies["IDactivitycopy"];
                        iscopy = cookie.Value.ToString();
                    }

                   
                }

                ActivitiesM activity = new ActivitiesM();
                if (iscopy != "")
                {
                    var sqlQueryText = string.Format("SELECT * FROM ActivitiesM WHERE query1 LIKE '{0}'", iscopy);
                    activity = db.ActivitiesM.SqlQuery(sqlQueryText).FirstOrDefault(); //returns 0 or more rows satisfying sql query
                    //activity = (from v in db.ActivitiesM where (v.query1.ToString() == iscopy) select v).FirstOrDefault();
                }
                else {
                    activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();
                }


                var customer = "";
                
                if (activity == null)
                {
                    var cookie2 = Request.Cookies["currentvisit"];

                    ViewBag.idvisitareal = cookie2.Value.ToString(); ;
                    ViewBag.idvisita = iscopy;

                }
                else {
                    if (iscopy != "")
                    {
                        var cookie2 = Request.Cookies["currentvisit"];
                        ViewBag.idvisitareal = cookie2.Value.ToString(); ;
                        ViewBag.idvisita = iscopy;
                    }
                    else {
                        customer = activity.Customer;

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;
                    }

                }

                    //FormsM formsM = db.FormsM.Find(activity.ID_form);
                    
                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    if (customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode==activity.ID_customer) select new { CardCode= b.CardCode, CardName=b.CardName }).OrderBy(b => b.CardName).ToArray();
                        string result = javaScriptSerializer.Serialize(customers);
                        ViewBag.customers = result;
                       
                        
                    }
                    else {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select new { CardCode = b.CardCode, CardName = b.CardName }).OrderBy(b => b.CardName).ToArray();
                          string result = javaScriptSerializer.Serialize(customers);
                        ViewBag.customers = result;
                    }

                    //DESCARGAMOS TODAS LAS MARCAS SI FUERA NECESARIO SINO, SOLO LAS ESPECIFICAS POR CUSTOMER
                    if (customer != "")
                    {
                        var lstbrands = COM_MKdb.view_CMKEditorB
                                .Where(i => i.U_CustomerCM == activity.ID_customer)
                                .Select(i => new brands { FirmCode = i.FirmCode.ToString(), FirmName = i.FirmName, isselected = false, Customer = i.U_CustomerCM })
                                .Distinct()
                                .OrderByDescending(i => i.FirmName)
                                .ToArray();
                        string resultarray = javaScriptSerializer.Serialize(lstbrands);
                        ViewBag.lstbrands = resultarray;
                    }
                    else {
                        var lstbrands = COM_MKdb.view_CMKEditorB                           
                            .Select(i => new brands { FirmCode = i.FirmCode.ToString(), FirmName = i.FirmName, isselected = false, Customer = i.U_CustomerCM })
                            .Distinct()
                            .OrderByDescending(i => i.FirmName)
                            .ToArray();

                        string resultarray = javaScriptSerializer.Serialize(lstbrands);
                        ViewBag.lstbrands = resultarray;
                    }
                    //DESCARGAMOS LISTA DE LINEA DE PRODUCTOS
                        var lstproductline = COM_MKdb.view_CMKEditorB
                    .Where(i => i.Id_subcategory != null)
                    .Select(i => new productline { Id_subcategory = i.Id_subcategory, SubCategory = i.SubCategory, isselected = false, Brand = i.FirmCode.ToString()  })
                    .Distinct()
                    .OrderByDescending(i => i.SubCategory)
                    .ToArray();
                    string productline = javaScriptSerializer.Serialize(lstproductline);
                    ViewBag.lstproductline = productline;
                    //DESCARGAMOS BRAND COMPETITORS
                    var lstbrandc = db.Brand_competitors
                            .Select(i => new brandcompetitor { Id_brandc = i.ID_competitor.ToString(), namec = i.Name, isselected = false, Brand = i.ID_brand })
                            .Distinct()
                            .OrderByDescending(i => i.namec)
                            .ToArray();
                    string bcompetitors = javaScriptSerializer.Serialize(lstbrandc);
                    ViewBag.lstbcompetitors = bcompetitors;


                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    //MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);
                    //MyObj_tablapadre[] categoriasListActivities = js.Deserialize<MyObj_tablapadre[]>("");

                 

                    //ViewBag.details = categoriasListActivities.ToList();

                    //ViewBag.detailssql = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();




                    return View();
                

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                //return View(FormsM_details.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Activityra(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {

                    FormsM formsM = db.FormsM.Find(activity.ID_form);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                    //Cargamos las marcas
                    //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                    //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                    //    .Distinct()
                    //    .OrderByDescending(i => i.FirmName)
                    //    .ToList();

                    //ViewBag.brands = brandlist;

                    //Cargamos las lineas de procuctos
                    //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                    //.Where(i => i.Id_subcategory != null)
                    //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                    //.Distinct()
                    //.OrderByDescending(i => i.SubCategory)
                    //.ToList();

                    //ViewBag.productline = productlinelist;

                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_details
                                                                    where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in db.FormsM_details
                                                                  where (item.ID_visit == activity.ID_activity && item.original == false)
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///

                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;

                    ViewBag.details = categoriasListActivities;






                    return View();
                }

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                //return View(FormsM_details.ToList());


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
            public string value { get; set; }
        }
        public class brands
        {
            public string FirmCode { get; set; }
            public string FirmName { get; set; }
            public string Customer { get; set; }
            public Boolean isselected { get; set; }
        }
        public class productline
        {
            public string Id_subcategory { get; set; }
            public string SubCategory { get; set; }
            public string Brand { get; set; }
            public Boolean isselected { get; set; }
        }

        public class brandcompetitor
        {
            public string Id_brandc { get; set; }
            public string namec { get; set; }
            public string Brand { get; set; }
            public Boolean isselected { get; set; }
        }
        public JsonResult Save_activityra(string id, List<MyObj_formtemplate> objects, string lat, string lng, string check_in)
        {
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);
                    ActivitiesM activity = db.ActivitiesM.Find(act);
                    activity.check_out = Convert.ToDateTime(check_in);
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (lat != null || lat != "")
                    {
                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = lat;
                        nuevoLog.longitude = lng;
                        nuevoLog.ID_usuario = IDU;
                        nuevoLog.ID_activity = Convert.ToInt32(id);
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
                        nuevoLog.query1 = "";
                        nuevoLog.query2 = "";
                        nuevoLog.action = "SAVE ACTIVITY - " + activity.description;
                        nuevoLog.ip = "";
                        nuevoLog.hostname = "";
                        nuevoLog.typeh = "";
                        nuevoLog.continent_name = "";
                        nuevoLog.country_code = "";
                        nuevoLog.country_name = "";
                        nuevoLog.region_code = "";
                        nuevoLog.region_name = "";
                        nuevoLog.city = "";

                        db.ActivitiesM_log.Add(nuevoLog);
                        db.SaveChanges();
                    }


                    //Guardamos el detalle del formlario
                    foreach (var item in objects)
                    {
                        int IDItem = Convert.ToInt32(item.id);
                        FormsM_details detail = (from f in db.FormsM_details where (f.ID_visit == activity.ID_activity && f.idkey == IDItem) select f).FirstOrDefault();
                        if (detail == null)
                        {

                        }
                        else
                        {
                            //if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                            //{
                            //    if (item.value == "" || item.value == null) { item.value = "0"; }
                            //    detail.fvalue = Convert.ToInt32(item.value);

                            //    db.Entry(detail).State = EntityState.Modified;                        
                            //    db.SaveChanges();

                            //}
                            if (detail.ID_formresourcetype == 5) //Picture
                            {
                                //
                                if (item.value == "100")
                                {
                                    var path = detail.fsource;
                                    //eliminamos la ruta
                                    detail.fsource = "";

                                    db.Entry(detail).State = EntityState.Modified;
                                    db.SaveChanges();


                                    if (System.IO.File.Exists(Server.MapPath(path)))
                                    {
                                        try
                                        {
                                            System.IO.File.Delete(Server.MapPath(path));
                                        }
                                        catch (System.IO.IOException e)
                                        {
                                            Console.WriteLine(e.Message);

                                        }
                                    }
                                }




                            }
                            else if (detail.ID_formresourcetype == 6 || detail.ID_formresourcetype == 9) //Input text y Electronic Signature
                            {

                                if (item.value == "" || item.value == null) { item.value = ""; }

                                detail.fsource = item.value;

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else if (detail.ID_formresourcetype == 18) //Input number
                            {

                                if (item.value == "" || item.value == null) { item.value = "0"; }

                                detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else if (detail.ID_formresourcetype == 21) // currency
                            {

                                if (item.value == "" || item.value == null) { item.value = "0"; }

                                detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                                //db.Entry(detail).State = EntityState.Modified;
                                //db.SaveChanges();
                            }
                            //Select, Customer, Brands,Product line, Brand Competitors
                            else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15)
                            {

                                detail.fvalueText = item.value; //Lo guardamos como texto por si colocan ID tipo cadena
                                detail.fdescription = item.text;

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            else if (detail.ID_formresourcetype == 19 || detail.ID_formresourcetype == 16) //Select
                            {

                                if (item.value == "" || item.value == null) { item.value = "false"; }
                                int seleccionado = 0;
                                if (item.value == "false")
                                {
                                    seleccionado = 0;
                                }
                                else if (item.value == "true")
                                {
                                    seleccionado = 1;
                                }
                                detail.fvalue = seleccionado; //Lo guardamos como entero

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                //No hacemos nada porque no esta registrado
                            }

                        }


                    }


                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        public JsonResult Save_activity(string id, List<MyObj_formtemplate> objects, string lat, string lng, string check_in)
        {
            List<FormsM_details> detailsForm = Session["detailsForm"] as List<FormsM_details>;
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);
                    //ActivitiesM activity = db.ActivitiesM.Find(act);
                    //activity.check_out = Convert.ToDateTime(check_in);
                    //db.Entry(activity).State = EntityState.Modified;             
                    //db.SaveChanges();

                    //if (lat != null || lat != "")
                    //{
                    //    //Guardamos el log de la actividad
                    //    ActivitiesM_log nuevoLog = new ActivitiesM_log();
                    //    nuevoLog.latitude = lat;
                    //    nuevoLog.longitude = lng;
                    //    nuevoLog.ID_usuario = IDU;
                    //    nuevoLog.ID_activity = Convert.ToInt32(id);
                    //    nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
                    //    nuevoLog.query1 = "";
                    //    nuevoLog.query2 = "";
                    //    nuevoLog.action = "SAVE ACTIVITY - " + activity.description;
                    //    nuevoLog.ip = "";
                    //    nuevoLog.hostname = "";
                    //    nuevoLog.typeh = "";
                    //    nuevoLog.continent_name = "";
                    //    nuevoLog.country_code = "";
                    //    nuevoLog.country_name = "";
                    //    nuevoLog.region_code = "";
                    //    nuevoLog.region_name = "";
                    //    nuevoLog.city = "";

                    //    db.ActivitiesM_log.Add(nuevoLog);
                    //    db.SaveChanges();
                    //}


                    //Guardamos el detalle del formlario
                    if(objects != null) { 
                    foreach (var item in objects)
                    {
                        int IDItem = Convert.ToInt32(item.id);
                        FormsM_details detail = (from f in detailsForm where (f.ID_visit == act &&  f.idkey== IDItem) select f).FirstOrDefault();
                        if (detail == null)
                        {

                        }
                        else
                        {
                                //if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                                //{
                                //    if (item.value == "" || item.value == null) { item.value = "0"; }
                                //    detail.fvalue = Convert.ToInt32(item.value);

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();

                                //}
                                //else 
                                if (detail.ID_formresourcetype == 5) //Picture
                                {
                                    //
                                    if (item.value == "100")
                                    {
                                        var path = detail.fsource;
                                        //eliminamos la ruta
                                        detail.fsource = "";

                                        db.Entry(detail).State = EntityState.Modified;



                                        if (System.IO.File.Exists(Server.MapPath(path)))
                                        {
                                            try
                                            {
                                                System.IO.File.Delete(Server.MapPath(path));
                                            }
                                            catch (System.IO.IOException e)
                                            {
                                                Console.WriteLine(e.Message);

                                            }
                                        }
                                    }




                                }
                                else if (detail.ID_formresourcetype == 9) //Input text y Electronic Signature
                                {

                                if (item.value == "" || item.value == null) { item.value = ""; }
                                    if (detail.fsource != item.value) {
                                        detail.fsource = item.value;

                                        db.Entry(detail).State = EntityState.Modified;
                                        
                                    }
 
                            }
                            //else if (detail.ID_formresourcetype == 18) //Input number
                            //{

                            //    if (item.value == "" || item.value == null) { item.value = "0"; }

                            //    detail.fvalueDecimal = Convert.ToDecimal(item.value);

                            //    db.Entry(detail).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                            //else if (detail.ID_formresourcetype == 21) // currency
                            //{

                            //    if (item.value == "" || item.value == null) { item.value = "0"; }

                            //    detail.fvalueDecimal = Convert.ToDecimal(item.value);

                            //    db.Entry(detail).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //    //db.Entry(detail).State = EntityState.Modified;
                            //    //db.SaveChanges();
                            //}
                            //Select, Customer, Brands,Product line, Brand Competitors
                            //else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15) 
                            //{                               
                                
                            //    detail.fvalueText = item.value; //Lo guardamos como texto por si colocan ID tipo cadena
                            //    detail.fdescription = item.text;

                            //    db.Entry(detail).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //}

                            //else if (detail.ID_formresourcetype == 19 || detail.ID_formresourcetype == 16) //Select
                            //{

                            //    if (item.value == "" || item.value == null) { item.value = "false"; }
                            //    int seleccionado = 0;
                            //    if (item.value == "false")
                            //    {
                            //        seleccionado = 0;
                            //    }
                            //    else if (item.value == "true") {
                            //        seleccionado = 1;
                            //    }
                            //    detail.fvalue = seleccionado; //Lo guardamos como entero

                            //    db.Entry(detail).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                            else
                            {
                                //No hacemos nada porque no esta registrado
                            }

                        }


                    }
                        db.SaveChanges();

                        Session["detailsForm"] = detailsForm;
                    }

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex) {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        public JsonResult Save_activityDemo(string id, List<MyObj_formtemplate> objects, string lat, string lng, string check_in)
        {
            List<FormsM_detailsDemos> detailsForm = Session["detailsForm"] as List<FormsM_detailsDemos>;
            try
            {
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);


                    //Guardamos el detalle del formlario
                    if (objects != null)
                    {
                        foreach (var item in objects)
                        {
                            int IDItem = Convert.ToInt32(item.id);
                            FormsM_detailsDemos detail = (from f in detailsForm where (f.ID_visit == act && f.idkey == IDItem) select f).FirstOrDefault();
                            if (detail == null)
                            {

                            }
                            else
                            {
                                //if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                                //{
                                //    if (item.value == "" || item.value == null) { item.value = "0"; }
                                //    detail.fvalue = Convert.ToInt32(item.value);

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();

                                //}
                                //else 
                                if (detail.ID_formresourcetype == 5) //Picture
                                {
                                    //
                                    if (item.value == "100")
                                    {
                                        var path = detail.fsource;
                                        //eliminamos la ruta
                                        detail.fsource = "";

                                        db.Entry(detail).State = EntityState.Modified;



                                        if (System.IO.File.Exists(Server.MapPath(path)))
                                        {
                                            try
                                            {
                                                System.IO.File.Delete(Server.MapPath(path));
                                            }
                                            catch (System.IO.IOException e)
                                            {
                                                Console.WriteLine(e.Message);

                                            }
                                        }
                                    }




                                }
                                else if (detail.ID_formresourcetype == 9) //Input text y Electronic Signature
                                {

                                    if (item.value == "" || item.value == null) { item.value = ""; }
                                    if (detail.fsource != item.value)
                                    {
                                        detail.fsource = item.value;

                                        db.Entry(detail).State = EntityState.Modified;

                                    }

                                }
                                //else if (detail.ID_formresourcetype == 18) //Input number
                                //{

                                //    if (item.value == "" || item.value == null) { item.value = "0"; }

                                //    detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}
                                //else if (detail.ID_formresourcetype == 21) // currency
                                //{

                                //    if (item.value == "" || item.value == null) { item.value = "0"; }

                                //    detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //    //db.Entry(detail).State = EntityState.Modified;
                                //    //db.SaveChanges();
                                //}
                                //Select, Customer, Brands,Product line, Brand Competitors
                                //else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15) 
                                //{                               

                                //    detail.fvalueText = item.value; //Lo guardamos como texto por si colocan ID tipo cadena
                                //    detail.fdescription = item.text;

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}

                                //else if (detail.ID_formresourcetype == 19 || detail.ID_formresourcetype == 16) //Select
                                //{

                                //    if (item.value == "" || item.value == null) { item.value = "false"; }
                                //    int seleccionado = 0;
                                //    if (item.value == "false")
                                //    {
                                //        seleccionado = 0;
                                //    }
                                //    else if (item.value == "true") {
                                //        seleccionado = 1;
                                //    }
                                //    detail.fvalue = seleccionado; //Lo guardamos como entero

                                //    db.Entry(detail).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}
                                else
                                {
                                    //No hacemos nada porque no esta registrado
                                }

                            }


                        }
                        db.SaveChanges();

                        Session["detailsForm"] = detailsForm;
                    }

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        public JsonResult Save_activityByitemDemos(string id, List<MyObj_formtemplate> objects)
        {
            try
            {
                List<FormsM_detailsDemos> detailsForm = Session["detailsForm"] as List<FormsM_detailsDemos>;
                int act = Convert.ToInt32(id);
                if (detailsForm != null)
                {

                }
                else
                {
                    using (var dbs = new dbComerciaEntities())
                    {

                        detailsForm = dbs.FormsM_detailsDemos.Where(a => a.ID_visit == act).ToList();
                    }

                }

                //int IDU = Convert.ToInt32(Session["IDusuario"]);
                    if (id != null)
                    {
                     
                        //Guardamos el detalle del formlario
                        foreach (var item in objects)
                        {
                            int IDItem = Convert.ToInt32(item.id);
                            var detail = (from f in detailsForm where (f.ID_visit == act && f.idkey == IDItem) select f).FirstOrDefault();
                            if (detail == null)
                            {

                            }
                            else
                            {
                                if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                                {
                                    if (item.value == "" || item.value == null) { item.value = "0"; }

                                    if (detail.fvalue != Convert.ToInt32(item.value))
                                    {
                                        detail.fvalue = Convert.ToInt32(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();


                                    }



                                }
                                else if (detail.ID_formresourcetype == 5) //Picture
                                {
                                    if (item.value == "100")
                                    {
                                        var path = detail.fsource;
                                        //eliminamos la ruta
                                        detail.fsource = "";

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();


                                        if (System.IO.File.Exists(Server.MapPath(path)))
                                        {
                                            try
                                            {
                                                System.IO.File.Delete(Server.MapPath(path));
                                            }
                                            catch (System.IO.IOException e)
                                            {
                                                Console.WriteLine(e.Message);

                                            }
                                        }
                                    }



                                }
                                else if (detail.ID_formresourcetype == 6 || detail.ID_formresourcetype == 9) //Input text y Electronic Signature
                                {

                                    if (item.value == "" || item.value == null) { item.value = ""; }
                                    if (detail.fsource != item.value)
                                    {
                                        detail.fsource = item.value;

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                                else if (detail.ID_formresourcetype == 18) //Input number
                                {

                                    if (item.value == "" || item.value == null) { item.value = "0"; }
                                    if (detail.fvalueDecimal != Convert.ToDecimal(item.value))
                                    {
                                        detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                                else if (detail.ID_formresourcetype == 21) // currency
                                {

                                    if (item.value == "" || item.value == null) { item.value = "0"; }

                                    if (detail.fvalueDecimal != Convert.ToDecimal(item.value))
                                    {
                                        detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }

                                else if (detail.ID_formresourcetype == 22) // Date
                                {

                                    if (item.value == "" || item.value == null) { item.value = ""; }

                                    try
                                    {
                                        detail.fvalueText = Convert.ToDateTime(item.value).ToShortDateString();
                                    }
                                    catch
                                    {
                                        detail.fvalueText = "";
                                    }


                                    db.Entry(detail).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //db.Entry(detail).State = EntityState.Modified;
                                    //db.SaveChanges();
                                }

                                //Select, Customer, Brands,Product line, Brand Competitors 
                                else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15)
                                {
                                    if (detail.fvalueText != item.value)
                                    {
                                        detail.fvalueText = item.value; //Lo guardamos como texto por si colocan ID tipo cadena
                                        detail.fdescription = item.text;

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }

                                else if (detail.ID_formresourcetype == 19 || detail.ID_formresourcetype == 16) //checkbox,radio
                                {

                                    if (item.value == "" || item.value == null) { item.value = "false"; }
                                    int seleccionado = 0;
                                    if (item.value == "false")
                                    {
                                        seleccionado = 0;
                                    }
                                    else if (item.value == "true")
                                    {
                                        seleccionado = 1;
                                    }

                                    if (detail.fvalue != seleccionado)
                                    {
                                        detail.fvalue = seleccionado; //Lo guardamos como entero

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }


                                }
                                else
                                {
                                    //No hacemos nada porque no esta registrado
                                }

                            }


                        }
                        Session["detailsForm"] = detailsForm;

                        return Json(new { Result = "Success" });
                    }
                //}
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        public JsonResult Save_activityByitem(string id, List<MyObj_formtemplate> objects)
        {
            try
            {
                List<FormsM_details> detailsForm = Session["detailsForm"] as List<FormsM_details>;
                int act = Convert.ToInt32(id);
                if (detailsForm != null)
                {

                }
                else {
                    using (var dbs = new dbComerciaEntities())
                    {
                    
                        detailsForm = dbs.FormsM_details.Where(a => a.ID_visit == act).ToList();
                    }
                
                }

                
                    //int IDU = Convert.ToInt32(Session["IDusuario"]);
                    if (id != null)
                    {
                       
                        //Guardamos el detalle del formlario
                        foreach (var item in objects)
                        {
                            int IDItem = Convert.ToInt32(item.id);
                            var detail = (from f in detailsForm where (f.ID_visit == act && f.idkey == IDItem) select f).FirstOrDefault();
                            if (detail == null)
                            {

                            }
                            else
                            {
                                if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                                {
                                    if (item.value == "" || item.value == null) { item.value = "0"; }

                                    if (detail.fvalue != Convert.ToInt32(item.value)) {
                                        detail.fvalue = Convert.ToInt32(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();

                                        
                                    }



                                }
                                else if (detail.ID_formresourcetype == 5) //Picture
                                {
                                    if (item.value == "100")
                                    {
                                        var path = detail.fsource;
                                        //eliminamos la ruta
                                        detail.fsource = "";

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();


                                        if (System.IO.File.Exists(Server.MapPath(path)))
                                        {
                                            try
                                            {
                                                System.IO.File.Delete(Server.MapPath(path));
                                            }
                                            catch (System.IO.IOException e)
                                            {
                                                Console.WriteLine(e.Message);

                                            }
                                        }
                                    }



                                }
                                else if (detail.ID_formresourcetype == 6 || detail.ID_formresourcetype == 9) //Input text y Electronic Signature
                                {

                                    if (item.value == "" || item.value == null) { item.value = ""; }
                                    if (detail.fsource != item.value) {
                                        detail.fsource = item.value;

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                                else if (detail.ID_formresourcetype == 18) //Input number
                                {

                                    if (item.value == "" || item.value == null) { item.value = "0"; }
                                    if (detail.fvalueDecimal != Convert.ToDecimal(item.value)) {
                                        detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
 
                                }
                                else if (detail.ID_formresourcetype == 21) // currency
                                {

                                    if (item.value == "" || item.value == null) { item.value = "0"; }

                                    if (detail.fvalueDecimal != Convert.ToDecimal(item.value))
                                    {
                                        detail.fvalueDecimal = Convert.ToDecimal(item.value);

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                       
                                }

                                else if (detail.ID_formresourcetype == 22) // Date
                                {

                                    if (item.value == "" || item.value == null) { item.value = ""; }

                                    try
                                    {
                                        detail.fvalueText = Convert.ToDateTime(item.value).ToShortDateString();
                                    }
                                    catch
                                    {
                                        detail.fvalueText = "";
                                    }


                                    db.Entry(detail).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //db.Entry(detail).State = EntityState.Modified;
                                    //db.SaveChanges();
                                }

                                //Select, Customer, Brands,Product line, Brand Competitors 
                                else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15)
                                {
                                    if (detail.fvalueText != item.value) {
                                        detail.fvalueText = item.value; //Lo guardamos como texto por si colocan ID tipo cadena
                                        detail.fdescription = item.text;

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }

                                else if (detail.ID_formresourcetype == 19 || detail.ID_formresourcetype == 16) //checkbox,radio
                                {

                                    if (item.value == "" || item.value == null) { item.value = "false"; }
                                    int seleccionado = 0;
                                    if (item.value == "false")
                                    {
                                        seleccionado = 0;
                                    }
                                    else if (item.value == "true")
                                    {
                                        seleccionado = 1;
                                    }

                                    if (detail.fvalue != seleccionado) {
                                        detail.fvalue = seleccionado; //Lo guardamos como entero

                                        db.Entry(detail).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

          
                                }
                                else
                                {
                                    //No hacemos nada porque no esta registrado
                                }

                            }


                        }
                        Session["detailsForm"] = detailsForm;

                        return Json(new { Result = "Success" });
                    }
                //}
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        [HttpPost]
        public ActionResult UploadFiles(string id,string idvisita, string orientation)
        {
            List<FormsM_details> detailsForm = Session["detailsForm"] as List<FormsM_details>;
            //buscamos el id del detalle
            int idf = Convert.ToInt32(id);
            if (detailsForm != null)
            {

            }
            else
            {
                using (var dbs = new dbComerciaEntities())
                {

                    detailsForm = dbs.FormsM_details.Where(a => a.ID_visit == idf).ToList();
                }

            }
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
                            int or = Convert.ToInt32(orientation);
                                
                                switch (or)
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
                        catch
                        {

                        }
                       
                        FormsM_details detail = new FormsM_details();
                        try
                        {
                            int idvisit = Convert.ToInt32(idvisita);
                            detail = (from d in detailsForm where (d.idkey == idf && d.ID_visit == idvisit) select d).FirstOrDefault();
                        }
                        catch {
                            var sqlQueryText = string.Format("SELECT * FROM FormsM_details WHERE query2 LIKE '{0}' and idkey='" + idf + "'", idvisita);
                            detail = db.FormsM_details.SqlQuery(sqlQueryText).FirstOrDefault(); //returns 0 or more rows satisfying sql query

                        }

                      
                        var pathimg = detail.fsource;

                        DateTime time = DateTime.Now;

                        var footer = (from a in db.ActivitiesM where (a.ID_activity == detail.ID_visit) select a).FirstOrDefault();
                        
                        var customer = "";
                        var date = "";
                        var activi = "";
                        var store = "";
                        if (footer != null) {
                            var visit = (from a in db.VisitsM where (a.ID_visit == footer.ID_visit) select a).FirstOrDefault();
                            if (visit != null) {
                                store = visit.ID_store + "-" + visit.store + ", " + visit.city;
                            }

                            customer = footer.ID_customer + "-" + footer.Customer;
                            date = visit.visit_date.ToShortDateString();
                            activi = footer.ID_activity + "-" + footer.description;
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

                            int footerHeight = 35;
                            Bitmap bitmapImg = new Bitmap(imagenfinal);// Original Image
                            Bitmap bitmapComment = new Bitmap(imagenfinal.Width, footerHeight);// Footer
                            Bitmap bitmapNewImage = new Bitmap(imagenfinal.Width, imagenfinal.Height + footerHeight);//New Image
                            Graphics graphicImage = Graphics.FromImage(bitmapNewImage);
                            graphicImage.Clear(Color.White);
                            graphicImage.DrawImage(bitmapImg, new Point(0, 0));
                            graphicImage.DrawImage(bitmapComment, new Point(bitmapComment.Width, 0));
                            graphicImage.DrawString((activi + " | " + customer +" | " + store +" | " + date), new Font("Arial", 20), new SolidBrush(Color.Black), 0, bitmapImg.Height + footerHeight / 6);
                            
             

                            var path = Path.Combine(Server.MapPath("~/SharedContent/images/activities"), id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg");


                            var tam = file.ContentLength;

                            //if (tam < 600000)
                            //{
                            bitmapNewImage.Save(path, ImageFormat.Jpeg);
                            //imagenfinal.Save(path, ImageFormat.Jpeg);
                            //}
                            //else {
                            //    //Antes de guardar cambiamos el tamano de la imagen
                            //    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                            //    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            //    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            //    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 10L);
                            //    myEncoderParameters.Param[0] = myEncoderParameter;


                            //    imagenfinal.Save(path, jpgEncoder, myEncoderParameters);
                            //}
                            bitmapImg.Dispose();
                            bitmapComment.Dispose();
                            bitmapNewImage.Dispose();


                        }


                        //fname = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                        //file.SaveAs(fname);

                        //Luego guardamos la url en la db
                        //Forms_details detail = db.Forms_details.Find(Convert.ToInt32(id));  //se movio hacia arriba
                        detail.fsource = "~/SharedContent/images/activities/" + id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg";

                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        Session["detailsForm"] = detailsForm;

                        //if (System.IO.File.Exists(Server.MapPath(pathimg)))
                        //{
                        //    try
                        //    {
                        //        System.IO.File.Delete(Server.MapPath(pathimg));
                        //    }
                        //    catch (System.IO.IOException e)
                        //    {
                        //        Console.WriteLine(e.Message);

                        //    }
                        //}
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
        [HttpPost]
        public ActionResult UploadFilesDemo(string id, string idvisita, string orientation)
        {
            List<FormsM_detailsDemos> detailsForm = Session["detailsForm"] as List<FormsM_detailsDemos>;
            int idf = Convert.ToInt32(id);
            if (detailsForm != null)
            {

            }
            else
            {
                using (var dbs = new dbComerciaEntities())
                {

                    detailsForm = dbs.FormsM_detailsDemos.Where(a => a.ID_visit == idf).ToList();
                }

            }
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
                            int or = Convert.ToInt32(orientation);

                            switch (or)
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
                        catch
                        {

                        }
                        //buscamos el id del detalle
                       
                        FormsM_detailsDemos detail = new FormsM_detailsDemos();
                        try
                        {
                            int idvisit = Convert.ToInt32(idvisita);
                            detail = (from d in detailsForm where (d.idkey == idf && d.ID_visit == idvisit) select d).FirstOrDefault();
                        }
                        catch
                        {
                            var sqlQueryText = string.Format("SELECT * FROM FormsM_detailsDemos WHERE query2 LIKE '{0}' and idkey='" + idf + "'", idvisita);
                            detail = db.FormsM_detailsDemos.SqlQuery(sqlQueryText).FirstOrDefault(); //returns 0 or more rows satisfying sql query

                        }


                        var pathimg = detail.fsource;

                        DateTime time = DateTime.Now;

                        var footer = (from a in db.Demos where (a.ID_demo == detail.ID_visit) select a).FirstOrDefault();

                        var customer = "";
                        var date = "";
                        var activi = "";
                        var store = "";
                        var brand ="";
                        if (footer != null)
                        {

                                store = footer.ID_Store + "-" + footer.store + ", " + footer.city;
                            

                            customer = footer.ID_Vendor + "-" + footer.vendor;
                            date = Convert.ToDateTime(footer.visit_date).ToShortDateString();
                            activi = footer.ID_demo + "-" + "DEMO ACTIVITY";
                            brand = footer.Brands;
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

                            int footerHeight = 35;
                            Bitmap bitmapImg = new Bitmap(imagenfinal);// Original Image
                            Bitmap bitmapComment = new Bitmap(imagenfinal.Width, footerHeight);// Footer
                            Bitmap bitmapNewImage = new Bitmap(imagenfinal.Width, imagenfinal.Height + footerHeight);//New Image
                            Graphics graphicImage = Graphics.FromImage(bitmapNewImage);
                            graphicImage.Clear(Color.White);
                            graphicImage.DrawImage(bitmapImg, new Point(0, 0));
                            graphicImage.DrawImage(bitmapComment, new Point(bitmapComment.Width, 0));
                            graphicImage.DrawString((activi + " | " + customer + " | " + brand + " | " + store + " | " + date), new Font("Arial", 20), new SolidBrush(Color.Black), 0, bitmapImg.Height + footerHeight / 6);



                            var path = Path.Combine(Server.MapPath("~/SharedContent/images/activities"), id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg");


                            var tam = file.ContentLength;

                            //if (tam < 600000)
                            //{
                            bitmapNewImage.Save(path, ImageFormat.Jpeg);
                            //imagenfinal.Save(path, ImageFormat.Jpeg);
                            //}
                            //else {
                            //    //Antes de guardar cambiamos el tamano de la imagen
                            //    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                            //    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            //    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            //    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 10L);
                            //    myEncoderParameters.Param[0] = myEncoderParameter;


                            //    imagenfinal.Save(path, jpgEncoder, myEncoderParameters);
                            //}
                            bitmapImg.Dispose();
                            bitmapComment.Dispose();
                            bitmapNewImage.Dispose();


                        }


                        //fname = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                        //file.SaveAs(fname);

                        //Luego guardamos la url en la db
                        //Forms_details detail = db.Forms_details.Find(Convert.ToInt32(id));  //se movio hacia arriba
                        detail.fsource = "~/SharedContent/images/activities/" + id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg";

                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        Session["detailsForm"] = detailsForm;

                        //if (System.IO.File.Exists(Server.MapPath(pathimg)))
                        //{
                        //    try
                        //    {
                        //        System.IO.File.Delete(Server.MapPath(pathimg));
                        //    }
                        //    catch (System.IO.IOException e)
                        //    {
                        //        Console.WriteLine(e.Message);

                        //    }
                        //}
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
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        //CREACION DE JERARQUIAS Y OBJETOS
        //FORMULARIOS Y DETALLES DE FORMULARIOS (Se utiliza en Activities)
        public class tablahijospadre
        {
            public int ID_details { get; set; }
            public int id_resource { get; set; }
            public string fsource { get; set; }
            public string fdescription { get; set; }
            public int fvalue { get; set; }
            public decimal fvalueDecimal { get; set; }
            public string fvalueText { get; set; }
            public int ID_formM { get; set; }
            public int ID_visit { get; set; }
            public bool original { get; set; }
            public int obj_order { get; set; }
            public int obj_group { get; set; }
            public int idkey { get; set; }
            public int parent { get; set; }
            public string query1 { get; set; }
            public string query2 { get; set; }
            public int ID_empresa { get; set; }
        
        }

        public class MyObj_tablapadre
        {
            public int ID_details { get; set; }
            public int id_resource { get; set; }
            public string fsource { get; set; }
            public string fdescription { get; set; }
            public int fvalue { get; set; }
            public decimal fvalueDecimal { get; set; }
            public string fvalueText { get; set; }
            public int ID_formM { get; set; }
            public int ID_visit { get; set; }
            public bool original { get; set; }
            public int obj_order { get; set; }
            public int obj_group { get; set; }
            public int idkey { get; set; }
            public int parent { get; set; }
            public string query1 { get; set; }
            public string query2 { get; set; }
            public int ID_empresa { get; set; }
            public List<MyObj_tablapadre> children { get; set; }
        }

        public static List<MyObj_tablapadre> ObtenerCategoriarJerarquiaByID(List<MyObj_tablapadre> Categoriaspadre, List<tablahijospadre> Categoriashijas)
        {


            List<MyObj_tablapadre> query = (from item in Categoriaspadre

                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                children = ObtenerHijosByID(item.idkey, Categoriashijas)
                                                
                                            }).ToList();

            return query;





        }

        private static List<MyObj_tablapadre> ObtenerHijosByID(int ID_parent, List<tablahijospadre> categoriashijas)
        {



            List<MyObj_tablapadre> query = (from item in categoriashijas

                                            where item.parent == ID_parent
                                            select new MyObj_tablapadre
                                            {
                                                ID_details = item.ID_details,
                                                id_resource = item.id_resource,
                                                fsource = item.fsource,
                                                fdescription = item.fdescription,
                                                fvalue = item.fvalue,
                                                fvalueDecimal = item.fvalueDecimal,
                                                fvalueText = item.fvalueText,
                                                ID_formM = item.ID_formM,
                                                ID_visit = item.ID_visit,
                                                original = item.original,
                                                obj_order = item.obj_order,
                                                obj_group = item.obj_group,
                                                idkey = item.idkey,
                                                parent = item.parent,
                                                query1 = item.query1,
                                                query2 = item.query2,
                                                ID_empresa = item.ID_empresa,
                                                children = ObtenerHijosByID(item.idkey, categoriashijas)
                                            }).ToList();

            return query;

        }
        public JsonResult Finish_activity(string id, string lat, string lng, string check_out)
        {
            try
            {
                //int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);
                    ActivitiesM activity = db.ActivitiesM.Find(act);

                    //if (lat != null || lat != "")
                    //{
                    //    //Guardamos el log de la actividad
                    //    ActivitiesM_log nuevoLog = new ActivitiesM_log();
                    //    nuevoLog.latitude = lat;
                    //    nuevoLog.longitude = lng;
                    //    nuevoLog.ID_usuario = IDU;
                    //    nuevoLog.ID_activity = Convert.ToInt32(id);
                    //    nuevoLog.fecha_conexion = Convert.ToDateTime(check_out);
                    //    nuevoLog.query1 = "";
                    //    nuevoLog.query2 = "";
                    //    nuevoLog.action = "FINISH ACTIVITY - " + activity.description;
                    //    nuevoLog.ip = "";
                    //    nuevoLog.hostname = "";
                    //    nuevoLog.typeh = "";
                    //    nuevoLog.continent_name = "";
                    //    nuevoLog.country_code = "";
                    //    nuevoLog.country_name = "";
                    //    nuevoLog.region_code = "";
                    //    nuevoLog.region_name = "";
                    //    nuevoLog.city = "";

                    //    db.ActivitiesM_log.Add(nuevoLog);
                    //    db.SaveChanges();
                    //}

                    activity.check_out = Convert.ToDateTime(check_out);
                    activity.isfinished = true;
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
                    //Enviamos correos

                    if (activity.ID_activitytype == 4)
                    {
                        try
                        {
                            var demo_header = activity;


                            var id_visit = demo_header.ID_visit;
                            var visit = (from b in db.VisitsM where (b.ID_visit == id_visit) select b).FirstOrDefault();

                            var usuario = (from u in COM_MKdb.OCRD where (u.CardCode == activity.ID_usuarioEndString) select u).FirstOrDefault();
                            if (usuario == null)
                            {

                            }
                            else
                            {
                                activity.ID_usuarioEndString = usuario.CardName;
                            }

                            activity.Customer = visit.store + ",  " + visit.address;



                            //Existen datos
                            //Buscamos los detalles
                            //3 - Products | 4- Products samples | 6 - Input_text | 10- GIFT
                            var detallesMaestro = (from b in db.FormsM_details where (b.ID_visit == activity.ID_activity) select b).OrderBy(b => b.ID_formresourcetype).ToList();


                            var demo_details = (from b in detallesMaestro where (b.ID_visit == activity.ID_activity && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6 || b.ID_formresourcetype == 10)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                            var result = demo_details
                                    .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                                    .Select(cl => new FormsM_details
                                    {
                                        ID_details = cl.First().ID_details,
                                        ID_formresourcetype = cl.First().ID_formresourcetype,
                                        fsource = cl.First().fsource,
                                        fdescription = cl.First().fdescription,
                                        fvalue = cl.Sum(c => c.fvalue),
                                        ID_formM = cl.First().ID_formM,
                                        ID_visit = cl.First().ID_visit,
                                        original = cl.First().original,
                                        obj_order = cl.First().obj_order,
                                        obj_group = cl.First().obj_group
                                    }).ToList();


                            ReportDocument rd = new ReportDocument();

                            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));

                            //Obtenemos el nombre de las marcas o brands por cada articulo
                            var listadeItems = (from d in detallesMaestro where (d.ID_visit == activity.ID_activity && d.ID_formresourcetype == 3) select d).ToList();

                            var oitm = (from h in COM_MKdb.OITM select h).ToList();
                            var omrc = (from i in COM_MKdb.OMRC select i).ToList();
                            foreach (var itemd in listadeItems)
                            {

                                itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                                if (itemd.fdescription == null)
                                {
                                    itemd.fdescription = "No data found";
                                }
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

                            demo_header.query1 = brandstoshow;

                            List<ActivitiesM> lst = new List<ActivitiesM>();
                            lst.Add(demo_header);
                            rd.SetDataSource(lst);

                            rd.Subreports[0].SetDataSource(result);

                            //Verificamos si existen fotos en el demo (MAX 4 fotos)
                            var fotos = (from c in detallesMaestro where (c.ID_visit == activity.ID_activity && c.ID_formresourcetype == 5) select c).ToList();

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
                            var firma = (from d in detallesMaestro where (d.ID_visit == activity.ID_activity && d.ID_formresourcetype == 9) select d).ToList();

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
                            Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Report; ");



                            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                            stream.Seek(0, SeekOrigin.Begin);


                            //PARA PREVISULIZACION
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
                            var filename = "DEMO REPORT" + "" + ".pdf";
                            path2 = Path.Combine(filePathOriginal, filename);
                            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);


                            //Para enviar correos



                            var emp = demo_header.ID_customer;
                            var id_empresa = (from j in db.Empresas where (j.ID_SAP == emp) select j.ID_empresa).FirstOrDefault();
                            var contactos = (from u in db.Usuarios where (u.ID_tipomembresia == 7 && u.ID_empresa == id_empresa && u.activo == true) select u).ToList();
                            if (contactos.Count > 0)
                            {
                                foreach (var item in contactos)
                                {
                                    if (item.correo != null)
                                    {
                                        dynamic email = new Email("DemoResume");
                                        email.To = item.correo;
                                        email.From = "donotreply@comerciamarketing.com";
                                        email.Subject = "DEMO REPORT FOR " + visit.store + "- " + visit.visit_date.ToShortDateString();
                                        email.Attach(new Attachment(path2));
                                        //email.Body = imagename;
                                        //return new EmailViewResult(email);


                                        email.Send();

                                    }

                                }

                            }
                        }
                        catch { }
                    }

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        public JsonResult Finish_activityDemo(string id, string lat, string lng, string check_out)
        {
            try
            {
                //int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);
                    Demos activity = db.Demos.Find(act);

                    //if (lat != null || lat != "")
                    //{
                    //    //Guardamos el log de la actividad
                    //    ActivitiesM_log nuevoLog = new ActivitiesM_log();
                    //    nuevoLog.latitude = lat;
                    //    nuevoLog.longitude = lng;
                    //    nuevoLog.ID_usuario = IDU;
                    //    nuevoLog.ID_activity = Convert.ToInt32(id);
                    //    nuevoLog.fecha_conexion = Convert.ToDateTime(check_out);
                    //    nuevoLog.query1 = "";
                    //    nuevoLog.query2 = "";
                    //    nuevoLog.action = "FINISH ACTIVITY DEMO";
                    //    nuevoLog.ip = "";
                    //    nuevoLog.hostname = "";
                    //    nuevoLog.typeh = "";
                    //    nuevoLog.continent_name = "";
                    //    nuevoLog.country_code = "";
                    //    nuevoLog.country_name = "";
                    //    nuevoLog.region_code = "";
                    //    nuevoLog.region_name = "";
                    //    nuevoLog.city = "";

                    //    db.ActivitiesM_log.Add(nuevoLog);
                    //    db.SaveChanges();
                    //}

                    activity.end_date = Convert.ToDateTime(check_out);                  
                    activity.ID_demostate = 4;
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
                    //Enviamos correos

                        //try
                        //{
                        //    var demo_header = activity;


                        //    var id_visit = demo_header.ID_demo;
                        //    var visit = (from b in db.VisitsM where (b.ID_visit == id_visit) select b).FirstOrDefault();
   
                        //        var usuario = (from u in COM_MKdb.OCRD where (u.CardCode == activity.ID_ExternalUser) select u).FirstOrDefault();
                        //        if (usuario == null)
                        //        {

                        //        }
                        //        else
                        //        {
                        //            activity.ID_ExternalUser = usuario.CardName;
                        //        }

                        //        activity.store = visit.store + ",  " + visit.address;

                            

                        //    //Existen datos
                        //    //Buscamos los detalles
                        //    //3 - Products | 4- Products samples | 6 - Input_text | 10- GIFT
                        //    var detallesMaestro = (from b in db.FormsM_details where (b.ID_visit == activity.ID_activity) select b).OrderBy(b => b.ID_formresourcetype).ToList();


                        //    var demo_details = (from b in detallesMaestro where (b.ID_visit == activity.ID_activity && (b.ID_formresourcetype == 3 || b.ID_formresourcetype == 4 || b.ID_formresourcetype == 6 || b.ID_formresourcetype == 10)) select b).OrderBy(b => b.ID_formresourcetype).ToList();
                        //    var result = demo_details
                        //            .GroupBy(l => new { ID_formresourcetype = l.ID_formresourcetype, fsource = l.fsource })
                        //            .Select(cl => new FormsM_details
                        //            {
                        //                ID_details = cl.First().ID_details,
                        //                ID_formresourcetype = cl.First().ID_formresourcetype,
                        //                fsource = cl.First().fsource,
                        //                fdescription = cl.First().fdescription,
                        //                fvalue = cl.Sum(c => c.fvalue),
                        //                ID_formM = cl.First().ID_formM,
                        //                ID_visit = cl.First().ID_visit,
                        //                original = cl.First().original,
                        //                obj_order = cl.First().obj_order,
                        //                obj_group = cl.First().obj_group
                        //            }).ToList();


                        //    ReportDocument rd = new ReportDocument();

                        //    rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptDemo.rpt"));

                        //    //Obtenemos el nombre de las marcas o brands por cada articulo
                        //    var listadeItems = (from d in detallesMaestro where (d.ID_visit == activity.ID_activity && d.ID_formresourcetype == 3) select d).ToList();

                        //    var oitm = (from h in COM_MKdb.OITM select h).ToList();
                        //    var omrc = (from i in COM_MKdb.OMRC select i).ToList();
                        //    foreach (var itemd in listadeItems)
                        //    {

                        //        itemd.fdescription = (from k in oitm join j in omrc on k.FirmCode equals j.FirmCode where (k.ItemCode == itemd.fsource) select j.FirmName).FirstOrDefault();
                        //        if (itemd.fdescription == null)
                        //        {
                        //            itemd.fdescription = "No data found";
                        //        }
                        //    }

                        //    var brands = listadeItems.GroupBy(test => test.fdescription).Select(grp => grp.First()).ToList();

                        //    var brandstoshow = "";
                        //    int count = 0;
                        //    foreach (var items in brands)
                        //    {
                        //        if (count == 0)
                        //        {
                        //            brandstoshow = items.fdescription.ToString();
                        //        }
                        //        else
                        //        {
                        //            brandstoshow += ", " + items.fdescription.ToString();
                        //        }
                        //        count += 1;
                        //    }
                        //    //*******************************

                        //    demo_header.query1 = brandstoshow;

                        //    List<ActivitiesM> lst = new List<ActivitiesM>();
                        //    lst.Add(demo_header);
                        //    rd.SetDataSource(lst);

                        //    rd.Subreports[0].SetDataSource(result);

                        //    //Verificamos si existen fotos en el demo (MAX 4 fotos)
                        //    var fotos = (from c in detallesMaestro where (c.ID_visit == activity.ID_activity && c.ID_formresourcetype == 5) select c).ToList();

                        //    int fotosC = fotos.Count();




                        //    if (fotosC == 4)
                        //    {
                        //        if (fotos[0].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg1", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                        //        }
                        //        if (fotos[1].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg2", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                        //        }
                        //        if (fotos[2].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg3", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                        //        }
                        //        if (fotos[3].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg4", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg4", Path.GetFullPath(Server.MapPath(fotos[3].fsource)));
                        //        }


                        //    }
                        //    else if (fotosC == 3)
                        //    {
                        //        if (fotos[0].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg1", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                        //        }
                        //        if (fotos[1].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg2", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                        //        }
                        //        if (fotos[2].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg3", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg3", Path.GetFullPath(Server.MapPath(fotos[2].fsource)));
                        //        }

                        //        rd.SetParameterValue("urlimg4", "");

                        //    }
                        //    else if (fotosC == 2)
                        //    {
                        //        if (fotos[0].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg1", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                        //        }
                        //        if (fotos[1].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg2", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg2", Path.GetFullPath(Server.MapPath(fotos[1].fsource)));
                        //        }

                        //        rd.SetParameterValue("urlimg3", "");

                        //        rd.SetParameterValue("urlimg4", "");

                        //    }
                        //    else if (fotosC == 1)
                        //    {
                        //        if (fotos[0].fsource == "")
                        //        {
                        //            rd.SetParameterValue("urlimg1", "");
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimg1", Path.GetFullPath(Server.MapPath(fotos[0].fsource)));
                        //        }

                        //        rd.SetParameterValue("urlimg2", "");

                        //        rd.SetParameterValue("urlimg3", "");

                        //        rd.SetParameterValue("urlimg4", "");

                        //    }
                        //    else
                        //    {

                        //        rd.SetParameterValue("urlimg1", "");
                        //        rd.SetParameterValue("urlimg2", "");
                        //        rd.SetParameterValue("urlimg3", "");
                        //        rd.SetParameterValue("urlimg4", "");
                        //    }


                        //    //Verificams si existe firma electronica
                        //    var firma = (from d in detallesMaestro where (d.ID_visit == activity.ID_activity && d.ID_formresourcetype == 9) select d).ToList();

                        //    int firmaC = firma.Count();




                        //    if (firmaC == 1)
                        //    {

                        //        string data = firma[0].fsource;
                        //        if (data != "")
                        //        {
                        //            var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                        //            var binData = Convert.FromBase64String(base64Data);

                        //            using (var streamf = new MemoryStream(binData))
                        //            {

                        //                Bitmap myImage = new Bitmap(streamf);

                        //                // Assumes myImage is the PNG you are converting
                        //                using (var b = new Bitmap(myImage.Width, myImage.Height))
                        //                {
                        //                    b.SetResolution(myImage.HorizontalResolution, myImage.VerticalResolution);

                        //                    using (var g = Graphics.FromImage(b))
                        //                    {
                        //                        g.Clear(Color.White);
                        //                        g.DrawImageUnscaled(myImage, 0, 0);
                        //                    }

                        //                    // Now save b as a JPEG like you normally would

                        //                    var path = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), "signdemod.jpg");
                        //                    b.Save(path, ImageFormat.Jpeg);


                        //                    rd.SetParameterValue("urlimgsign", Path.GetFullPath(path));
                        //                }



                        //            }
                        //        }
                        //        else
                        //        {
                        //            rd.SetParameterValue("urlimgsign", "");

                        //        }

                        //    }
                        //    else
                        //    {
                        //        rd.SetParameterValue("urlimgsign", "");
                        //    }


                        //    var filePathOriginal = Server.MapPath("/Reportes/pdf");

                        //    Response.Buffer = false;

                        //    Response.ClearContent();

                        //    Response.ClearHeaders();


                        //    //PARA VISUALIZAR
                        //    Response.AppendHeader("Content-Disposition", "inline; filename=" + "Demo Report; ");



                        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                        //    stream.Seek(0, SeekOrigin.Begin);


                        //    //PARA PREVISULIZACION
                        //    //return File(stream, System.Net.Mime.MediaTypeNames.Application.Pdf);

                        //    //PARA ENVIAR POR CORREO

                        //    try

                        //    {
                        //        //limpiamos el directorio

                        //        System.IO.DirectoryInfo di = new DirectoryInfo(filePathOriginal);

                        //        foreach (FileInfo file in di.GetFiles())

                        //        {

                        //            file.Delete();

                        //        }

                        //        foreach (DirectoryInfo dir in di.GetDirectories())

                        //        {

                        //            dir.Delete(true);

                        //        }

                        //    }

                        //    catch (Exception e)

                        //    {

                        //        var mensaje = e.ToString();

                        //    }

                        //    var path2 = "";
                        //    var filename = "DEMO REPORT" + "" + ".pdf";
                        //    path2 = Path.Combine(filePathOriginal, filename);
                        //    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path2);


                        //    //Para enviar correos



                        //    var emp = demo_header.ID_customer;
                        //    var id_empresa = (from j in db.Empresas where (j.ID_SAP == emp) select j.ID_empresa).FirstOrDefault();
                        //    var contactos = (from u in db.Usuarios where (u.ID_tipomembresia == 7 && u.ID_empresa == id_empresa && u.activo == true) select u).ToList();
                        //    if (contactos.Count > 0)
                        //    {
                        //        foreach (var item in contactos)
                        //        {
                        //            if (item.correo != null)
                        //            {
                        //                dynamic email = new Email("DemoResume");
                        //                email.To = item.correo;
                        //                email.From = "donotreply@comerciamarketing.com";
                        //                email.Subject = "DEMO REPORT FOR " + visit.store + "- " + visit.visit_date.ToShortDateString();
                        //                email.Attach(new Attachment(path2));
                        //                //email.Body = imagename;
                        //                //return new EmailViewResult(email);


                        //                email.Send();

                        //            }

                        //        }

                        //    }
                        //}
                        //catch { }
                    

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }

        public ActionResult Check_in(string ID_activity, string check_in)
        {
            try
            {
                int id = Convert.ToInt32(ID_activity);
                Demos actividad = db.Demos.Find(Convert.ToInt32(id));
                if (actividad != null)
                {
                    actividad.ID_demostate = 2;
                    actividad.check_in = Convert.ToDateTime(check_in);
                    db.Entry(actividad).State = EntityState.Modified;
                    db.SaveChanges();


                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = "";
                        nuevoLog.longitude = "";
                        nuevoLog.ID_usuario = 0;
                        nuevoLog.ID_activity = 0;
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_in);
                        nuevoLog.query1 = actividad.ID_demo.ToString();
                        nuevoLog.query2 = actividad.ID_ExternalUser;
                        nuevoLog.action = "DEMO CHECK IN  - " + actividad.ID_Vendor + " - " + actividad.vendor + " - " + actividad.Brands;
                        nuevoLog.ip = "";
                        nuevoLog.hostname = "";
                        nuevoLog.typeh = "";
                        nuevoLog.continent_name = "";
                        nuevoLog.country_code = "";
                        nuevoLog.country_name = "";
                        nuevoLog.region_code = "";
                        nuevoLog.region_name = "";
                        nuevoLog.city = "";

                        db.ActivitiesM_log.Add(nuevoLog);
                        db.SaveChanges();
                    

                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Fail" });
            }
            catch
            {
                return Json(new { Result = "Error" });
            }



        }
        public ActionResult Activitysoon(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

            int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {

                    FormsM formsM = db.FormsM.Find(activity.ID_form);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {
                        if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                        {
                            var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customers = customers.ToList();
                        }
                        else
                        {

                            var customers = (from b in COM_MKdb.OCRD where (datosUsuario.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customers = customers.ToList();
                        }
    
                    }
                    var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();

                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                    where (item.parent == 0)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).OrderBy(a=> a.obj_order).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                         
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).OrderBy(a => a.obj_order).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///
                    var showbuttondynamic = (from item in FormsMDet
                                             where (item.ID_formresourcetype == 3)
                                             select item).Count();

                    if (showbuttondynamic > 0)
                    {

                        ViewBag.mostrarboton = 1;
                    }
                    else {
                        ViewBag.mostrarboton = 0;
                    }
                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;

                    ViewBag.details = categoriasListActivities;

                    ViewBag.detailssql = FormsMDet;

                    Session["detailsForm"] = FormsMDet;
                    VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                    ViewBag.storename = visitsM.store;
                    ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                    return View();
                }

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                //return View(FormsM_details.ToList());

        }
        public ActionResult ActivityonDF(int? id)
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    //return RedirectToAction("Main", "Home");
                }
                else
                {

                    
                   
                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = 0; //activity.ID_activity;

                    VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                    ViewBag.storename = visitsM.store;
                    ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                    
                }
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Activityon(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }

            int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {

                    FormsM formsM = db.FormsM.Find(activity.ID_form);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {
                        if ((datosUsuario.ID_tipomembresia == 8 && datosUsuario.ID_rol == 8) || datosUsuario.ID_tipomembresia == 1)//Administrador
                        {
                            var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customers = customers.ToList();
                        }
                        else
                        {

                            var customers = (from b in COM_MKdb.OCRD where (datosUsuario.estados_influencia.Contains(b.CardCode)) select b).OrderBy(b => b.CardName).ToList();
                            ViewBag.customers = customers.ToList();
                        }

                    }
                    
                    var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();


                    //NUEVO
                    //ID VISIT SE UTILIZA COMO RELACION
                    List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                    where (item.parent == 0)
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).OrderBy(a => a.obj_order).ToList();


                    List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                                 select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).OrderBy(a => a.obj_order).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                    ///
                    var showbuttondynamic = (from item in FormsMDet
                                             where (item.ID_formresourcetype == 3)
                                             select item).Count();

                    if (showbuttondynamic > 0)
                    {

                        ViewBag.mostrarboton = 1;
                    }
                    else
                    {
                        ViewBag.mostrarboton = 0;
                    }
                    //Deserealizamos  los datos
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;

                    ViewBag.details = categoriasListActivities;

                    ViewBag.detailssql = FormsMDet;

                    Session["detailsForm"] = FormsMDet;
                    VisitsM visitsM = db.VisitsM.Where(a=> a.ID_visit ==activity.ID_visit).FirstOrDefault();
                    ViewBag.storename = visitsM.store;
                    ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                    return View();
                }



        }

        public ActionResult Activityraon(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }
            int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

                var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

                if (activity == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {

                    FormsM formsM = db.FormsM.Find(activity.ID_form);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }

                //Cargamos las marcas
                //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                //    .Distinct()
                //    .OrderByDescending(i => i.FirmName)
                //    .ToList();

                //ViewBag.brands = brandlist;

                //Cargamos las lineas de procuctos
                //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                //.Where(i => i.Id_subcategory != null)
                //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                //.Distinct()
                //.OrderByDescending(i => i.SubCategory)
                //.ToList();

                //ViewBag.productline = productlinelist;

                //NUEVO
                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();

                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                    select
                                                                       new MyObj_tablapadre
                                                                       {
                                                                           ID_details = item.ID_details,
                                                                           id_resource = item.ID_formresourcetype,
                                                                           fsource = item.fsource,
                                                                           fdescription = item.fdescription,
                                                                           fvalue = item.fvalue,
                                                                           fvalueDecimal = item.fvalueDecimal,
                                                                           fvalueText = item.fvalueText,
                                                                           ID_formM = item.ID_formM,
                                                                           ID_visit = item.ID_visit,
                                                                           original = item.original,
                                                                           obj_order = item.obj_order,
                                                                           obj_group = item.obj_group,
                                                                           idkey = item.idkey,
                                                                           parent = item.parent,
                                                                           query1 = item.query1,
                                                                           query2 = item.query2,
                                                                           ID_empresa = item.ID_empresa
                                                                       }
                                          ).ToList();


                    //foreach (var t in listapadresActivities) {
                    //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                    //    if (s > 0)
                    //    {

                    //    }
                    //    else {
                    //        listapadresActivities.Remove(t);
                    //    }

                    //}


                    List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                                  where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype !=3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                  select new tablahijospadre
                                                                  {
                                                                      ID_details = item.ID_details,
                                                                      id_resource = item.ID_formresourcetype,
                                                                      fsource = item.fsource,
                                                                      fdescription = item.fdescription,
                                                                      fvalue = item.fvalue,
                                                                      fvalueDecimal = item.fvalueDecimal,
                                                                      fvalueText = item.fvalueText,
                                                                      ID_formM = item.ID_formM,
                                                                      ID_visit = item.ID_visit,
                                                                      original = item.original,
                                                                      obj_order = item.obj_order,
                                                                      obj_group = item.obj_group,
                                                                      idkey = item.idkey,
                                                                      parent = item.parent,
                                                                      query1 = item.query1,
                                                                      query2 = item.query2,
                                                                      ID_empresa = item.ID_empresa

                                                                  }).ToList();


                    List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                //PRODUCTOS FILAS

                List<MyObj_tablapadre> listapadresActivitiesProducts = (from item in FormsMDet
                                                                        where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype == 3 || item.ID_formresourcetype == 16 || item.ID_formresourcetype == 21))
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                      ).ToList();


                //foreach (var t in listapadresActivities) {
                //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                //    if (s > 0)
                //    {

                //    }
                //    else {
                //        listapadresActivities.Remove(t);
                //    }

                //}


                List<tablahijospadre> listahijasActivitiesProducts = (from item in FormsMDet
                                                                      where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).ToList();


                List<MyObj_tablapadre> categoriasListActivitiesProducts = ObtenerCategoriarJerarquiaByID(listapadresActivitiesProducts, listahijasActivitiesProducts);

                ViewBag.productRows = categoriasListActivitiesProducts;

                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 11)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {
                    ViewBag.dinamicos = 1;
                    var existproducts = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 3)
                                         select item).Count();
                    if (existproducts > 0)
                    {
                        ViewBag.mostrarboton = 0; //Lo ocultamos
                    }
                    else
                    {
                        ViewBag.mostrarboton = 1; //Lo mostramos
                    }

                }
                else
                {
                    ViewBag.mostrarboton = 0;
                    ViewBag.dinamicos = 0;
                }

                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                    MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                    ViewBag.idvisitareal = activity.ID_visit;
                    ViewBag.idvisita = activity.ID_activity;

                    ViewBag.details = categoriasListActivities.OrderBy(c => c.obj_order).ToList();


                    Session["detailsForm"] = (from f in FormsMDet where (f.ID_visit == id) select f).ToList();

                    VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                    ViewBag.storename = visitsM.store;
                    ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                    return View();
                }

                //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

                //return View(FormsM_details.ToList());

        }
        public ActionResult Activityraonresume(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }
            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

            var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

            if (activity == null)
            {
                return RedirectToAction("Main", "Home");
            }
            else
            {

                FormsM formsM = db.FormsM.Find(activity.ID_form);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                if (activity.Customer != "")
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }
                else
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }

                //Cargamos las marcas
                //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                //    .Distinct()
                //    .OrderByDescending(i => i.FirmName)
                //    .ToList();

                //ViewBag.brands = brandlist;

                //Cargamos las lineas de procuctos
                //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                //.Where(i => i.Id_subcategory != null)
                //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                //.Distinct()
                //.OrderByDescending(i => i.SubCategory)
                //.ToList();

                //ViewBag.productline = productlinelist;

                //NUEVO
                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();

                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                                          ).ToList();


                //foreach (var t in listapadresActivities) {
                //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                //    if (s > 0)
                //    {

                //    }
                //    else {
                //        listapadresActivities.Remove(t);
                //    }

                //}


                List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                              where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                //PRODUCTOS FILAS

                List<MyObj_tablapadre> listapadresActivitiesProducts = (from item in FormsMDet
                                                                        where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype == 3 || item.ID_formresourcetype == 16 || item.ID_formresourcetype == 21))
                                                                        select
                                                                           new MyObj_tablapadre
                                                                           {
                                                                               ID_details = item.ID_details,
                                                                               id_resource = item.ID_formresourcetype,
                                                                               fsource = item.fsource,
                                                                               fdescription = item.fdescription,
                                                                               fvalue = item.fvalue,
                                                                               fvalueDecimal = item.fvalueDecimal,
                                                                               fvalueText = item.fvalueText,
                                                                               ID_formM = item.ID_formM,
                                                                               ID_visit = item.ID_visit,
                                                                               original = item.original,
                                                                               obj_order = item.obj_order,
                                                                               obj_group = item.obj_group,
                                                                               idkey = item.idkey,
                                                                               parent = item.parent,
                                                                               query1 = item.query1,
                                                                               query2 = item.query2,
                                                                               ID_empresa = item.ID_empresa
                                                                           }
                      ).ToList();


                //foreach (var t in listapadresActivities) {
                //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                //    if (s > 0)
                //    {

                //    }
                //    else {
                //        listapadresActivities.Remove(t);
                //    }

                //}


                List<tablahijospadre> listahijasActivitiesProducts = (from item in FormsMDet
                                                                      where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                      select new tablahijospadre
                                                                      {
                                                                          ID_details = item.ID_details,
                                                                          id_resource = item.ID_formresourcetype,
                                                                          fsource = item.fsource,
                                                                          fdescription = item.fdescription,
                                                                          fvalue = item.fvalue,
                                                                          fvalueDecimal = item.fvalueDecimal,
                                                                          fvalueText = item.fvalueText,
                                                                          ID_formM = item.ID_formM,
                                                                          ID_visit = item.ID_visit,
                                                                          original = item.original,
                                                                          obj_order = item.obj_order,
                                                                          obj_group = item.obj_group,
                                                                          idkey = item.idkey,
                                                                          parent = item.parent,
                                                                          query1 = item.query1,
                                                                          query2 = item.query2,
                                                                          ID_empresa = item.ID_empresa

                                                                      }).ToList();


                List<MyObj_tablapadre> categoriasListActivitiesProducts = ObtenerCategoriarJerarquiaByID(listapadresActivitiesProducts, listahijasActivitiesProducts);

                ViewBag.productRows = categoriasListActivitiesProducts;

                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 11)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {
                    ViewBag.dinamicos = 1;
                    var existproducts = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 3)
                                         select item).Count();
                    if (existproducts > 0)
                    {
                        ViewBag.mostrarboton = 0; //Lo ocultamos
                    }
                    else
                    {
                        ViewBag.mostrarboton = 1; //Lo mostramos
                    }

                }
                else
                {
                    ViewBag.mostrarboton = 0;
                    ViewBag.dinamicos = 0;
                }

                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.idvisitareal = activity.ID_visit;
                ViewBag.idvisita = activity.ID_activity;

                ViewBag.details = categoriasListActivities;


                Session["detailsForm"] = (from f in FormsMDet where (f.ID_visit == id) select f).ToList();

                VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                ViewBag.storename = visitsM.store;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                return View();
            }

            //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

            //return View(FormsM_details.ToList());

        }

        public ActionResult ActivityraonresumeR(int? id)
        {
            Usuarios activeuser = Session["activeUser"] as Usuarios;
            if (activeuser != null)
            {
            }
            else
            {
                return RedirectToAction("Index", "Home", new { access = false });

            }
            int ID = Convert.ToInt32(Session["IDusuario"]);
            var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

            ViewBag.usuario = datosUsuario.nombre + " " + datosUsuario.apellido;

            var activity = (from v in db.ActivitiesM where (v.ID_activity == id) select v).FirstOrDefault();

            if (activity == null)
            {
                return RedirectToAction("Main", "Home");
            }
            else
            {

                FormsM formsM = db.FormsM.Find(activity.ID_form);

                //LISTADO DE CLIENTES
                //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                if (activity.Customer != "")
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode == activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }
                else
                {
                    var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardName != null && b.CardName != "") select b).OrderBy(b => b.CardName).ToList();
                    ViewBag.customers = customers.ToList();
                }

                //Cargamos las marcas
                //List<brands> brandlist = COM_MKdb.view_CMKEditorB
                //    .Select(i => new brands{ Customer= i.U_CustomerCM, FirmCode= i.FirmCode.ToString(), FirmName= i.FirmName })
                //    .Distinct()
                //    .OrderByDescending(i => i.FirmName)
                //    .ToList();

                //ViewBag.brands = brandlist;

                //Cargamos las lineas de procuctos
                //List<productline> productlinelist = COM_MKdb.view_CMKEditorB
                //.Where(i => i.Id_subcategory != null)
                //.Select(i => new productline{  Brand =i.FirmCode.ToString(), Id_subcategory= i.Id_subcategory, SubCategory= i.SubCategory })
                //.Distinct()
                //.OrderByDescending(i => i.SubCategory)
                //.ToList();

                //ViewBag.productline = productlinelist;

                //NUEVO
                var FormsMDet = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();

                //ID VISIT SE UTILIZA COMO RELACION
                List<MyObj_tablapadre> listapadresActivities = (from item in FormsMDet
                                                                where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                select
                                                                   new MyObj_tablapadre
                                                                   {
                                                                       ID_details = item.ID_details,
                                                                       id_resource = item.ID_formresourcetype,
                                                                       fsource = item.fsource,
                                                                       fdescription = item.fdescription,
                                                                       fvalue = item.fvalue,
                                                                       fvalueDecimal = item.fvalueDecimal,
                                                                       fvalueText = item.fvalueText,
                                                                       ID_formM = item.ID_formM,
                                                                       ID_visit = item.ID_visit,
                                                                       original = item.original,
                                                                       obj_order = item.obj_order,
                                                                       obj_group = item.obj_group,
                                                                       idkey = item.idkey,
                                                                       parent = item.parent,
                                                                       query1 = item.query1,
                                                                       query2 = item.query2,
                                                                       ID_empresa = item.ID_empresa
                                                                   }
                                          ).ToList();


                //foreach (var t in listapadresActivities) {
                //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                //    if (s > 0)
                //    {

                //    }
                //    else {
                //        listapadresActivities.Remove(t);
                //    }

                //}


                List<tablahijospadre> listahijasActivities = (from item in FormsMDet
                                                              where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                              select new tablahijospadre
                                                              {
                                                                  ID_details = item.ID_details,
                                                                  id_resource = item.ID_formresourcetype,
                                                                  fsource = item.fsource,
                                                                  fdescription = item.fdescription,
                                                                  fvalue = item.fvalue,
                                                                  fvalueDecimal = item.fvalueDecimal,
                                                                  fvalueText = item.fvalueText,
                                                                  ID_formM = item.ID_formM,
                                                                  ID_visit = item.ID_visit,
                                                                  original = item.original,
                                                                  obj_order = item.obj_order,
                                                                  obj_group = item.obj_group,
                                                                  idkey = item.idkey,
                                                                  parent = item.parent,
                                                                  query1 = item.query1,
                                                                  query2 = item.query2,
                                                                  ID_empresa = item.ID_empresa

                                                              }).ToList();


                List<MyObj_tablapadre> categoriasListActivities = ObtenerCategoriarJerarquiaByID(listapadresActivities, listahijasActivities);

                ///
                //PRODUCTOS FILAS

                List<MyObj_tablapadre> listapadresActivitiesProducts = (from item in FormsMDet
                                                                        where (item.parent == 0 && item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype == 3 || item.ID_formresourcetype == 16 || item.ID_formresourcetype == 21))
                                                                        select
                                                                           new MyObj_tablapadre
                                                                           {
                                                                               ID_details = item.ID_details,
                                                                               id_resource = item.ID_formresourcetype,
                                                                               fsource = item.fsource,
                                                                               fdescription = item.fdescription,
                                                                               fvalue = item.fvalue,
                                                                               fvalueDecimal = item.fvalueDecimal,
                                                                               fvalueText = item.fvalueText,
                                                                               ID_formM = item.ID_formM,
                                                                               ID_visit = item.ID_visit,
                                                                               original = item.original,
                                                                               obj_order = item.obj_order,
                                                                               obj_group = item.obj_group,
                                                                               idkey = item.idkey,
                                                                               parent = item.parent,
                                                                               query1 = item.query1,
                                                                               query2 = item.query2,
                                                                               ID_empresa = item.ID_empresa
                                                                           }
                      ).ToList();


                //foreach (var t in listapadresActivities) {
                //    var s = (from e in db.FormsM_details where (e.parent == t.idkey) select e).Count();
                //    if (s > 0)
                //    {

                //    }
                //    else {
                //        listapadresActivities.Remove(t);
                //    }

                //}


                List<tablahijospadre> listahijasActivitiesProducts = (from item in FormsMDet
                                                                      where (item.ID_visit == activity.ID_activity && item.original == false && (item.ID_formresourcetype != 3 || item.ID_formresourcetype != 16 || item.ID_formresourcetype != 21))
                                                                      select new tablahijospadre
                                                                      {
                                                                          ID_details = item.ID_details,
                                                                          id_resource = item.ID_formresourcetype,
                                                                          fsource = item.fsource,
                                                                          fdescription = item.fdescription,
                                                                          fvalue = item.fvalue,
                                                                          fvalueDecimal = item.fvalueDecimal,
                                                                          fvalueText = item.fvalueText,
                                                                          ID_formM = item.ID_formM,
                                                                          ID_visit = item.ID_visit,
                                                                          original = item.original,
                                                                          obj_order = item.obj_order,
                                                                          obj_group = item.obj_group,
                                                                          idkey = item.idkey,
                                                                          parent = item.parent,
                                                                          query1 = item.query1,
                                                                          query2 = item.query2,
                                                                          ID_empresa = item.ID_empresa

                                                                      }).ToList();


                List<MyObj_tablapadre> categoriasListActivitiesProducts = ObtenerCategoriarJerarquiaByID(listapadresActivitiesProducts, listahijasActivitiesProducts);

                ViewBag.productRows = categoriasListActivitiesProducts;

                var showbuttondynamic = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 11)
                                         select item).Count();

                if (showbuttondynamic > 0)
                {
                    ViewBag.dinamicos = 1;
                    var existproducts = (from item in FormsMDet
                                         where (item.ID_visit == activity.ID_activity && item.ID_formresourcetype == 3)
                                         select item).Count();
                    if (existproducts > 0)
                    {
                        ViewBag.mostrarboton = 0; //Lo ocultamos
                    }
                    else
                    {
                        ViewBag.mostrarboton = 1; //Lo mostramos
                    }

                }
                else
                {
                    ViewBag.mostrarboton = 0;
                    ViewBag.dinamicos = 0;
                }

                //Deserealizamos  los datos
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyObj[] details = js.Deserialize<MyObj[]>(formsM.query2);

                ViewBag.idvisitareal = activity.ID_visit;
                ViewBag.idvisita = activity.ID_activity;

                ViewBag.details = categoriasListActivities;


                Session["detailsForm"] = (from f in FormsMDet where (f.ID_visit == id) select f).ToList();

                VisitsM visitsM = db.VisitsM.Where(a => a.ID_visit == activity.ID_visit).FirstOrDefault();
                ViewBag.storename = visitsM.store;
                ViewBag.address = visitsM.address + ", " + visitsM.state + ", " + visitsM.city + ", " + visitsM.zipcode;

                return View();
            }

            //var FormsM_details = db.FormsM_details.Where(c => c.ID_formM == id && c.original == true).OrderBy(c => c.obj_group).ThenBy(c => c.obj_order);

            //return View(FormsM_details.ToList());

        }

        public ActionResult GetDynamicProductsAudit(string activityID, string ID_customer, string ID_brand)
        {
            try
            {
                int idact = Convert.ToInt32(activityID);
                List<BI_Dim_Products> lstproduct = new List<BI_Dim_Products>();
                string vendoriD = ID_customer;
                int brand = Convert.ToInt32(ID_brand);

                //int sub = Convert.ToInt32(ID_subcategory);

                using (COM_MKEntities dbmk = new COM_MKEntities())
                {
                    lstproduct = (dbmk.BI_Dim_Products.Where(x => x.Id_Brand == brand)).OrderBy(c => c.Id_Brand).ThenBy(c=>c.Id_SubCategory).ToList<BI_Dim_Products>();
                }

                if (lstproduct.Count > 0)
                {


                    ActivitiesM act = (from actd in db.ActivitiesM where (actd.ID_activity == idact) select actd).FirstOrDefault();
                    var countItems = (from a in db.FormsM_details where (a.ID_visit == idact) select a).Count();

                    var nuevacuenta = countItems + 2;
                    var activeSub = "";
                    var countp = 0;
                    var totalpro = lstproduct.Count();

                    var subcatid = 0;
                    var subcatname = "";

                    List<FormsM_details> detailstoinsert = new List<FormsM_details>();
                    foreach (var item in lstproduct)
                    {
                        try
                        {
                            if (countp == 0)
                            {
                                FormsM_details detalle_subcatnuevo = new FormsM_details(); //Cabecera de Marca o Subcategoria(depende para que se programe)


                                detalle_subcatnuevo.ID_formresourcetype = 95;
                                detalle_subcatnuevo.fsource = "";
                                detalle_subcatnuevo.fdescription = "";
                                detalle_subcatnuevo.fvalue = 0;
                                detalle_subcatnuevo.fvalueDecimal = 0;
                                detalle_subcatnuevo.fvalueText = item.SubCatName;
                                detalle_subcatnuevo.ID_formM = act.ID_form;

                                detalle_subcatnuevo.ID_visit = idact;
                                detalle_subcatnuevo.original = false;
                                //Colocamos numero de orden
                                detalle_subcatnuevo.obj_order = nuevacuenta;
                                //Colocamos grupo si tiene
                                detalle_subcatnuevo.obj_group = Convert.ToInt32(item.Id_SubCategory);
                                //Colocamos ID generado por editor
                                detalle_subcatnuevo.idkey = nuevacuenta;
                                detalle_subcatnuevo.query1 = "";
                                detalle_subcatnuevo.query2 = "";
                                detalle_subcatnuevo.parent = 0;
                                detalle_subcatnuevo.ID_empresa = 2;



                                detailstoinsert.Add(detalle_subcatnuevo);

                                nuevacuenta++;

                                subcatid = Convert.ToInt32(item.Id_SubCategory);
                                subcatname = item.SubCatName;
                                activeSub = item.SubCatName;

                            }

                            if (activeSub == item.SubCatName)
                            {

                            }
                            else
                            {
                                //inicial
                                FormsM_details detalle_subcatnuevo2 = new FormsM_details(); //Producto


                                detalle_subcatnuevo2.ID_formresourcetype = 95; //Este ID no existe en tabla de recursos, pero se define como las cabeceras de las subcategorias
                                detalle_subcatnuevo2.fsource = "";
                                detalle_subcatnuevo2.fdescription = "";
                                detalle_subcatnuevo2.fvalue = 0;
                                detalle_subcatnuevo2.fvalueDecimal = 0;
                                detalle_subcatnuevo2.fvalueText = item.SubCatName;
                                detalle_subcatnuevo2.ID_formM = act.ID_form;

                                detalle_subcatnuevo2.ID_visit = idact;
                                detalle_subcatnuevo2.original = false;
                                //Colocamos numero de orden
                                detalle_subcatnuevo2.obj_order = nuevacuenta;
                                //Colocamos grupo si tiene
                                detalle_subcatnuevo2.obj_group = Convert.ToInt32(item.Id_SubCategory);
                                //Colocamos ID generado por editor
                                detalle_subcatnuevo2.idkey = nuevacuenta;
                                detalle_subcatnuevo2.query1 = "";
                                detalle_subcatnuevo2.query2 = "";
                                detalle_subcatnuevo2.parent = 0;
                                detalle_subcatnuevo2.ID_empresa = 2;



                                detailstoinsert.Add(detalle_subcatnuevo2);

                                nuevacuenta++;


                                subcatid = Convert.ToInt32(item.Id_SubCategory);
                                subcatname = item.SubCatName;
                                activeSub = item.SubCatName;

                            }
                            countp++;

                            FormsM_details detalle_nuevo = new FormsM_details(); //Producto


                            detalle_nuevo.ID_formresourcetype = 3;
                            detalle_nuevo.fsource = item.ItemCode;
                            detalle_nuevo.fdescription = item.ItemName;
                            detalle_nuevo.fvalue = 0;
                            detalle_nuevo.fvalueDecimal = 0;
                            detalle_nuevo.fvalueText = item.SubCatName;
                            detalle_nuevo.ID_formM = act.ID_form;

                            detalle_nuevo.ID_visit = idact;
                            detalle_nuevo.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo.obj_group = Convert.ToInt32(item.Id_SubCategory);
                            //Colocamos ID generado por editor
                            detalle_nuevo.idkey = nuevacuenta;
                            detalle_nuevo.query1 = "";
                            detalle_nuevo.query2 = "";
                            detalle_nuevo.parent = 0;
                            detalle_nuevo.ID_empresa = 2;



                            detailstoinsert.Add(detalle_nuevo);
                            //dbcmk.SaveChanges();


                            var padrec = nuevacuenta;
                            nuevacuenta++;

                            FormsM_details detalle_nuevo2 = new FormsM_details(); //Disponibilidad


                            detalle_nuevo2.ID_formresourcetype = 16;
                            detalle_nuevo2.fsource = "";
                            detalle_nuevo2.fdescription = "Disponible";
                            detalle_nuevo2.fvalue = 0;
                            detalle_nuevo2.fvalueDecimal = 0;
                            detalle_nuevo2.fvalueText = "";
                            detalle_nuevo2.ID_formM = act.ID_form;

                            detalle_nuevo2.ID_visit = idact;
                            detalle_nuevo2.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo2.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo2.obj_group = 0;
                            //Colocamos ID generado por editor
                            detalle_nuevo2.idkey = nuevacuenta;
                            detalle_nuevo2.query1 = "";
                            detalle_nuevo2.query2 = "";
                            detalle_nuevo2.parent = padrec;
                            detalle_nuevo2.ID_empresa = 2;



                            detailstoinsert.Add(detalle_nuevo2);
                            //dbcmk.SaveChanges();
                            nuevacuenta++;


                            FormsM_details detalle_nuevo5 = new FormsM_details(); //Promocionado


                            detalle_nuevo5.ID_formresourcetype = 16;
                            detalle_nuevo5.fsource = "";
                            detalle_nuevo5.fdescription = "Promocionado";
                            detalle_nuevo5.fvalue = 0;
                            detalle_nuevo5.fvalueDecimal = 0;
                            detalle_nuevo5.fvalueText = "";
                            detalle_nuevo5.ID_formM = act.ID_form;

                            detalle_nuevo5.ID_visit = idact;
                            detalle_nuevo5.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo5.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo5.obj_group = 0;
                            //Colocamos ID generado por editor
                            detalle_nuevo5.idkey = nuevacuenta;
                            detalle_nuevo5.query1 = "";
                            detalle_nuevo5.query2 = "";
                            detalle_nuevo5.parent = padrec;
                            detalle_nuevo5.ID_empresa = 2;



                            detailstoinsert.Add(detalle_nuevo5);
                            //dbcmk.SaveChanges();
                            nuevacuenta++;


                            FormsM_details detalle_nuevo3 = new FormsM_details(); //Precio


                            detalle_nuevo3.ID_formresourcetype = 21;
                            detalle_nuevo3.fsource = "";
                            detalle_nuevo3.fdescription = "Precio";
                            detalle_nuevo3.fvalue = 0;
                            detalle_nuevo3.fvalueDecimal = 0; //PRECIO
                            detalle_nuevo3.fvalueText = "";
                            detalle_nuevo3.ID_formM = act.ID_form;

                            detalle_nuevo3.ID_visit = idact;
                            detalle_nuevo3.original = false;
                            //Colocamos numero de orden
                            detalle_nuevo3.obj_order = nuevacuenta;
                            //Colocamos grupo si tiene
                            detalle_nuevo3.obj_group = 0;
                            //Colocamos ID generado por editor
                            detalle_nuevo3.idkey = nuevacuenta;
                            detalle_nuevo3.query1 = "";
                            detalle_nuevo3.query2 = "";
                            detalle_nuevo3.parent = padrec;
                            detalle_nuevo3.ID_empresa = 2;



                            detailstoinsert.Add(detalle_nuevo3);
                            //dbcmk.SaveChanges();
                            nuevacuenta++;

                            if (countp == totalpro)
                            {
                            }


                        }
                        catch (Exception ex)
                        {
                            var error = ex.Message;

                        }

                    }
                    //Insertamos los datos usando insercion masiva
                    db.BulkInsert(detailstoinsert);

                    //Si existieran datos al final del formulario hacemos este calculo(esto para respetar el orden de ingreso
                    //FormsM_details lastitem = (from a in dbcmk.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems - 2)) select a).FirstOrDefault();

                    //lastitem.obj_order = nuevacuenta + 200;
                    //lastitem.idkey = nuevacuenta + 200;
                    //dbcmk.Entry(lastitem).State = EntityState.Modified;
                    ////dbcmk.SaveChanges();
                    //nuevacuenta++;
                    //FormsM_details lastitem2 = (from a in dbcmk.FormsM_details where (a.ID_visit == idact && a.idkey == (countItems - 1)) select a).FirstOrDefault();

                    //lastitem2.obj_order = nuevacuenta + 200;
                    //lastitem2.idkey = nuevacuenta + 200;
                    //dbcmk.Entry(lastitem2).State = EntityState.Modified;
                    ////dbcmk.SaveChanges();
                    //nuevacuenta++;
                    //FormsM_details lastitem3 = (from a in dbcmk.FormsM_details where (a.ID_visit == idact && a.idkey == countItems) select a).FirstOrDefault();

                    //lastitem3.obj_order = nuevacuenta + 200;
                    //lastitem3.idkey = nuevacuenta + 200;
                    //dbcmk.Entry(lastitem3).State = EntityState.Modified;
                    //dbcmk.SaveChanges();

                    string result = "Success";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    string result = "Nodata";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                string result = "Error";
                return Json(result, JsonRequestBehavior.AllowGet);
            }



        }

    }


}
