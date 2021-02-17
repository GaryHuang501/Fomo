import { singleQuoteDataPath } from '../../app/FirebasePaths';

/*
Listens for any notification that a stock in a portfolio has data updates.
*/
class PortfolioStockListener {

  constructor(portfolioSymbol, lastUpdated) {
    this.portfolioSymbol = portfolioSymbol;
    this.lastUpdated = lastUpdated;
  }

  clearListener() {
    this.stockRef.off('value');
  }

  bindListener(notifyCallback, firebase) {

    this.stockRef = firebase.database().ref(`${singleQuoteDataPath}/${this.portfolioSymbol.symbolId}`);  

    // Receive notification that a stock was updated. Update immediately.
    this.stockRef.on('value', (snapshot) => {
      const stockChangedNotification = snapshot.val();
      const serverSymbolUpdateDate = stockChangedNotification ? stockChangedNotification.lastUpdated : null;

      const newerUpdateExists = serverSymbolUpdateDate && Date.parse(serverSymbolUpdateDate) > Date.parse(this.lastUpdated);
      
      if(newerUpdateExists){
        notifyCallback(this.portfolioSymbol.symbolId);
      }
    });
  }
}

export default PortfolioStockListener;