using System.Data.Entity;
using TechnicalIndicators;

namespace BrokerAdapter.BackTest
{
    public class AskBidCandleContext : DbContext
    {
        public AskBidCandleContext() 
            : base("name=ForexContext") 
        {
        }
        public DbSet<AskBidCandle> AskBidCandles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AskBidCandle>().ToTable("dbo.2016_JAN-OCT-GBPUSD_m1_BidAndAsk");
        }
    }
}
