import './Portfolio.css';

import React, { useEffect } from 'react';
import { fetchStockSingleQuoteDatas, selectStocksLastUpdatedDates } from './../stocks/stocksSlice'
import { shallowEqual, useDispatch, useSelector } from 'react-redux'

import firebase from 'firebase/app';
import { selectPortfolio } from './PortfolioSlice';
import { singleQuoteDataPath } from '../../app/FireBasePaths';

/**
 * Listens to any changes to any stock data changes for the symbols in the portfolio.
 * Uses firebase realtime database to listen for any changes.
 */
export const PortfolioListener = function(){


    const portfolio = useSelector(selectPortfolio);
    const stockLastUpdatedDates = useSelector(selectStocksLastUpdatedDates, shallowEqual);

    const dispatch = useDispatch();
  
    useEffect(() => {
        let portfolioSymbolsToUpdate = [];
        const listeners = [];
        let interval;

        function checkStockNeedsRefreshing(portfolioSymbol, stockChangedNotification){

            const currentLastUpdatedDate = stockLastUpdatedDates[portfolioSymbol.symbolId];
            const noDataExists = !currentLastUpdatedDate || currentLastUpdatedDate.lastUpdated === null || !stockChangedNotification;
            const isStaleData = noDataExists || Date.parse(stockChangedNotification.lastUpdated) > Date.parse(currentLastUpdatedDate);

            if(isStaleData){
                portfolioSymbolsToUpdate.push(portfolioSymbol);
            }
        }


        function addListenersPortfolioSymbol(){
            for(const portfolioSymbol of portfolio.portfolioSymbols){        
                const stockRef = firebase.database().ref(`${singleQuoteDataPath}/${portfolioSymbol.symbolId}`);
                listeners.push(stockRef);

                // Receives notification that the data has changed.
                // Push the symbol to be updated by interval if it the data is newer.
                stockRef.on('value', (snapshot) => {
                    const stockChangedNotification = snapshot.val();

                    checkStockNeedsRefreshing(portfolioSymbol, stockChangedNotification);
                });
            }
        }

        function clearListeners(){
            for(const listener of listeners){
                listener.off('value');
            }
        }

        // Update stock symbols in interval so the calls are batched to reduce chattiness.
        function setIntervalUpdateSymbols(){
            interval = setInterval(() => {
                if(portfolioSymbolsToUpdate.length === 0){
                    return;
                }
                
                const symbolIds = portfolioSymbolsToUpdate.map(s => s.symbolId);

                dispatch(fetchStockSingleQuoteDatas(symbolIds));
              
                portfolioSymbolsToUpdate = [];

            }, process.env.REACT_APP_STOCK_REFRESH_RATE_MS);
        }

        addListenersPortfolioSymbol();
        setIntervalUpdateSymbols();

        return () => { 
            clearListeners();
            clearInterval(interval);
        };
      }, [portfolio, stockLastUpdatedDates, dispatch]);

    return (null);
}

