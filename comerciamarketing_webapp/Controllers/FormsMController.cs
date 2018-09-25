using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using comerciamarketing_webapp.Models;
using Newtonsoft.Json;

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

                detalle_nuevo.parent = 0;
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

                if (Convert.ToInt32(item.id_resource) == 3) { //PARA PRODUCTO
                    foreach (var itemColumna in listaColumnas)
                    {
                        //AUMENTAMOS EL CONTADOR PARA ORDENAMIENTO
                        order2++;
                        if (itemColumna.fvalue == "16") {
                            //Multiple choice
                            FormsM_details Subdetalle_nuevo = new FormsM_details();

                            Subdetalle_nuevo.parent = detalle_nuevo.idkey;
                            Subdetalle_nuevo.ID_formresourcetype = 16;
                            Subdetalle_nuevo.fsource = "";
                            Subdetalle_nuevo.fvalueText = "";

                            Subdetalle_nuevo.fdescription = Convert.ToString(itemColumna.fdescription);
                            Subdetalle_nuevo.fvalue = 0;
                            Subdetalle_nuevo.fvalueDecimal = 0;

                            Subdetalle_nuevo.ID_formM = formid;
                            //colocamos 0 ya que esta seria la plantila
                            Subdetalle_nuevo.ID_visit = 0;
                            //Se coloca true ya que con esto identificamos que es un item del template original
                            Subdetalle_nuevo.original = true;
                            //Colocamos numero de orden
                            Subdetalle_nuevo.obj_order = order2;
                            //Colocamos grupo si tiene
                            Subdetalle_nuevo.obj_group = 0;
                            //Colocamos ID generado por editor
                            Subdetalle_nuevo.idkey = order2;
                            Subdetalle_nuevo.query1 = "";
                            Subdetalle_nuevo.query2 = "";
                            Subdetalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;

                            db.FormsM_details.Add(Subdetalle_nuevo);
                            db.SaveChanges();
                        }
                        else if (itemColumna.fvalue == "21")
                        {
                            //Currency
                            FormsM_details Subdetalle_nuevo = new FormsM_details();

                            Subdetalle_nuevo.parent = detalle_nuevo.idkey;
                            Subdetalle_nuevo.ID_formresourcetype = 21;
                            Subdetalle_nuevo.fsource = "";
                            Subdetalle_nuevo.fvalueText = "";

                            Subdetalle_nuevo.fdescription = Convert.ToString(itemColumna.fdescription);
                            Subdetalle_nuevo.fvalue = 0;
                            Subdetalle_nuevo.fvalueDecimal = 0;

                            Subdetalle_nuevo.ID_formM = formid;
                            //colocamos 0 ya que esta seria la plantila
                            Subdetalle_nuevo.ID_visit = 0;
                            //Se coloca true ya que con esto identificamos que es un item del template original
                            Subdetalle_nuevo.original = true;
                            //Colocamos numero de orden
                            Subdetalle_nuevo.obj_order = order2;
                            //Colocamos grupo si tiene
                            Subdetalle_nuevo.obj_group = 0;
                            //Colocamos ID generado por editor
                            Subdetalle_nuevo.idkey = order2;
                            Subdetalle_nuevo.query1 = "";
                            Subdetalle_nuevo.query2 = "";
                            Subdetalle_nuevo.ID_empresa = GlobalVariables.ID_EMPRESA_USUARIO;

                            db.FormsM_details.Add(Subdetalle_nuevo);
                            db.SaveChanges();
                        }
                    }

                }


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
            if (customerID != null) {
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
        public ActionResult Activityresume(int? id)
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



                    ViewBag.detailssql = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();


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
        public ActionResult Activity(int? id)
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
                else {

                    FormsM formsM = db.FormsM.Find(activity.ID_form);

                    //LISTADO DE CLIENTES
                    //VERIFICAMOS SI SELECCIONARON CLIENTE PREDEFINIDO

                    if (activity.Customer != "")
                    {
                        var customers = (from b in COM_MKdb.OCRD where (b.Series == 61 && b.CardCode==activity.ID_customer) select b).OrderBy(b => b.CardName).ToList();
                        ViewBag.customers = customers.ToList();
                    }
                    else {
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
                    List<MyObj_tablapadre> listapadresActivities = (from item in db.FormsM_details where(item.parent ==0 && item.ID_visit == activity.ID_activity && item.original==false)
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

                    ViewBag.detailssql = (from a in db.FormsM_details where (a.ID_visit == activity.ID_activity && a.original == false) select a).ToList();




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

        public JsonResult Save_activity(string id, List<MyObj_formtemplate> objects, string lat, string lng, string check_in)
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
                        FormsM_details detail = (from f in db.FormsM_details where(f.ID_visit == activity.ID_activity &&  f.idkey== IDItem) select f).FirstOrDefault();
                        if (detail == null)
                        {

                        }
                        else
                        {
                            if (detail.ID_formresourcetype == 3 || detail.ID_formresourcetype == 4 || detail.ID_formresourcetype == 10)//Products, Samples,Gift
                            {
                                if (item.value == "" || item.value == null) { item.value = "0"; }
                                detail.fvalue = Convert.ToInt32(item.value);

                                db.Entry(detail).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else if (detail.ID_formresourcetype == 5) //Picture
                            {
                                if (item.value == "100") {
                                    var path = detail.fsource;
                                    //eliminamos la ruta
                                    detail.fsource = "";

                                    db.Entry(detail).State = EntityState.Modified;
                                    db.SaveChanges();


                                    if(System.IO.File.Exists(Server.MapPath(path)))
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
                                //Esta opcion se anulo, ya que las imagenes se guardan en tiempo real
                                //Pero la habilitaremos para cuando el usuario remueva la foto seleccionada



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
                            }
                            //Select, Customer, Brands,Product line, Brand Competitors
                            else if (detail.ID_formresourcetype == 17 || detail.ID_formresourcetype == 12 || detail.ID_formresourcetype == 13 || detail.ID_formresourcetype == 14 || detail.ID_formresourcetype == 15) 
                            {

                                if (item.value == "" || item.value == null) { item.value = ""; }
                                if (item.text == "" || item.text == null) { item.text = ""; }
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
                                else if (item.value == "true") {
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
            catch (Exception ex) {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
        [HttpPost]
        public ActionResult UploadFiles(string id,string idvisita)
        {


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
                            if (TargetImg.PropertyIdList.Contains(0x0112))
                            {
                                int rotationValue = TargetImg.GetPropertyItem(0x0112).Value[0];
                                switch (rotationValue)
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
                        }
                        catch
                        {

                        }
                        //buscamos el id del detalle
                        int idf = Convert.ToInt32(id);
                        int idvisit = Convert.ToInt32(idvisita);
                        FormsM_details detail = (from d in db.FormsM_details where(d.idkey == idf && d.ID_visit == idvisit) select d).FirstOrDefault();
                        var pathimg = detail.fsource;

                        DateTime time = DateTime.Now;


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
                            var path = Path.Combine(Server.MapPath("~/Content/images/activities"), id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg");
                            imagenfinal.Save(path, ImageFormat.Jpeg);

                        }


                        //fname = Path.Combine(Server.MapPath("~/Content/images/ftp_demo"), fname);
                        //file.SaveAs(fname);

                        //Luego guardamos la url en la db
                        //Forms_details detail = db.Forms_details.Find(Convert.ToInt32(id));  //se movio hacia arriba
                        detail.fsource = "~/Content/images/activities/" + id + "_activity_" + detail.ID_visit + "_" + time.Minute + time.Second + ".jpg";

                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();

                        if (System.IO.File.Exists(Server.MapPath(pathimg)))
                        {
                            try
                            {
                                System.IO.File.Delete(Server.MapPath(pathimg));
                            }
                            catch (System.IO.IOException e)
                            {
                                Console.WriteLine(e.Message);

                            }
                        }
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
                int IDU = Convert.ToInt32(Session["IDusuario"]);
                if (id != null)
                {
                    int act = Convert.ToInt32(id);
                    ActivitiesM activity = db.ActivitiesM.Find(act);

                    if (lat != null || lat != "")
                    {
                        //Guardamos el log de la actividad
                        ActivitiesM_log nuevoLog = new ActivitiesM_log();
                        nuevoLog.latitude = lat;
                        nuevoLog.longitude = lng;
                        nuevoLog.ID_usuario = IDU;
                        nuevoLog.ID_activity = Convert.ToInt32(id);
                        nuevoLog.fecha_conexion = Convert.ToDateTime(check_out);
                        nuevoLog.query1 = "";
                        nuevoLog.query2 = "";
                        nuevoLog.action = "FINISH ACTIVITY - " + activity.description;
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

                    activity.check_out = Convert.ToDateTime(check_out);                  
                    activity.isfinished = true;
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
      


                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Warning" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Warning" + ex.Message });
            }


        }
    }

}
