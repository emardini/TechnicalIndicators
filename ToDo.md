*	~~Check halted state~~
*	Logging (important events, traces)
*	Add rates, candles, orders and results in permanent store for backtesting, compalints and debugging
*	~~Verify rates and candles completion~~
	*	~~Make check rates call get last candle~~
	*	~~Prevent adding repeated candles~~
	*	~~Complete missing candles~~
*	Compare adx calculation against other systems
*	Add retries
*	Health monitoring and alerts
*	Expose configuration
	*	Enable/disable auto transactions
*	Implement kelly criterion
*	Limit amount of data to be kept 
*	Backtest
	*	Prebuilt data
	*	Historical data
	*	Uptrend
	*	Downtrend
	*	No trend
	*	Exit
	*	Stop losses
*	~~Make data provider to get east time~~
*	~~Validate the candle timestamp matches the rate date~~
*	Define if restrictions apply by time of the day
*	Make list of all posible risks, unexpected or undesirable events
*	What happen if the system cannot get rates?
*	~~Validates the amount of data required to calculate indicators, including any holes in data~~
*	Define rule to restrict transactions based on the size of stop loss
*	Implement function to calculate the period based on the values passed by parameter
