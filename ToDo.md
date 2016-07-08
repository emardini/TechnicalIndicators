*	~~Check halted state~~
*   Change logic to prioritize close/modify order in case of unstable conditions
*	Health monitoring, alerts and logging (important events, traces)
*	Add rates, candles, orders and results in permanent store for backtesting, compalints and debugging
*	~~Verify rates and candles completion~~
	*	~~Make check rates call get last candle~~
	*	~~Prevent adding repeated candles~~
	*	~~Complete missing candles~~
*	Compare adx calculation against other systems: in progress, seems good with visual inspection
*	Add retries
*	Expose configuration
	*	Enable/disable auto transactions
*	Implement kelly criterion
*	Limit amount of data to be kept 
*	Backtest
	*	Prebuilt data
	*	Historical data: in progress, seems good, checking 05/17/2016 9:30PM, it should be a good long position with profit
	*	Uptrend
	*	Downtrend
	*	No trend
	*	Exit
	*	Stop losses
*	~~Make data provider to get east time~~
*	~~Validate the candle timestamp matches the rate date~~
*	Define if restrictions apply by time of the day
*	Make list of all posible risks, unexpected or undesirable events
*   https://www.fxcm.com/legal/trading-execution-risks/
*	What happen if the system cannot get rates?
*	~~Validates the amount of data required to calculate indicators, including any holes in data~~
*	Define rule to restrict transactions based on the size of stop loss
*	~~Implement function to calculate the period based on the values passed by parameter~~
*   Check candles lagging behind in live account
*   Handle error on leverage and insuficient funds, maybe limiting the position size or stop los size (minimum size)
*   Verify that the candles are not lagging behind as we do with rates
*   Check workflow to verify is it is needed to close a position in case of anomaly