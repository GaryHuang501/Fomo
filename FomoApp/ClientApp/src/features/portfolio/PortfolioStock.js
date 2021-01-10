import './PortfolioStock.css';

import React from 'react';
import { selectStockData } from '../stocks/stocksSlice';
import { useSelector } from 'react-redux'

export const PortfolioStock = (props) => {

    const stockData = useSelector(state => selectStockData(state, props.portfolioSymbol));

    return (
        <tr className="portfolio-row portfolio-stock">
            <td className="portfolio-column portfolio-row-name">{props.portfolioSymbol.ticker}</td>
            <td className="portfolio-column portoflio-stock-market-price">{stockData.price}</td>
            <td className="portfolio-column portfolio-row-change">{Number.isFinite(stockData.changePercent) ? stockData.changePercent.toFixed(2): "--"}%</td>
            <td className="portfolio-column portfolio-row-average-price">{stockData.averagePrice ?? "--"}</td>
            <td className="portfolio-column portfolio-row-return">{stockData.return ?? "--"}</td>
            <td className="portfolio-column portoflio-stock-bull-icon">{stockData.bull ?? 0}</td>
            <td className="portfolio-column portfolio-row-bear-icon">{stockData.bear ?? 0}</td>
            <td className="portfolio-column portfolio-row-options">
                <span className="portfolio-row-options-field">Up</span>
                <span className="portfolio-row-options-field">Down</span>
                <span className="portfolio-row-options-field">Edit</span>
                <span className="portfolio-row-options-field">Remove</span>
                <span className="portfolio-row-options-field">Chart</span>
            </td>
        </tr>
    );
}