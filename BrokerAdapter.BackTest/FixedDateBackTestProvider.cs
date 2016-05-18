using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerAdapter.BackTest
{
    using TechnicalIndicators;

    public class FixedDateBackTestProvider : IDateProvider
    {
        public DateTime GetCurrentDate()
        {
            return new DateTime(2016, 5, 11, 21, 0, 0);
        }

        public DateTime GetCurrentUtcDate()
        {
            return this.GetCurrentDate();
        }
    }
}
