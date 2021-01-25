import './Portfolio.css';

import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux'

import { PortfolioStock } from './PortfolioStock';
import { selectSelectedPortfolio } from './PortfolioSlice';

export const Portfolio = function () {
 const portfolio = useSelector(selectSelectedPortfolio);
 const dispatch = useDispatch();
 
  useEffect(() => {
    // Render when portfolio Id is fetched from portfolio page.
    if(portfolio.Id > 0){
      dispatch(fetchPortfolio(portfolio.Id));
    }
  }, [portfolio.Id, dispatch]);
  
  const portfolioStocks = [];

  for(const portfolioSymbol of portfolio.portfolioSymbols){
    const stock = <PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}></PortfolioStock>
    portfolioStocks.push(stock);
  }
  
  return (
    <table id="portfolio">
      <thead>
        <tr className="portfolio-row portfolio-header" role="row">
            <th className="portfolio-column portfolio-header-column" role="columnHeader">Symbol</th>
            <th className="portfolio-column portfolio-header-column" role="columnHeader">MKT $</th>
            <th className="portfolio-column portfolio-header-column" role="columnHeader">Change</th>
            <th className="portfolio-column portfolio-header-column" role="columnHeader">Avg $</th>
            <th className="portfolio-column portfolio-header-column" role="columnHeader">ROI</th>
            <th className="portfolio-column portfolio-header-column" role="columnHeader">Votes</th>
            <th className="portfolio-column portfolio-header-column portfolio-row-options-header" role="columnHeader">         
                <span>Up</span>
                <span>Down</span>
                <span>Edit</span>
                <span>Remove</span>
                <span>Chart</span>
            </th>
        </tr>
      </thead>
      <tbody>
        <tr className="portfolio-row-spacing" role="row">
        </tr>
        {portfolioStocks}
      </tbody>
    </table>
  );
}