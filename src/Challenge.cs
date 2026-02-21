// DESAFIO: Sistema de Menus Hierárquicos

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // Componente base do Composite - interface uniforme
    public interface IMenuComponent
    {
        string Title { get; }
        string Url { get; }
        string Icon { get; }
        bool IsActive { get; set; }
        void Render(int indent = 0);
        int CountItems();
        void DisableAllItems();
        IMenuComponent FindByUrl(string url);
    }

    public class MenuItem : IMenuComponent
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }

        public MenuItem(string title, string url, string icon = "")
        {
            Title = title;
            Url = url;
            Icon = icon;
            IsActive = true;
        }

        public void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} → {Url}");
        }

        public int CountItems() => 1;

        public void DisableAllItems() => IsActive = false;

        public IMenuComponent FindByUrl(string url) => Url == url ? this : null;
    }

    public class MenuGroup : IMenuComponent
    {
        public string Title { get; set; }
        public string Url => "";
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        private readonly List<IMenuComponent> _children = new();

        public MenuGroup(string title, string icon = "")
        {
            Title = title;
            Icon = icon;
            IsActive = true;
        }

        public void Add(IMenuComponent component) => _children.Add(component);

        public void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} ▼");

            foreach (var child in _children)
                child.Render(indent + 1);
        }

        public int CountItems()
        {
            int count = 0;
            foreach (var child in _children)
                count += child.CountItems();
            return count;
        }

        public void DisableAllItems()
        {
            IsActive = false;
            foreach (var child in _children)
                child.DisableAllItems();
        }

        public IMenuComponent FindByUrl(string url)
        {
            foreach (var child in _children)
            {
                var found = child.FindByUrl(url);
                if (found != null) return found;
            }
            return null;
        }
    }

    public class MenuManager
    {
        private readonly List<IMenuComponent> _root = new();

        public void Add(IMenuComponent component) => _root.Add(component);

        public void RenderMenu()
        {
            Console.WriteLine("=== Menu Principal ===\n");
            foreach (var component in _root)
                component.Render();
        }

        public int GetTotalItems()
        {
            int count = 0;
            foreach (var component in _root)
                count += component.CountItems();
            return count;
        }

        public IMenuComponent FindByUrl(string url)
        {
            foreach (var component in _root)
            {
                var found = component.FindByUrl(url);
                if (found != null) return found;
            }
            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Menus CMS (Composite Pattern) ===\n");

            var manager = new MenuManager();

            manager.Add(new MenuItem("Home", "/", "🏠"));

            var productsMenu = new MenuGroup("Produtos", "📦");
            productsMenu.Add(new MenuItem("Todos", "/produtos"));
            productsMenu.Add(new MenuItem("Categorias", "/categorias"));
            productsMenu.Add(new MenuItem("Ofertas", "/ofertas"));

            var clothingMenu = new MenuGroup("Roupas", "👕");
            clothingMenu.Add(new MenuItem("Camisetas", "/roupas/camisetas"));
            clothingMenu.Add(new MenuItem("Calças", "/roupas/calcas"));
            productsMenu.Add(clothingMenu);

            manager.Add(productsMenu);

            var adminMenu = new MenuGroup("Administração", "⚙️");
            adminMenu.Add(new MenuItem("Usuários", "/admin/usuarios"));
            adminMenu.Add(new MenuItem("Configurações", "/admin/config"));
            manager.Add(adminMenu);

            manager.RenderMenu();

            Console.WriteLine($"\nTotal de itens no menu: {manager.GetTotalItems()}");

            var item = manager.FindByUrl("/roupas/camisetas");
            if (item != null)
                Console.WriteLine($"\n✓ Item encontrado: {item.Title}");

        }
    }
}
