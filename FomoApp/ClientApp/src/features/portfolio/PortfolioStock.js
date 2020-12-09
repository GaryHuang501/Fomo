import React from 'react';
import './PortfolioStock.css';
import { useSelector } from 'react-redux'
import { selectStockData } from '../stocks/stocksSlice';

export const PortfolioStock = (props) => {

    const stockData = useSelector(state => selectStockData(state, props.symbol));

    return (
        <tr className="portfolio-row portfolio-stock">
            <td className="portfolio-column portfolio-row-name">{props.symbol.ticker}</td>
            <td className="portfolio-column portoflio-stock-market-price">{stockData.marketPrice}</td>
            <td className="portfolio-column portfolio-row-average-price">{stockData.averagePrice}</td>
            <td className="portfolio-column portfolio-row-change">{stockData.change}</td>
            <td className="portfolio-column portoflio-stock-bull-icon">{stockData.bull}</td>
            <td className="portfolio-column portfolio-row-bear-icon">{stockData.bear}</td>
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