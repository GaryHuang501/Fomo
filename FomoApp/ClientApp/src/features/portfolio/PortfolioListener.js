import './Portfolio.css';

import React, { useEffect, useState } from 'react';
import { fetchStockSingleQuoteDatas, fetchVoteData } from './../stocks/stocksSlice';
import { useDispatch, useSelector } from 'react-redux';

import PortfolioStockListener from './PortfolioStockListener';
import { selectSelectedPortfolioSymbols } from './PortfolioSlice';

/**
 * Listens to any changes to any stock data changes for the symbols in the portfolio using firebase.
 * Any stale stocks will queue up a request to fetch new data in regular intervals.
 */
export default function PortfolioListener(){

    const portfolioSymbols = useSelector(selectSelectedPortfolioSymbols);

    const [symbolsToUpdateQueue] = useState([]);
    const dispatch = useDispatch();

    function notifyStockDataChanged(symbolId){
        symbolsToUpdateQueue.push(symbolId);
    }
    
    useEffect(() => {

        if(portfolioSymbols.length === 0){
            return;
        }
        
        function batchUpdateStocks(){
            if(symbolsToUpdateQueue.length > 0){
                dispatch(fetchStockSingleQuoteDatas(symbolsToUpdateQueue));
            }

            symbolsToUpdateQueue.length = 0;           
        }
        
        function refreshData(){
            const symbolIds = portfolioSymbols.map(s => s.symbolId);

            if(symbolIds.length > 0){
                dispatch(fetchStockSingleQuoteDatas(symbolIds));
                dispatch(fetchVoteData(symbolIds));
            }  
        }

        // This will periodically poll for any changes and notify the server that
        // these stocks are awaiting updates. The server will prioritize stocks
        // with the most subscriber for a set interval.
        const subscribeInterval = setInterval(refreshData, Math.max(60000, process.env.REACT_APP_STOCK_SUBSCRIBE_RATE_MS));

        // For stocks that received a notification to update, they will be batched together and updated in intervals.
        const batchInterval = setInterval(batchUpdateStocks, Math.max(1000, process.env.REACT_APP_STOCK_REFRESH_RATE_MS));
   
        refreshData();

        return () => { 
            clearInterval(subscribeInterval);
            clearInterval(batchInterval);
        }
        
    }, [dispatch, symbolsToUpdateQueue, portfolioSymbols]);

    const portfolioStockListeners = portfolioSymbols.map( ps => 
        <PortfolioStockListener key={ps.symbolId} portfolioSymbol={ps} symbolsToUpdateQueue={symbolsToUpdateQueue} notifyStockDataChanged={notifyStockDataChanged} 
    />);

    return (<div id='portfolio-stock-listeners'>{portfolioStockListeners}</div>)
}

