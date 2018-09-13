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
    public class Stock_itemsController : Controller
    {
        private dbComerciaEntities db = new dbComerciaEntities();
        private COM_MKEntities COM_MKdb = new COM_MKEntities();
        // GET: Stock_items
        public ActionResult Index()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                return View(db.Stock_items.ToList());


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
  
        }

        // GET: Stock_items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock_items stock_items = db.Stock_items.Find(id);
            if (stock_items == null)
            {
                return HttpNotFound();
            }
            return View(stock_items);
        }

        // GET: Stock_items/Create
        public ActionResult Create()
        {
            if (Session["IDusuario"] != null)
            {
                int ID = Convert.ToInt32(Session["IDusuario"]);
                var datosUsuario = (from c in db.Usuarios where (c.ID_usuario == ID) select c).FirstOrDefault();

                ViewBag.usuario = datosUsuario.correo;
                ViewBag.nomusuarioSAP = datosUsuario.Empresas.nombre;

                ViewBag.itemCode = new SelectList(COM_MKdb.OITM.Where(x => x.ItmsGrpCod == 108 || x.ItmsGrpCod == 107).OrderBy(x => x.ItemName), "ItemCode", "ItemName");
                return View();


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        // POST: Stock_items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_stock,itemCode,itemName,quantity,odate,ID_usuario,comment")] Stock_items stock_items)
        {
            int ID = Convert.ToInt32(Session["IDusuario"]);
            stock_items.ID_usuario = ID.ToString();
            stock_items.odate = DateTime.UtcNow;
            stock_items.comment = stock_items.comment + " - " + "PROVIENE DE MODULO INVENTARIO"; 
            if (ModelState.IsValid)
            {
                db.Stock_items.Add(stock_items);
                db.SaveChanges();
                TempData["exito"] = "Stock receipt created successfully.";
                return RedirectToAction("Index");
            }

            return View(stock_items);
        }

        // GET: Stock_items/Edit/5
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
                Stock_items stock_items = db.Stock_items.Find(id);
                if (stock_items == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.itemCode = new SelectList(COM_MKdb.OITM.Where(x => x.ItmsGrpCod == 108 || x.ItmsGrpCod == 107).OrderBy(x => x.ItemName), "ItemCode", "ItemName",id);
                return View(stock_items);


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Stock_items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_stock,itemCode,itemName,quantity,odate,ID_usuario,comment")] Stock_items stock_items)
        {
            int ID = Convert.ToInt32(Session["IDusuario"]);
            stock_items.ID_usuario = ID.ToString();
            stock_items.odate = DateTime.UtcNow;
            stock_items.comment = stock_items.comment + " - " + "PROVIENE DE MODULO INVENTARIO";
            if (ModelState.IsValid)
            {
                db.Entry(stock_items).State = EntityState.Modified;
                db.SaveChanges();
                TempData["exito"] = "Stock receipt saved successfully.";
                return RedirectToAction("Index");
            }
            return View(stock_items);
        }

        // GET: Stock_items/Delete/5
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
                Stock_items stock_items = db.Stock_items.Find(id);
                if (stock_items == null)
                {
                    return RedirectToAction("Index");
                }
                return View(stock_items);


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Stock_items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock_items stock_items = db.Stock_items.Find(id);
            db.Stock_items.Remove(stock_items);
            db.SaveChanges();
            TempData["exito"] = "Stock receipt deleted successfully.";
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
