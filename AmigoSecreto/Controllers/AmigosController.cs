using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AmigoSecreto.DAL;
using AmigoSecreto.Models;
using AmigoSecreto.Servicos;

namespace AmigoSecreto.Controllers
{
    [Authorize]
    public class AmigosController : Controller
    {
        private readonly AmigoSecretoContext db = new AmigoSecretoContext();

        // GET: Amigoes
        public ActionResult Index()
        {
            return View(db.Amigos.ToList());
        }

        // GET: Amigoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // GET: Amigoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Amigoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,NTelemovel")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                amigo.UltimoEnvioSMS = DateTime.Parse("1900-01-01");
                amigo.ResultadoUltimoEnvioSMS = "";

                db.Amigos.Add(amigo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(amigo);
        }

        // GET: Amigoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // POST: Amigoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,NTelemovel,UltimoEnvioSMS,ResultadoUltimoEnvioSMS")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(amigo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(amigo);
        }

        // GET: Amigoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // POST: Amigoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Amigo amigo = db.Amigos.Find(id);
            db.Amigos.Remove(amigo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TestarSorteio()
        {
            GereSorteioAmigos.SortearAmigosSecretos(db, true);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult TestarEnvioSMS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestarEnvioSMS(bool testar = true)
        {
            foreach (var amigo in db.Amigos)
                SendTestMessage(amigo);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SortearAmigos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SortearAmigos(bool testar = true)
        {
            GereSorteioAmigos.SortearAmigosSecretos(db, false);

            return RedirectToAction("Index");
        }

        private void SendTestMessage(Amigo amigo)
        {
            if (amigo == null)
                return;

            string mensagem = amigo.Nome  + ", isto e um teste para o sorteio do amigo secreto. " +
                              "Se nao recebeu esta mensagem, reclame com o Pai Natal!";

            var result = SMSService.SendSMS(amigo.NTelemovel, mensagem);

            amigo.UltimoEnvioSMS = DateTime.Now;
            amigo.ResultadoUltimoEnvioSMS = result;
            db.Entry(amigo).State = EntityState.Modified;
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
