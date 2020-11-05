using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Demo.WebClient.Models;
using Rebus.Bus;
using Demo.Domain.Commands;
using Demo.WebClient.DbModels;
using Demo.Domain.Events.InventoryItem;
using System.Threading;

namespace Demo.WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBus _bus;
        private readonly InventoryContext _context;

        public HomeController(ILogger<HomeController> logger, IBus bus, InventoryContext context)
        {
            _logger = logger;
            _bus = bus;
            _context = context;
        }

        public ActionResult Index()
        {
            var listItems = _context.ItemList.Select(x => x)
                .ToList();

            return View(listItems);
        }
        
        public ActionResult Details(Guid id)
        {
            var details = _context.ItemDetail
                .Where(x => x.Id == id)
                .Single();

            return View(details);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            _bus.Publish(new CreateInventoryItem(Guid.NewGuid(), name));

            Thread.Sleep(400); // Hack

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            var details = _context.ItemDetail
                .Where(x => x.Id == id)
                .Single();
            return View(details);
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string newName, int version)
        {
            _bus.Publish(new RenameInventoryItem(id, newName, version));

            Thread.Sleep(400); // Hack

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id)
        {
            var details = _context.ItemDetail
                .Where(x => x.Id == id)
                .Single();
            return View(details);
        }
        
        [HttpPost]
        public ActionResult Deactivate(Guid id, int version)
        {
            _bus.Publish(new DeactivateInventoryItem(id, version));

            Thread.Sleep(400); // Hack

            return RedirectToAction("Index");
        }
        
        public ActionResult CheckIn(Guid id)
        {
            var details = _context.ItemDetail
                .Where(x => x.Id == id)
                .Single();
            return View(details);
        }
        
        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            _bus.Publish(new CheckInItemsToInventory(id, number, version));
            Thread.Sleep(400); // Hack
            return RedirectToAction("Index");
        }
        
        public ActionResult Remove(Guid id)
        {
            var details = _context.ItemDetail
                .Where(x => x.Id == id)
                .Single();
            return View(details);
        }
        
        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            _bus.Publish(new RemoveItemsFromInventory(id, number, version));
            Thread.Sleep(400); // Hack
            return RedirectToAction("Index");
        }
    }
}
