using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionApp.Data
{
   public class ItemsDataSource
    {
        private static List<Item> _items = new List<Item>()
        {
            new Item()
            {
                Id = 0,
                DateCreated = DateTime.Now,
                Title = "Native Plants",
                Text = @"The native flora of New Zealand is unique as it evolved in isolation for millions of years.",
                Url = "http://www.doc.govt.nz/nature/native-plants/"
            },
            new Item()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                Title = "Native Animals",
                Text = @"New Zealand has many unique native fish, insects, birds, lizards and frogs. Our only native mammals are bats and marine mammals.",
                Url = "http://www.doc.govt.nz/nature/native-animals/"
            },
            new Item()
            {
                Id = 2,
                DateCreated = DateTime.Now,
                Title = "Pests and Threats",
                Text = @"New Zealand's unique biodiversity is at risk from pests, weeds and other threats.",
                Url = "http://www.doc.govt.nz/nature/pests-and-threats/"
            },
            new Item()
            {
                Id = 3,
                DateCreated = DateTime.Now,
                Title = "Habitats",
                Text = @"Habitats are the environments in which particular plants and animals live.",
                Url = "http://www.doc.govt.nz/nature/habitats/"
            }
        };

        public static IList<Item> GetAllItems()
        {
            return _items;
        }

        public static Item GetItemById(int id)
        {
            return _items[id];
        }
    }
}
