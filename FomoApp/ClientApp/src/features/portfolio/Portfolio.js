import './Portfolio.css';

import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { selectPortfolio } from './PortfolioSlice';
import { useSelector } from 'react-redux'

export const Portfolio = function () {
  const portfolio = useSelector(selectPortfolio);

  const portfolioStocks = [];

  for(const symbol of portfolio.portfolioSymbols){
    const stock = <PortfolioStock key={symbol.symbolId} symbol={symbol}></PortfolioStock>
    portfolioStocks.push(stock);
  }
  
  return (
    <table id="portfolio">
      <thead>
        <tr className="portfolio-row portfolio-header">
            <th className="portfolio-column portfolio-header-column">Symbol</th>
            <th className="portfolio-column portfolio-header-column">MKT $</th>
            <th className="portfolio-column portfolio-header-column">Avg $</th>
            <th className="portfolio-column portfolio-header-column">Change</th>
            <th className="portfolio-column portfolio-header-column">Bull</th>
            <th className="portfolio-column portfolio-header-column">Bear</th>
            <th className="portfolio-column portfolio-header-column portfolio-row-options-header">         
                <span>Up</span>
                <span>Down</span>
                <span>Edit</span>
                <span>Remove</span>
                <span>Chart</span>
            </th>
        </tr>
      </thead>
      <tbody>
        <tr className="portfolio-row-spacing">
        </tr>
        {portfolioStocks}
      </tbody>
    </table>
  );
}