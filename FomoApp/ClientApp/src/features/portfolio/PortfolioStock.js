import './PortfolioStock.css';

import React from 'react';
import { selectStockData } from '../stocks/stocksSlice';
import { useSelector } from 'react-redux'

export const PortfolioStock = (props) => {

    const stockData = useSelector(state => selectStockData(state, props.portfolioSymbol));

    return (
        <tr className="portfolio-row portfolio-stock" role='row'>
            <td className="portfolio-column portfolio-row-name" role='cell'>{props.portfolioSymbol.ticker}</td>
            <td className="portfolio-column portoflio-row-price" role='cell'>{stockData.price}</td>
            <td className="portfolio-column portfolio-row-change" role='cell'>{Number.isFinite(stockData.changePercent) ? stockData.changePercent.toFixed(2): "--"}%</td>
            <td className="portfolio-column portfolio-row-average-price" role='cell'>{stockData.averagePrice ?? "--"}</td>
            <td className="portfolio-column portfolio-row-return" role='cell'>{stockData.return ?? "--"}%</td>
            <td className="portfolio-column portoflio-row-votes" role='cell'>{stockData.votes}</td>
            <td className="portfolio-column portfolio-row-options" role='cell'>
                <span className="portfolio-row-options-field">Up</span>
                <span className="portfolio-row-options-field">Down</span>
                <span className="portfolio-row-options-field">Edit</span>
                <span className="portfolio-row-options-field">Remove</span>
                <span className="portfolio-row-options-field">Chart</span>
            </td>
        </tr>
    );
}