import './Portfolio.css';

import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { selectPortfolio } from './PortfolioSlice';
import { useSelector } from 'react-redux'

export const Portfolio = function () {
  const portfolio = useSelector(selectPortfolio);

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