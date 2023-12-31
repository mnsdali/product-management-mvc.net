﻿using Microsoft.AspNetCore.Mvc;
using MiniProjet_.NET.Models;
using MiniProjet_.NET.Models.Repositories;

namespace MiniProjet_.NET.Controllers
{
    public class ArticleController : Controller
    {
        //Injection de la dépendance
        readonly IArticleRepository ArticleRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ArticleController(IArticleRepository ArtRepository, IWebHostEnvironment hostingEnvironment)
        {
            ArticleRepository = ArtRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ArticleController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ArticleController/Details/5
        public ActionResult Details(int id)
        {
            var article = ArticleRepository.GetArticle(id);

            return View(article);
        }

        // GET: ArticleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArticleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ArticleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ArticleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ArticleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ArticleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
