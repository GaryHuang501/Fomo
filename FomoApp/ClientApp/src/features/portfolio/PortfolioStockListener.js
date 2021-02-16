import { singleQuoteDataPath } from '../../app/FirebasePaths';

/*
Listens for any notification that a stock in a portfolio has data updates.
*/
class PortfolioStockListener {

  constructor(portfolioSymbol, lastUpdated) {
    this.portfolioSymbol = portfolioSymbol;
    this.lastUpdated = lastUpdated;

    // Means data does not exist and hence is stale.
    this.isDataStale = lastUpdated === null || lastUpdated == undefined; 
  }

  clearListener() {
    this.stockRef.off('value');
  }

  bindListener(firebase) {

    this.stockRef = firebase.database().ref(`${singleQuoteDataPath}/${this.portfolioSymbol.symbolId}`);  
    this.stockRef.on('value', (snapshot) => {
      const stockChangedNotification = snapshot.val();
      const serverSymbolUpdateDate = stockChangedNotification ? stockChangedNotification.lastUpdated : null;

      const newerUpdateExists = serverSymbolUpdateDate && Date.parse(serverSymbolUpdateDate) > Date.parse(this.lastUpdated);
      if(newerUpdateExists){
        this.isDataStale = true;
      }
    });
  }
}

export default PortfolioStockListener;