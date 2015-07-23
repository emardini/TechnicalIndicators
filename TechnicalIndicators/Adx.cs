namespace TechnicalIndicators
{
    using System.Collections.Generic;

    //Use http://www.dinosaurtech.com/2007/average-directional-movement-index-adx-formula-in-c-2/
    //Or http://www.theforexguy.com/average-directional-index-indicator/
    public class Adx
    {
        public IEnumerable<decimal> Values { get; set; }
    }
}