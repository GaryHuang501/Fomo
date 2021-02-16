import './Portfolio.css';

import { fetchStockSingleQuoteDatas, selectStocksLastUpdatedDates } from './../stocks/stocksSlice';
import { shallowEqual, useDispatch, useSelector } from 'react-redux';

import PortfolioStockListener from './PortfolioStockListener';
import firebase from 'firebase/app';
import { selectSelectedPortfolio } from './PortfolioSlice';
import { singleQuoteDataPath } from '../../app/FirebasePaths';
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
        let interval;
        const portfolioStockListeners = [];
      
        function clearListeners(){
            for(const listener of portfolioStockListeners){
                listener.clearListener();
            }
        }

        function createListeners(){
            for(const portfolioSymbol of portfolio.portfolioSymbols){
                const listener = new PortfolioStockListener(portfolioSymbol, stockLastUpdatedDates[portfolioSymbol.symbolId]);
                listener.bindListener(firebase);
                portfolioStockListeners.push(listener);
            }
        }
     
        function checkStockUpdates(){
            if(portfolioStockListeners.length === 0){
                return;
            }

            const symbolIdsToUpdate = [];

            for(const listener of portfolioStockListeners){
                if(listener.isDataStale){
                    listener.isDataStale = false; // mark as not stale since it will be queued for updates.
                    symbolIdsToUpdate.push(listener.portfolioSymbol.symbolId);
                }
            }
            
            if(symbolIdsToUpdate.length > 0){
                dispatch(fetchStockSingleQuoteDatas(symbolIdsToUpdate));             
            }
        }

        createListeners();
        checkStockUpdates(); // trigger an immediate update to handle page load and refreshes.
        interval = setInterval(checkStockUpdates, process.env.REACT_APP_STOCK_REFRESH_RATE_MS);
        
        return () => { 
            clearListeners();
            clearInterval(interval);
        };
      }, [portfolio, stockLastUpdatedDates, dispatch]);

    return (null);
}

