import './EditPortfolioForm.css';

import React, { useState } from 'react';

import { updateAvergePricePortfolioStock } from './PortfolioSlice';
import { useDispatch } from 'react-redux';

/*
    Form to edit the portfolio stock settings or values
*/
export default function EditPortfolioForm(props){

    const maxValue = 10000000;
    const ticker = props.ticker;
    const dispatch = useDispatch();
    const [averagePrice, setAveragePrice] = useState(0);
    const [error, setError] = useState(false);

    function handleChange(e){
        const newAveragePrice = parseFloat(e.target.value);
        const error = validatePrice(newAveragePrice);

        setAveragePrice(newAveragePrice);
        setError(error);
    }

    function validatePrice(price)
    {
        if(price <= 0 || price == NaN)
        {
            return "Value must be positive number.";
        }

        if(price > maxValue)
        {
            return `Max value is ${maxValue}`;
        }

        return null;
    }

    function onSubmit(e){
        e.preventDefault();

        if(error === null){
            const payload = {portfolioSymbolId: props.portfolioSymbolId, averagePrice: parseFloat(averagePrice).toFixed(2)};
            dispatch(updateAvergePricePortfolioStock(payload));
            props.onSubmit();
        }       
    }

    return (    
        <form className='edit-portfolio-form'>
            <h3>{ticker}</h3>
            <label htmlFor="input-portfolio-avg-price">Set avg Price: </label><br></br>
            <input max={maxValue} min='0' type='number' className="form-input" name="input-portfolio-avg-price" value={averagePrice} onChange={handleChange}/><br></br>
            { error != null ? <p className='input-portfolio-avg-price-error'>{error}</p> : <br></br>}
            <input className='edit-portfolio-form-submit submit-input' type="submit" value="Submit" onClick={onSubmit}></input>
        </form>
    );
}