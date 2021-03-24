import './PortfolioStock.css';

import React, { useState } from 'react';
import { selectStockData, selectVoteData } from '../stocks/stocksSlice';

import PercentageColumn from './PercentageColumn';
import VoteColumn from './VoteColumn';
import { useSelector } from 'react-redux'

/*
    The row in the portfolio table that represents a stock in the portfolio.
*/
export const PortfolioStock = (props) => {

    const stockData = useSelector(state => selectStockData(state, props.portfolioSymbol));
    const voteData = useSelector(state => selectVoteData(state, props.portfolioSymbol.symbolId));

    const [isEditMode, setIsEditMode] = useState(false);


    function onMouseOver(){
        setIsEditMode(true);
    }

    function onMouseLeave(){
        setIsEditMode(false);
    }

    return (
        <tr className="portfolio-row portfolio-stock" role='row' onMouseOver={onMouseOver} onMouseLeave={onMouseLeave}>
            <td className="portfolio-column portfolio-row-name" role='cell'>{props.portfolioSymbol.ticker}</td>
            <td className="portfolio-column portoflio-row-price" role='cell'>{stockData.price}</td>
            <PercentageColumn value={stockData.changePercent} columnValueClassName='portoflio-row-change'></PercentageColumn>
            <td className="portfolio-column portfolio-row-average-price" role='cell'>{stockData.averagePrice ?? "--"}</td>
            <PercentageColumn value={stockData.return} columnValueClassName='portoflio-row-return'></PercentageColumn>
            <VoteColumn symbolId={voteData.symbolId} count={voteData.count} myVoteDirection={voteData.myVoteDirection} isEditMode={isEditMode}></VoteColumn>
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