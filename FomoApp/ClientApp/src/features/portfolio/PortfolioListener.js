import './Portfolio.css';

import { fetchStockSingleQuoteDatas, fetchVoteData, selectStocksLastUpdatedDates } from './../stocks/stocksSlice';
import { shallowEqual, useDispatch, useSelector } from 'react-redux';

import PortfolioStockListener from './PortfolioStockListener';
import firebase from 'firebase/app';
import { selectSelectedPortfolio } from './PortfolioSlice';
import { useEffect } from 'react';

/**
 * Listens to any changes to any stock data changes for the symbols in the portfolio using firebase.
 * Any stale stocks will queue up a request to fetch new data in regular intervals.
 */
export default function PortfolioListener(){

    const portfolio = useSelector(selectSelectedPortfolio, shallowEqual);

    // When a stock is stale, it'll trigger an update request and this list stockLastUpdatedDates will refresh
    // recreating the portfolio stock listeners with the new dates and info.
    const stockLastUpdatedDates = useSelector(selectStocksLastUpdatedDates, shallowEqual);

    const dispatch = useDispatch();
  
    useEffect(() => {
        const portfolioStockListeners = [];

        let subscribeInterval;
        let batchInterval;
        let symbolIdsToUpdate = [];
      
        function clearListeners(){
            for(const listener of portfolioStockListeners){
                listener.clearListener();
            }
        }

        function notifyStockDataChanged(symbolId){
            symbolIdsToUpdate.push(symbolId);
        }

        function createListeners(){
            for(const portfolioSymbol of portfolio.portfolioSymbols){
                const listener = new PortfolioStockListener(portfolioSymbol, stockLastUpdatedDates[portfolioSymbol.symbolId]);
                listener.bindListener(notifyStockDataChanged, firebase);
                portfolioStockListeners.push(listener);
            }
        }
     
        // gets server data and subscribe to any further update
        function subscribeStockUpdates(){
            const symbolIds = portfolio.portfolioSymbols.map(s => s.symbolId);

            if(symbolIds.length > 0){
                dispatch(fetchStockSingleQuoteDatas(symbolIds));
                dispatch(fetchVoteData(symbolIds));
            }       
        }

        function batchUpdateStocks(){
            if(symbolIdsToUpdate.length > 0){
                dispatch(fetchStockSingleQuoteDatas(symbolIdsToUpdate));
            }

            symbolIdsToUpdate = [];             
        }

        createListeners();
        subscribeStockUpdates();

        // This will periodically poll for any changes and notify the server that
        // these stocks are awaiting updates. The server will prioritize stocks
        // with the most subscriber for a set interval.
        subscribeInterval = setInterval(subscribeStockUpdates, process.env.REACT_APP_STOCK_SUBSCRIBE_RATE_MS);

        // For stocks that received a notification to update, they will be batched together for an interval.
        batchInterval = setInterval(batchUpdateStocks, process.env.REACT_APP_STOCK_REFRESH_RATE_MS);

        return () => { 
            clearListeners();
            clearInterval(subscribeInterval);
            clearInterval(batchInterval);
        };
      }, [portfolio, stockLastUpdatedDates, dispatch]);

    return (null);
}

