import './Portfolio.css';

import React, { useEffect } from 'react';

import firebase from 'firebase/app';
import { selectPortfolio } from './PortfolioSlice';
import { useSelector } from 'react-redux'

// Listens for any changes to the portfolio stocks and updates the store.
export const PortfolioListener = function(){

    const portfolio = useSelector(selectPortfolio);

    useEffect(() => {
        const listeners = [];

        for(const symbol in portfolio.portfolioSymbols){        
            const stockRef = firebase.database().ref(`singlequote/${symbol.symbolId}`);
            listeners.push(stockRef);

            stockRef.on('value', (snapshot) => {
                const data = snapshot.val();

                if(data){
                    console.log(data);
                }
            });
        }

        return () => {
            for(const listener in listeners){
                firebase.off(listener);
                console.log('remove listeners');
            }
        };
      }, [portfolio]);

    return (null);
}

