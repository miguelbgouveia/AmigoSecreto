using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AmigoSecreto.DAL;
using AmigoSecreto.Models;

namespace AmigoSecreto.Controllers
{
    [Authorize]
    public class ImpedimentosController : Controller
    {
        private AmigoSecretoContext db = new AmigoSecretoContext();

        // GET: Impedimentos
        public ActionResult Index()
        {
            var impedimentoes = db.Impedimentoes.Include(i => i.Amigo);
            return View(impedimentoes.ToList());
        }

        // GET: Impedimentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impedimento impedimento = db.Impedimentoes.Find(id);
            if (impedimento == null)
            {
                return HttpNotFound();
            }
            return View(impedimento);
        }

        // GET: Impedimentos/Create
        public ActionResult Create()
        {
            ViewBag.AmigoID = new SelectList(db.Amigos, "ID", "Nome");
            return View();
        }

        // POST: Impedimentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NomeLike,AmigoID")] Impedimento impedimento)
        {
            if (ModelState.IsValid)
            {
                db.Impedimentoes.Add(impedimento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AmigoID = new SelectList(db.Amigos, "ID", "Nome", impedimento.AmigoID);
            return View(impedimento);
        }

        // GET: Impedimentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impedimento impedimento = db.Impedimentoes.Find(id);
            if (impedimento == null)
            {
                return HttpNotFound();
            }
            ViewBag.AmigoID = new SelectList(db.Amigos, "ID", "Nome", impedimento.AmigoID);
            return View(impedimento);
        }

        // POST: Impedimentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NomeLike,AmigoID")] Impedimento impedimento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(impedimento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AmigoID = new SelectList(db.Amigos, "ID", "Nome", impedimento.AmigoID);
            return View(impedimento);
        }

        // GET: Impedimentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impedimento impedimento = db.Impedimentoes.Find(id);
            if (impedimento == null)
            {
                return HttpNotFound();
            }
            return View(impedimento);
        }

        // POST: Impedimentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Impedimento impedimento = db.Impedimentoes.Find(id);
            db.Impedimentoes.Remove(impedimento);
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
