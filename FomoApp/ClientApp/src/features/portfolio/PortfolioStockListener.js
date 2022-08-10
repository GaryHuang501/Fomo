import { useEffect, useState } from 'react';

import firebase from 'firebase/app';
import { selectStockLastUpdated } from '../stocks/stocksSlice'
import { singleQuoteDataPath } from '../../app/FirebasePaths';
import { useSelector } from 'react-redux';

/*
Listens for any notification that a stock in a portfolio has data updates.
*/
export default function PortfolioStockListener(props) {

  const {portfolioSymbol, notifyStockDataChanged} = props;
  const [stockRef, setStockRef] = useState(null);
  const lastUpdated = useSelector(state => selectStockLastUpdated(state, portfolioSymbol.symbolId));
  
  useEffect(() => {

    function bindListener() {
      let currentStockRef = stockRef;

      if (!currentStockRef) {
        currentStockRef = firebase.database().ref(`${singleQuoteDataPath}/${portfolioSymbol.symbolId}`);

        // Receive notification that a stock was updated. Update immediately.
        currentStockRef.on('value', (snapshot) => {
          const stockChangedNotification = snapshot.val();
          const serverSymbolUpdateDate = stockChangedNotification ? stockChangedNotification.lastUpdated : null;
  
          const newerUpdateExists = !lastUpdated || (serverSymbolUpdateDate && Date.parse(serverSymbolUpdateDate) > Date.parse(lastUpdated));
  
          if (newerUpdateExists) {
            notifyStockDataChanged(portfolioSymbol.symbolId);
          }
        });

        setStockRef(currentStockRef);
      }     
    }

    bindListener();

    return () => {
      if(stockRef){
        stockRef.off('value');
        setStockRef(null);
      }
    }
  }, [portfolioSymbol, lastUpdated, notifyStockDataChanged, stockRef]);

  return (null);
}

