using Domain.Abstract;
using Domain.AutoShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUl.Controllers
{
    public class AdminController : Controller
    {
        ISkodaRepository repository;

        public AdminController(ISkodaRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index(string sortOrder, string skodasModel, string searchString)
        {
            ViewBag.ModelSortParm = String.IsNullOrEmpty(sortOrder) ? "model_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            var model = from s in repository.Skodas
                           select s;
            var ModelLst = new List<string>();

            var ModelQry = from d in repository.Skodas
                           orderby d.Model
                           select d.Model;

            ModelLst.AddRange(ModelQry.Distinct());
            ViewBag.skodasModel = new SelectList(ModelLst);

            //var model = from m in repository.Skodas
            //            select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.Name.Contains(searchString));
                   
            }

            if (!string.IsNullOrEmpty(skodasModel))
            {
                model = model.Where(x => x.Model == skodasModel);
            }
            switch (sortOrder)
            {
                case "model_desc":
                    model = model.OrderByDescending(s => s.Model);
                    break;
                case "Price":
                    model = model.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    model = model.OrderByDescending(s => s.Price);
                    break;
                default:
                    model = model.OrderBy(s => s.Model);
                    break;
            }
            
          
            return View(model);
           
        }
        public ViewResult Edit(int Id)
        {
           Skoda game = repository.Skodas
                .FirstOrDefault(g => g.Id == Id);
            return View(game);
        }
        // Перегруженная версия Edit() для сохранения изменений
        [HttpPost]
        public ActionResult Edit(Skoda game)
        {
            if (ModelState.IsValid)
            {
                repository.SaveSkoda(game);
                TempData["message"] = string.Format("Изменения в Auto City Skoda \"{0} {1}\" были сохранены", game.Name, game.Model);
                return RedirectToAction("Index");
            }
            else
            {
                // Что-то не так со значениями данных
                return View(game);
            }
        }
        public ViewResult Create()
        {
            return View("Edit", new Skoda());
        }
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            Skoda deletedSkoda = repository.DeleteSkoda(Id);
            if (deletedSkoda != null)
            {
                TempData["message"] = string.Format("Машина \"{0}\" была удалена",
                    deletedSkoda.Name);
            }
            return RedirectToAction("Index");
        }
        }

    }

