import './Portfolio.css';

import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { selectSelectedPortfolio } from './PortfolioSlice';
import { useSelector } from 'react-redux'

export const Portfolio = function (props) {
  const portfolio = useSelector(selectSelectedPortfolio);
  const isMyUserPage = props.isMyUserPage;

  const portfolioStocks = portfolio.portfolioSymbols.map(portfolioSymbol => {
    return <PortfolioStock 
              key={portfolioSymbol.symbolId} 
              portfolioId={portfolio.id} 
              portfolioSymbol={portfolioSymbol}
              showOptions={isMyUserPage}
              ></PortfolioStock>
  });
  
  return (
    <table id="portfolio">
      <thead>
        <tr className="portfolio-row portfolio-header" role="row">
            <th className="portfolio-column portfolio-header-column" role="columnheader">Symbol</th>
            <th className="portfolio-column portfolio-header-column" role="columnheader">MKT $</th>
            <th className="portfolio-column portfolio-header-column" role="columnheader">Change</th>
            <th className="portfolio-column portfolio-header-column" role="columnheader">Avg $</th>
            <th className="portfolio-column portfolio-header-column" role="columnheader">ROI</th>
            <th className="portfolio-column portfolio-header-column" role="columnheader">Votes</th>
            <th className="portfolio-column portfolio-header-column portfolio-row-options-header" role="columnheader">{ isMyUserPage ? <i className="fas fa-ellipsis-v"/> : null}</th>
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