*	~~Check halted state~~
*   Find a way to select pairs to trade in a daily or weekly basis
*   Modify calculation of position sizing to have crosses with the account balance in USD
*   Check why in big trends, the exit seems like not working
*   Add max allowable spread and schedule constraints as a parameter
*   Widen the exit using a bigger ema or adx below level or both
*   Modify calculation of position sizing to have crosses with the account balance in USD, notice the pips values in the position are given by the second currency in the pair, in EURUSD, the pips are in USD.
*   Using the ATR as a filter: The average range is also a good indicator to use for filtering out trades. Traders typically need volatility to make any money so if you have a system that generates lots of different signals, you can filter out those ones that are low in volatility by discarding those with a low ATR. Concentrating on markets with the highest ATR’s mean you can trade the markets that are experiencing the most movement and therefore the most profit potential.
*   Explore partially close positions that hits a certain target
*   Open chance to trade an instrument if there is no current risk
*   Save candle data to database using days, hours, 15, 5, 1 minute and 5 seconds
*   Modify backtester to separate from rates adapter
*   Use backtester to identify issues with slow entries
*   Need to know how long it takes to evaluate a rate and improve performance
*   ~~Build job for GPBUSD and USDJPY~~
*   	~~Build job for USDCHF~~
*   Start transactions if ADX > 25, even if it is not increasing?
*   Put stop loss two more pips above the minimum
*   When calculating position size, decrease the risk amount using the spread taking into account that in case of a stop, the stop is calculated with the same price the order as placed and not with the opposite price. 
*   Add agents for 30 and 60 minutes on the four pairs: 8 more agents and accounts
*       ~~In mondays take the candles from friday~~
*   ~~The decision on closing or modify order depends on new candles, but when there is an open trade, there are no new candles, should modify the procedure to get candles first and then handle the open trade. it will be necesary now to decide what to do if no new candles or prices are coming~~
*   ~~In mondays take the candles from friday~~
*   ~~Check why the job was stuck in getting candles~~
*   Investigate why the risk strategy allowed for a loss bigger than the max risk
*       ~~Check why the job was stuck in getting candles~~
*   ~~Investigate why the risk strategy allowed for a loss bigger than the max risk~~
*   Investigate using RSI, Stochastic, MACD to spot/confirm trend
*   Dump all the algorithm state before exiting the check, including instrument, adx, moving averages and any other reason causing exit without issuing transaction
*   Look for an option to Oanda to keep alternatives open
*   Hide connection strings and azure settings and put it in configuration
*   ~~Need to allow for a job to execute if there is an open order~~
*   ~~Change logic to prioritize close/modify order in case of unstable conditions~~
*	Health monitoring, alerts and logging (important events, traces)
*	Add rates, candles, orders and results in permanent store for backtesting, complaints and debugging
*	~~Verify rates and candles completion~~
	*	~~Make check rates call get last candle~~
	*	~~Prevent adding repeated candles~~
	*	~~Complete missing candles~~
*	Compare adx calculation against other systems: in progress, seems good with visual inspection: USE QUANT CONNECT
*	Add retries to get values or execute orders
*	Expose configuration
	*	Enable/disable auto transactions
	*       Stop/start agent
	*   Set history to calculate adx direction in app config or check if we want to check direction
	*   Azure credentials
	*   Oanda Credentials
	*   Rate period
	*   Currency pair
	*   Other configurations in the trading class
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
*   ~~Check candles lagging behind in live account~~
*   Handle error on leverage and insuficient funds, maybe limiting the position size or stop los size (minimum size)
*   ~~Verify that the candles are not lagging behind as we do with rates~~
*   ~~Verify that the candles are not lagging behind as we do with rates~~
*   Check workflow to verify is it is needed to close a position in case of anomaly
*   Check workflow to verify is it is needed to close a position in case of anomaly
*   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
*   Develop a mean reversion strategy (https://www.quantopian.com/posts/trading-strategy-mean-reversion, https://www.dukascopy.com/fxcomm/fx-article-contest/?Mean-Reversion-Trading-Strategy=&action=read&id=2503&language=en)
*   Decide on how to share the balance between several strategies
*   http://tradingmarkets.com/recent/how_to_use__average_true_range_for_short-term_trading-677507.html
*   http://www.forexfactory.com/showthread.php?p=5326183
*   http://www.investopedia.com/articles/trading/08/atr.asp


