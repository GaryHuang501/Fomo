import './PortfolioStock.css';

import React, { useState } from 'react';
import { SortDirectionType, movePortfolioStock, removePortfolioStock } from './PortfolioSlice';
import { selectStockData, selectVoteData } from '../stocks/stocksSlice';
import { useDispatch, useSelector } from 'react-redux'

import EditPortfolioForm  from './EditPortfolioForm';
import Modal from '../../app/Modal';
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
    const showOptions = props.showOptions;
    const roi = calculateRoi();

    const [showEditPortfolioModal, setShowEditPortfolioModal] = useState(false);

    const [isEditMode, setIsEditMode] = useState(false);

    function calculateRoi(){
        if(!stockData.price){
            return null;
        }

        if(stockData.price === 0){
            return -100;
        }
        
        if(!portfolioSymbol.averagePrice || portfolioSymbol.averagePrice === 0){
            return 0;
        }

        return ((stockData.price - portfolioSymbol.averagePrice) /  portfolioSymbol.averagePrice) * 100;
    }

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
        setShowEditPortfolioModal(true);
    }

    function onCloseEdit(){
        setShowEditPortfolioModal(false);
    }

    function onShowChart(){
        window.open(`${process.env.REACT_APP_CHART_URL}/${portfolioSymbol.ticker}`, "_blank"); 
    }

    function onRemove(){
        dispatch(removePortfolioStock({portfolioId: portfolioId, portfolioSymbolId: portfolioSymbol.id}));
    }

    function formatAmount(amount, defaultVal){
        return amount > 0 ? amount.toFixed(2) : defaultVal;
    }

    return (
        <tr className="portfolio-row portfolio-stock" role='row' onMouseOver={onMouseOver} onMouseLeave={onMouseLeave}>
            <td className="portfolio-column portfolio-row-name" role='cell'>{portfolioSymbol.ticker}</td>
            <td className="portfolio-column portoflio-row-price" role='cell'>{formatAmount(stockData.price, "Pending")}</td>
            <PercentageColumn value={stockData.changePercent} columnValueClassName='portoflio-row-change'></PercentageColumn>
            <td className="portfolio-column portfolio-row-average-price" role='cell'>{formatAmount(portfolioSymbol.averagePrice, "--")}</td>
            <PercentageColumn value={roi} columnValueClassName='portoflio-row-return'></PercentageColumn>
            <VoteColumn symbolId={voteData.symbolId} count={voteData.count} myVoteDirection={voteData.myVoteDirection} ticker={stockData.ticker} isEditMode={isEditMode}></VoteColumn>
            <td className="portfolio-column" role='cell'>
                {
                    !showOptions ? null :
                    <div className="portfolio-row-options">
                        <span className="portfolio-row-options-edit-modal-container">
                            {showEditPortfolioModal 
                                ? <Modal onCancel={onCloseEdit}><EditPortfolioForm onSubmit={onCloseEdit} portfolioSymbolId={portfolioSymbol.id}/></Modal> 
                                : null
                            }
                        </span>
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
                }
            </td>
        </tr>
    );
}