using System.Collections.Generic;

namespace PanPizza.Model
{
    internal interface IPizza
    {
        Sizes Size { get; }
        List<Toppings> Toppings { get;}
        double GetPrice();
    }
}