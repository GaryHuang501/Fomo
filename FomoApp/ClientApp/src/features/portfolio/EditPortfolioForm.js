import './EditPortfolioForm.css';

import React, { useState } from 'react';

import { updateAvergePricePortfolioStock } from './PortfolioSlice';
import { useDispatch } from 'react-redux';

/*
    Form to edit the portfolio stock settings or values
*/
export default function EditPortfolioForm(props){

    const ticker = props.ticker;
    const dispatch = useDispatch();
    const [averagePrice, setAveragePrice] = useState(0);
    const [showError, setShowError] = useState(false);

    function handleChange(e){
        const newAveragePrice = e.target.value;
        setAveragePrice(newAveragePrice);
        setShowError(parseFloat(newAveragePrice) <= 0);
    }

    function onSubmit(e){
        e.preventDefault();

        if(!showError){
            const payload = {portfolioSymbolId: props.portfolioSymbolId, averagePrice: parseFloat(averagePrice).toFixed(2)};
            dispatch(updateAvergePricePortfolioStock(payload));
            props.onSubmit();
        }       
    }

    return (    
        <div className='edit-portfolio-form'>
            <h3>{ticker}</h3>
            <label htmlFor="input-portfolio-avg-price">Set avg Price: </label><br></br>
            <input type='number' className="form-input" name="input-portfolio-avg-price" value={averagePrice} onChange={handleChange}/><br></br>
            { showError ? <p className='input-portfolio-avg-price-error'>Value must be positive</p> : <br></br>}
            <input className='edit-portfolio-form-submit submit-input' type="submit" value="Submit" onClick={onSubmit}></input>
        </div>
    );
}