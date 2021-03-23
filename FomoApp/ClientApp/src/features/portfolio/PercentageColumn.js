import './PercentageColumn.css';

import React from 'react';

/*
    Column in the portfolio stock row that displays percentage data.
*/
export default function PercentageColumn(props){

    const positiveClassName = 'portfolio-stock-positive-delta';
    const negativeClassName = 'portfolio-stock-negative-delta';

    let displayValue = props.value;
    let colorClassName = '';

    function getColorClassName(value){
        if(value > 0){
            return positiveClassName;
        }
        else if (value < 0){
            return negativeClassName;
        }

        return '';
    }

    if(!Number.isFinite(displayValue)){
        displayValue = "--";
    }
    else{    
        colorClassName = getColorClassName(displayValue);
        displayValue = displayValue.toFixed(2);
    }

    const classes = `portfolio-column ${props.columnValueClassName} ${colorClassName}`;

    return (
        <td className={classes} role='cell'>{displayValue}%</td>
    );
}