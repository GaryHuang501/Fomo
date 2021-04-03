import './EditPortfolioForm.css';

import React, { useState } from 'react';

import { updateAvergePricePortfolioStock } from './PortfolioSlice';
import { useDispatch } from 'react-redux';

export default function EditPortfolioForm(props){

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
            <h3>RKT</h3>
            <label htmlFor="input-portfolio-avg-price">Set avg Price: </label>
            <input type='number' className="input-portfolio-avg-price" name="input-portfolio-avg-price" value={averagePrice} onChange={handleChange}/>
            { showError ? <p className='input-portfolio-avg-price-error'>Value must be positive</p> : <br></br>}
            <br></br>
            <input type="submit" value="Submit" onClick={onSubmit}></input>
        </div>
    );
}