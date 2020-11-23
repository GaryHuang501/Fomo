import React from 'react';
import './PortfolioStock.css';

export const PortfolioStock = (props) => {
    return (
        <tr className="portfolio-row portfolio-stock">
            <td className="portfolio-column portfolio-row-name">{props.symbol.ticker}</td>
            <td className="portfolio-column portoflio-stock-market-price">Market Price</td>
            <td className="portfolio-column portfolio-row-average-price">Average Price</td>
            <td className="portfolio-column portfolio-row-change">Change</td>
            <td className="portfolio-column portoflio-stock-bull-icon">Bull</td>
            <td className="portfolio-column portfolio-row-bear-icon">Bear</td>
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