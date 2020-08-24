using System.Collections.Generic;
using System.Linq;

namespace PanPizza.Model
{
    class PizzaPan : IPizza
    {
        public PizzaPan(Sizes size, List<Toppings> toppings)
        {
            Size = size;
            Toppings = toppings;
            Price = CalculatePrice();
        }

        public Sizes Size { get; protected set; }
        public List<Toppings> Toppings { get; protected set; }
        public double Price { get; protected set; }

        public double GetPrice()
        {
            return Price;
        }
        private double CalculatePrice()
        {
            Price = 100;
            if (Size == Sizes.Big)
                Price += 200;
            else if (Size == Sizes.Medium)
                Price += 100;

            if (Toppings.Any())
            {
                foreach(var t in Toppings)
                {
                    Price += (int)t;
                }
            }
            return Price;
        }
    }
}
