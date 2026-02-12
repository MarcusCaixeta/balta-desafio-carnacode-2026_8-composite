// DESAFIO: Sistema de Menus Hierárquicos
// PROBLEMA: Um sistema de gestão de conteúdo precisa construir menus com itens simples e submenus aninhados
// O código atual trata itens individuais e grupos de forma diferente, complicando operações recursivas

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // Contexto: Sistema CMS que precisa renderizar menus complexos com múltiplos níveis
    // Alguns itens são links simples, outros são menus que contêm mais itens
    
    public class MenuItem
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

        public int CountItems()
        {
            return 1;
        }
    }

    public class MenuGroup
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public List<MenuItem> Items { get; set; }
        public List<MenuGroup> SubGroups { get; set; }

        public MenuGroup(string title, string icon = "")
        {
            Title = title;
            Icon = icon;
            IsActive = true;
            Items = new List<MenuItem>();
            SubGroups = new List<MenuGroup>();
        }

        // Problema: Lógica complexa para renderizar itens e subgrupos
        public void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} ▼");

            // Precisa iterar sobre duas coleções diferentes
            foreach (var item in Items)
            {
                item.Render(indent + 1);
            }

            foreach (var subGroup in SubGroups)
            {
                subGroup.Render(indent + 1);
            }
        }

        // Problema: Contagem recursiva complexa
        public int CountItems()
        {
            int count = 0;
            
            count += Items.Count;
            
            foreach (var subGroup in SubGroups)
            {
                count += subGroup.CountItems();
            }
            
            return count;
        }

        // Problema: Operações em lote exigem código duplicado
        public void DisableAllItems()
        {
            foreach (var item in Items)
            {
                item.IsActive = false;
            }

            foreach (var subGroup in SubGroups)
            {
                subGroup.DisableAllItems();
            }
        }
    }

    public class MenuManager
    {
        private List<MenuItem> _topLevelItems;
        private List<MenuGroup> _topLevelGroups;

        public MenuManager()
        {
            _topLevelItems = new List<MenuItem>();
            _topLevelGroups = new List<MenuGroup>();
        }

        // Problema: Precisa gerenciar dois tipos diferentes no nível raiz
        public void AddItem(MenuItem item)
        {
            _topLevelItems.Add(item);
        }

        public void AddGroup(MenuGroup group)
        {
            _topLevelGroups.Add(group);
        }

        // Problema: Renderização trata itens e grupos separadamente
        public void RenderMenu()
        {
            Console.WriteLine("=== Menu Principal ===\n");

            foreach (var item in _topLevelItems)
            {
                item.Render();
            }

            foreach (var group in _topLevelGroups)
            {
                group.Render();
            }
        }

        // Problema: Operações precisam iterar sobre ambas as coleções
        public int GetTotalItems()
        {
            int count = _topLevelItems.Count;

            foreach (var group in _topLevelGroups)
            {
                count += group.CountItems();
            }

            return count;
        }

        // Problema: Busca em toda hierarquia é complicada
        public MenuItem FindItemByUrl(string url)
        {
            foreach (var item in _topLevelItems)
            {
                if (item.Url == url)
                    return item;
            }

            foreach (var group in _topLevelGroups)
            {
                // Precisa buscar recursivamente em cada grupo
                var found = FindInGroup(group, url);
                if (found != null)
                    return found;
            }

            return null;
        }

        private MenuItem FindInGroup(MenuGroup group, string url)
        {
            foreach (var item in group.Items)
            {
                if (item.Url == url)
                    return item;
            }

            foreach (var subGroup in group.SubGroups)
            {
                var found = FindInGroup(subGroup, url);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Menus CMS ===\n");

            var manager = new MenuManager();

            // Item simples no nível raiz
            manager.AddItem(new MenuItem("Home", "/", "🏠"));

            // Grupo com itens
            var productsMenu = new MenuGroup("Produtos", "📦");
            productsMenu.Items.Add(new MenuItem("Todos", "/produtos"));
            productsMenu.Items.Add(new MenuItem("Categorias", "/categorias"));
            productsMenu.Items.Add(new MenuItem("Ofertas", "/ofertas"));

            // Subgrupo dentro de grupo
            var clothingMenu = new MenuGroup("Roupas", "👕");
            clothingMenu.Items.Add(new MenuItem("Camisetas", "/roupas/camisetas"));
            clothingMenu.Items.Add(new MenuItem("Calças", "/roupas/calcas"));
            productsMenu.SubGroups.Add(clothingMenu);

            manager.AddGroup(productsMenu);

            // Outro grupo
            var adminMenu = new MenuGroup("Administração", "⚙️");
            adminMenu.Items.Add(new MenuItem("Usuários", "/admin/usuarios"));
            adminMenu.Items.Add(new MenuItem("Configurações", "/admin/config"));
            manager.AddGroup(adminMenu);

            manager.RenderMenu();

            Console.WriteLine($"\nTotal de itens no menu: {manager.GetTotalItems()}");

            // Problema: Buscar item requer lógica especial para navegar hierarquia
            var item = manager.FindItemByUrl("/roupas/camisetas");
            if (item != null)
            {
                Console.WriteLine($"\n✓ Item encontrado: {item.Title}");
            }

            Console.WriteLine("\n=== PROBLEMAS ===");
            Console.WriteLine("✗ MenuItem e MenuGroup são tratados de forma diferente");
            Console.WriteLine("✗ Operações recursivas requerem código duplicado");
            Console.WriteLine("✗ Cliente precisa saber se está lidando com item ou grupo");
            Console.WriteLine("✗ Adicionar nova operação = modificar ambas as classes");
            Console.WriteLine("✗ Não há interface uniforme para tratar a hierarquia");

            // Perguntas para reflexão:
            // - Como tratar itens individuais e grupos de forma uniforme?
            // - Como simplificar operações recursivas na hierarquia?
            // - Como permitir que o cliente trate toda a estrutura sem saber os detalhes?
            // - Como facilitar adicionar novas operações que percorrem a árvore?
        }
    }
}
