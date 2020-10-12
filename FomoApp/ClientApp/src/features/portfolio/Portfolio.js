import React from 'react';
import { PortfolioStock } from './PortfolioStock';
import './Portfolio.css';
import { faspanList } from '@fortawesome/free-solid-svg-icons';

export const Portfolio = function () {

  const portfolioStocks = [];

  for (let i = 0; i < 5; i++) {
    portfolioStocks.push(<PortfolioStock></PortfolioStock>)
  }
  return (
    <table id="portfolio">
      <tr className="portfolio-row portfolio-header">
          <th className="portfolio-column">Symbol</th>
          <th className="portfolio-column">MKT $</th>
          <th className="portfolio-column">Avg $</th>
          <th className="portfolio-column">Change</th>
          <th className="portfolio-column">Bull</th>
          <th className="portfolio-column">Bear</th>
          <th className="portfolio-column portfolio-row-options-header">         
              <span>Up</span>
              <span>Down</span>
              <span>Edit</span>
              <span>Remove</span>
              <span>Chart</span>
          </th>
      </tr>
      <tr className="portfolio-row-spacing">
      </tr>
      {portfolioStocks}
    </table>
  );
}