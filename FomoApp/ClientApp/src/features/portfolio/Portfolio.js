import React, { useState, useEffect } from 'react';
import './Portfolio.css';
import { PortfolioStock } from './PortfolioStock';
import { useSelector, useDispatch } from 'react-redux'
import { fetchPortfolio, fetchPortfolioIds, selectPortfolioIds } from './PortfolioSlice';

export const Portfolio = function () {

  const dispatch = useDispatch();
  const portfolioIds = useSelector(selectPortfolioIds);
  const [selectedPortfolioId, setSelectedPortfolioId] = useState('');

  useEffect(() => {
    dispatch(fetchPortfolioIds());

    if(portfolioIds.length > 0){
      
      // Currently only one portfolio is supported.
      setSelectedPortfolioId(portfolioIds[0]);
      console.log(portfolioIds[0]);
    }
  }, [portfolioIds]);

  const portfolioStocks = [];

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