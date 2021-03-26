import './PortfolioStock.css';

import React, { useState } from 'react';
import { SortDirectionType, movePortfolioStock, removePortfolioStock } from './PortfolioSlice';
import { selectStockData, selectVoteData } from '../stocks/stocksSlice';
import { useDispatch, useSelector } from 'react-redux'

import PercentageColumn from './PercentageColumn';
import VoteColumn from './VoteColumn';

/*
    The row in the portfolio table that represents a stock in the portfolio.
*/
export const PortfolioStock = (props) => {

    const { portfolioId, portfolioSymbol } = props;
    const dispatch = useDispatch();
     
    const stockData = useSelector(state => selectStockData(state, portfolioSymbol));
    const voteData = useSelector(state => selectVoteData(state, portfolioSymbol.symbolId));

    const [isEditMode, setIsEditMode] = useState(false);


    function onMouseOver(){
        setIsEditMode(true);
    }

    function onMouseLeave(){
        setIsEditMode(false);
    }

    function onMoveUp(){
        dispatch(movePortfolioStock({portfolioSymbolId: portfolioSymbol.id, sortDirection: SortDirectionType.UP}));
    }

    function onMoveDown(){
        dispatch(movePortfolioStock({portfolioSymbolId: portfolioSymbol.id, sortDirection: SortDirectionType.DOWN}));
    }

    function onEdit(){

    }

    function onShowChart(){
        window.open(`https://www.tradingview.com/symbols/${portfolioSymbol.ticker}`, "_blank"); 
    }

    function onRemove(){
        dispatch(removePortfolioStock({portfolioId: portfolioId, portfolioSymbolId: portfolioSymbol.id}));
    }

    return (
        <tr className="portfolio-row portfolio-stock" role='row' onMouseOver={onMouseOver} onMouseLeave={onMouseLeave}>
            <td className="portfolio-column portfolio-row-name" role='cell'>{portfolioSymbol.ticker}</td>
            <td className="portfolio-column portoflio-row-price" role='cell'>{stockData.price}</td>
            <PercentageColumn value={stockData.changePercent} columnValueClassName='portoflio-row-change'></PercentageColumn>
            <td className="portfolio-column portfolio-row-average-price" role='cell'>{stockData.averagePrice ?? "--"}</td>
            <PercentageColumn value={stockData.return} columnValueClassName='portoflio-row-return'></PercentageColumn>
            <VoteColumn symbolId={voteData.symbolId} count={voteData.count} myVoteDirection={voteData.myVoteDirection} isEditMode={isEditMode}></VoteColumn>
            <td className="portfolio-column" role='cell'>
                <div className="portfolio-row-options">
                    <span className="portfolio-row-options-field portfolio-row-options-field-move-up">
                        <i className="fas fa-caret-up" title="Move Up" role="button" onClick={onMoveUp}/>
                    </span>
                    <span className="portfolio-row-options-field portfolio-row-options-field-move-down">
                        <i className="fas fa-caret-down" title="Move Down" role="button"  onClick={onMoveDown}/>
                    </span>
                    <span className="portfolio-row-options-field portfolio-row-options-field-edit">
                        <i className="far fa-edit" title="Edit" role="button" onClick={onEdit}/>
                    </span>
                    <span className="portfolio-row-options-field portfolio-row-options-field-chart">
                        <i className="fas fa-chart-line" title="Show Chart" role="button" onClick={onShowChart}/>
                    </span>
                    <span className="portfolio-row-options-field portfolio-row-options-field-remove">
                        <i className="fas fa-trash" title="Remove" role="button" onClick={onRemove}/>
                    </span>
                </div>
            </td>
        </tr>
    );
}